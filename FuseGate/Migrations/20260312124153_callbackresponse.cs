using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsbJson.API.Migrations
{
    /// <inheritdoc />
    public partial class callbackresponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_msureRequests_PartnersProducts_ProductsId",
                table: "msureRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnersProducts_Partners_PartnerId1",
                table: "PartnersProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartnersProducts",
                table: "PartnersProducts");

            migrationBuilder.DropIndex(
                name: "IX_PartnersProducts_PartnerId1",
                table: "PartnersProducts");

            migrationBuilder.DropColumn(
                name: "PartnerId1",
                table: "PartnersProducts");

            migrationBuilder.DropColumn(
                name: "AverageIncomeId",
                table: "Customers");

            migrationBuilder.RenameTable(
                name: "PartnersProducts",
                newName: "partnersProducts");

            migrationBuilder.RenameColumn(
                name: "Dob",
                table: "Guardian",
                newName: "DateOfBirth");

            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "Customers",
                newName: "MemberNo");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "Customers",
                newName: "Fullname");

            migrationBuilder.AlterColumn<int>(
                name: "PartnerId",
                table: "partnersProducts",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "partnersProducts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "partnersProducts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "partnersProducts",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Step",
                table: "Customers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Stage",
                table: "Customers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "RecordProcessed",
                table: "Customers",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Customers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "Processed",
                table: "Customers",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "MailSent",
                table: "Customers",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Customers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<bool>(
                name: "Complete",
                table: "Customers",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<double>(
                name: "AverageIncome",
                table: "Customers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Firstname",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MemberNumber",
                table: "Customers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberNo",
                table: "Beneficiaries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerCode",
                table: "Beneficiaries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_partnersProducts",
                table: "partnersProducts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CallBackResponse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartnerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Request = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Responded = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CallbackUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Callbackerror = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallBackResponse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "customerProducts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Product = table.Column<int>(type: "int", nullable: false),
                    RefNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Complete = table.Column<bool>(type: "bit", nullable: false),
                    Processed = table.Column<bool>(type: "bit", nullable: true),
                    IsPicked = table.Column<bool>(type: "bit", nullable: true),
                    MailSentOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestCreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AgentId = table.Column<int>(type: "int", nullable: true),
                    PaymentComplete = table.Column<bool>(type: "bit", nullable: true),
                    PaymentCompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestSource = table.Column<int>(type: "int", nullable: true),
                    Filelocation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Filebytes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    Activated = table.Column<bool>(type: "bit", nullable: true),
                    Validated = table.Column<bool>(type: "bit", nullable: true),
                    ValidationErrorCount = table.Column<int>(type: "int", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: true),
                    ValidationErrors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextRetryPeriod = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActivationMode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customerProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FuneralExpenseOnboardings",
                columns: table => new
                {
                    MemberId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuoteId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Residency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PolicyStatus = table.Column<int>(type: "int", nullable: false),
                    MonthlyIncomeRange = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartnerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignatureBase64 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallbackUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuneralExpenseOnboardings", x => x.MemberId);
                });

            migrationBuilder.CreateTable(
                name: "FuneralExpenseQuotations",
                columns: table => new
                {
                    QuoteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PolicyType = table.Column<int>(type: "int", nullable: false),
                    PartnerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SumAssured = table.Column<double>(type: "float", nullable: false),
                    CoverPremium = table.Column<double>(type: "float", nullable: false),
                    CompensationLevy = table.Column<double>(type: "float", nullable: false),
                    PolicyFee = table.Column<double>(type: "float", nullable: false),
                    TotalPremium = table.Column<double>(type: "float", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PolicyTerm = table.Column<int>(type: "int", nullable: false),
                    PaymentFrequency = table.Column<int>(type: "int", nullable: false),
                    Maturity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NaturalDeath = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AccidentialDeath = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CriticalIllness = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PTDNatural = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PTDAccidental = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AgentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallbackUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuneralExpenseQuotations", x => x.QuoteId);
                });

            migrationBuilder.CreateTable(
                name: "MemberHealths",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MemberId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuoteId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriorDeclinedInsurance = table.Column<bool>(type: "bit", nullable: false),
                    ExistingConditions = table.Column<bool>(type: "bit", nullable: false),
                    DrugOrAlcoholAbuse = table.Column<bool>(type: "bit", nullable: false),
                    Respiratory = table.Column<bool>(type: "bit", nullable: false),
                    HeartOrCirculation = table.Column<bool>(type: "bit", nullable: false),
                    ChronicConditions = table.Column<bool>(type: "bit", nullable: false),
                    Wellness = table.Column<bool>(type: "bit", nullable: false),
                    ImmuneOrViral = table.Column<bool>(type: "bit", nullable: false),
                    Senses = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberHealths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mpesaSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MpesaUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaybillPass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaybillId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsumerKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shortcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UniqueURLCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Registered = table.Column<bool>(type: "bit", nullable: true),
                    ConsumerSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityCredential = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrantType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaybillName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverPartyIdentifierType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    B2CUtilityAccountAvailableFunds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    B2CWorkingAccountAvailableFunds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SaltKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: true),
                    DefaultPaybill = table.Column<bool>(type: "bit", nullable: false),
                    paybillType = table.Column<int>(type: "int", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mpesaSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OTPOnboardings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailPlaceHolder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsUsed = table.Column<bool>(type: "bit", nullable: true),
                    ISsent = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTPOnboardings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OTPs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailPlaceHolder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ISSMSSent = table.Column<bool>(type: "bit", nullable: true),
                    IsEmailSent = table.Column<bool>(type: "bit", nullable: true),
                    isEmailPicked = table.Column<bool>(type: "bit", nullable: true),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    ISsent = table.Column<bool>(type: "bit", nullable: false),
                    LinkGenerated = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    notificationType = table.Column<int>(type: "int", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendTrial = table.Column<int>(type: "int", nullable: true),
                    EmailDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailTemplateType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTPs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentContributions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MemberId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CallbackUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentContributions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyActivations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MemberId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignatureBase64 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyActivations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    productenum = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Emailtemplate = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mpesaToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Access_token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expires_in = table.Column<int>(type: "int", nullable: false),
                    MpesaId = table.Column<int>(type: "int", nullable: false),
                    paybillid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mpesaToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mpesaToken_mpesaSettings_MpesaId",
                        column: x => x.MpesaId,
                        principalTable: "mpesaSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberPayments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentContributionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentMode = table.Column<int>(type: "int", nullable: false),
                    PaymentReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MpesaNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberPayments_PaymentContributions_PaymentContributionId",
                        column: x => x.PaymentContributionId,
                        principalTable: "PaymentContributions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_partnersProducts_PartnerId",
                table: "partnersProducts",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_partnersProducts_ProductId",
                table: "partnersProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberPayments_PaymentContributionId",
                table: "MemberPayments",
                column: "PaymentContributionId");

            migrationBuilder.CreateIndex(
                name: "IX_mpesaToken_MpesaId",
                table: "mpesaToken",
                column: "MpesaId");

            migrationBuilder.AddForeignKey(
                name: "FK_msureRequests_partnersProducts_ProductsId",
                table: "msureRequests",
                column: "ProductsId",
                principalTable: "partnersProducts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_partnersProducts_Partners_PartnerId",
                table: "partnersProducts",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_partnersProducts_Products_ProductId",
                table: "partnersProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_msureRequests_partnersProducts_ProductsId",
                table: "msureRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_partnersProducts_Partners_PartnerId",
                table: "partnersProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_partnersProducts_Products_ProductId",
                table: "partnersProducts");

            migrationBuilder.DropTable(
                name: "CallBackResponse");

            migrationBuilder.DropTable(
                name: "customerProducts");

            migrationBuilder.DropTable(
                name: "FuneralExpenseOnboardings");

            migrationBuilder.DropTable(
                name: "FuneralExpenseQuotations");

            migrationBuilder.DropTable(
                name: "MemberHealths");

            migrationBuilder.DropTable(
                name: "MemberPayments");

            migrationBuilder.DropTable(
                name: "mpesaToken");

            migrationBuilder.DropTable(
                name: "OTPOnboardings");

            migrationBuilder.DropTable(
                name: "OTPs");

            migrationBuilder.DropTable(
                name: "PolicyActivations");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "PaymentContributions");

            migrationBuilder.DropTable(
                name: "mpesaSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_partnersProducts",
                table: "partnersProducts");

            migrationBuilder.DropIndex(
                name: "IX_partnersProducts_PartnerId",
                table: "partnersProducts");

            migrationBuilder.DropIndex(
                name: "IX_partnersProducts_ProductId",
                table: "partnersProducts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "partnersProducts");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "partnersProducts");

            migrationBuilder.DropColumn(
                name: "AverageIncome",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Firstname",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MemberNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MemberNo",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "PartnerCode",
                table: "Beneficiaries");

            migrationBuilder.RenameTable(
                name: "partnersProducts",
                newName: "PartnersProducts");

            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "Guardian",
                newName: "Dob");

            migrationBuilder.RenameColumn(
                name: "MemberNo",
                table: "Customers",
                newName: "Surname");

            migrationBuilder.RenameColumn(
                name: "Fullname",
                table: "Customers",
                newName: "CustomerName");

            migrationBuilder.AlterColumn<string>(
                name: "PartnerId",
                table: "PartnersProducts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "PartnersProducts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartnerId1",
                table: "PartnersProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Step",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Stage",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "RecordProcessed",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Processed",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "MailSent",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Customers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Complete",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AverageIncomeId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartnersProducts",
                table: "PartnersProducts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PartnersProducts_PartnerId1",
                table: "PartnersProducts",
                column: "PartnerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_msureRequests_PartnersProducts_ProductsId",
                table: "msureRequests",
                column: "ProductsId",
                principalTable: "PartnersProducts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnersProducts_Partners_PartnerId1",
                table: "PartnersProducts",
                column: "PartnerId1",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
