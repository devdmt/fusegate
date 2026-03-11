using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsbJson.API.Migrations
{
    /// <inheritdoc />
    public partial class beneficiary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Processed",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalSourceOfIncome",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AverageIncomeId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessName",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Complete",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EmployerName",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmploymentTerms",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdType",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroupMember",
                table: "Customers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MailSent",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "MailSentOn",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Modified",
                table: "Customers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NatureOfBusiness",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherNames",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pin",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PolicyPath",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RecordProcessed",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleInBusiness",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignaturePath",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceOfIncome",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stage",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Step",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxIdNumber",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "USAddress",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Validated",
                table: "Customers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidationDate",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ValidationErrorCount",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidationErrors",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerHealthDeclarations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ContextKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HeightCm = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    WeightKg = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    Bmi = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    HealthDeclaration = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerHealthDeclarations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guardian",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryId = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    OtherNames = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Dob = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    IdNumber = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    IdType = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Relationship = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    PartnerResponseDesc = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    PartnerResponseCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Approved = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    UpdatedName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    DeletedByUser = table.Column<string>(type: "nvarchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guardian", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerHealthDeclarationAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeclarationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Question = table.Column<int>(type: "int", nullable: false),
                    Answer = table.Column<bool>(type: "bit", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(Max)", maxLength: 2147483647, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerHealthDeclarationAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerHealthDeclarationAnswers_CustomerHealthDeclarations_DeclarationId",
                        column: x => x.DeclarationId,
                        principalTable: "CustomerHealthDeclarations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Beneficiaries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PartnerBeneficiaryCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Firstname = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    OtherNames = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Relationship = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    IdNumber = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Date_of_birthRaw = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Date_of_birth = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    GuardianId = table.Column<long>(type: "bigint", nullable: true),
                    PartnerResponseDesc = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    PartnerResponseCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Approved = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Confirmed = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedByUser = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_Guardian_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardian",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_GuardianId",
                table: "Beneficiaries",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerHealthDeclarationAnswers_DeclarationId",
                table: "CustomerHealthDeclarationAnswers",
                column: "DeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerHealthDeclarationAnswers_DeclarationId_Question",
                table: "CustomerHealthDeclarationAnswers",
                columns: new[] { "DeclarationId", "Question" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerHealthDeclarations_CustomerId_ContextKey",
                table: "CustomerHealthDeclarations",
                columns: new[] { "CustomerId", "ContextKey" },
                unique: true,
                filter: "[ContextKey] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beneficiaries");

            migrationBuilder.DropTable(
                name: "CustomerHealthDeclarationAnswers");

            migrationBuilder.DropTable(
                name: "Guardian");

            migrationBuilder.DropTable(
                name: "CustomerHealthDeclarations");

            migrationBuilder.DropColumn(
                name: "AdditionalSourceOfIncome",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AverageIncomeId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BusinessName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Complete",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmployerName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmploymentTerms",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IdType",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsGroupMember",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MailSent",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MailSentOn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "NatureOfBusiness",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "OtherNames",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Pin",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PolicyPath",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "RecordProcessed",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "RoleInBusiness",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Signature",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "SignaturePath",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "SourceOfIncome",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Stage",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Step",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "TaxIdNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "USAddress",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Validated",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ValidationDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ValidationErrorCount",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ValidationErrors",
                table: "Customers");

            migrationBuilder.AlterColumn<bool>(
                name: "Processed",
                table: "Customers",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
