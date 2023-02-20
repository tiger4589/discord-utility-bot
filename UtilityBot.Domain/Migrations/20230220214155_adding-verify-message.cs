using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UtilityBot.Domain.Migrations
{
    /// <inheritdoc />
    public partial class addingverifymessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "VerifyConfigurations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "VerifyConfigurationAudits",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "VerifyConfigurations");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "VerifyConfigurationAudits");
        }
    }
}
