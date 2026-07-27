using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaBoard.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOrderName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Order",
                table: "song",
                newName: "order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "order",
                table: "song",
                newName: "Order");
        }
    }
}
