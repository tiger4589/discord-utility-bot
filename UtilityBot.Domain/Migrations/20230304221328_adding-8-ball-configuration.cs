using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UtilityBot.Domain.Migrations
{
    /// <inheritdoc />
    public partial class adding8ballconfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MagicEightBallConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicEightBallConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MagicEightBallResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicEightBallResponses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "MagicEightBallResponses",
                columns: new[] { "Id", "Message" },
                values: new object[,]
                {
                    { 1, "It is certain." },
                    { 2, "It is decidedly so." },
                    { 3, "Without a doubt." },
                    { 4, "Yes definitely." },
                    { 5, "You may rely on it." },
                    { 6, "As I see it, yes." },
                    { 7, "Most likely." },
                    { 8, "Outlook good." },
                    { 9, "Yes." },
                    { 10, "Signs point to yes." },
                    { 11, "Reply hazy, try again." },
                    { 12, "Ask again later." },
                    { 13, "Better not tell you now." },
                    { 14, "Cannot predict now." },
                    { 15, "Concentrate and ask again." },
                    { 16, "Don't count on it." },
                    { 17, "My reply is no." },
                    { 18, "My sources say no." },
                    { 19, "Outlook not so good." },
                    { 20, "Very doubtful." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MagicEightBallConfigurations");

            migrationBuilder.DropTable(
                name: "MagicEightBallResponses");
        }
    }
}
