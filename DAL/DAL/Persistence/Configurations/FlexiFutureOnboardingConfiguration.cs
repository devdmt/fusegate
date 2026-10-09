using DAL.Model.FlexiFuture;
using DAL.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Persistence.Configurations;

public class FlexiFuturePolicyConfiguration : IEntityTypeConfiguration<FlexiFuturePolicy>
{
    public void Configure(EntityTypeBuilder<FlexiFuturePolicy> builder)
    {
        try
        {
            builder.ToTable("FlexiFuturePolicy");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.PolicyStatus).HasMaxLength(32).IsRequired();
            builder.Property(x => x.OnboardingStep).HasMaxLength(32).IsRequired();
            builder.Property(x => x.PolicyNo).HasMaxLength(64);
            builder.Property(x => x.ExternalRefId).HasMaxLength(100);
            builder.Property(x => x.PartnerCode).HasMaxLength(50);
            builder.Property(x => x.CallbackUrl).HasMaxLength(500);
            builder.Property(x => x.ApprovedBy).HasMaxLength(128);
            builder.Property(x => x.EffectiveDate).HasDateOnlyConversion();
            builder.Property(x => x.CreatedBy).HasMaxLength(128);
            builder.Property(x => x.SignaturePath).HasMaxLength(500);
            builder.Property(x => x.CompletionMethod).HasMaxLength(16);
            builder.HasIndex(x => x.QuoteId).IsUnique();
            builder.HasIndex(x => x.CustomerId);
            builder.HasIndex(x => x.PolicyStatus);
            builder.HasIndex(x => x.OnboardingStep);
            builder.HasIndex(x => x.PolicyNo).IsUnique().HasFilter("[PolicyNo] IS NOT NULL");
            builder.HasIndex(x => x.ExternalRefId).IsUnique().HasFilter("[ExternalRefId] IS NOT NULL");
            builder.HasOne(x => x.Quote).WithMany().HasForeignKey(x => x.QuoteId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.FamilyMembers).WithOne(x => x.Policy!).HasForeignKey(x => x.PolicyId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Beneficiaries).WithOne(x => x.Policy!).HasForeignKey(x => x.PolicyId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.HealthInfo).WithOne(x => x.Policy!).HasForeignKey(x => x.PolicyId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.OccupationHazards).WithOne(x => x.Policy!).HasForeignKey(x => x.PolicyId).OnDelete(DeleteBehavior.Cascade);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class FlexiFutureFamilyMemberConfiguration : IEntityTypeConfiguration<FlexiFutureFamilyMember>
{
    public void Configure(EntityTypeBuilder<FlexiFutureFamilyMember> builder)
    {
        try
        {
            builder.ToTable("FlexiFutureFamilyMembers");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.OtherNames).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Surname).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Gender).HasMaxLength(16);
            builder.Property(x => x.EmailAddress).HasMaxLength(200);
            builder.Property(x => x.PhoneNumber).HasMaxLength(40);
            builder.Property(x => x.IDNumber).HasMaxLength(64);
            builder.Property(x => x.KRAPinNo).HasMaxLength(32);
            builder.Property(x => x.Nationality).HasMaxLength(100);
            builder.Property(x => x.Residency).HasMaxLength(100);
            builder.Property(x => x.USAddress).HasMaxLength(500);
            builder.Property(x => x.Relationship).HasMaxLength(64);
            builder.Property(x => x.TIN).HasMaxLength(64);
            builder.Property(x => x.IsValidIDNumber)
                .HasDefaultValue(FlexiFutureIdValidationStatuses.Pending);
            builder.Property(x => x.IPRSFullName).HasMaxLength(300);
            builder.Property(x => x.AddedBy).HasMaxLength(128);
            builder.Property(x => x.DateOfBirth).HasDateOnlyConversion();
            builder.HasIndex(x => x.PolicyId);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class FlexiFutureBeneficiaryConfiguration : IEntityTypeConfiguration<FlexiFutureBeneficiary>
{
    public void Configure(EntityTypeBuilder<FlexiFutureBeneficiary> builder)
    {
        try
        {
            builder.ToTable("FlexiFutureBeneficiaries");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.OtherNames).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Surname).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Gender).HasMaxLength(16);
            builder.Property(x => x.EmailAddress).HasMaxLength(200);
            builder.Property(x => x.PhoneNumber).HasMaxLength(40);
            builder.Property(x => x.IDNumber).HasMaxLength(64);
            builder.Property(x => x.Relationship).HasMaxLength(64);
            builder.Property(x => x.PercentageShare).HasPrecision(5, 2);
            builder.Property(x => x.DateOfBirth).HasDateOnlyConversion();
            builder.HasIndex(x => x.PolicyId);
            builder.HasMany(x => x.Guardians).WithOne(x => x.Beneficiary!).HasForeignKey(x => x.BeneficiaryId).OnDelete(DeleteBehavior.Cascade);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class FlexiFutureGuardianConfiguration : IEntityTypeConfiguration<FlexiFutureGuardian>
{
    public void Configure(EntityTypeBuilder<FlexiFutureGuardian> builder)
    {
        try
        {
            builder.ToTable("FlexiFutureGuardians");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.OtherNames).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Surname).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Gender).HasMaxLength(16);
            builder.Property(x => x.EmailAddress).HasMaxLength(200);
            builder.Property(x => x.PhoneNumber).HasMaxLength(40);
            builder.Property(x => x.IDNumber).HasMaxLength(64);
            builder.Property(x => x.Relationship).HasMaxLength(64);
            builder.Property(x => x.DateOfBirth).HasDateOnlyConversion();
            builder.HasIndex(x => x.BeneficiaryId);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class FlexiFutureHealthInfoConfiguration : IEntityTypeConfiguration<FlexiFutureHealthInfo>
{
    public void Configure(EntityTypeBuilder<FlexiFutureHealthInfo> builder)
    {
        try
        {
            builder.ToTable("FlexiFutureHealthInfo");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Question).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.QuestionKey).HasMaxLength(64).IsRequired();
            builder.Property(x => x.Context).HasMaxLength(32).IsRequired();
            builder.HasIndex(x => new { x.PolicyId, x.QuestionId, x.Context, x.ContextId }).IsUnique();
            builder.HasIndex(x => x.PolicyId);
            builder.HasIndex(x => x.QuestionId);
            builder.HasOne(x => x.HealthQuestion).WithMany().HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Restrict);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class FlexiFutureOccupationHazardConfiguration : IEntityTypeConfiguration<FlexiFutureOccupationHazard>
{
    public void Configure(EntityTypeBuilder<FlexiFutureOccupationHazard> builder)
    {
        try
        {
            builder.ToTable("FlexiFutureOccupationHazards");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Question).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.QuestionKey).HasMaxLength(64).IsRequired();
            builder.HasIndex(x => new { x.PolicyId, x.QuestionId }).IsUnique();
            builder.HasIndex(x => x.PolicyId);
            builder.HasIndex(x => x.QuestionId);
            builder.HasOne(x => x.HealthQuestion).WithMany().HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Restrict);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class HealthQuestionsLibraryConfiguration : IEntityTypeConfiguration<HealthQuestionsLibrary>
{
    public void Configure(EntityTypeBuilder<HealthQuestionsLibrary> builder)
    {
        try
        {
            builder.ToTable("HealthQuestionsLibrary");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ClientQuestionCode).HasMaxLength(16);
            builder.Property(x => x.Question).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.QuestionKey).HasMaxLength(64).IsRequired();
            builder.Property(x => x.QuestionType).HasMaxLength(32).IsRequired();
            builder.Property(x => x.Category).HasMaxLength(64).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.Placeholder).HasMaxLength(200);
            builder.Property(x => x.SpouseQuestion).HasMaxLength(1000);
            builder.Property(x => x.IsRequired).HasDefaultValue(true);
            builder.HasIndex(x => x.QuestionKey).IsUnique();
            builder.HasIndex(x => x.ClientQuestionCode).IsUnique().HasFilter("[ClientQuestionCode] IS NOT NULL");
            builder.HasIndex(x => x.ParentQuestionId);
            builder.HasIndex(x => new { x.IsActive, x.QuestionOrder });
            builder.HasIndex(x => new { x.Category, x.QuestionOrder });
            builder.HasOne(x => x.ParentQuestion)
                .WithMany(x => x.ChildQuestions)
                .HasForeignKey(x => x.ParentQuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
