using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UtilityBot.Domain.Migrations
{
    /// <inheritdoc />
    public partial class horseraces2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HorseRaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrackId = table.Column<int>(type: "int", nullable: false),
                    WinnerId = table.Column<int>(type: "int", nullable: false),
                    RunnerUpId = table.Column<int>(type: "int", nullable: false),
                    ThirdPlaceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorseRaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Horses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OddsToOne = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Length = table.Column<int>(type: "int", nullable: false),
                    TimeBetweenRacesInMinutes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Horses",
                columns: new[] { "Id", "Name", "OddsToOne" },
                values: new object[,]
                {
                    { 1, "Thunderhoof", 2 },
                    { 2, "Midnight Mirage", 3 },
                    { 3, "Blaze Lightning", 4 },
                    { 4, "Mystic Gallop", 5 },
                    { 5, "Starstrider", 6 },
                    { 6, "Shadowfax", 7 },
                    { 7, "Solar Eclipse", 8 },
                    { 8, "Velvet Victory", 9 },
                    { 9, "Lunar Lullaby", 10 },
                    { 10, "Silver Streak", 11 },
                    { 11, "Quantum Quicksilver", 12 },
                    { 12, "Celestial Sprinter", 2 },
                    { 13, "Ember Echo", 3 },
                    { 14, "Neptune's Nudge", 4 },
                    { 15, "Serendipity Stride", 5 },
                    { 16, "Dreamweaver", 6 },
                    { 17, "Galactic Glide", 7 },
                    { 18, "Aurora Borealis", 8 },
                    { 19, "Phoenix Fury", 9 },
                    { 20, "Nebula Nova", 10 },
                    { 21, "Velvet Vortex", 11 },
                    { 22, "Whispering Wind", 12 },
                    { 23, "Radiant Rainstorm", 2 },
                    { 24, "Spectral Sprint", 3 },
                    { 25, "Dragonfly Dash", 4 },
                    { 26, "Enchanted Equestrian", 5 },
                    { 27, "Pegasus Prism", 6 },
                    { 28, "Radiant Racer", 7 },
                    { 29, "Zenith Zephyr", 8 },
                    { 30, "Cosmic Cascade", 9 },
                    { 31, "Twilight Thunderbolt", 10 },
                    { 32, "Mystic Moonlight", 11 },
                    { 33, "Nebulous Nectar", 12 },
                    { 34, "Stardust Sizzle", 2 },
                    { 35, "Quantum Quasar", 3 },
                    { 36, "Shadow Symphony", 4 },
                    { 37, "Celestial Cyclone", 5 },
                    { 38, "Luminous Lightning", 6 },
                    { 39, "Velvet Vibration", 7 },
                    { 40, "Solar Serenity", 8 },
                    { 41, "Comet Canter", 9 },
                    { 42, "Starlight Symphony", 10 },
                    { 43, "Phoenix Phenomenon", 11 },
                    { 44, "Midnight Melody", 12 },
                    { 45, "Silver Seraph", 2 },
                    { 46, "Lunar Luminary", 4 },
                    { 47, "Nebula Nomad", 6 },
                    { 48, "Whispering Whirlwind", 8 },
                    { 49, "Ethereal Elixir", 10 },
                    { 50, "Radiant Roamer", 12 }
                });

            migrationBuilder.InsertData(
                table: "Tracks",
                columns: new[] { "Id", "Length", "Name", "TimeBetweenRacesInMinutes" },
                values: new object[,]
                {
                    { 1, 2000, "Galactic Grand Circuit", 5 },
                    { 2, 1500, "Celestial Speedway", 10 },
                    { 3, 2500, "Nebula Nook Raceway", 15 },
                    { 4, 1800, "Lunar Loop Lanes", 20 },
                    { 5, 2200, "Cosmic Carousel Circuit", 25 },
                    { 6, 2700, "Starry Stride Speedway", 30 },
                    { 7, 3000, "Enchanted Equine Expressway", 35 },
                    { 8, 1200, "Solar Sprints Circuit", 40 },
                    { 9, 2000, "Midnight Mirage Oval", 45 },
                    { 10, 1600, "Mystic Meadow Racetrack", 50 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HorseRaces");

            migrationBuilder.DropTable(
                name: "Horses");

            migrationBuilder.DropTable(
                name: "Tracks");
        }
    }
}
