using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaBoard.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddSongRatingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SongRatings",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SongId = table.Column<int>(type: "integer", nullable: false),
                    AlbumId = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<short>(type: "smallint", nullable: false),
                    Review = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongRatings", x => new { x.UserId, x.SongId });
                    table.ForeignKey(
                        name: "FK_SongRatings_album_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "album",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SongRatings_app_user_UserId",
                        column: x => x.UserId,
                        principalTable: "app_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SongRatings_song_SongId",
                        column: x => x.SongId,
                        principalTable: "song",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SongRatings_AlbumId",
                table: "SongRatings",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_SongRatings_SongId",
                table: "SongRatings",
                column: "SongId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SongRatings");
        }
    }
}
