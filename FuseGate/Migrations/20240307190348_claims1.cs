using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsbJson.API.Migrations
{
    /// <inheritdoc />
    public partial class claims1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IMEINO",
                table: "claimRequests",
                newName: "Narration");

            migrationBuilder.AddColumn<string>(
                name: "IMEINumber1",
                table: "claimRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IMEINumber2",
                table: "claimRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LabourCost",
                table: "claimRequests",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PartCost",
                table: "claimRequests",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IMEINumber1",
                table: "claimRequests");

            migrationBuilder.DropColumn(
                name: "IMEINumber2",
                table: "claimRequests");

            migrationBuilder.DropColumn(
                name: "LabourCost",
                table: "claimRequests");

            migrationBuilder.DropColumn(
                name: "PartCost",
                table: "claimRequests");

            migrationBuilder.RenameColumn(
                name: "Narration",
                table: "claimRequests",
                newName: "IMEINO");
        }
    }
}
