using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UtilityBot.Domain.Migrations
{
    /// <inheritdoc />
    public partial class firstmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JoinedServers",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActivated = table.Column<bool>(type: "bit", nullable: false),
                    IsConnected = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinedServers", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguredActions",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ConfigurationType = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguredActions", x => new { x.GuildId, x.ConfigurationType });
                    table.ForeignKey(
                        name: "FK_ConfiguredActions_JoinedServers_GuildId",
                        column: x => x.GuildId,
                        principalTable: "JoinedServers",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserJoinConfigurations",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJoinConfigurations", x => new { x.GuildId, x.Action });
                    table.ForeignKey(
                        name: "FK_UserJoinConfigurations_JoinedServers_GuildId",
                        column: x => x.GuildId,
                        principalTable: "JoinedServers",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserJoinMessages",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    ChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJoinMessages", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_UserJoinMessages_JoinedServers_GuildId",
                        column: x => x.GuildId,
                        principalTable: "JoinedServers",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserJoinRoles",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    RoleId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJoinRoles", x => new { x.GuildId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserJoinRoles_JoinedServers_GuildId",
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
                name: "ConfiguredActions");

            migrationBuilder.DropTable(
                name: "UserJoinConfigurations");

            migrationBuilder.DropTable(
                name: "UserJoinMessages");

            migrationBuilder.DropTable(
                name: "UserJoinRoles");

            migrationBuilder.DropTable(
                name: "JoinedServers");
        }
    }
}
