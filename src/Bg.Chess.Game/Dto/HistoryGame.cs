namespace Bg.Chess.Game
{
    using Bg.Chess.Common.Enums;

    using System.Collections.Generic;

    public class HistoryGame
    {
        public int Id { get; set; }
        public FinishReason FinishReason { get; set; }
        public GameSide WinSide { get; set; }
        public List<Move> Moves { get; set; }
        public string Positions { get; set; }
        public Player WhitePlayer { get; set; }
        public Player BlackPlayer { get; set; }
    }
}
