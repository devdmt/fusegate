using System.Text.Json;
using DAL.Model;
using DAL.Model.FlexiFuture;
using DAL.Model.Pensioner;
using DAL.Model.FlexiFund;
using DAL.ModelView;
using DAL.ModelView.FlexiFuture;
using DAL.ModelView.FlexiFuturePlus;
using DAL.ModelView.Settings;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Application.FlexiFuturePlus;

public partial class FlexiFuturePlusManager
{
    private async Task<FlexiFuturePlusResponse<FlexiFuturePlusOnboardingQuestionsDto>> GetOnboardingQuestionsInternalAsync(
        Guid quoteId,
        PartnerContext partner,
        CancellationToken cancellationToken)
    {
        var response = new FlexiFuturePlusResponse<FlexiFuturePlusOnboardingQuestionsDto>();
        try
        {
            var quote = await _akiba.FlexiFutureQuotes
                .AsNoTracking()
                .Include(q => q.Spouses)
                .FirstOrDefaultAsync(q => q.Id == quoteId, cancellationToken)
                ?? throw new InvalidOperationException($"Quote '{quoteId}' was not found.");

            EnsureQuoteAccessible(quote, partner);

            var questions = await _akiba.HealthQuestionsLibrary
                .AsNoTracking()
                .Include(q => q.ChildQuestions.Where(c => c.IsActive))
                .Where(q => q.IsActive && q.ParentQuestionId == null)
                .OrderBy(q => q.QuestionOrder)
                .ToListAsync(cancellationToken);

            var healthQuestionRows = questions
                .Where(q => q.Category != FlexiFutureHealthQuestionCategories.Hazardous)
                .ToList();

            var hazardQuestionRows = questions
                .Where(q => q.Category == FlexiFutureHealthQuestionCategories.Hazardous)
                .ToList();

            var requiredContexts = GetRequiredHealthContexts(quote);
            var catalog = BuildPartnerHealthQuestionCatalog(
                healthQuestionRows,
                hazardQuestionRows,
                requiredContexts);

            response.Success = true;
            response.Message = "Onboarding questions retrieved successfully.";
            response.Data = new FlexiFuturePlusOnboardingQuestionsDto
            {
                QuoteId = quoteId,
                RequiredContexts = requiredContexts,
                Main = catalog.Main,
                Spouse1 = catalog.Spouse1,
                Spouse2 = catalog.Spouse2,
                QuestionLibrary = catalog.QuestionLibrary,
                Hazardous = catalog.Hazardous
            };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(GetOnboardingQuestionsInternalAsync));
        }

