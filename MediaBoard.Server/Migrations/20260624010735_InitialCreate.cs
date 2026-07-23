using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MediaBoard.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateTable(
                name: "album",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    main_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: true),
                    image_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("album_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "app_user",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("app_user_pkey", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "artist",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    real_name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("artist_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "genre",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("genre_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "music_style",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("music_style_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rating",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    media_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    review = table.Column<string>(type: "text", nullable: true),
                    rating = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "rating_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "album_artist",
                columns: table => new
                {
                    album_id = table.Column<int>(type: "integer", nullable: false),
                    artist_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("album_artist_pkey", x => new { x.album_id, x.artist_id });
                    table.ForeignKey(
                        name: "album_artist_album_id_fkey",
                        column: x => x.album_id,
                        principalTable: "album",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "album_artist_artist_id_fkey",
                        column: x => x.artist_id,
                        principalTable: "artist",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "artist_metrics",
                columns: table => new
                {
                    artist_id = table.Column<int>(type: "integer", nullable: false),
                    total_clicks = table.Column<int>(type: "integer", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("artist_metrics_pkey", x => x.artist_id);
                    table.ForeignKey(
                        name: "artist_metrics_artist_id_fkey",
                        column: x => x.artist_id,
                        principalTable: "artist",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "album_genre",
                columns: table => new
                {
                    album_id = table.Column<int>(type: "integer", nullable: false),
                    genre_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("album_genre_pkey", x => new { x.album_id, x.genre_id });
                    table.ForeignKey(
                        name: "album_genre_album_id_fkey",
                        column: x => x.album_id,
                        principalTable: "album",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "album_genre_genre_id_fkey",
                        column: x => x.genre_id,
                        principalTable: "genre",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "album_style",
                columns: table => new
                {
                    album_id = table.Column<int>(type: "integer", nullable: false),
                    style_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("album_style_pkey", x => new { x.album_id, x.style_id });
                    table.ForeignKey(
                        name: "album_style_album_id_fkey",
                        column: x => x.album_id,
                        principalTable: "album",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "album_style_style_id_fkey",
                        column: x => x.style_id,
                        principalTable: "music_style",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_album_artist_album_id",
                table: "album_artist",
                column: "album_id");

            migrationBuilder.CreateIndex(
                name: "ix_album_artist_artist_id",
                table: "album_artist",
                column: "artist_id");

            migrationBuilder.CreateIndex(
                name: "IX_album_genre_genre_id",
                table: "album_genre",
                column: "genre_id");

            migrationBuilder.CreateIndex(
                name: "IX_album_style_style_id",
                table: "album_style",
                column: "style_id");

            migrationBuilder.CreateIndex(
                name: "app_user_email_key",
                table: "app_user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "app_user_username_key",
                table: "app_user",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "artist_name_index",
                table: "artist",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_rating_user_id",
                table: "rating",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "album_artist");

            migrationBuilder.DropTable(
                name: "album_genre");

            migrationBuilder.DropTable(
                name: "album_style");

            migrationBuilder.DropTable(
                name: "artist_metrics");

            migrationBuilder.DropTable(
                name: "rating");

            migrationBuilder.DropTable(
                name: "genre");

            migrationBuilder.DropTable(
                name: "album");

            migrationBuilder.DropTable(
                name: "music_style");

            migrationBuilder.DropTable(
                name: "artist");

            migrationBuilder.DropTable(
                name: "app_user");
        }
    }
}
