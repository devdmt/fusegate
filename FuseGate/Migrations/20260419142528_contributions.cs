using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsbJson.API.Migrations
{
    /// <inheritdoc />
    public partial class contributions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverPartyIdentifierType",
                table: "mpesaSettings");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "mpesaSettings");

            migrationBuilder.DropColumn(
                name: "MemberNumber",
                table: "Customers");

            migrationBuilder.AddColumn<bool>(
                name: "CallbackProcessed",
                table: "CallBackResponse",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "CallBackResponse",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ProcessResponse",
                table: "CallBackResponse",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "CallBackResponse",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Contribution",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomerProductId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Product = table.Column<int>(type: "int", nullable: false),
                    RefNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProductRef = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MemberNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    Processed = table.Column<int>(type: "int", nullable: false),
                    Acknowledged = table.Column<bool>(type: "bit", nullable: false),
                    PartnerId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentGatewayRef = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcknowledgedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentMode = table.Column<int>(type: "int", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contribution", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contribution");

            migrationBuilder.DropColumn(
                name: "CallbackProcessed",
                table: "CallBackResponse");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "CallBackResponse");

            migrationBuilder.DropColumn(
                name: "ProcessResponse",
                table: "CallBackResponse");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "CallBackResponse");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverPartyIdentifierType",
                table: "mpesaSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionType",
                table: "mpesaSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MemberNumber",
                table: "Customers",
                type: "bit",
                nullable: true);
        }
    }
}
