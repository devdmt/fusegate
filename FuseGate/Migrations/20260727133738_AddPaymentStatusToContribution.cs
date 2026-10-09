using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsbJson.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentStatusToContribution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "Contribution",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Productenum",
                table: "CallBackResponse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CustomerApiLeads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DateOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductEnum = table.Column<int>(type: "int", nullable: false),
                    StartingContribution = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MonthlyContribution = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PartnerCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartnerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetirementAge = table.Column<int>(type: "int", nullable: false),
                    Names = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerApiLeads", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerApiLeads");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Contribution");

            migrationBuilder.DropColumn(
                name: "Productenum",
                table: "CallBackResponse");
        }
    }
}
