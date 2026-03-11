using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsbJson.API.Migrations
{
    /// <inheritdoc />
    public partial class HealthDeclarationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_CustomerHealthDeclarations_CustomerId_ContextKey",
                table: "CustomerHealthDeclarations",
                columns: new[] { "CustomerId", "ContextKey" },
                unique: true,
                filter: "[ContextKey] IS NOT NULL");

            migrationBuilder.CreateTable(
                name: "CustomerHealthDeclarationAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeclarationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Question = table.Column<int>(type: "int", nullable: false),
                    Answer = table.Column<bool>(type: "bit", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(max)", nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_CustomerHealthDeclarationAnswers_DeclarationId",
                table: "CustomerHealthDeclarationAnswers",
                column: "DeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerHealthDeclarationAnswers_DeclarationId_Question",
                table: "CustomerHealthDeclarationAnswers",
                columns: new[] { "DeclarationId", "Question" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "CustomerHealthDeclarationAnswers");
            migrationBuilder.DropTable(name: "CustomerHealthDeclarations");
        }
    }
}
