using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcLibrary.Migrations
{
    /// <inheritdoc />
    public partial class StartingPage02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLibrarian",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLibrarian",
                table: "AspNetUsers");
        }
    }
}
