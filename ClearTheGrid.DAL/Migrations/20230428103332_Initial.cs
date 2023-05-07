using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearTheGrid.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LevelSolutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelSolutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GenerationCount = table.Column<int>(type: "int", nullable: false),
                    PopulationSize = table.Column<int>(type: "int", nullable: false),
                    SelectionCount = table.Column<int>(type: "int", nullable: false),
                    MutationFactor = table.Column<double>(type: "float", nullable: false),
                    CrossoverFactor = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResultMoves",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LevelSolutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Move = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultMoves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultMoves_LevelSolutions_LevelSolutionId",
                        column: x => x.LevelSolutionId,
                        principalTable: "LevelSolutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "CrossoverFactor", "GenerationCount", "MutationFactor", "Name", "PopulationSize", "SelectionCount" },
                values: new object[] { new Guid("a829abb3-b012-4597-a2a2-15374a36cd8c"), 0.050000000000000003, 10000, 0.050000000000000003, "BaseSettings", 250, 15 });

            migrationBuilder.CreateIndex(
                name: "IX_ResultMoves_LevelSolutionId",
                table: "ResultMoves",
                column: "LevelSolutionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultMoves");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "LevelSolutions");
        }
    }
}
