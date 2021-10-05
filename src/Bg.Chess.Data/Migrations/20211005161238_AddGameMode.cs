using Microsoft.EntityFrameworkCore.Migrations;

namespace Bg.Chess.Data.Migrations
{
    public partial class AddGameMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameMode",
                table: "ChessGame",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE dbo.ChessGame
SET GameMode = 0");

            migrationBuilder.AlterColumn<int>(
                name: "GameMode",
                table: "ChessGame",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
