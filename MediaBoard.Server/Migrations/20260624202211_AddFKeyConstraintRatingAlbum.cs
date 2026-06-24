using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaBoard.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddFKeyConstraintRatingAlbum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "rating_user_id_fkey",
                table: "rating");

            migrationBuilder.DropIndex(
                name: "IX_rating_user_id_media_id",
                table: "rating");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "rating",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "rating",
                table: "rating",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.Sql("ALTER TABLE \"rating\" ALTER COLUMN media_id TYPE integer USING media_id::integer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_rating",
                table: "rating",
                columns: new[] { "user_id", "media_id" });

            migrationBuilder.CreateIndex(
                name: "IX_rating_media_id",
                table: "rating",
                column: "media_id");

            migrationBuilder.AddForeignKey(
                name: "rating_media_id_fkey",
                table: "rating",
                column: "media_id",
                principalTable: "album",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "rating_user_id_fkey",
                table: "rating",
                column: "user_id",
                principalTable: "app_user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "rating_media_id_fkey",
                table: "rating");

            migrationBuilder.DropForeignKey(
                name: "rating_user_id_fkey",
                table: "rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_rating",
                table: "rating");

            migrationBuilder.DropIndex(
                name: "IX_rating_media_id",
                table: "rating");

            migrationBuilder.AlterColumn<short>(
                name: "rating",
                table: "rating",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.Sql("ALTER TABLE \"rating\" ALTER COLUMN media_id TYPE character varying(100)");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "rating",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_rating_user_id_media_id",
                table: "rating",
                columns: new[] { "user_id", "media_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "rating_user_id_fkey",
                table: "rating",
                column: "user_id",
                principalTable: "app_user",
                principalColumn: "user_id");
        }
    }
}
