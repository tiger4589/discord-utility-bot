using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UtilityBot.Domain.Migrations
{
    /// <inheritdoc />
    public partial class addinglogconfigurationchannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogConfigurations",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogConfigurations", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_LogConfigurations_JoinedServers_GuildId",
                        column: x => x.GuildId,
                        principalTable: "JoinedServers",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogConfigurations");
        }
    }
}
