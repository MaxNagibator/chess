namespace Bg.Chess.Game
{
    using Bg.Chess.Common.Enums;
    using System.Collections.Generic;

    public interface IHistoryGame
    {
        int Id { get; }
        int WhitePlayerId { get; }
        int BlackPlayerId { get; }
        GameStatus Status { get; }
        List<Move> Moves { get; }
        string Positions { get; }
    }

    public class HistoryGame : IHistoryGame
    {
        public int Id { get; set; }
        public int WhitePlayerId { get; set; }
        public int BlackPlayerId { get; set; }
        public GameStatus Status { get; set; }
        public List<Move> Moves { get; set; }
        public string Positions { get; set; }
    }

    //todo добавить интерфейсы
    public class Move
    {
        /// <summary>
        /// Откуда.
        /// </summary>
        public Position From { get; set; }

        /// <summary>
        /// Куда.
        /// </summary>
        public Position To { get; set; }

        /// <summary>
        /// Кто ходил.
        /// </summary>
        public string Runner { get; set; }

        /// <summary>
        /// Если после хода умерла вражеская фигура.
        /// </summary>
        public string KillEnemy { get; set; }

        /// <summary>
        /// При ходе короля, двигалась и ладья, но это один.
        /// </summary>
        public Move AdditionalMove { get; set; }
    }

    public class Position
    {
        /// <summary>
        /// По горизонтали.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// По вертикали.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Фигура.
        /// </summary>
        public string Piece { get; set; }
    }
}
