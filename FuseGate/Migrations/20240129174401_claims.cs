using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsbJson.API.Migrations
{
    /// <inheritdoc />
    public partial class claims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "Customername",
            //    table: "MsureRequests",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Gender",
            //    table: "MsureRequests",
            //    type: "nvarchar(max)",
            //    nullable: true);

            migrationBuilder.CreateTable(
                name: "claimRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartnerID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimRefNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IMEINO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimType = table.Column<int>(type: "int", nullable: true),
                    DamagePart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacementCost = table.Column<double>(type: "float", nullable: true),
                    IncidentDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClaimDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abstract = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Processed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_claimRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhoneInsuranceRequest",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PartnerID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProductID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DateofBirth = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneModel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IMEINumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneCost = table.Column<double>(type: "float", nullable: true),
                    ModeOfPurchase = table.Column<int>(type: "int", nullable: true),
                    LoanRefNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RepaymentTerms = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LoanAmount = table.Column<double>(type: "float", nullable: true),
                    InterestRate = table.Column<double>(type: "float", nullable: true),
                    PremiumPaid = table.Column<double>(type: "float", nullable: true),
                    PurchaseDate = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Processed = table.Column<bool>(type: "bit", nullable: true),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PolicyStatus = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneInsuranceRequest", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "claimRequests");

            migrationBuilder.DropTable(
                name: "PhoneInsuranceRequest");

            migrationBuilder.DropColumn(
                name: "Customername",
                table: "MsureRequests");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "MsureRequests");
        }
    }
}
