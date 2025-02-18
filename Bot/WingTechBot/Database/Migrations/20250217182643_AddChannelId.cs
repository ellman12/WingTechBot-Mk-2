using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WingTechBot.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ChannelId",
                table: "Reactions",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "Reactions");
        }
    }
}
