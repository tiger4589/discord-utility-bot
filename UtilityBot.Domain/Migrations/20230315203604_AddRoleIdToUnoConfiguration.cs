using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UtilityBot.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleIdToUnoConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RoleId",
                table: "UnoConfigurations",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "UnoConfigurations");
        }
    }
}
