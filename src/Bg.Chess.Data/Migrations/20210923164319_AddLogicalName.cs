using Microsoft.EntityFrameworkCore.Migrations;

namespace Bg.Chess.Data.Migrations
{
    public partial class AddLogicalName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogicalName",
                table: "ChessGame",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE dbo.ChessGame
SET LogicalName = '20210901000000'+cast(id as nvarchar(max))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogicalName",
                table: "ChessGame");
        }
    }
}
