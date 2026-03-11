using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsbJson.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_APIUSERS_Partners_PartnerId",
                table: "APIUSERS");

            migrationBuilder.DropForeignKey(
                name: "FK_MsureRequests_Partners_PartnersId",
                table: "MsureRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_MsureRequests_partnersProducts_ProductsId",
                table: "MsureRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_partnersProducts_Partners_PartnerId",
                table: "partnersProducts");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "claimRequests");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OnboardingRequests");

            migrationBuilder.DropTable(
                name: "PartCosts");

            migrationBuilder.DropTable(
                name: "PartnerAdminUser");

            migrationBuilder.DropTable(
                name: "PhoneInsuranceRequest");

            migrationBuilder.DropTable(
                name: "PortalActions");

            migrationBuilder.DropTable(
                name: "RepairShops");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "phoneInsuranceCustomers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_partnersProducts",
                table: "partnersProducts");

            migrationBuilder.DropIndex(
                name: "IX_partnersProducts_PartnerId",
                table: "partnersProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MsureRequests",
                table: "MsureRequests");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "APIUSERS");

            migrationBuilder.DropIndex(
                name: "IX_APIUSERS_PartnerId",
                table: "APIUSERS");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "APIUSERS");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "partnersProducts");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "APIUSERS");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "APIUSERS");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "APIUSERS");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "APIUSERS");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "APIUSERS");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "APIUSERS");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "APIUSERS");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "APIUSERS");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "APIUSERS");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "APIUSERS");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "APIUSERS");

            migrationBuilder.RenameTable(
                name: "partnersProducts",
                newName: "PartnersProducts");

            migrationBuilder.RenameTable(
                name: "MsureRequests",
                newName: "msureRequests");

            migrationBuilder.RenameIndex(
                name: "IX_MsureRequests_ProductsId",
                table: "msureRequests",
                newName: "IX_msureRequests_ProductsId");

            migrationBuilder.RenameIndex(
                name: "IX_MsureRequests_PartnersId",
                table: "msureRequests",
                newName: "IX_msureRequests_PartnersId");

            migrationBuilder.RenameColumn(
                name: "AccessFailedCount",
                table: "APIUSERS",
                newName: "accountType");

            migrationBuilder.AlterColumn<string>(
                name: "PartnerId",
                table: "PartnersProducts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartnerId1",
                table: "PartnersProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "PartnerId",
                table: "APIUSERS",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsEnabled",
                table: "APIUSERS",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "ConsumerKey",
                table: "APIUSERS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Configuration",
                table: "APIUSERS",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartnersProducts",
                table: "PartnersProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_msureRequests",
                table: "msureRequests",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PartnerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Residency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Processed = table.Column<bool>(type: "bit", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

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
                name: "FK_msureRequests_Partners_PartnersId",
                table: "msureRequests",
                column: "PartnersId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnersProducts_Partners_PartnerId1",
                table: "PartnersProducts",
                column: "PartnerId1",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_msureRequests_PartnersProducts_ProductsId",
                table: "msureRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_msureRequests_Partners_PartnersId",
                table: "msureRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnersProducts_Partners_PartnerId1",
                table: "PartnersProducts");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartnersProducts",
                table: "PartnersProducts");

            migrationBuilder.DropIndex(
                name: "IX_PartnersProducts_PartnerId1",
                table: "PartnersProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_msureRequests",
                table: "msureRequests");

            migrationBuilder.DropColumn(
                name: "PartnerId1",
                table: "PartnersProducts");

            migrationBuilder.RenameTable(
                name: "PartnersProducts",
                newName: "partnersProducts");

            migrationBuilder.RenameTable(
                name: "msureRequests",
                newName: "MsureRequests");

            migrationBuilder.RenameIndex(
                name: "IX_msureRequests_ProductsId",
                table: "MsureRequests",
                newName: "IX_MsureRequests_ProductsId");

            migrationBuilder.RenameIndex(
                name: "IX_msureRequests_PartnersId",
                table: "MsureRequests",
                newName: "IX_MsureRequests_PartnersId");

            migrationBuilder.RenameColumn(
                name: "accountType",
                table: "APIUSERS",
                newName: "AccessFailedCount");

            migrationBuilder.AlterColumn<int>(
                name: "PartnerId",
                table: "partnersProducts",
                type: "int",
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

            migrationBuilder.AlterColumn<int>(
                name: "PartnerId",
                table: "APIUSERS",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsEnabled",
                table: "APIUSERS",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConsumerKey",
                table: "APIUSERS",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Configuration",
                table: "APIUSERS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "APIUSERS",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "APIUSERS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "APIUSERS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "APIUSERS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "APIUSERS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "APIUSERS",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "APIUSERS",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "APIUSERS",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "APIUSERS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "APIUSERS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "APIUSERS",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_partnersProducts",
                table: "partnersProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MsureRequests",
                table: "MsureRequests",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_APIUSERS_UserId",
                        column: x => x.UserId,
                        principalTable: "APIUSERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_APIUSERS_UserId",
                        column: x => x.UserId,
                        principalTable: "APIUSERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_APIUSERS_UserId",
                        column: x => x.UserId,
                        principalTable: "APIUSERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notificationType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnboardingRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PartnerId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    BeneficiaryMobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BeneficiaryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BenefitOption = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Premium = table.Column<double>(type: "float", nullable: true),
                    Processed = table.Column<bool>(type: "bit", nullable: false),
                    RegNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnboardingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnboardingRequests_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OnboardingRequests_partnersProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "partnersProducts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PartCosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaborCost = table.Column<double>(type: "float", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartCosts = table.Column<double>(type: "float", nullable: false),
                    ReplacementLimit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartCosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartnerAdminUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Configuration = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HostPort = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HostUrl = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    loginType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerAdminUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "phoneInsuranceCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IdNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NextofkinId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nextofkinname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phoneInsuranceCustomers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortalActions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerIdNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedByUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IncidenceDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    PhoneModel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shopname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairShops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    County = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedByUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    Phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondaryPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShopOwner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subcountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Town = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ward = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    iSActive = table.Column<bool>(type: "bit", nullable: false),
                    loginType = table.Column<int>(type: "int", nullable: false),
                    shopType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairShops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_APIUSERS_UserId",
                        column: x => x.UserId,
                        principalTable: "APIUSERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "claimRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneInsuranceCustomerId = table.Column<int>(type: "int", nullable: false),
                    Abstract = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClaimRefNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClaimType = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DamagePart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dispatched = table.Column<bool>(type: "bit", nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IMEINumber1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IMEINumber2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncidentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LabourCost = table.Column<double>(type: "float", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartCost = table.Column<double>(type: "float", nullable: false),
                    PartId = table.Column<int>(type: "int", nullable: true),
                    PartnerCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartnerID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Processed = table.Column<bool>(type: "bit", nullable: false),
                    ProductID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacementCost = table.Column<double>(type: "float", nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    claimStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_claimRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_claimRequests_phoneInsuranceCustomers_PhoneInsuranceCustomerId",
                        column: x => x.PhoneInsuranceCustomerId,
                        principalTable: "phoneInsuranceCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneInsuranceRequest",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PhoneInsuranceCustomerId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: true),
                    IMEINumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IMEINumber1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IMEINumber2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterestRate = table.Column<double>(type: "float", nullable: true),
                    LoanAmount = table.Column<double>(type: "float", nullable: true),
                    LoanRefNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ModeOfPurchase = table.Column<int>(type: "int", nullable: true),
                    PartnerID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneCost = table.Column<double>(type: "float", nullable: true),
                    PhoneModel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PolicyStatus = table.Column<int>(type: "int", nullable: true),
                    PremiumPaid = table.Column<double>(type: "float", nullable: true),
                    Processed = table.Column<bool>(type: "bit", nullable: true),
                    ProductID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PurchaseDate = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RepaymentTerms = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneInsuranceRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhoneInsuranceRequest_phoneInsuranceCustomers_PhoneInsuranceCustomerId",
                        column: x => x.PhoneInsuranceCustomerId,
                        principalTable: "phoneInsuranceCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_partnersProducts_PartnerId",
                table: "partnersProducts",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "APIUSERS",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_APIUSERS_PartnerId",
                table: "APIUSERS",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "APIUSERS",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_claimRequests_PhoneInsuranceCustomerId",
                table: "claimRequests",
                column: "PhoneInsuranceCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingRequests_PartnerId",
                table: "OnboardingRequests",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingRequests_ProductId",
                table: "OnboardingRequests",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneInsuranceRequest_PhoneInsuranceCustomerId",
                table: "PhoneInsuranceRequest",
                column: "PhoneInsuranceCustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_APIUSERS_Partners_PartnerId",
                table: "APIUSERS",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MsureRequests_Partners_PartnersId",
                table: "MsureRequests",
                column: "PartnersId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MsureRequests_partnersProducts_ProductsId",
                table: "MsureRequests",
                column: "ProductsId",
                principalTable: "partnersProducts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_partnersProducts_Partners_PartnerId",
                table: "partnersProducts",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id");
        }
    }
}
