using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tarah.API.Migrations
{
    /// <inheritdoc />
    public partial class ImagesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrls",
                table: "Products",
                newName: "Images");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Images",
                table: "Products",
                newName: "ImageUrls");
        }
    }
}
