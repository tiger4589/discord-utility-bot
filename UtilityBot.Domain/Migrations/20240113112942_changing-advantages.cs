using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UtilityBot.Domain.Migrations
{
    /// <inheritdoc />
    public partial class changingadvantages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Nebula", "Mirage" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdvantageOn",
                value: "Crystal");

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Inferno", "Crystal" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Oasis", "Inferno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Frostbite", "Oasis" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Techno", "Frostbite" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Coral", "Jungle" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Serenity", "Coral" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Mirage", "Serenity" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Nebula", "Mirage" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Crystal", "Nebula" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Inferno", "Crystal" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Oasis", "Inferno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Frostbite", "Oasis" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Techno", "Frostbite" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Coral", "Jungle" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Serenity", "Coral" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Mirage", "Serenity" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Nebula", "Mirage" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Crystal", "Nebula" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 23,
                column: "AdvantageOn",
                value: "Inferno");

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Oasis", "Inferno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 25,
                column: "DisadvantageOn",
                value: "Oasis");

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Techno", "Frostbite" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Coral", "Jungle" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Serenity", "Coral" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Mirage", "Serenity" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Nebula", "Mirage" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 32,
                column: "AdvantageOn",
                value: "Crystal");

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Inferno", "Crystal" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Oasis", "Inferno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Frostbite", "Oasis" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 36,
                column: "DisadvantageOn",
                value: "Frostbite");

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Coral", "Jungle" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 39,
                column: "DisadvantageOn",
                value: "Coral");

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 40,
                column: "DisadvantageOn",
                value: "Serenity");

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Nebula", "Mirage" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Crystal", "Nebula" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Inferno", "Crystal" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 44,
                column: "DisadvantageOn",
                value: "Inferno");

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Frostbite", "Oasis" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Techno", "Frostbite" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Jungle", "Techno" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Coral", "Jungle" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Serenity", "Coral" });

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "AdvantageOn", "DisadvantageOn" },
                values: new object[] { "Mirage", "Serenity" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                column: "AdvantageOn",
                value: "Mirage");

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
                column: "AdvantageOn",
                value: "Serenity");

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
                column: "DisadvantageOn",
                value: "Serenity");

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
                column: "AdvantageOn",
                value: "Mirage");

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
                column: "DisadvantageOn",
                value: "Oasis");

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
                column: "DisadvantageOn",
                value: "Crystal");

            migrationBuilder.UpdateData(
                table: "Horses",
                keyColumn: "Id",
                keyValue: 40,
                column: "DisadvantageOn",
                value: "Nebula");

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
                column: "DisadvantageOn",
                value: "Serenity");

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
        }
    }
}
