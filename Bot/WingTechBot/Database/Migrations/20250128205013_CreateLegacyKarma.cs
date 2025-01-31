using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WingTechBot.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateLegacyKarma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LegacyKarma",
                columns: table => new
                {
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Upvotes = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Downvotes = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Silver = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Gold = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Platinum = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegacyKarma", x => new { x.UserId, x.Year });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LegacyKarma");
        }
    }
}
