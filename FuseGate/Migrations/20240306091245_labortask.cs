using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsbJson.API.Migrations
{
    /// <inheritdoc />
    public partial class labortask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LabourCost",
                table: "PartCosts");

            migrationBuilder.CreateTable(
                name: "LabourCost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Desc = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Cost = table.Column<double>(type: "float", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    DeletedByUser = table.Column<string>(type: "nvarchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabourCost", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairShops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Phonenumber = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ShopLocation = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    County = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Subcountry = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Ward = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Town = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    SecondaryPhone = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    ShopOwner = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    iSActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    DeletedByUser = table.Column<string>(type: "nvarchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairShops", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabourCost");

            migrationBuilder.DropTable(
                name: "RepairShops");

            migrationBuilder.AddColumn<double>(
                name: "LabourCost",
                table: "PartCosts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
