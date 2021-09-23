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

        public class Player
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
