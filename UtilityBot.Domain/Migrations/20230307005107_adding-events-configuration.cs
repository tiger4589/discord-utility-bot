using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UtilityBot.Domain.Migrations
{
    /// <inheritdoc />
    public partial class addingeventsconfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventsConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventsConfigurations", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "EventsConfigurations",
                columns: new[] { "Id", "EventName", "IsEnabled" },
                values: new object[,]
                {
                    { 1, "UserJoined", true },
                    { 2, "UserUnbanned", true },
                    { 3, "UserLeft", true },
                    { 4, "UserBanned", true },
                    { 5, "RoleCreated", true },
                    { 6, "RoleDeleted", true },
                    { 7, "RoleUpdated", true },
                    { 8, "GuildMemberUpdated", true },
                    { 9, "UserUpdated", true },
                    { 10, "MessageUpdated", true },
                    { 11, "MessageDeleted", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventsConfigurations");
        }
    }
}
