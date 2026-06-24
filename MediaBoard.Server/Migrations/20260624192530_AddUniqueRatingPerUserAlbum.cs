using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaBoard.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueRatingPerUserAlbum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_rating_user_id_media_id",
                table: "rating",
                columns: new[] { "user_id", "media_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_rating_user_id_media_id",
                table: "rating");
        }
    }
}
