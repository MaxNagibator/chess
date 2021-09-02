using Microsoft.EntityFrameworkCore.Migrations;

namespace Bg.Chess.Web.Data.Migrations
{
    public partial class AddColumnStatusToGameTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ChessGame",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ChessGame");
        }
    }
}