        return response;
    }

    private async Task<FlexiFuturePlusResponse<FlexiFuturePlusOnboardingStateDto>> SaveConsolidatedOnboardingInternalAsync(
        FlexiFuturePlusOnboardingRequestDto request,
        PartnerContext partner,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken)
    {
        var response = new FlexiFuturePlusResponse<FlexiFuturePlusOnboardingStateDto>();
        try
        {
            if (request.QuoteId == Guid.Empty)
                throw new InvalidOperationException("QuoteId is required.");

            var quote = await _akiba.FlexiFutureQuotes
                .Include(q => q.Spouses)
                .FirstOrDefaultAsync(q => q.Id == request.QuoteId, cancellationToken)
                ?? throw new InvalidOperationException($"Quote '{request.QuoteId}' was not found.");

            EnsureQuoteAccessible(quote, partner);
            EnsureQuoteNotCancelled(quote);

            ApplyReferralToQuote(quote, request.CustomerDetails?.RefferalCode ?? request.RefferalCode);

            Guid? customerId = null;
            FlexiFuturePolicy? policy = null;

            if (request.CustomerDetails != null)
            {
                ValidateCustomerDetails(request.CustomerDetails, quote);
                customerId = await EnsureCustomerAsync(
                    request.CustomerDetails,
                    request.RefferalCode,
                    partner,
                    ipAddress,
                    cancellationToken);

                SyncQuoteClientFromCustomerDetails(quote, request.CustomerDetails);

                policy = await EnsurePolicyAsync(
                    request,
                    quote,
                    customerId.Value,
                    partner,
                    ipAddress,
                    browser,
                    cancellationToken);
            }
            else
            {
                policy = await _akiba.FlexiFuturePolicies
                    .FirstOrDefaultAsync(p => p.QuoteId == request.QuoteId, cancellationToken);

                if (policy == null && HasNonCustomerSections(request))
                    throw new InvalidOperationException("customerDetails must be saved before other onboarding sections.");

                if (policy != null)
                {
                    policy.CallbackUrl = request.CallbackUrl ?? policy.CallbackUrl;
                    policy.UpdatedAt = DateTime.UtcNow;
                    customerId = policy.CustomerId;
                }
            }

            if (policy == null)
                throw new InvalidOperationException("Onboarding policy was not found. Submit customerDetails first.");

            EnsureNotComplete(policy);

            await SaveFamilyMembersAsync(policy, request.FamilyMembers, cancellationToken);
            await SaveBeneficiariesAsync(policy, request.Beneficiaries, cancellationToken);
            if (request.HealthQuestions != null)
            {
                await SaveHealthAnswersAsync(
                    policy,
                    quote,
                    request.HealthQuestions,
                    cancellationToken);
            }

            if (HasSubmittedHazardous(request.Hazardous))
                await SaveOccupationHazardAnswersAsync(policy, request.Hazardous!, cancellationToken);

            if (request.FamilyMembers.Count > 0)
            {
                await RefreshQuoteFromOnboardingAsync(quote, policy, partner, cancellationToken);
            }

            if (request.CustomerDetails != null || request.FamilyMembers.Count > 0)
            {
                await SyncFlexiFundFromQuoteAsync(policy, quote, cancellationToken);
            }

            if (HasSubmittedConsent(request.Consent))
            {
                await CompleteConsentAsync(
                    policy,
                    quote,
                    request.Consent!,
                    partner,
                    cancellationToken);
            }
            else
            {
                policy.OnboardingStep = DetermineOnboardingStep(request, policy);
            }
            policy.UpdatedAt = DateTime.UtcNow;
            quote.UpdatedAt = DateTime.UtcNow;

            await _akiba.SaveChangesAsync(cancellationToken);

            var customer = await _akiba.customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == policy.CustomerId, cancellationToken);

            response.Success = true;
            response.Message = HasSubmittedConsent(request.Consent)
                ? policy.IsApproved
                    ? "Onboarding completed successfully. Policy approved."
                    : "Onboarding completed successfully."
                : "Onboarding saved successfully.";
            response.Data = await MapOnboardingStateAsync(policy, quote, customer, request, cancellationToken);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(SaveConsolidatedOnboardingInternalAsync));
        }

        return response;
    }

    private async Task<FlexiFuturePlusResponse<FlexiFuturePlusOnboardingStateDto>> GetOnboardingStateInternalAsync(
        Guid quoteId,
        PartnerContext partner,
        CancellationToken cancellationToken)
    {
        var response = new FlexiFuturePlusResponse<FlexiFuturePlusOnboardingStateDto>();
        try
        {
            var quote = await _akiba.FlexiFutureQuotes
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == quoteId, cancellationToken)
                ?? throw new InvalidOperationException($"Quote '{quoteId}' was not found.");

            EnsureQuoteAccessible(quote, partner);

            var policy = await _akiba.FlexiFuturePolicies
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.QuoteId == quoteId, cancellationToken);

            if (policy == null)
            {
                response.Success = true;
                response.Message = "No onboarding policy found for this quote.";
                response.Data = new FlexiFuturePlusOnboardingStateDto
                {
                    QuoteId = quoteId,
                    OnboardingStep = FlexiFutureOnboardingSteps.KYC,
                    PolicyStatus = FlexiFuturePolicyStatuses.Draft,
                    SectionStatus = new FlexiFuturePlusSectionStatusDto()
                };
                return response;
            }

            response.Success = true;
            response.Message = "Onboarding state retrieved successfully.";
            response.Data = await MapOnboardingStateAsync(
                policy,
                quote,
                await _akiba.customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == policy.CustomerId, cancellationToken),
                null,
                cancellationToken);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(GetOnboardingStateInternalAsync));
        }

        return response;
    }

    private async Task<FlexiFuturePlusResponse<object>> SendConsentOtpInternalAsync(
        FlexiFuturePlusSendOtpRequestDto request,
        PartnerContext partner,
        CancellationToken cancellationToken)
    {
        var response = new FlexiFuturePlusResponse<object>();
        try
        {
            if (request.QuoteId == Guid.Empty)
                throw new InvalidOperationException("QuoteId is required.");

            var policy = await _akiba.FlexiFuturePolicies
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.QuoteId == request.QuoteId, cancellationToken)
                ?? throw new InvalidOperationException("Onboarding policy was not found for this quote.");

            var quote = await _akiba.FlexiFutureQuotes
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == request.QuoteId, cancellationToken)
                ?? throw new InvalidOperationException($"Quote '{request.QuoteId}' was not found.");

            EnsureQuoteAccessible(quote, partner);
            EnsureNotComplete(policy);

            var customer = await _akiba.customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == policy.CustomerId, cancellationToken)
                ?? throw new InvalidOperationException($"Customer '{policy.CustomerId}' was not found.");

            var otpCode = _settings.GenerateRadomCode(_otpSettings.Length);
            var otp = new OTP
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id.ToString(),
                ProductRef = policy.Id.ToString(),
                Code = otpCode,
                Phonenumber = customer.PhoneNumber ?? string.Empty,
                Message = $"Your FlexiFuture Plus consent OTP is {otpCode}.",
                notificationType = NotificationType.SMS,
                CreatedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddMinutes(_otpSettings.ExpiryMinutes),
                IsUsed = false,
                ISsent = false,
                ISSMSSent = false,
                IsEmailSent = false
            };

            _db.OTPs.Add(otp);
            await _db.SaveChangesAsync(cancellationToken);

            response.Success = true;
            response.Message = "Consent OTP queued successfully.";
            response.Data = new { QuoteId = request.QuoteId, OtpReference = otp.Id };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(SendConsentOtpInternalAsync));
        }

        return response;
    }

    private async Task<Guid> EnsureCustomerAsync(
        FlexiFuturePlusCustomerDetailsDto details,
        string? topLevelReferralCode,
        PartnerContext partner,
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        var idNumber = details.IdNumber.Trim();
        var existing = await _akiba.customers
            .FirstOrDefaultAsync(c => c.Idnumber == idNumber, cancellationToken);

        var gender = ParseGender(details.Gender);
        var referralCode = details.RefferalCode ?? topLevelReferralCode;

        if (existing != null)
        {
            existing.Firstname = details.FirstName.Trim();
            existing.Lastname = details.LastName.Trim();
            existing.Fullname = $"{details.FirstName.Trim()} {details.LastName.Trim()}".Trim();
            existing.Email = details.Email.Trim();
            existing.PhoneNumber = details.PhoneNumber.Trim();
            existing.KRAPin = details.KraPin.Trim();
            existing.DateOfBirth = details.DateOfBirth.ToString("yyyy-MM-dd");
            existing.Gender = gender;
            existing.Citizenship = details.Citizenship.Trim();
            existing.Residency = details.Residency.Trim();
            existing.Occupation = details.Occupation.Trim();
            if (!string.IsNullOrWhiteSpace(referralCode) && string.IsNullOrWhiteSpace(existing.RefferalCode))
                existing.RefferalCode = referralCode.Trim();

            await _akiba.SaveChangesAsync(cancellationToken);
            await PersistExtendedKycAsync(existing.Id, details, cancellationToken);
            await EnsurePartnerCustomerLinkAsync(existing.Id, partner, existing.MemberNumber, cancellationToken);
            return existing.Id;
        }

        var customerId = Guid.NewGuid();
        var accountUserId = await EnsurePartnerAccountAsync(details, cancellationToken);
        var fullname = $"{details.FirstName.Trim()} {details.LastName.Trim()}".Trim();
        var memberNo = await GenerateMemberNumberAsync(cancellationToken);
        var generatedReferral = referralCode?.Trim();
        if (string.IsNullOrWhiteSpace(generatedReferral))
            generatedReferral = await GenerateReferralCodeAsync(cancellationToken);

        const string addCustomerQuery = """
            INSERT INTO [dbo].[Customers]
            ([Id],[Fullname],[CustomerType],[DateOfBirth],[Email],[PhoneNumber],[KRAPin],[AddressLine1],
             [Created],[CreatedBy],[CreatedFromIP],[Approved],[Confirmed],[ApprovalStatus],[Firstname],
             [Lastname],[MemberNumber],[Gender],[Idnumber],[Residency],[CountryCode],[RefferalCode],
             [RegisterChannel],[UserId],[Citizenship],[Occupation])
            VALUES
            (@Id,@Fullname,@CustomerType,@DateOfBirth,@Email,@PhoneNumber,@KRAPin,@AddressLine1,
             GETDATE(),@CreatedBy,@CreatedFromIP,@Approved,@Confirmed,@ApprovalStatus,@Firstname,
             @Lastname,@MemberNumber,@Gender,@Idnumber,@Residency,@CountryCode,@RefferalCode,
             @RegisterChannel,@UserId,@Citizenship,@Occupation)
            """;

        await _akiba.Connection.ExecuteAsync(addCustomerQuery, new
        {
            Id = customerId,
            Fullname = fullname,
            CustomerType = (int)CustomerType.Individual,
            DateOfBirth = details.DateOfBirth.ToString("yyyy-MM-dd"),
            Email = details.Email.Trim(),
            PhoneNumber = details.PhoneNumber.Trim(),
            KRAPin = details.KraPin.Trim(),
            AddressLine1 = string.Empty,
            CreatedBy = "API",
            CreatedFromIP = ipAddress ?? string.Empty,
            Approved = false,
            Confirmed = false,
            ApprovalStatus = 0,
            Firstname = details.FirstName.Trim(),
            Lastname = details.LastName.Trim(),
            MemberNumber = memberNo,
            Gender = (int)gender,
            Idnumber = idNumber,
            Residency = details.Residency.Trim(),
            CountryCode = "254",
            RefferalCode = generatedReferral,
            RegisterChannel = (int)RegistrationChannel.API,
            UserId = accountUserId,
            Citizenship = details.Citizenship.Trim(),
            Occupation = details.Occupation.Trim()
        });

        await PersistExtendedKycAsync(customerId, details, cancellationToken);
        await EnsurePartnerCustomerLinkAsync(customerId, partner, memberNo, cancellationToken);
        return customerId;
    }

    private async Task<string> EnsurePartnerAccountAsync(
        FlexiFuturePlusCustomerDetailsDto details,
        CancellationToken cancellationToken)
    {
        try
        {
            string userId;
            do
            {
                userId = Guid.NewGuid().ToString();
            }
            while (await _akiba.Connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM [dbo].[Accounts] WHERE [Id] = @Id",
                new { Id = userId }) > 0);

            var phoneDigits = LastNineDigits(details.PhoneNumber);
            if (phoneDigits.Length < 9)
                throw new InvalidOperationException("customerDetails.phoneNumber must contain at least 9 digits.");

            const string insertAccountQuery = """
                INSERT INTO Accounts
                    (Id, Email, PhoneNumber, UserName, FirstName, LastName, PinSalt, PinHash,
                     PhoneNumberConfirmed, EmailConfirmed, IsActive, Created, CreatedBy, Deleted,
                     TwoFactorEnabled, LockoutEnabled, AcountVerified, ConfirmedOn, RegisterChannel, SecurityStamp)
                VALUES
                    (@Id, @Email, @PhoneNumber, @UserName, @FirstName, @LastName, @PinSalt, @PinHash,
                     @PhoneNumberConfirmed, @EmailConfirmed, @IsActive, @Created, @CreatedBy, @Deleted,
                     '0', '0', '0', GETDATE(), @RegisterChannel, NEWID())
                """;

            await _akiba.Connection.ExecuteAsync(insertAccountQuery, new
            {
                Id = userId,
                Email = details.Email.Trim(),
                PhoneNumber = phoneDigits,
                UserName = details.Email.Trim(),
                FirstName = details.FirstName.Trim(),
                LastName = details.LastName.Trim(),
                PinSalt = string.Empty,
                PinHash = string.Empty,
                PhoneNumberConfirmed = false,
                EmailConfirmed = false,
                IsActive = true,
                Created = DateTime.UtcNow,
                CreatedBy = "API",
                Deleted = (DateTime?)null,
                RegisterChannel = (int)RegistrationChannel.Web
            });

            await _akiba.customerRoles.AddAsync(new CustomerRoles
            {
                RoleId = CustomerRoleId.customer,
                UserId = userId
            });
            await _akiba.SaveChangesAsync(cancellationToken);

            return userId;
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException("Unable to create customer account.", ex);
        }
    }

    private async Task EnsurePartnerCustomerLinkAsync(
        Guid customerId,
        PartnerContext partner,
        string? memberRef,
        CancellationToken cancellationToken)
    {
        try
        {
            const string existsQuery = """
                SELECT COUNT(1)
                FROM dbo.PartnerCustomers
                WHERE CustomerId = @CustomerId AND PartnerCode = @PartnerCode
                """;

            if (_akiba.Connection.State != System.Data.ConnectionState.Open)
                _akiba.Connection.Open();

            var exists = await _akiba.Connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    existsQuery,
                    new { CustomerId = customerId.ToString(), PartnerCode = partner.PartnerCode },
                    cancellationToken: cancellationToken));

            if (exists > 0)
                return;

            const string addCustomerPartnerQuery = """
                INSERT INTO [dbo].[PartnerCustomers]
                ([CustomerId],[PartnerId],[PartnerCode],[PartnerName],[AgentCode],[Created],[MemberRef])
                VALUES (@CustomerId,@PartnerId,@PartnerCode,@PartnerName,@AgentCode,@Created,@MemberRef)
                """;

            await _akiba.Connection.ExecuteAsync(addCustomerPartnerQuery, new
            {
                CustomerId = customerId.ToString(),
                PartnerId = partner.PartnerId,
                PartnerCode = partner.PartnerCode,
                PartnerName = partner.PartnerName,
                AgentCode = string.Empty,
                Created = DateTime.Now,
                MemberRef = memberRef ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            // PartnerCustomers is optional in some Akiba environments; partner origin is on quote/policy.
            _logger.LogWarning(
                ex,
                "Skipping PartnerCustomers link for customer {CustomerId} and partner {PartnerCode}.",
                customerId,
                partner.PartnerCode);
        }
    }

    private async Task<FlexiFuturePolicy> EnsurePolicyAsync(
        FlexiFuturePlusOnboardingRequestDto request,
        FlexiFutureQuote quote,
        Guid customerId,
        PartnerContext partner,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken)
    {
        var policy = await _akiba.FlexiFuturePolicies
            .FirstOrDefaultAsync(p => p.QuoteId == request.QuoteId, cancellationToken);

        if (policy != null)
        {
            policy.CallbackUrl = request.CallbackUrl ?? policy.CallbackUrl;
            policy.UpdatedAt = DateTime.UtcNow;
            return policy;
        }

        var now = DateTime.UtcNow;
        policy = new FlexiFuturePolicy
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            QuoteId = quote.Id,
            PolicyStatus = FlexiFuturePolicyStatuses.Draft,
            OnboardingStep = FlexiFutureOnboardingSteps.KYC,
            Source = PartnerApiSource,
            PartnerCode = partner.PartnerCode,
            CallbackUrl = request.CallbackUrl,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = "API"
        };

        _akiba.FlexiFuturePolicies.Add(policy);

        var fund = new FlexiFund
        {
            Id = Guid.NewGuid(),
            PolicyId = policy.Id.ToString(),
            Status = FlexiFuturePolicyStatuses.Draft,
            CalculationMode = quote.CalculationMode,
            IssueDate = quote.IssueDate,
            PolicyTerm = quote.PolicyTerm,
            Frequency = quote.Frequency,
            DeathBenefitPct = quote.DeathBenefitPct,
            MaturityBenefitPayments = quote.MaturityBenefitPayments,
            TargetPremium = quote.TargetPremium,
            TargetSumAssured = quote.TargetSumAssured,
            SumAssured = quote.SumAssured,
            MonthlyEquivalent = quote.MonthlyEquivalent,
            SavingsPremium = quote.SavingsPremium,
            RidersTotal = quote.RidersTotal,
            Phcl = quote.Phcl,
            TotalPremiumPayable = quote.TotalPremiumPayable,
            MaturityDate = quote.MaturityDate,
            RequiresApproval = quote.RequiresApproval,
            Created = DateTimeOffset.UtcNow,
            CreatedBy = customerId.ToString(),
            CreatedFromIP = Truncate(ipAddress, 100),
            CreatedFromBrowser = Truncate(browser, 1000)
        };
        _akiba.FlexiFunds.Add(fund);

        return policy;
    }

    private async Task SaveFamilyMembersAsync(
        FlexiFuturePolicy policy,
        List<FlexiFuturePlusFamilyMemberDto> familyMembers,
        CancellationToken cancellationToken)
    {
        if (familyMembers.Count == 0)
            return;

        var quote = await _akiba.FlexiFutureQuotes
            .AsNoTracking()
            .Include(q => q.Spouses)
            .FirstOrDefaultAsync(q => q.Id == policy.QuoteId, cancellationToken)
            ?? throw new InvalidOperationException($"Quote '{policy.QuoteId}' was not found.");

        await ValidateFamilyMembersAsync(familyMembers, quote, cancellationToken);

        var customer = await _akiba.customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == policy.CustomerId, cancellationToken);

        var mainGender = ResolveMainLifeGenderForSpouseDerivation(customer, quote);
        var derivedSpouseGender = DeriveSpouseGender(mainGender);

        var existing = await _akiba.FlexiFutureFamilyMembers
            .Where(f => f.PolicyId == policy.Id)
            .ToListAsync(cancellationToken);
        _akiba.FlexiFutureFamilyMembers.RemoveRange(existing);

        var now = DateTime.UtcNow;
        foreach (var member in familyMembers)
        {
            var memberGender = IsSpouseFamilyContext(member.Context)
                ? derivedSpouseGender
                : member.Gender?.Trim();

            _akiba.FlexiFutureFamilyMembers.Add(new FlexiFutureFamilyMember
            {
                Id = Guid.NewGuid(),
                PolicyId = policy.Id,
                OtherNames = member.OtherNames.Trim(),
                Surname = member.Surname.Trim(),
                Gender = memberGender,
                EmailAddress = member.EmailAddress?.Trim(),
                PhoneNumber = member.PhoneNumber?.Trim(),
                IDNumber = member.IdNumber?.Trim(),
                DateOfBirth = member.DateOfBirth,
                Relationship = NormalizeFamilyRelationship(member.Relationship),
                CreatedAt = now,
                UpdatedAt = now,
                AddedBy = member.Context.Trim()
            });
        }
    }

    private async Task ValidateFamilyMembersAsync(
        IReadOnlyList<FlexiFuturePlusFamilyMemberDto> familyMembers,
        FlexiFutureQuote quote,
        CancellationToken cancellationToken)
    {
        var spouseContexts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var childContexts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var pack = await GetActiveRatesAsync(cancellationToken);
        var minEntryAge = pack.Settings.MinEntryAge;
        var maxEntryAge = pack.Settings.MaxEntryAge;
        var maxSpouses = pack.Settings.MaxSpouses;
        var issueDate = quote.IssueDate;

        foreach (var member in familyMembers)
        {
            if (string.IsNullOrWhiteSpace(member.Context))
                throw new InvalidOperationException("familyMembers[].context is required.");
            if (string.IsNullOrWhiteSpace(member.OtherNames))
                throw new InvalidOperationException("familyMembers[].otherNames is required.");
            if (string.IsNullOrWhiteSpace(member.Surname))
                throw new InvalidOperationException("familyMembers[].surname is required.");
            if (string.IsNullOrWhiteSpace(member.Relationship))
                throw new InvalidOperationException("familyMembers[].relationship is required.");

            var relationship = NormalizeFamilyRelationship(member.Relationship);
            var context = member.Context.Trim();
            var isSpouse = context.StartsWith("Spouse", StringComparison.OrdinalIgnoreCase);
            var isChild = context.StartsWith("Child", StringComparison.OrdinalIgnoreCase);

            if (isSpouse)
            {
                if (!string.Equals(relationship, "Spouse", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException(
                        $"familyMembers[].relationship must be 'Spouse' when context is '{context}'.");

                if (!spouseContexts.Add(context))
                    throw new InvalidOperationException($"Duplicate family member context '{context}'.");
            }
            else if (isChild)
            {
                if (!string.Equals(relationship, "Child", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException(
                        $"familyMembers[].relationship must be 'Child' when context is '{context}'.");

                if (!childContexts.Add(context))
                    throw new InvalidOperationException($"Duplicate family member context '{context}'.");
            }
            else
            {
                throw new InvalidOperationException(
                    "familyMembers[].context must be Spouse1, Spouse2, or Child1 through Child6.");
            }

            if (!member.DateOfBirth.HasValue)
                throw new InvalidOperationException($"familyMembers[].dateOfBirth is required for context '{context}'.");

            var dateOfBirth = member.DateOfBirth.Value;
            if (dateOfBirth > issueDate)
            {
                throw new InvalidOperationException(
                    $"familyMembers[].dateOfBirth for context '{context}' cannot be after the quote issue date.");
            }

            var label = $"{member.OtherNames.Trim()} {member.Surname.Trim()}".Trim();
            if (isSpouse)
            {
                var anb = ComputeAnb(dateOfBirth, issueDate);
                if (anb < minEntryAge || anb > maxEntryAge)
                {
                    throw new InvalidOperationException(
                        $"Spouse '{label}' ANB must be between {minEntryAge} and {maxEntryAge} (got {anb}).");
                }
            }
            else if (isChild)
            {
                var anb = ComputeChildAnb(dateOfBirth, issueDate);
                ValidateChildAnb(anb, label);
            }
        }

        var expectedSpouseCount = quote.Spouses.Count > 0
            ? quote.Spouses.Max(s => s.SpouseIndex)
            : 0;

        for (var i = 1; i <= expectedSpouseCount; i++)
        {
            var requiredContext = i switch
            {
                1 => FlexiFuturePartnerHealthQuestionContexts.Spouse1,
                2 => FlexiFuturePartnerHealthQuestionContexts.Spouse2,
                _ => $"Spouse{i}"
            };

            if (!spouseContexts.Contains(requiredContext))
            {
                throw new InvalidOperationException(
                    $"familyMembers must include context '{requiredContext}' to match the quote.");
            }
        }

        if (spouseContexts.Count > maxSpouses)
            throw new InvalidOperationException($"Maximum {maxSpouses} spouse family members allowed.");
        if (childContexts.Count > 6)
            throw new InvalidOperationException("Maximum 6 child family members allowed.");
    }

    private static string NormalizeFamilyRelationship(string relationship)
    {
        var normalized = relationship.Trim();
        if (string.Equals(normalized, "Spouse", StringComparison.OrdinalIgnoreCase))
            return "Spouse";
        if (string.Equals(normalized, "Child", StringComparison.OrdinalIgnoreCase))
            return "Child";

        throw new InvalidOperationException("familyMembers[].relationship must be either 'Spouse' or 'Child'.");
    }

    private async Task SaveBeneficiariesAsync(
        FlexiFuturePolicy policy,
        List<FlexiFuturePlusBeneficiaryDto> beneficiaries,
        CancellationToken cancellationToken)
    {
        if (beneficiaries.Count == 0)
            return;

        ValidateBeneficiaries(beneficiaries);

        var existing = await _akiba.FlexiFutureBeneficiaries
            .Include(b => b.Guardians)
            .Where(b => b.PolicyId == policy.Id)
            .ToListAsync(cancellationToken);

        foreach (var beneficiary in existing)
            _akiba.FlexiFutureGuardians.RemoveRange(beneficiary.Guardians);
        _akiba.FlexiFutureBeneficiaries.RemoveRange(existing);

        var now = DateTime.UtcNow;
        foreach (var beneficiary in beneficiaries)
        {
            var entityId = Guid.NewGuid();
            var entity = new FlexiFutureBeneficiary
            {
                Id = entityId,
                PolicyId = policy.Id,
                OtherNames = beneficiary.OtherNames.Trim(),
                Surname = beneficiary.Surname.Trim(),
                Gender = beneficiary.Gender?.Trim(),
                EmailAddress = beneficiary.EmailAddress?.Trim(),
                PhoneNumber = beneficiary.PhoneNumber?.Trim(),
                IDNumber = beneficiary.IdNumber?.Trim(),
                DateOfBirth = beneficiary.DateOfBirth,
                Relationship = beneficiary.Relationship?.Trim(),
                PercentageShare = beneficiary.PercentageShare,
                CreatedAt = now,
                UpdatedAt = now
            };

            if (beneficiary.Guardian != null)
            {
                var guardian = beneficiary.Guardian;
                entity.Guardians.Add(new FlexiFutureGuardian
                {
                    Id = Guid.NewGuid(),
                    BeneficiaryId = entityId,
                    OtherNames = guardian.OtherNames.Trim(),
                    Surname = guardian.Surname.Trim(),
                    Gender = guardian.Gender?.Trim(),
                    EmailAddress = guardian.EmailAddress?.Trim(),
                    PhoneNumber = guardian.PhoneNumber?.Trim(),
                    IDNumber = guardian.IdNumber?.Trim(),
                    DateOfBirth = guardian.DateOfBirth,
                    Relationship = guardian.Relationship?.Trim(),
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }

            _akiba.FlexiFutureBeneficiaries.Add(entity);
        }
    }

    private static void ValidateBeneficiaries(IReadOnlyList<FlexiFuturePlusBeneficiaryDto> beneficiaries)
    {
        var totalShare = beneficiaries.Sum(b => b.PercentageShare);
        if (Math.Abs(totalShare - 100m) > 0.01m)
        {
            throw new InvalidOperationException(
                "beneficiaries[].percentageShare values must total 100.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var beneficiary in beneficiaries)
        {
            if (beneficiary.PercentageShare <= 0 || beneficiary.PercentageShare > 100)
            {
                throw new InvalidOperationException(
                    "Each beneficiaries[].percentageShare must be greater than 0 and at most 100.");
            }

            if (beneficiary.DateOfBirth > today)
            {
                throw new InvalidOperationException(
                    "beneficiaries[].dateOfBirth cannot be in the future.");
            }

            if (GetAgeInYears(beneficiary.DateOfBirth, today) < 18)
            {
                ValidateMinorBeneficiaryGuardian(beneficiary, today);
            }
            else if (beneficiary.Guardian != null)
            {
                ValidateGuardianAge(beneficiary.Guardian, today);
            }
        }
    }

    private static void ValidateMinorBeneficiaryGuardian(
        FlexiFuturePlusBeneficiaryDto beneficiary,
        DateOnly asOfDate)
    {
        var guardian = beneficiary.Guardian;
        if (guardian == null
            || string.IsNullOrWhiteSpace(guardian.OtherNames)
            || string.IsNullOrWhiteSpace(guardian.Surname))
        {
            var name = $"{beneficiary.OtherNames.Trim()} {beneficiary.Surname.Trim()}".Trim();
            throw new InvalidOperationException(
                $"beneficiaries[].guardian with otherNames and surname is required for minor beneficiary '{name}'.");
        }

        ValidateGuardianAge(guardian, asOfDate);
    }

    private static void ValidateGuardianAge(FlexiFuturePlusGuardianDto guardian, DateOnly asOfDate)
    {
        if (!guardian.DateOfBirth.HasValue)
            throw new InvalidOperationException("beneficiaries[].guardian.dateOfBirth is required.");

        if (guardian.DateOfBirth.Value > asOfDate)
        {
            throw new InvalidOperationException(
                "beneficiaries[].guardian.dateOfBirth cannot be in the future.");
        }

        if (GetAgeInYears(guardian.DateOfBirth.Value, asOfDate) < 18)
        {
            throw new InvalidOperationException(
                "beneficiaries[].guardian must be at least 18 years old.");
        }
    }

    private static int GetAgeInYears(DateOnly dateOfBirth, DateOnly asOfDate)
    {
        var age = asOfDate.Year - dateOfBirth.Year;
        if (dateOfBirth > asOfDate.AddYears(-age))
            age--;

        return age;
    }

    private async Task SaveHealthAnswersAsync(
        FlexiFuturePolicy policy,
        FlexiFutureQuote quote,
        FlexiFuturePartnerHealthQuestionsInputDto healthQuestions,
        CancellationToken cancellationToken)
    {
        if (!HasSubmittedHealthQuestions(healthQuestions))
            return;

        var requiredContexts = GetRequiredHealthContexts(quote);
        ValidateSubmittedHealthContexts(healthQuestions, requiredContexts);

        var questions = await _akiba.HealthQuestionsLibrary
            .AsNoTracking()
            .Where(q => q.IsActive)
            .ToListAsync(cancellationToken);

        var questionByKey = questions
            .GroupBy(q => q.QuestionKey, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        var spouseContextIds = await ResolveSpouseContextIdsAsync(policy, cancellationToken);
        ValidateNatureNarration(healthQuestions);
        ValidateWeightTrendAnswers(healthQuestions, requiredContexts);

        var internalAnswers = ConvertPartnerHealthAnswers(
            healthQuestions,
            policy,
            requiredContexts,
            spouseContextIds,
            questionByKey);

        AppendNatureNarrationAnswer(
            internalAnswers,
            healthQuestions.Narration,
            policy,
            questionByKey);

        ValidateRequiredPartnerHealthAnswers(
            policy,
            questions,
            requiredContexts,
            spouseContextIds,
            internalAnswers);

        var existing = await _akiba.FlexiFutureHealthInfo
            .Where(h => h.PolicyId == policy.Id)
            .ToListAsync(cancellationToken);
        _akiba.FlexiFutureHealthInfo.RemoveRange(existing);

        var now = DateTime.UtcNow;
        foreach (var answer in internalAnswers)
        {
            if (!questionByKey.TryGetValue(answer.QuestionKey, out var question))
                throw new InvalidOperationException($"Unknown health question '{answer.QuestionKey}'.");

            if (string.Equals(question.Category, FlexiFutureHealthQuestionCategories.Hazardous, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    $"Question '{answer.QuestionKey}' belongs to hazardous. Submit it under hazardous[] instead.");

            if (string.Equals(question.QuestionType, FlexiFutureHealthQuestionTypes.Group, StringComparison.OrdinalIgnoreCase))
                continue;

            ValidateHealthAnswer(question, answer.Answer);

            _akiba.FlexiFutureHealthInfo.Add(new FlexiFutureHealthInfo
            {
                Id = Guid.NewGuid(),
                PolicyId = policy.Id,
                QuestionId = question.Id,
                Question = question.Question,
                QuestionKey = question.QuestionKey,
                Answer = answer.Answer,
                Context = answer.Context,
                ContextId = answer.ContextId,
                CreatedAt = now,
                UpdatedAt = now
            });
        }
    }

    private async Task SaveOccupationHazardAnswersAsync(
        FlexiFuturePolicy policy,
        FlexiFuturePartnerHazardousDto hazardous,
        CancellationToken cancellationToken)
    {
        if (!HasSubmittedHazardous(hazardous))
            return;

        ValidateHazardousNarration(hazardous);

        var questions = await _akiba.HealthQuestionsLibrary
            .AsNoTracking()
            .Where(q => q.IsActive)
            .ToListAsync(cancellationToken);

        var questionByKey = questions
            .GroupBy(q => q.QuestionKey, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        ValidateRequiredPartnerHazardousAnswers(hazardous);

        var existing = await _akiba.FlexiFutureOccupationHazards
            .Where(h => h.PolicyId == policy.Id)
            .ToListAsync(cancellationToken);
        _akiba.FlexiFutureOccupationHazards.RemoveRange(existing);

        var now = DateTime.UtcNow;
        if (HasPartnerJsonAnswer(hazardous.HazardousIntent))
        {
            if (!questionByKey.TryGetValue(
                    FlexiFutureHealthQuestionKeys.HazardousIntent,
                    out var intentQuestion))
            {
                throw new InvalidOperationException("Hazardous question 'hazardousIntent' is not configured.");
            }

            var serializedAnswer = SerializePartnerAnswer(hazardous.HazardousIntent);
            ValidateHealthAnswer(intentQuestion, serializedAnswer);

            _akiba.FlexiFutureOccupationHazards.Add(new FlexiFutureOccupationHazard
            {
                Id = Guid.NewGuid(),
                PolicyId = policy.Id,
                QuestionId = intentQuestion.Id,
                Question = intentQuestion.Question,
                QuestionKey = intentQuestion.QuestionKey,
                Answer = serializedAnswer,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        if (!string.IsNullOrWhiteSpace(hazardous.Narration))
        {
            if (!questionByKey.TryGetValue(
                    FlexiFutureHealthQuestionKeys.HazardousIntentExplanation,
                    out var explanationQuestion))
            {
                throw new InvalidOperationException(
                    "Hazardous question 'hazardousIntentExplanation' is not configured.");
            }

            _akiba.FlexiFutureOccupationHazards.Add(new FlexiFutureOccupationHazard
            {
                Id = Guid.NewGuid(),
                PolicyId = policy.Id,
                QuestionId = explanationQuestion.Id,
                Question = explanationQuestion.Question,
                QuestionKey = explanationQuestion.QuestionKey,
                Answer = hazardous.Narration.Trim(),
                CreatedAt = now,
                UpdatedAt = now
            });
        }
    }

    private static string DetermineOnboardingStep(
        FlexiFuturePlusOnboardingRequestDto request,
        FlexiFuturePolicy policy)
    {
        if (HasSubmittedConsent(request.Consent))
            return FlexiFutureOnboardingSteps.Complete;
        if (request.Beneficiaries.Count > 0)
            return FlexiFutureOnboardingSteps.Beneficiaries;
        if (HasSubmittedHazardous(request.Hazardous))
            return FlexiFutureOnboardingSteps.OccupationHazards;
        if (HasSubmittedHealthQuestions(request.HealthQuestions))
            return FlexiFutureOnboardingSteps.HealthDeclaration;
        if (request.FamilyMembers.Count > 0)
            return FlexiFutureOnboardingSteps.FamilyMembers;
        if (request.CustomerDetails != null)
            return FlexiFutureOnboardingSteps.FamilyMembers;
        return policy.OnboardingStep;
    }

    private static void ValidateCustomerDetails(FlexiFuturePlusCustomerDetailsDto details, FlexiFutureQuote quote)
    {
        if (string.IsNullOrWhiteSpace(details.FirstName))
            throw new InvalidOperationException("customerDetails.firstName is required.");
        if (string.IsNullOrWhiteSpace(details.LastName))
            throw new InvalidOperationException("customerDetails.lastName is required.");
        if (string.IsNullOrWhiteSpace(details.IdNumber))
            throw new InvalidOperationException("customerDetails.idNumber is required.");
        if (string.IsNullOrWhiteSpace(details.PhoneNumber))
            throw new InvalidOperationException("customerDetails.phoneNumber is required.");
        if (string.IsNullOrWhiteSpace(details.Email))
            throw new InvalidOperationException("customerDetails.email is required.");
        if (string.IsNullOrWhiteSpace(details.KraPin))
            throw new InvalidOperationException("customerDetails.kraPin is required.");
        if (details.KraPin.Length > 15)
            throw new InvalidOperationException("customerDetails.kraPin must not exceed 15 characters.");
        if (string.IsNullOrWhiteSpace(details.Citizenship))
            throw new InvalidOperationException("customerDetails.citizenship is required.");
        if (string.IsNullOrWhiteSpace(details.Residency))
            throw new InvalidOperationException("customerDetails.residency is required.");
        if (string.IsNullOrWhiteSpace(details.Occupation))
            throw new InvalidOperationException("customerDetails.occupation is required.");
        if (string.IsNullOrWhiteSpace(details.MonthlyIncome))
            throw new InvalidOperationException("customerDetails.monthlyIncome is required.");
        if (string.IsNullOrWhiteSpace(details.Gender))
            throw new InvalidOperationException("customerDetails.gender is required.");

        if (details.DateOfBirth != quote.DateOfBirth)
            throw new InvalidOperationException("customerDetails.dateOfBirth must match the quote client date of birth.");

        if (!string.IsNullOrWhiteSpace(quote.Gender)
            && !string.Equals(details.Gender.Trim(), quote.Gender.Trim(), StringComparison.OrdinalIgnoreCase)
            && !string.Equals(ParseGender(details.Gender).ToString(), quote.Gender.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("customerDetails.gender must match the quote client gender.");
        }

        if (IsUsResidencyOrCitizenship(details.Citizenship, details.Residency))
        {
            if (string.IsNullOrWhiteSpace(details.UsAddress))
                throw new InvalidOperationException("customerDetails.usAddress is required when residency or citizenship is US.");
            if (string.IsNullOrWhiteSpace(details.TaxIdNumber))
                throw new InvalidOperationException("customerDetails.taxIdNumber is required when residency or citizenship is US.");
            if (!details.TaxRegisteredOtherCountry.HasValue)
                throw new InvalidOperationException("customerDetails.taxRegisteredOtherCountry is required when residency or citizenship is US.");
        }

        if (!string.IsNullOrWhiteSpace(details.RefferalCode) && details.RefferalCode.Length > 100)
            throw new InvalidOperationException("customerDetails.refferalCode must not exceed 100 characters.");
    }

    private static void ApplyReferralToQuote(FlexiFutureQuote quote, string? referralCode)
    {
        if (string.IsNullOrWhiteSpace(referralCode))
            return;

        if (referralCode.Length > 100)
            throw new InvalidOperationException("refferalCode must not exceed 100 characters.");

        quote.RefferalCode = referralCode.Trim();
    }

    private static void SyncQuoteClientFromCustomerDetails(FlexiFutureQuote quote, FlexiFuturePlusCustomerDetailsDto details)
    {
        quote.ClientName = $"{details.FirstName.Trim()} {details.LastName.Trim()}".Trim();
        quote.DateOfBirth = details.DateOfBirth;
        quote.Gender = details.Gender.Trim();
        quote.PhoneNumber = details.PhoneNumber.Trim();
        quote.Email = details.Email.Trim();
        quote.IdNumber = details.IdNumber.Trim();
    }

    private static bool HasNonCustomerSections(FlexiFuturePlusOnboardingRequestDto request)
    {
        return request.FamilyMembers.Count > 0
               || request.Beneficiaries.Count > 0
               || HasSubmittedHealthQuestions(request.HealthQuestions)
               || HasSubmittedHazardous(request.Hazardous)
               || HasSubmittedConsent(request.Consent);
    }

    private static bool HasSubmittedConsent(FlexiFuturePlusConsentDto? consent)
    {
        if (consent == null)
            return false;

        return !string.IsNullOrWhiteSpace(consent.Otp)
               || HasSubmittedSignature(consent.Signature);
    }

    private static bool HasSubmittedSignature(FileUploadDTO? signature)
    {
        return signature != null && !string.IsNullOrWhiteSpace(signature.Data);
    }

    private static bool HasSubmittedHealthQuestions(FlexiFuturePartnerHealthQuestionsInputDto? healthQuestions)
    {
        if (healthQuestions == null)
            return false;

        return healthQuestions.Main.Count > 0
               || healthQuestions.Spouse1.Count > 0
               || healthQuestions.Spouse2.Count > 0
               || !string.IsNullOrWhiteSpace(healthQuestions.Narration);
    }

    private static bool HasSubmittedHazardous(FlexiFuturePartnerHazardousDto? hazardous)
    {
        if (hazardous == null)
            return false;

        return HasPartnerJsonAnswer(hazardous.HazardousIntent)
               || !string.IsNullOrWhiteSpace(hazardous.Narration);
    }

    private static bool HasPartnerJsonAnswer(JsonElement answer)
    {
        if (answer.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
            return false;

        return !string.IsNullOrWhiteSpace(SerializePartnerAnswer(answer));
    }

    private static DAL.Model.Pensioner.Gender ParseGender(string gender)
    {
        if (Enum.TryParse<DAL.Model.Pensioner.Gender>(gender, true, out var parsed))
            return parsed;

        if (gender.Equals("M", StringComparison.OrdinalIgnoreCase))
            return DAL.Model.Pensioner.Gender.Male;
        if (gender.Equals("F", StringComparison.OrdinalIgnoreCase))
            return DAL.Model.Pensioner.Gender.Female;

        throw new InvalidOperationException($"Invalid gender value '{gender}'.");
    }

    private static string DeriveSpouseGender(string? mainGender)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(mainGender))
            {
                throw new InvalidOperationException(
                    "Main life gender is required before spouse gender can be derived.");
            }

            return ParseGender(mainGender) == DAL.Model.Pensioner.Gender.Male ? "Female" : "Male";
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to derive spouse gender.", ex);
        }
    }

    private static bool IsSpouseFamilyContext(string? context)
    {
        try
        {
            return !string.IsNullOrWhiteSpace(context)
                   && context.Trim().StartsWith("Spouse", StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to evaluate spouse family context.", ex);
        }
    }

    private static string ResolveMainLifeGenderForSpouseDerivation(
        PensionCustomer? customer,
        FlexiFutureQuote quote)
    {
        try
        {
            if (customer != null && customer.Gender != default)
                return customer.Gender.ToString();

            if (!string.IsNullOrWhiteSpace(quote.Gender))
                return quote.Gender.Trim();

            throw new InvalidOperationException(
                "Main life gender must be set on the quote or customer before saving spouse family members.");
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to resolve main life gender.", ex);
        }
    }

    private static bool IsUsResidencyOrCitizenship(string? citizenship, string? residency)
    {
        return IsUsValue(citizenship) || IsUsValue(residency);
    }

    private static bool IsUsValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalized = value.Trim();
        return normalized.Equals("US", StringComparison.OrdinalIgnoreCase)
               || normalized.Equals("USA", StringComparison.OrdinalIgnoreCase)
               || normalized.Equals("United States", StringComparison.OrdinalIgnoreCase);
    }

    private async Task PersistExtendedKycAsync(
        Guid customerId,
        FlexiFuturePlusCustomerDetailsDto details,
        CancellationToken cancellationToken)
    {
        const string updateQuery = """
            UPDATE [dbo].[Customers]
            SET [MonthlyIncome] = @MonthlyIncome,
                [EmployerName] = @EmployerName,
                [BusinessName] = @BusinessName,
                [USAddress] = @USAddress,
                [TaxIdNumber] = @TaxIdNumber,
                [TaxRegisteredOtherCountry] = @TaxRegisteredOtherCountry
            WHERE [Id] = @CustomerId
            """;

        if (_akiba.Connection.State != System.Data.ConnectionState.Open)
            _akiba.Connection.Open();

        await _akiba.Connection.ExecuteAsync(new CommandDefinition(
            updateQuery,
            new
            {
                CustomerId = customerId,
                MonthlyIncome = details.MonthlyIncome.Trim(),
                EmployerName = details.EmployerName?.Trim() ?? string.Empty,
                BusinessName = details.BusinessName?.Trim() ?? string.Empty,
                USAddress = details.UsAddress?.Trim() ?? string.Empty,
                TaxIdNumber = details.TaxIdNumber?.Trim() ?? string.Empty,
                TaxRegisteredOtherCountry = details.TaxRegisteredOtherCountry ?? false
            },
            cancellationToken: cancellationToken));
    }

    private async Task<CustomerKycExtendedFields?> LoadExtendedKycFieldsAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        const string query = """
            SELECT [MonthlyIncome], [EmployerName], [BusinessName], [USAddress], [TaxIdNumber], [TaxRegisteredOtherCountry]
            FROM [dbo].[Customers]
            WHERE [Id] = @CustomerId
            """;

        if (_akiba.Connection.State != System.Data.ConnectionState.Open)
            _akiba.Connection.Open();

        return await _akiba.Connection.QueryFirstOrDefaultAsync<CustomerKycExtendedFields>(
            new CommandDefinition(query, new { CustomerId = customerId }, cancellationToken: cancellationToken));
    }

    private async Task<bool> IsCustomerDetailsCompleteAsync(
        PensionCustomer customer,
        CancellationToken cancellationToken)
    {
        var extended = await LoadExtendedKycFieldsAsync(customer.Id, cancellationToken);
        var hasName = !string.IsNullOrWhiteSpace(customer.Firstname)
                      || !string.IsNullOrWhiteSpace(customer.Lastname)
                      || !string.IsNullOrWhiteSpace(customer.Fullname);

        return hasName
               && !string.IsNullOrWhiteSpace(customer.Idnumber)
               && !string.IsNullOrWhiteSpace(customer.Email)
               && !string.IsNullOrWhiteSpace(customer.PhoneNumber)
               && !string.IsNullOrWhiteSpace(customer.KRAPin)
               && !string.IsNullOrWhiteSpace(customer.Citizenship)
               && !string.IsNullOrWhiteSpace(customer.Residency)
               && !string.IsNullOrWhiteSpace(customer.Occupation)
               && !string.IsNullOrWhiteSpace(extended?.MonthlyIncome);
    }

    private async Task<FlexiFuturePlusCustomerDetailsDto?> MapCustomerDetailsFromCustomerAsync(
        PensionCustomer customer,
        CancellationToken cancellationToken)
    {
        if (customer == null)
            return null;

        var extended = await LoadExtendedKycFieldsAsync(customer.Id, cancellationToken);
        DateOnly dateOfBirth = default;
        if (!string.IsNullOrWhiteSpace(customer.DateOfBirth)
            && DateOnly.TryParse(customer.DateOfBirth, out var parsedDob))
        {
            dateOfBirth = parsedDob;
        }

        return new FlexiFuturePlusCustomerDetailsDto
        {
            FirstName = customer.Firstname ?? string.Empty,
            LastName = customer.Lastname ?? string.Empty,
            IdNumber = customer.Idnumber ?? string.Empty,
            DateOfBirth = dateOfBirth,
            Gender = customer.Gender.ToString(),
            PhoneNumber = customer.PhoneNumber ?? string.Empty,
            Email = customer.Email ?? string.Empty,
            KraPin = customer.KRAPin ?? string.Empty,
            Citizenship = customer.Citizenship ?? string.Empty,
            Residency = customer.Residency ?? string.Empty,
            Occupation = customer.Occupation ?? string.Empty,
            MonthlyIncome = extended?.MonthlyIncome ?? string.Empty,
            EmployerName = extended?.EmployerName,
            BusinessName = extended?.BusinessName,
            UsAddress = extended?.USAddress,
            TaxIdNumber = extended?.TaxIdNumber,
            TaxRegisteredOtherCountry = extended?.TaxRegisteredOtherCountry,
            RefferalCode = customer.RefferalCode
        };
    }

    private async Task<FlexiFuturePlusOnboardingStateDto> MapOnboardingStateAsync(
        FlexiFuturePolicy policy,
        FlexiFutureQuote quote,
        PensionCustomer? customer,
        FlexiFuturePlusOnboardingRequestDto? request,
        CancellationToken cancellationToken)
    {
        var familyCount = await _akiba.FlexiFutureFamilyMembers
            .AsNoTracking()
            .CountAsync(f => f.PolicyId == policy.Id, cancellationToken);
        var beneficiaryCount = await _akiba.FlexiFutureBeneficiaries
            .AsNoTracking()
            .CountAsync(b => b.PolicyId == policy.Id, cancellationToken);
        var healthCount = await _akiba.FlexiFutureHealthInfo
            .AsNoTracking()
            .CountAsync(h => h.PolicyId == policy.Id, cancellationToken);
        var hazardCount = await _akiba.FlexiFutureOccupationHazards
            .AsNoTracking()
            .CountAsync(h => h.PolicyId == policy.Id, cancellationToken);

        var customerDetailsComplete = customer != null
            && await IsCustomerDetailsCompleteAsync(customer, cancellationToken);

        FlexiFuturePlusCustomerDetailsDto? customerDetails = request?.CustomerDetails;
        if (customerDetails == null && customer != null)
            customerDetails = await MapCustomerDetailsFromCustomerAsync(customer, cancellationToken);

        return new FlexiFuturePlusOnboardingStateDto
        {
            PolicyId = policy.Id,
            QuoteId = policy.QuoteId,
            CustomerId = policy.CustomerId,
            PolicyStatus = policy.PolicyStatus,
            OnboardingStep = policy.OnboardingStep,
            CustomerDetailsComplete = customerDetailsComplete,
            PolicyNo = policy.PolicyNo,
            IsApproved = policy.IsApproved,
            InitialPaymentComplete = policy.InitialPaymentComplete,
            PartnerCode = policy.PartnerCode ?? quote.PartnerCode,
            CallbackUrl = policy.CallbackUrl,
            CustomerDetails = customerDetails,
            FamilyMembers = await MapStoredFamilyMembersAsync(policy, cancellationToken),
            HealthQuestions = await MapStoredHealthAnswersByContextAsync(
                policy,
                cancellationToken),
            Hazardous = await MapStoredHazardousAnswersAsync(
                policy,
                cancellationToken),
            Beneficiaries = await MapStoredBeneficiariesAsync(policy, cancellationToken),
            Consent = MapStoredConsent(policy),
            SectionStatus = BuildSectionStatus(
                policy,
                customerDetailsComplete,
                familyCount,
                beneficiaryCount,
                healthCount,
                hazardCount)
        };
    }

    private static FlexiFuturePlusConsentStateDto? MapStoredConsent(FlexiFuturePolicy policy)
    {
        if (!string.Equals(policy.OnboardingStep, FlexiFutureOnboardingSteps.Complete, StringComparison.OrdinalIgnoreCase)
            && !policy.CompletedAt.HasValue)
        {
            return null;
        }

        return new FlexiFuturePlusConsentStateDto
        {
            CompletionMethod = policy.CompletionMethod,
            CompletedAt = policy.CompletedAt
        };
    }

    private static FlexiFuturePlusSectionStatusDto BuildSectionStatus(
        FlexiFuturePolicy policy,
        bool customerDetailsComplete,
        int familyCount,
        int beneficiaryCount,
        int healthCount,
        int hazardCount)
    {
        var consentComplete = string.Equals(
                                policy.OnboardingStep,
                                FlexiFutureOnboardingSteps.Complete,
                                StringComparison.OrdinalIgnoreCase)
                            || policy.CompletedAt.HasValue;

        return new FlexiFuturePlusSectionStatusDto
        {
            CustomerDetails = customerDetailsComplete ? "Complete" : "Incomplete",
            FamilyMembers = familyCount > 0 ? "Complete" : "NotStarted",
            HealthQuestions = healthCount > 0 ? "Complete" : "NotStarted",
            Hazardous = hazardCount > 0 ? "Complete" : "NotStarted",
            Beneficiaries = beneficiaryCount > 0 ? "Complete" : "NotStarted",
            Consent = consentComplete ? "Complete" : "NotStarted"
        };
    }

    private sealed class CustomerKycExtendedFields
    {
        public string? MonthlyIncome { get; set; }
        public string? EmployerName { get; set; }
        public string? BusinessName { get; set; }
        public string? USAddress { get; set; }
        public string? TaxIdNumber { get; set; }
        public bool? TaxRegisteredOtherCountry { get; set; }
    }

    private static List<string> GetRequiredHealthContexts(FlexiFutureQuote quote)
    {
        var contexts = new List<string> { FlexiFuturePartnerHealthQuestionContexts.Main };
        var spouseCount = quote.Spouses.Count > 0
            ? quote.Spouses.Max(s => s.SpouseIndex)
            : 0;

        if (spouseCount >= 1)
            contexts.Add(FlexiFuturePartnerHealthQuestionContexts.Spouse1);
        if (spouseCount >= 2)
            contexts.Add(FlexiFuturePartnerHealthQuestionContexts.Spouse2);

        return contexts;
    }

    private static readonly string[] NatureTriggerQuestionKeys =
    {
        "critical",
        "claim",
        "professional",
        "minor",
        "symptoms"
    };

    private static readonly string[] WeightTrendAllowedValues =
    {
        "Increasing",
        "Decreasing"
    };

    private sealed class PartnerHealthQuestionCatalog
    {
        public List<string> Main { get; set; } = new();
        public List<string> Spouse1 { get; set; } = new();
        public List<string> Spouse2 { get; set; } = new();
        public FlexiFuturePartnerHazardousQuestionsDto Hazardous { get; set; } = new();
        public List<FlexiFutureHealthQuestionLibraryItemDto> QuestionLibrary { get; set; } = new();
    }

    private static PartnerHealthQuestionCatalog BuildPartnerHealthQuestionCatalog(
        IReadOnlyList<HealthQuestionsLibrary> healthQuestions,
        IReadOnlyList<HealthQuestionsLibrary> hazardousQuestions,
        IReadOnlyList<string> requiredContexts)
    {
        var visibleHealth = healthQuestions
            .Where(IsPartnerVisibleHealthQuestion)
            .OrderBy(q => q.QuestionOrder)
            .ToList();

        var visibleHazardous = hazardousQuestions
            .Where(IsPartnerVisibleHazardousQuestion)
            .OrderBy(q => q.QuestionOrder)
            .ToList();

        var healthQuestionKeys = BuildPartnerHealthQuestionKeyList(visibleHealth, healthQuestions);
        var libraryQuestions = visibleHealth
            .Concat(healthQuestions.Where(IsPartnerWeightTrendQuestion));

        var catalog = new PartnerHealthQuestionCatalog
        {
            Main = healthQuestionKeys,
            Hazardous = BuildPartnerHazardousQuestionsCatalog(hazardousQuestions),
            QuestionLibrary = libraryQuestions
                .GroupBy(q => q.QuestionKey, StringComparer.OrdinalIgnoreCase)
                .Select(g => MapQuestionLibraryItem(g.First()))
                .ToList()
        };

        if (requiredContexts.Any(c =>
                string.Equals(c, FlexiFuturePartnerHealthQuestionContexts.Spouse1, StringComparison.OrdinalIgnoreCase)))
        {
            catalog.Spouse1 = healthQuestionKeys;
        }

        if (requiredContexts.Any(c =>
                string.Equals(c, FlexiFuturePartnerHealthQuestionContexts.Spouse2, StringComparison.OrdinalIgnoreCase)))
        {
            catalog.Spouse2 = healthQuestionKeys;
        }

        return catalog;
    }

    private static List<string> BuildPartnerHealthQuestionKeyList(
        IReadOnlyList<HealthQuestionsLibrary> visibleHealth,
        IReadOnlyList<HealthQuestionsLibrary> allHealthQuestions)
    {
        var keys = visibleHealth.Select(q => q.QuestionKey).ToList();
        AppendPartnerWeightTrendQuestionKey(keys, allHealthQuestions);
        return keys;
    }

    private static void AppendPartnerWeightTrendQuestionKey(
        List<string> keys,
        IReadOnlyList<HealthQuestionsLibrary> allHealthQuestions)
    {
        if (keys.Any(k =>
                string.Equals(k, FlexiFutureHealthQuestionKeys.WeightTrend, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        if (!allHealthQuestions.Any(q =>
                q.IsActive
                && string.Equals(q.QuestionKey, FlexiFutureHealthQuestionKeys.WeightTrend, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        var stationaryIndex = keys.FindIndex(k =>
            string.Equals(k, FlexiFutureHealthQuestionKeys.WeightStationary, StringComparison.OrdinalIgnoreCase));

        if (stationaryIndex >= 0)
            keys.Insert(stationaryIndex + 1, FlexiFutureHealthQuestionKeys.WeightTrend);
        else
            keys.Add(FlexiFutureHealthQuestionKeys.WeightTrend);
    }

    private static FlexiFuturePartnerHazardousQuestionsDto BuildPartnerHazardousQuestionsCatalog(
        IReadOnlyList<HealthQuestionsLibrary> hazardousQuestions)
    {
        var intentQuestion = hazardousQuestions.FirstOrDefault(q =>
            string.Equals(
                q.QuestionKey,
                FlexiFutureHealthQuestionKeys.HazardousIntent,
                StringComparison.OrdinalIgnoreCase));

        var explanationQuestion = hazardousQuestions.FirstOrDefault(q =>
            string.Equals(
                q.QuestionKey,
                FlexiFutureHealthQuestionKeys.HazardousIntentExplanation,
                StringComparison.OrdinalIgnoreCase));

        return new FlexiFuturePartnerHazardousQuestionsDto
        {
            HazardousIntent = new FlexiFuturePartnerHazardousFieldCatalogDto
            {
                Question = intentQuestion?.Question ?? string.Empty,
                AllowedAnswers = FlexiFuturePartnerHealthAnswerTypes.YesNo
            },
            Narration = new FlexiFuturePartnerHazardousFieldCatalogDto
            {
                Question = explanationQuestion?.Question ?? string.Empty,
                AllowedAnswers = FlexiFuturePartnerHealthAnswerTypes.Open
            }
        };
    }

    private static bool IsPartnerWeightTrendQuestion(HealthQuestionsLibrary question)
    {
        return question.IsActive
               && string.Equals(
                   question.QuestionKey,
                   FlexiFutureHealthQuestionKeys.WeightTrend,
                   StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPartnerVisibleHealthQuestion(HealthQuestionsLibrary question)
    {
        return !string.IsNullOrWhiteSpace(question.QuestionKey)
               && !string.Equals(question.QuestionKey, FlexiFutureHealthQuestionKeys.Nature, StringComparison.OrdinalIgnoreCase)
               && string.IsNullOrWhiteSpace(question.ConditionalLogic)
               && !string.Equals(question.Category, FlexiFutureHealthQuestionCategories.Hazardous, StringComparison.OrdinalIgnoreCase)
               && !string.Equals(question.QuestionType, FlexiFutureHealthQuestionTypes.Group, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPartnerVisibleHazardousQuestion(HealthQuestionsLibrary question)
    {
        return !string.IsNullOrWhiteSpace(question.QuestionKey)
               && !string.Equals(
                   question.QuestionKey,
                   FlexiFutureHealthQuestionKeys.HazardousIntentExplanation,
                   StringComparison.OrdinalIgnoreCase)
               && string.IsNullOrWhiteSpace(question.ConditionalLogic)
               && !string.Equals(question.QuestionType, FlexiFutureHealthQuestionTypes.Group, StringComparison.OrdinalIgnoreCase);
    }

    private static FlexiFutureHealthQuestionLibraryItemDto MapQuestionLibraryItem(HealthQuestionsLibrary question)
    {
        return new FlexiFutureHealthQuestionLibraryItemDto
        {
            QuestionKey = question.QuestionKey,
            Question = question.Question,
            AllowedAnswers = MapAllowedAnswers(question)
        };
    }

    private static string MapAllowedAnswers(HealthQuestionsLibrary question)
    {
        if (string.Equals(
                question.QuestionKey,
                FlexiFutureHealthQuestionKeys.WeightTrend,
                StringComparison.OrdinalIgnoreCase))
        {
            return FlexiFuturePartnerHealthAnswerTypes.WeightTrend;
        }

        if (string.Equals(question.QuestionType, FlexiFutureHealthQuestionTypes.Text, StringComparison.OrdinalIgnoreCase))
            return FlexiFuturePartnerHealthAnswerTypes.Open;

        return FlexiFuturePartnerHealthAnswerTypes.YesNo;
    }

    private static void ValidateHazardousNarration(FlexiFuturePartnerHazardousDto hazardous)
    {
        if (RequiresHazardousNarration(hazardous)
            && string.IsNullOrWhiteSpace(hazardous.Narration))
        {
            throw new InvalidOperationException(
                "hazardous.narration is required when hazardousIntent is Yes.");
        }
    }

    private static bool RequiresHazardousNarration(FlexiFuturePartnerHazardousDto hazardous)
    {
        return HasPartnerJsonAnswer(hazardous.HazardousIntent)
               && IsYesAnswer(SerializePartnerAnswer(hazardous.HazardousIntent));
    }

    private static void ValidateNatureNarration(FlexiFuturePartnerHealthQuestionsInputDto healthQuestions)
    {
        try
        {
            if (healthQuestions.Main.Any(a =>
                    string.Equals(a.QuestionKey, FlexiFutureHealthQuestionKeys.Nature, StringComparison.OrdinalIgnoreCase))
                || healthQuestions.Spouse1.Any(a =>
                    string.Equals(a.QuestionKey, FlexiFutureHealthQuestionKeys.Nature, StringComparison.OrdinalIgnoreCase))
                || healthQuestions.Spouse2.Any(a =>
                    string.Equals(a.QuestionKey, FlexiFutureHealthQuestionKeys.Nature, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "Do not submit questionKey 'nature'. Use healthQuestions.narration instead.");
            }

            if (RequiresNatureNarration(healthQuestions)
                && string.IsNullOrWhiteSpace(healthQuestions.Narration))
            {
                throw new InvalidOperationException(
                    "healthQuestions.narration is required when any main life or spouse answers Yes to critical, claim, professional, minor, or symptoms.");
            }
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to validate health question narration.", ex);
        }
    }

    private static bool RequiresNatureNarration(FlexiFuturePartnerHealthQuestionsInputDto healthQuestions)
    {
        return RequiresNatureNarrationForAnswers(healthQuestions.Main)
               || RequiresNatureNarrationForAnswers(healthQuestions.Spouse1)
               || RequiresNatureNarrationForAnswers(healthQuestions.Spouse2);
    }

    private static bool RequiresNatureNarrationForAnswers(IEnumerable<FlexiFuturePartnerHealthAnswerDto> answers)
    {
        return answers.Any(answer =>
            NatureTriggerQuestionKeys.Contains(answer.QuestionKey.Trim(), StringComparer.OrdinalIgnoreCase)
            && IsYesAnswer(SerializePartnerAnswer(answer.Answer)));
    }

    private static void AppendNatureNarrationAnswer(
        ICollection<InternalHealthAnswer> target,
        string? narration,
        FlexiFuturePolicy policy,
        IReadOnlyDictionary<string, HealthQuestionsLibrary> questionByKey)
    {
        if (string.IsNullOrWhiteSpace(narration))
            return;

        if (!questionByKey.TryGetValue(FlexiFutureHealthQuestionKeys.Nature, out var natureQuestion))
            throw new InvalidOperationException("Health question 'nature' is not configured.");

        target.Add(new InternalHealthAnswer
        {
            QuestionKey = natureQuestion.QuestionKey,
            Answer = narration.Trim(),
            Context = FlexiFutureHealthContexts.Customer,
            ContextId = policy.CustomerId
        });
    }

    private static void ValidateWeightTrendAnswers(
        FlexiFuturePartnerHealthQuestionsInputDto healthQuestions,
        IReadOnlyList<string> requiredContexts)
    {
        try
        {
            ValidateWeightTrendForContext(
                healthQuestions.Main,
                FlexiFuturePartnerHealthQuestionContexts.Main);

            if (requiredContexts.Any(c =>
                    string.Equals(c, FlexiFuturePartnerHealthQuestionContexts.Spouse1, StringComparison.OrdinalIgnoreCase)))
            {
                ValidateWeightTrendForContext(
                    healthQuestions.Spouse1,
                    FlexiFuturePartnerHealthQuestionContexts.Spouse1);
            }

            if (requiredContexts.Any(c =>
                    string.Equals(c, FlexiFuturePartnerHealthQuestionContexts.Spouse2, StringComparison.OrdinalIgnoreCase)))
            {
                ValidateWeightTrendForContext(
                    healthQuestions.Spouse2,
                    FlexiFuturePartnerHealthQuestionContexts.Spouse2);
            }
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to validate weight trend answers.", ex);
        }
    }

    private static void ValidateWeightTrendForContext(
        IReadOnlyList<FlexiFuturePartnerHealthAnswerDto> answers,
        string partnerContext)
    {
        var weightTrendAnswer = answers.FirstOrDefault(a =>
            string.Equals(
                a.QuestionKey.Trim(),
                FlexiFutureHealthQuestionKeys.WeightTrend,
                StringComparison.OrdinalIgnoreCase));

        if (!IsWeightNotStationary(answers))
        {
            if (weightTrendAnswer != null
                && !string.IsNullOrWhiteSpace(SerializePartnerAnswer(weightTrendAnswer.Answer)))
            {
                NormalizeWeightTrendValue(
                    SerializePartnerAnswer(weightTrendAnswer.Answer)!,
                    partnerContext);
            }

            return;
        }

        if (weightTrendAnswer == null
            || string.IsNullOrWhiteSpace(SerializePartnerAnswer(weightTrendAnswer.Answer)))
        {
            throw new InvalidOperationException(
                $"Missing answer for required health question 'weightTrend' in healthQuestions.{partnerContext.ToLowerInvariant()} when weightStationary is No.");
        }

        NormalizeWeightTrendValue(
            SerializePartnerAnswer(weightTrendAnswer.Answer)!,
            partnerContext);
    }

    private static bool IsWeightNotStationary(IEnumerable<FlexiFuturePartnerHealthAnswerDto> answers)
    {
        return answers.Any(answer =>
            string.Equals(
                answer.QuestionKey.Trim(),
                FlexiFutureHealthQuestionKeys.WeightStationary,
                StringComparison.OrdinalIgnoreCase)
            && IsNoAnswer(SerializePartnerAnswer(answer.Answer)));
    }

    private static string NormalizeWeightTrendValue(string value, string partnerContext)
    {
        var normalized = WeightTrendAllowedValues.FirstOrDefault(
            allowed => string.Equals(allowed, value.Trim(), StringComparison.OrdinalIgnoreCase));

        if (normalized == null)
        {
            throw new InvalidOperationException(
                $"Answer for question 'weightTrend' in healthQuestions.{partnerContext.ToLowerInvariant()} must be 'Increasing' or 'Decreasing'.");
        }

        return normalized;
    }

    private static bool IsYesAnswer(string? answer)
    {
        if (string.IsNullOrWhiteSpace(answer))
            return false;

        var trimmed = answer.Trim();
        if (trimmed.Equals("Yes", StringComparison.OrdinalIgnoreCase))
            return true;

        try
        {
            using var document = JsonDocument.Parse(trimmed.StartsWith('{') || trimmed.StartsWith('"') ? trimmed : JsonSerializer.Serialize(trimmed));
            if (document.RootElement.ValueKind == JsonValueKind.String)
                return string.Equals(document.RootElement.GetString(), "Yes", StringComparison.OrdinalIgnoreCase);

            if (document.RootElement.ValueKind == JsonValueKind.Object
                && document.RootElement.TryGetProperty("value", out var valueProp))
            {
                return string.Equals(valueProp.GetString(), "Yes", StringComparison.OrdinalIgnoreCase);
            }
        }
        catch (JsonException)
        {
            return false;
        }

        return false;
    }

    private static bool IsNoAnswer(string? answer)
    {
        if (string.IsNullOrWhiteSpace(answer))
            return false;

        var trimmed = answer.Trim();
        if (trimmed.Equals("No", StringComparison.OrdinalIgnoreCase))
            return true;

        try
        {
            using var document = JsonDocument.Parse(trimmed.StartsWith('{') || trimmed.StartsWith('"') ? trimmed : JsonSerializer.Serialize(trimmed));
            if (document.RootElement.ValueKind == JsonValueKind.String)
                return string.Equals(document.RootElement.GetString(), "No", StringComparison.OrdinalIgnoreCase);

            if (document.RootElement.ValueKind == JsonValueKind.Object
                && document.RootElement.TryGetProperty("value", out var valueProp))
            {
                return string.Equals(valueProp.GetString(), "No", StringComparison.OrdinalIgnoreCase);
            }
        }
        catch (JsonException)
        {
            return false;
        }

        return false;
    }

    private static void ValidateRequiredPartnerHealthAnswers(
        FlexiFuturePolicy policy,
        IReadOnlyList<HealthQuestionsLibrary> questions,
        IReadOnlyList<string> requiredContexts,
        IReadOnlyDictionary<string, Guid> spouseContextIds,
        IReadOnlyList<InternalHealthAnswer> submittedAnswers)
    {
        try
        {
            var requiredQuestions = questions
                .Where(IsPartnerVisibleHealthQuestion)
                .Where(q => q.IsRequired)
                .Where(q => !string.Equals(
                    q.QuestionType,
                    FlexiFutureHealthQuestionTypes.Group,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var question in requiredQuestions)
            {
                if (!HasSubmittedHealthAnswer(
                        submittedAnswers,
                        question.QuestionKey,
                        FlexiFutureHealthContexts.Customer,
                        policy.CustomerId))
                {
                    throw new InvalidOperationException(
                        $"Missing answer for required health question '{question.QuestionKey}' in healthQuestions.main.");
                }

                if (requiredContexts.Any(c =>
                        string.Equals(c, FlexiFuturePartnerHealthQuestionContexts.Spouse1, StringComparison.OrdinalIgnoreCase)))
                {
                    if (!spouseContextIds.TryGetValue(FlexiFuturePartnerHealthQuestionContexts.Spouse1, out var spouse1Id))
                    {
                        throw new InvalidOperationException(
                            "healthQuestions.spouse1 requires a saved family member with context Spouse1.");
                    }

                    if (!HasSubmittedHealthAnswer(
                            submittedAnswers,
                            question.QuestionKey,
                            FlexiFutureHealthContexts.FamilyMember,
                            spouse1Id))
                    {
                        throw new InvalidOperationException(
                            $"Missing answer for required health question '{question.QuestionKey}' in healthQuestions.spouse1.");
                    }
                }

                if (requiredContexts.Any(c =>
                        string.Equals(c, FlexiFuturePartnerHealthQuestionContexts.Spouse2, StringComparison.OrdinalIgnoreCase)))
                {
                    if (!spouseContextIds.TryGetValue(FlexiFuturePartnerHealthQuestionContexts.Spouse2, out var spouse2Id))
                    {
                        throw new InvalidOperationException(
                            "healthQuestions.spouse2 requires a saved family member with context Spouse2.");
                    }

                    if (!HasSubmittedHealthAnswer(
                            submittedAnswers,
                            question.QuestionKey,
                            FlexiFutureHealthContexts.FamilyMember,
                            spouse2Id))
                    {
                        throw new InvalidOperationException(
                            $"Missing answer for required health question '{question.QuestionKey}' in healthQuestions.spouse2.");
                    }
                }
            }
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to validate required health answers.", ex);
        }
    }

    private static void ValidateRequiredPartnerHazardousAnswers(FlexiFuturePartnerHazardousDto hazardous)
    {
        try
        {
            if (!HasPartnerJsonAnswer(hazardous.HazardousIntent))
            {
                throw new InvalidOperationException("hazardous.hazardousIntent is required.");
            }
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to validate required hazardous answers.", ex);
        }
    }

    private static bool HasSubmittedHealthAnswer(
        IReadOnlyList<InternalHealthAnswer> answers,
        string questionKey,
        string context,
        Guid contextId)
    {
        return answers.Any(a =>
            string.Equals(a.QuestionKey, questionKey, StringComparison.OrdinalIgnoreCase)
            && string.Equals(a.Context, context, StringComparison.OrdinalIgnoreCase)
            && a.ContextId == contextId
            && !string.IsNullOrWhiteSpace(a.Answer));
    }

    private static void ValidateSubmittedHealthContexts(
        FlexiFuturePartnerHealthQuestionsInputDto healthQuestions,
        IReadOnlyList<string> requiredContexts)
    {
        if (healthQuestions.Spouse1.Count > 0
            && !requiredContexts.Any(c =>
                string.Equals(c, FlexiFuturePartnerHealthQuestionContexts.Spouse1, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "healthQuestions.spouse1 was submitted but this quote does not include a first spouse context.");
        }

        if (healthQuestions.Spouse2.Count > 0
            && !requiredContexts.Any(c =>
                string.Equals(c, FlexiFuturePartnerHealthQuestionContexts.Spouse2, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "healthQuestions.spouse2 was submitted but this quote does not include a second spouse context.");
        }
    }

    private async Task<Dictionary<string, Guid>> ResolveSpouseContextIdsAsync(
        FlexiFuturePolicy policy,
        CancellationToken cancellationToken)
    {
        var spouses = await _akiba.FlexiFutureFamilyMembers
            .AsNoTracking()
            .Where(m => m.PolicyId == policy.Id)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        var spouseMembers = spouses
            .Where(m => IsSpouseRelationship(m.Relationship))
            .ToList();

        var map = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var spouse1 = spouseMembers.FirstOrDefault(m =>
            string.Equals(m.AddedBy, FlexiFuturePartnerHealthQuestionContexts.Spouse1, StringComparison.OrdinalIgnoreCase));
        var spouse2 = spouseMembers.FirstOrDefault(m =>
            string.Equals(m.AddedBy, FlexiFuturePartnerHealthQuestionContexts.Spouse2, StringComparison.OrdinalIgnoreCase));

        if (spouse1 != null)
            map[FlexiFuturePartnerHealthQuestionContexts.Spouse1] = spouse1.Id;
        else if (spouseMembers.Count > 0)
            map[FlexiFuturePartnerHealthQuestionContexts.Spouse1] = spouseMembers[0].Id;

        if (spouse2 != null)
            map[FlexiFuturePartnerHealthQuestionContexts.Spouse2] = spouse2.Id;
        else if (spouseMembers.Count > 1)
            map[FlexiFuturePartnerHealthQuestionContexts.Spouse2] = spouseMembers[1].Id;

        return map;
    }

    private static List<InternalHealthAnswer> ConvertPartnerHealthAnswers(
        FlexiFuturePartnerHealthQuestionsInputDto healthQuestions,
        FlexiFuturePolicy policy,
        IReadOnlyList<string> requiredContexts,
        IReadOnlyDictionary<string, Guid> spouseContextIds,
        IReadOnlyDictionary<string, HealthQuestionsLibrary> questionByKey)
    {
        var answers = new List<InternalHealthAnswer>();
        AppendPartnerContextAnswers(
            answers,
            healthQuestions.Main,
            FlexiFuturePartnerHealthQuestionContexts.Main,
            FlexiFutureHealthContexts.Customer,
            policy.CustomerId,
            questionByKey);

        if (requiredContexts.Any(c =>
                string.Equals(c, FlexiFuturePartnerHealthQuestionContexts.Spouse1, StringComparison.OrdinalIgnoreCase)))
        {
            if (!spouseContextIds.TryGetValue(FlexiFuturePartnerHealthQuestionContexts.Spouse1, out var spouse1Id))
                throw new InvalidOperationException(
                    "healthQuestions.spouse1 requires a saved family member with context Spouse1.");

            AppendPartnerContextAnswers(
                answers,
                healthQuestions.Spouse1,
                FlexiFuturePartnerHealthQuestionContexts.Spouse1,
                FlexiFutureHealthContexts.FamilyMember,
                spouse1Id,
                questionByKey);
        }

        if (requiredContexts.Any(c =>
                string.Equals(c, FlexiFuturePartnerHealthQuestionContexts.Spouse2, StringComparison.OrdinalIgnoreCase)))
        {
            if (!spouseContextIds.TryGetValue(FlexiFuturePartnerHealthQuestionContexts.Spouse2, out var spouse2Id))
                throw new InvalidOperationException(
                    "healthQuestions.spouse2 requires a saved family member with context Spouse2.");

            AppendPartnerContextAnswers(
                answers,
                healthQuestions.Spouse2,
                FlexiFuturePartnerHealthQuestionContexts.Spouse2,
                FlexiFutureHealthContexts.FamilyMember,
                spouse2Id,
                questionByKey);
        }

        return answers;
    }

    private static void AppendPartnerContextAnswers(
        ICollection<InternalHealthAnswer> target,
        IEnumerable<FlexiFuturePartnerHealthAnswerDto> source,
        string partnerContext,
        string storageContext,
        Guid contextId,
        IReadOnlyDictionary<string, HealthQuestionsLibrary> questionByKey)
    {
        var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var answer in source)
        {
            if (string.IsNullOrWhiteSpace(answer.QuestionKey))
                throw new InvalidOperationException($"healthQuestions.{partnerContext.ToLowerInvariant()}[].questionKey is required.");

            var questionKey = answer.QuestionKey.Trim();
            if (!seenKeys.Add(questionKey))
            {
                throw new InvalidOperationException(
                    $"Duplicate health question '{questionKey}' in healthQuestions.{partnerContext.ToLowerInvariant()}.");
            }
            if (string.Equals(questionKey, FlexiFutureHealthQuestionKeys.Nature, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Do not submit questionKey 'nature'. Use healthQuestions.narration instead.");
            }

            if (!questionByKey.ContainsKey(questionKey))
                throw new InvalidOperationException($"Unknown health question '{questionKey}' for context '{partnerContext}'.");

            var serializedAnswer = SerializePartnerAnswer(answer.Answer);
            if (string.Equals(questionKey, FlexiFutureHealthQuestionKeys.WeightTrend, StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(serializedAnswer))
            {
                serializedAnswer = NormalizeWeightTrendValue(serializedAnswer, partnerContext);
            }

            target.Add(new InternalHealthAnswer
            {
                QuestionKey = questionKey,
                Answer = serializedAnswer,
                Context = storageContext,
                ContextId = contextId
            });
        }
    }

    private async Task<List<FlexiFuturePlusFamilyMemberDto>> MapStoredFamilyMembersAsync(
        FlexiFuturePolicy policy,
        CancellationToken cancellationToken)
    {
        var members = await _akiba.FlexiFutureFamilyMembers
            .AsNoTracking()
            .Where(m => m.PolicyId == policy.Id)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        var spouseIndex = 0;
        var childIndex = 0;
        var mapped = new List<FlexiFuturePlusFamilyMemberDto>(members.Count);

        foreach (var member in members)
        {
            var context = member.AddedBy;
            if (string.IsNullOrWhiteSpace(context))
            {
                if (IsSpouseRelationship(member.Relationship))
                {
                    spouseIndex++;
                    context = spouseIndex == 1
                        ? FlexiFuturePartnerHealthQuestionContexts.Spouse1
                        : FlexiFuturePartnerHealthQuestionContexts.Spouse2;
                }
                else
                {
                    childIndex++;
                    context = $"Child{childIndex}";
                }
            }

            mapped.Add(new FlexiFuturePlusFamilyMemberDto
            {
                Context = context,
                OtherNames = member.OtherNames,
                Surname = member.Surname,
                Gender = member.Gender,
                EmailAddress = member.EmailAddress,
                PhoneNumber = member.PhoneNumber,
                IdNumber = member.IDNumber,
                DateOfBirth = member.DateOfBirth,
                Relationship = MapStoredFamilyRelationship(member.Relationship, context)
            });
        }

        return mapped;
    }

    private static string MapStoredFamilyRelationship(string? relationship, string context)
    {
        if (!string.IsNullOrWhiteSpace(relationship))
        {
            if (string.Equals(relationship, "Spouse", StringComparison.OrdinalIgnoreCase))
                return "Spouse";
            if (string.Equals(relationship, "Child", StringComparison.OrdinalIgnoreCase))
                return "Child";
        }

        return context.StartsWith("Spouse", StringComparison.OrdinalIgnoreCase) ? "Spouse" : "Child";
    }

    private async Task<List<FlexiFuturePlusBeneficiaryDto>> MapStoredBeneficiariesAsync(
        FlexiFuturePolicy policy,
        CancellationToken cancellationToken)
    {
        var beneficiaries = await _akiba.FlexiFutureBeneficiaries
            .AsNoTracking()
            .Include(b => b.Guardians)
            .Where(b => b.PolicyId == policy.Id)
            .OrderBy(b => b.CreatedAt)
            .ToListAsync(cancellationToken);

        return beneficiaries.Select(beneficiary => new FlexiFuturePlusBeneficiaryDto
        {
            OtherNames = beneficiary.OtherNames,
            Surname = beneficiary.Surname,
            DateOfBirth = beneficiary.DateOfBirth ?? default,
            Gender = beneficiary.Gender,
            EmailAddress = beneficiary.EmailAddress,
            PhoneNumber = beneficiary.PhoneNumber,
            IdNumber = beneficiary.IDNumber,
            Relationship = beneficiary.Relationship,
            PercentageShare = beneficiary.PercentageShare,
            Guardian = MapStoredGuardian(beneficiary.Guardians)
        }).ToList();
    }

    private static FlexiFuturePlusGuardianDto? MapStoredGuardian(IEnumerable<FlexiFutureGuardian> guardians)
    {
        var guardian = guardians.OrderBy(g => g.CreatedAt).FirstOrDefault();
        if (guardian == null)
            return null;

        return new FlexiFuturePlusGuardianDto
        {
            OtherNames = guardian.OtherNames,
            Surname = guardian.Surname,
            Gender = guardian.Gender,
            EmailAddress = guardian.EmailAddress,
            PhoneNumber = guardian.PhoneNumber,
            IdNumber = guardian.IDNumber,
            DateOfBirth = guardian.DateOfBirth,
            Relationship = guardian.Relationship
        };
    }

    private async Task<FlexiFuturePartnerHealthAnswersByContextDto?> MapStoredHealthAnswersByContextAsync(
        FlexiFuturePolicy policy,
        CancellationToken cancellationToken)
    {
        var rows = await _akiba.FlexiFutureHealthInfo
            .AsNoTracking()
            .Where(h => h.PolicyId == policy.Id)
            .OrderBy(h => h.CreatedAt)
            .ToListAsync(cancellationToken);

        if (rows.Count == 0)
            return null;

        var spouseContextIds = await ResolveSpouseContextIdsAsync(policy, cancellationToken);
        spouseContextIds.TryGetValue(FlexiFuturePartnerHealthQuestionContexts.Spouse1, out var spouse1Id);
        spouseContextIds.TryGetValue(FlexiFuturePartnerHealthQuestionContexts.Spouse2, out var spouse2Id);

        var result = new FlexiFuturePartnerHealthAnswersByContextDto();

        foreach (var row in rows)
        {
            if (string.Equals(row.QuestionKey, FlexiFutureHealthQuestionKeys.Nature, StringComparison.OrdinalIgnoreCase)
                && string.Equals(row.Context, FlexiFutureHealthContexts.Customer, StringComparison.OrdinalIgnoreCase)
                && row.ContextId == policy.CustomerId)
            {
                result.Narration = row.Answer;
                continue;
            }

            var dto = new FlexiFuturePartnerHealthAnswerDto
            {
                QuestionKey = row.QuestionKey,
                Answer = ParseStoredAnswer(row.Answer)
            };

            if (string.Equals(row.Context, FlexiFutureHealthContexts.Customer, StringComparison.OrdinalIgnoreCase)
                && row.ContextId == policy.CustomerId)
            {
                result.Main.Add(dto);
                continue;
            }

            if (string.Equals(row.Context, FlexiFutureHealthContexts.FamilyMember, StringComparison.OrdinalIgnoreCase)
                && row.ContextId == spouse1Id)
            {
                result.Spouse1.Add(dto);
                continue;
            }

            if (string.Equals(row.Context, FlexiFutureHealthContexts.FamilyMember, StringComparison.OrdinalIgnoreCase)
                && row.ContextId == spouse2Id)
            {
                result.Spouse2.Add(dto);
            }
        }

        return result;
    }

    private async Task<FlexiFuturePartnerHazardousDto?> MapStoredHazardousAnswersAsync(
        FlexiFuturePolicy policy,
        CancellationToken cancellationToken)
    {
        var rows = await _akiba.FlexiFutureOccupationHazards
            .AsNoTracking()
            .Where(h => h.PolicyId == policy.Id)
            .OrderBy(h => h.CreatedAt)
            .ToListAsync(cancellationToken);

        if (rows.Count == 0)
            return null;

        var result = new FlexiFuturePartnerHazardousDto();
        foreach (var row in rows)
        {
            if (string.Equals(
                    row.QuestionKey,
                    FlexiFutureHealthQuestionKeys.HazardousIntentExplanation,
                    StringComparison.OrdinalIgnoreCase))
            {
                result.Narration = row.Answer;
                continue;
            }

            if (string.Equals(
                    row.QuestionKey,
                    FlexiFutureHealthQuestionKeys.HazardousIntent,
                    StringComparison.OrdinalIgnoreCase))
            {
                result.HazardousIntent = ParseStoredAnswer(row.Answer);
            }
        }

        return result;
    }

    private static JsonElement ParseStoredAnswer(string? answer)
    {
        if (string.IsNullOrWhiteSpace(answer))
            return JsonSerializer.SerializeToElement(string.Empty);

        try
        {
            using var document = JsonDocument.Parse(answer);
            return document.RootElement.Clone();
        }
        catch (JsonException)
        {
            return JsonSerializer.SerializeToElement(answer);
        }
    }

    private static string? SerializePartnerAnswer(JsonElement answer)
    {
        return answer.ValueKind switch
        {
            JsonValueKind.String => answer.GetString(),
            JsonValueKind.Null or JsonValueKind.Undefined => null,
            _ => answer.GetRawText()
        };
    }

    private static void ValidateHealthAnswer(HealthQuestionsLibrary question, string? answer)
    {
        if (string.Equals(question.QuestionType, FlexiFutureHealthQuestionTypes.Group, StringComparison.OrdinalIgnoreCase))
            return;

        if (string.IsNullOrWhiteSpace(answer))
            throw new InvalidOperationException($"Answer is required for question '{question.QuestionKey}'.");
    }

    private static bool IsSpouseRelationship(string? relationship)
    {
        return string.Equals(relationship?.Trim(), "Spouse", StringComparison.OrdinalIgnoreCase);
    }

    private const int MaxConsentSignatureBytes = 20 * 1024 * 1024;

    private async Task CompleteConsentAsync(
        FlexiFuturePolicy policy,
        FlexiFutureQuote quote,
        FlexiFuturePlusConsentDto consent,
        PartnerContext partner,
        CancellationToken cancellationToken)
    {
        try
        {
            ValidateConsentInput(consent);
            await EnsureOnboardingReadyForConsentAsync(policy, quote, cancellationToken);

            var signature = NormalizeConsentSignature(consent.Signature);
            var hasSignature = signature != null;
            var hasOtp = !string.IsNullOrWhiteSpace(consent.Otp);

            if (hasSignature == hasOtp)
            {
                throw new InvalidOperationException(
                    "Provide exactly one of consent.otp or consent.signature to complete onboarding.");
            }

            var now = DateTime.UtcNow;

            if (hasSignature)
            {
                policy.Signature = ToSignatureDataUri(signature!);
                policy.SignaturePath = signature!.Name;
                policy.CompletionMethod = FlexiFutureOnboardingCompletionMethods.Signature;
            }
            else
            {
                await ConsumeConsentOtpAsync(policy, consent.Otp!.Trim(), cancellationToken);
                policy.Signature = null;
                policy.SignaturePath = null;
                policy.CompletionMethod = FlexiFutureOnboardingCompletionMethods.Otp;
            }

            policy.OnboardingStep = FlexiFutureOnboardingSteps.Complete;
            policy.CompletedAt = now;

            if (string.Equals(policy.PolicyStatus, FlexiFuturePolicyStatuses.Draft, StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(policy.PolicyStatus))
            {
                policy.PolicyStatus = FlexiFuturePolicyStatuses.Submitted;
            }

            policy.UpdatedAt = now;
            await EnqueueOnboardingCallbackAsync(policy, quote, partner, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to complete onboarding consent.", ex);
        }
    }

    private static void ValidateConsentInput(FlexiFuturePlusConsentDto consent)
    {
        try
        {
            if (consent.Otp?.Length > 32)
                throw new InvalidOperationException("consent.otp must not exceed 32 characters.");
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to validate consent.", ex);
        }
    }

    private async Task EnsureOnboardingReadyForConsentAsync(
        FlexiFuturePolicy policy,
        FlexiFutureQuote quote,
        CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _akiba.customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == policy.CustomerId, cancellationToken)
                ?? throw new InvalidOperationException("customerDetails must be saved before consent.");

            if (!await IsCustomerDetailsCompleteAsync(customer, cancellationToken))
                throw new InvalidOperationException("customerDetails must be complete before consent.");

            var spouseCount = quote.Spouses.Count > 0
                ? quote.Spouses.Max(s => s.SpouseIndex)
                : 0;

            if (spouseCount > 0)
            {
                var familyCount = await _akiba.FlexiFutureFamilyMembers
                    .AsNoTracking()
                    .CountAsync(f => f.PolicyId == policy.Id, cancellationToken);

                if (familyCount <= 0)
                    throw new InvalidOperationException("familyMembers must be saved before consent.");
            }

            var healthCount = await _akiba.FlexiFutureHealthInfo
                .AsNoTracking()
                .CountAsync(h => h.PolicyId == policy.Id, cancellationToken);
            if (healthCount <= 0)
                throw new InvalidOperationException("healthQuestions must be saved before consent.");

            var hazardCount = await _akiba.FlexiFutureOccupationHazards
                .AsNoTracking()
                .CountAsync(h => h.PolicyId == policy.Id, cancellationToken);
            if (hazardCount <= 0)
                throw new InvalidOperationException("hazardous must be saved before consent.");

            var beneficiaryCount = await _akiba.FlexiFutureBeneficiaries
                .AsNoTracking()
                .CountAsync(b => b.PolicyId == policy.Id, cancellationToken);
            if (beneficiaryCount <= 0)
                throw new InvalidOperationException("beneficiaries must be saved before consent.");
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to validate onboarding readiness for consent.", ex);
        }
    }

    private async Task ConsumeConsentOtpAsync(
        FlexiFuturePolicy policy,
        string otpCode,
        CancellationToken cancellationToken)
    {
        try
        {
            var customerId = policy.CustomerId.ToString();
            var otp = await _db.OTPs
                .Where(o =>
                    o.CustomerId == customerId
                    && o.Code != null
                    && o.Code.ToLower() == otpCode.ToLower()
                    && !o.IsUsed
                    && (o.ProductRef == null || o.ProductRef == policy.Id.ToString()))
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new InvalidOperationException("Invalid OTP code.");

            if (otp.ExpiredAt.HasValue && otp.ExpiredAt.Value < DateTime.UtcNow)
                throw new InvalidOperationException("OTP has expired.");

            otp.IsUsed = true;
            otp.UsedAt = DateTime.UtcNow;
            otp.UsedBy = customerId;
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to validate consent OTP.", ex);
        }
    }

    private static FileUploadDTO? NormalizeConsentSignature(FileUploadDTO? signature)
    {
        try
        {
            if (signature == null || string.IsNullOrWhiteSpace(signature.Data))
                return null;

            signature.Data = signature.Data.Trim();
            if (string.IsNullOrWhiteSpace(signature.Extension))
                signature.Extension = "png";
            if (string.IsNullOrWhiteSpace(signature.Name))
                signature.Name = "signature";

            ValidateConsentSignatureSize(signature.Data);
            return signature;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to validate consent signature.", ex);
        }
    }

    private static void ValidateConsentSignatureSize(string data)
    {
        var payload = data.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
            ? data[(data.IndexOf(',') + 1)..]
            : data;

        payload = payload.Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal);

        var estimatedBytes = (payload.Length * 3) / 4;
        if (estimatedBytes > MaxConsentSignatureBytes)
        {
            throw new InvalidOperationException("consent.signature must not exceed 20 MB.");
        }
    }

    private static string ToSignatureDataUri(FileUploadDTO signature)
    {
        var data = signature.Data.Trim();
        if (data.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return data;

        var extension = signature.Extension.Trim().TrimStart('.');
        if (string.IsNullOrWhiteSpace(extension))
            extension = "png";

        var payload = data.Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal);

        return $"data:image/{extension};base64,{payload}";
    }

    private async Task EnqueueOnboardingCallbackAsync(
        FlexiFuturePolicy policy,
        FlexiFutureQuote quote,
        PartnerContext partner,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(policy.CallbackUrl))
                return;

            var callback = new CallBackResponse
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                CallbackUrl = policy.CallbackUrl,
                PartnerCode = partner.PartnerCode,
                PartnerId = partner.PartnerId.ToString(),
                Productenum = Productenum.flexifutureplus,
                RequestType = "FlexiFutureOnboarding",
                Request = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    quoteId = quote.Id,
                    policyId = policy.Id,
                    policyStatus = policy.PolicyStatus,
                    onboardingStep = policy.OnboardingStep,
                    isApproved = policy.IsApproved,
                    policyNo = policy.PolicyNo
                }),
                Response = string.Empty,
                CorrelationId = policy.Id.ToString(),
                RequestId = quote.Id.ToString(),
                CallbackProcessed = false,
                ProcessResponse = false,
                Responded = false,
                Status = "Pending",
                StatusMessage = "Pending onboarding callback dispatch."
            };

            _db.callBackResponse.Add(callback);
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Onboarding completed for quote {QuoteId} but callback enqueue failed.",
                quote.Id);
        }
    }

    private static void EnsureNotComplete(FlexiFuturePolicy policy)
    {
        try
        {
            if (string.Equals(policy.OnboardingStep, FlexiFutureOnboardingSteps.Complete, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Onboarding is already complete and cannot be modified.");
            }

            if (string.Equals(policy.PolicyStatus, FlexiFuturePolicyStatuses.Cancelled, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Cancelled policies cannot be modified.");
            }
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to validate onboarding policy state.", ex);
        }
    }

    private sealed class InternalHealthAnswer
    {
        public string QuestionKey { get; set; } = string.Empty;
        public string? Answer { get; set; }
        public string Context { get; set; } = string.Empty;
        public Guid ContextId { get; set; }
    }

    private static FlexiFutureHealthQuestionDto MapHealthQuestion(HealthQuestionsLibrary question)
    {
        return new FlexiFutureHealthQuestionDto
        {
            Id = question.Id,
            ClientQuestionCode = question.ClientQuestionCode,
            Question = question.Question,
            QuestionKey = question.QuestionKey,
            QuestionType = question.QuestionType,
            Category = question.Category,
            Description = question.Description,
            Placeholder = question.Placeholder,
            IsPerMember = question.IsPerMember,
            Required = question.IsRequired,
            SpouseQuestion = question.SpouseQuestion,
            QuestionOrder = question.QuestionOrder,
            Options = ParseQuestionOptions(question.Options),
            Validation = ParseJsonObject(question.Validations),
            Condition = ParseJsonObject(question.ConditionalLogic),
            ChildQuestions = question.ChildQuestions
                .Where(c => c.IsActive)
                .OrderBy(c => c.QuestionOrder)
                .Select(MapHealthQuestion)
                .ToList()
        };
    }

    private static List<FlexiFutureHealthQuestionOptionDto> ParseQuestionOptions(string? optionsJson)
    {
        if (string.IsNullOrWhiteSpace(optionsJson))
            return new List<FlexiFutureHealthQuestionOptionDto>();

        try
        {
            return JsonSerializer.Deserialize<List<FlexiFutureHealthQuestionOptionDto>>(optionsJson)
                   ?? new List<FlexiFutureHealthQuestionOptionDto>();
        }
        catch
        {
            return new List<FlexiFutureHealthQuestionOptionDto>();
        }
    }

    private static object? ParseJsonObject(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<object>(json);
        }
        catch
        {
            return null;
        }
    }

    private async Task<string> GenerateMemberNumberAsync(CancellationToken cancellationToken)
    {
        string memberNo;
        do
        {
            memberNo = DateTime.Now.ToString("yy") + "-" + _settings.GenerateRadomCode(6);
        }
        while (await _akiba.customers.AnyAsync(c => c.MemberNumber == memberNo, cancellationToken));

        return memberNo;
    }

    private async Task<string> GenerateReferralCodeAsync(CancellationToken cancellationToken)
    {
        string referralCode;
        do
        {
            referralCode = _settings.GenerateRadomCode();
        }
        while (await _akiba.customers.AnyAsync(c => c.RefferalCode == referralCode, cancellationToken));

        return referralCode;
    }
}
