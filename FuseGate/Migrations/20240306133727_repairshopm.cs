using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsbJson.API.Migrations
{
    /// <inheritdoc />
    public partial class repairshopm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabourCost");

            migrationBuilder.AddColumn<int>(
                name: "loginType",
                table: "RepairShops",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "shopType",
                table: "RepairShops",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PartCosts",
                table: "PartCosts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "loginType",
                table: "RepairShops");

            migrationBuilder.DropColumn(
                name: "shopType",
                table: "RepairShops");

            migrationBuilder.DropColumn(
                name: "PartCosts",
                table: "PartCosts");

            migrationBuilder.CreateTable(
                name: "LabourCost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cost = table.Column<double>(type: "float", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedByUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabourCost", x => x.Id);
                });
        }
    }
}
