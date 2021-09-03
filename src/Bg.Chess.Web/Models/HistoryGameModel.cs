namespace Bg.Chess.Web.Models
{
    using Bg.Chess.Common.Enums;
    using System.Collections.Generic;

    public class HistoryGameModel
    {
        public int Id { get; set; }

        public int WhitePlayerId { get; set; }

        public int BlackPlayerId { get; set; }

        public GameStatus Status { get; set; }

        public List<Move> Moves { get; set; }

        public string Positions { get; set; }

        public class Move
        {
            /// <summary>
            /// ������.
            /// </summary>
            public Position From { get; set; }

            /// <summary>
            /// ����.
            /// </summary>
            public Position To { get; set; }

            /// <summary>
            /// ��� �����.
            /// </summary>
            public string Runner { get; set; }

            /// <summary>
            /// ���� ����� ���� ������ ��������� ������.
            /// </summary>
            public string KillEnemy { get; set; }

            /// <summary>
            /// ��� ���� ������, ��������� � �����, �� ��� ����.
            /// </summary>
            public Move AdditionalMove { get; set; }
        }

        public class Position
        {
            /// <summary>
            /// �� �����������.
            /// </summary>
            public int X { get; set; }

            /// <summary>
            /// �� ���������.
            /// </summary>
            public int Y { get; set; }

            /// <summary>
            /// ������.
            /// </summary>
            public string Piece { get; set; }
        }
    }
}
