using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsbJson.API.Migrations
{
    /// <inheritdoc />
    public partial class phonecustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "PhoneInsuranceRequest");

            migrationBuilder.DropColumn(
                name: "DateofBirth",
                table: "PhoneInsuranceRequest");

            migrationBuilder.RenameColumn(
                name: "ResponseId",
                table: "PhoneInsuranceRequest",
                newName: "RequestId");

            migrationBuilder.RenameColumn(
                name: "ResponseId",
                table: "claimRequests",
                newName: "RequestId");

            migrationBuilder.AddColumn<int>(
                name: "PhoneInsuranceCustomerId",
                table: "PhoneInsuranceRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PhoneInsuranceCustomerId",
                table: "claimRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "phoneInsuranceCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IdNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Nextofkinname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextofkinId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phoneInsuranceCustomers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhoneInsuranceRequest_PhoneInsuranceCustomerId",
                table: "PhoneInsuranceRequest",
                column: "PhoneInsuranceCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_claimRequests_PhoneInsuranceCustomerId",
                table: "claimRequests",
                column: "PhoneInsuranceCustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_claimRequests_phoneInsuranceCustomers_PhoneInsuranceCustomerId",
                table: "claimRequests",
                column: "PhoneInsuranceCustomerId",
                principalTable: "phoneInsuranceCustomers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhoneInsuranceRequest_phoneInsuranceCustomers_PhoneInsuranceCustomerId",
                table: "PhoneInsuranceRequest",
                column: "PhoneInsuranceCustomerId",
                principalTable: "phoneInsuranceCustomers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_claimRequests_phoneInsuranceCustomers_PhoneInsuranceCustomerId",
                table: "claimRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PhoneInsuranceRequest_phoneInsuranceCustomers_PhoneInsuranceCustomerId",
                table: "PhoneInsuranceRequest");

            migrationBuilder.DropTable(
                name: "phoneInsuranceCustomers");

            migrationBuilder.DropIndex(
                name: "IX_PhoneInsuranceRequest_PhoneInsuranceCustomerId",
                table: "PhoneInsuranceRequest");

            migrationBuilder.DropIndex(
                name: "IX_claimRequests_PhoneInsuranceCustomerId",
                table: "claimRequests");

            migrationBuilder.DropColumn(
                name: "PhoneInsuranceCustomerId",
                table: "PhoneInsuranceRequest");

            migrationBuilder.DropColumn(
                name: "PhoneInsuranceCustomerId",
                table: "claimRequests");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "PhoneInsuranceRequest",
                newName: "ResponseId");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "claimRequests",
                newName: "ResponseId");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "PhoneInsuranceRequest",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateofBirth",
                table: "PhoneInsuranceRequest",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
