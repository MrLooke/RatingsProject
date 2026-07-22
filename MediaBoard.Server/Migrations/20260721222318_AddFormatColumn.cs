using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaBoard.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddFormatColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "format",
                table: "album",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "format",
                table: "album");
        }
    }
}
