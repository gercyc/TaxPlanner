using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxPlanner.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPostThumbnail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailAlt",
                table: "Posts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Posts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailAlt",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Posts");
        }
    }
}
