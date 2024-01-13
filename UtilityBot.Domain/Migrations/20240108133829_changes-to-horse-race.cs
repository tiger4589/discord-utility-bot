using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UtilityBot.Domain.Migrations
{
    /// <inheritdoc />
    public partial class changestohorserace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RunnerUpId",
                table: "HorseRaces");

            migrationBuilder.DropColumn(
                name: "ThirdPlaceId",
                table: "HorseRaces");

            migrationBuilder.DropColumn(
                name: "WinnerId",
                table: "HorseRaces");

            migrationBuilder.AddColumn<DateTime>(
                name: "RaceDate",
                table: "HorseRaces",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "RaceStandings",
                columns: table => new
                {
                    RaceId = table.Column<int>(type: "int", nullable: false),
                    HorseId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceStandings", x => new { x.RaceId, x.HorseId });
                });

            migrationBuilder.CreateTable(
                name: "UserPredictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    CorrectPredictions = table.Column<int>(type: "int", nullable: false),
                    WrongPredictions = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPredictions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPredictions_UserId",
                table: "UserPredictions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaceStandings");

            migrationBuilder.DropTable(
                name: "UserPredictions");

            migrationBuilder.DropColumn(
                name: "RaceDate",
                table: "HorseRaces");

            migrationBuilder.AddColumn<int>(
                name: "RunnerUpId",
                table: "HorseRaces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThirdPlaceId",
                table: "HorseRaces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WinnerId",
                table: "HorseRaces",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
