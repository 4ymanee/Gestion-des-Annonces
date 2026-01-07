using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_AIA.Migrations
{
    /// <inheritdoc />
    public partial class StoreImagesInDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Medias",
                newName: "FileName");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Medias",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageData",
                table: "Medias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Medias");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Medias",
                newName: "Url");
        }
    }
}
