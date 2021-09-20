using Microsoft.EntityFrameworkCore.Migrations;

namespace Bg.Chess.Data.Migrations
{
    public partial class DropStatusColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER TABLE dbo.ChessGame
	ADD WinSide int NULL
GO
ALTER TABLE dbo.ChessGame
	ADD FinishReason int NULL
GO
UPDATE dbo.ChessGame
SET FinishReason = 0
	
UPDATE dbo.ChessGame
SET WinSide = 0
WHERE Status = 2 -- white win
	
UPDATE dbo.ChessGame
SET WinSide = 1 
WHERE Status = 3 -- black win
	
GO
DECLARE @ObjectName NVARCHAR(100)
SELECT @ObjectName = OBJECT_NAME([default_object_id]) FROM SYS.COLUMNS
WHERE [object_id] = OBJECT_ID('[dbo].[ChessGame]') AND [name] = 'Status';
EXEC('ALTER TABLE [dbo].[ChessGame] DROP CONSTRAINT ' + @ObjectName)
GO

ALTER TABLE dbo.ChessGame 
DROP Column Status
GO

ALTER TABLE dbo.ChessGame
	ALTER COLUMN WinSide int NOT NULL
GO
ALTER TABLE dbo.ChessGame
	ALTER COLUMN FinishReason int NOT NULL
GO
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
