using Bg.Chess.Common.Enums;
using System.Collections.Generic;

namespace Bg.Chess.Web.Models
{
    public class HistoryGameModel
    {
        public string Id { get; set; }

        public Player WhitePlayer { get; set; }

        public Player BlackPlayer { get; set; }

        public List<Move> Moves { get; set; }

        public string Positions { get; set; }

        public FinishReason FinishReason { get; set; }

        public GameSide WinSide { get; set; }

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

        public class Player
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
