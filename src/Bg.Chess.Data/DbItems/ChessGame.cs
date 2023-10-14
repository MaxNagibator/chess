using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bg.Chess.Data
{
    [Table("ChessGame")]
    public class ChessGame
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string LogicalName { get; set; }

        public int WhitePlayerId { get; set; }

        public int BlackPlayerId { get; set; }

        public int? FinishReason { get; set; }

        public int? WinSide { get; set; }

        public int GameMode { get; set; }

        public int GameType { get; set; }

        public string Data { get; set; }
    }
}
