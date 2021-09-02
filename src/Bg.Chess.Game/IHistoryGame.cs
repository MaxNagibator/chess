namespace Bg.Chess.Game
{
    using Bg.Chess.Common.Enums;

    public interface IHistoryGame
    {
        int Id { get; }
        int WhitePlayerId { get; }
        int BlackPlayerId { get; }
        GameStatus Status { get; }
    }

    public class HistoryGame : IHistoryGame
    {
        public int Id { get; set; }
        public int WhitePlayerId { get; set; }
        public int BlackPlayerId { get; set; }
        public GameStatus Status { get; set; }
    }
}
