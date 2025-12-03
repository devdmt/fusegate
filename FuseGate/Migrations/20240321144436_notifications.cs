using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsbJson.API.Migrations
{
    /// <inheritdoc />
    public partial class notifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "PhoneInsuranceRequest",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IMEINumber1",
                table: "PhoneInsuranceRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IMEINumber2",
                table: "PhoneInsuranceRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneName",
                table: "PhoneInsuranceRequest",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Dispatched",
                table: "claimRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "PhoneInsuranceRequest");

            migrationBuilder.DropColumn(
                name: "IMEINumber1",
                table: "PhoneInsuranceRequest");

            migrationBuilder.DropColumn(
                name: "IMEINumber2",
                table: "PhoneInsuranceRequest");

            migrationBuilder.DropColumn(
                name: "PhoneName",
                table: "PhoneInsuranceRequest");

            migrationBuilder.DropColumn(
                name: "Dispatched",
                table: "claimRequests");
        }
    }
}
