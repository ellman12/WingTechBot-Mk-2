using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WingTechBot.Database.Migrations
{
    /// <inheritdoc />
    public partial class GatoStoresBytes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Gatos",
                newName: "Filename");

            migrationBuilder.AddColumn<byte[]>(
                name: "Media",
                table: "Gatos",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Media",
                table: "Gatos");

            migrationBuilder.RenameColumn(
                name: "Filename",
                table: "Gatos",
                newName: "Url");
        }
    }
}
