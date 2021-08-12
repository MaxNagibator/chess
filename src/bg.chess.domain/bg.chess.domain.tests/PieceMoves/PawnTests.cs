using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain.PieceMoves
{
    /// <summary>
    /// ����� ������ �� ������������ �����.
    /// </summary>
    public class PawnTests
    {
        /// <summary>
        /// ������� ���� �� �������� ������ ����� ��� ���� � ���� ����
        /// </summary>
        [Test]
        public void PawnMoveWithAtackTest()
        {
            var rules = new Rules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            rules.Positions = new List<Position>
            {
                new Position(1, 1, new Pawn(Side.White)),
                new Position(2, 5, new Knight(Side.Black))
            };

            var field = new Field(rules);

            field.Move(Side.White, 1, 1, 1, 2);
            field.Move(Side.White, 1, 2, 1, 3);
            field.Move(Side.White, 1, 3, 1, 4);
            field.Move(Side.White, 1, 4, 2, 5);

            Assert.AreEqual(Side.White, field[2, 5].Piece.Side);
        }

    }
}