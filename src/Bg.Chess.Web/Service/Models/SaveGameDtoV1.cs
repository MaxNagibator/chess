using Bg.Chess.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bg.Chess.Web.Service
{
    public class SaveGameDtoV1
    {
        public List<Move> Moves { get; set; }

        public List<Position> Positions { get; set; }

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
            public Piece Runner { get; set; }

            /// <summary>
            /// Если после хода умерла вражеская фигура.
            /// </summary>
            public Piece KillEnemy { get; set; }

            /// <summary>
            /// При ходе короля, двигалась и ладья, но это один.
            /// </summary>
            public Move AdditionalMove { get; set; }
        }

        public class Piece
        {
            /// <summary>
            /// Тип фигуры.
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// Кому пренадлежит фигура.
            /// </summary>
            public string Side { get; set; }
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
            public Piece Piece { get; set; }
        }
    }
}
