using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UtilityBot.Domain.Migrations
{
    /// <inheritdoc />
    public partial class addingadvantagesanddisadvantages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdvantageOn",
                table: "Horses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisadvantageOn",
                table: "Horses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Crystal", "Jungle" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Mirage", "Nebula" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Frostbite", "Serenity" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Coral", "Inferno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Serenity", "Crystal" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Mirage", "Nebula" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Frostbite" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Nebula", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Frostbite", "Coral" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Techno", "Serenity" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Coral", "Inferno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Nebula" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Frostbite", "Serenity" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Serenity", "Crystal" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Mirage", "Nebula" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Frostbite", "Serenity" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Coral", "Inferno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Techno", "Serenity" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Coral", "Inferno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Serenity", "Crystal" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Mirage", "Nebula" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Frostbite", "Serenity" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Oasis", "Inferno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Techno", "Serenity" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Coral", "Inferno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Serenity", "Crystal" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Mirage", "Nebula" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Frostbite", "Serenity" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Coral", "Inferno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Techno", "Oasis" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Coral", "Inferno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Serenity", "Crystal" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Mirage", "Nebula" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Frostbite", "Serenity" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Coral", "Inferno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Oasis", "Serenity" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Coral", "Inferno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Serenity", "Crystal" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Mirage", "Nebula" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Frostbite", "Serenity" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Length", "Type" },
                values: new object[] { 100, "Nebula" });

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Length", "Type" },
                values: new object[] { 150, "Crystal" });

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Length", "Type" },
                values: new object[] { 200, "Inferno" });

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Length", "Type" },
                values: new object[] { 250, "Oasis" });

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Length", "Type" },
                values: new object[] { 300, "Frostbite" });

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Length", "Type" },
                values: new object[] { 350, "Techno" });

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Length", "Type" },
                values: new object[] { 400, "Jungle" });

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Length", "Type" },
                values: new object[] { 450, "Coral" });

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Length", "Type" },
                values: new object[] { 500, "Serenity" });

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Length", "Type" },
                values: new object[] { 1000, "Mirage" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "AdvantageOn",
                table: "Horses");

            migrationBuilder.DropColumn(
                name: "DisadvantageOn",
                table: "Horses");

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 1,
                column: "Length",
                value: 2000);

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 2,
                column: "Length",
                value: 1500);

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 3,
                column: "Length",
                value: 2500);

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 4,
                column: "Length",
                value: 1800);

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 5,
                column: "Length",
                value: 2200);

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 6,
                column: "Length",
                value: 2700);

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 7,
                column: "Length",
                value: 3000);

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 8,
                column: "Length",
                value: 1200);

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 9,
                column: "Length",
                value: 2000);

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: 10,
                column: "Length",
                value: 1600);
        }
    }
}
