using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain.Tests.PieceMoves
{
    /// <summary>
    /// ����� ������ �� ������������ �����.
    /// </summary>
    public class BishopMoveTests
    {
        /// <summary>
        /// ���� �� ������ � ��������� ����� ������ ����� 7 �����.
        /// </summary>
        [TestCase(0)]
        [TestCase(7)]
        public void BishopFirstLineTest(int x)
        {
            for (var y = 0; y < 8; y++)
            {
                var rules = new Rules();
                rules.FieldWidth = 8;
                rules.FieldHeight = 8;
                var piece = new Bishop(Side.White);
                rules.Positions = new List<Position> { new Position(x, y, piece) };

                var field = new Field(rules);

                var moves = field[x, y].GetAvailableMoves();
                Assert.AreEqual(7, moves.Count);
            }
        }

        /// <summary>
        /// � ������ ������ ����� ���� ����� ������ ���������� �����.
        /// </summary>
        [TestCase(1, 1, 9)]
        [TestCase(1, 4, 9)]
        [TestCase(3, 3, 13)]
        [TestCase(5, 5, 11)]
        public void BishopCustomPositionTest(int x, int y, int movesCount)
        {
            var rules = new Rules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            var piece = new Bishop(Side.White);
            rules.Positions = new List<Position> { new Position(x, y, piece) };

            var field = new Field(rules);

            var moves = field[x, y].GetAvailableMoves();
            Assert.AreEqual(movesCount, moves.Count);
        }

        /// <summary>
        /// ���� � ������ ����, �� �� ������� ���� ��� ����� �������, ���� ��� ������
        /// </summary>
        /// <remarks>
        /// 3 + 3 ���� � ���� �� ��������� ������������.
        /// ������ �� ���������� 2 ������ �� ������ ������������. 
        /// ���� ��� �������, �� ���� ���, ���� ��������� �� ��� ��������� ����.
        /// </remarks>
        [TestCase(1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, 1)]
        [TestCase(-1, -1)]
        public void BishopWithTeamMateTest(int first, int second)
        {
            var rules = new Rules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            rules.Positions = new List<Position>
            {
                new Position(4, 4, new Bishop(Side.White)),
                new Position(6, 6, new Pawn(first == 1 ? Side.White : Side.Black)),
                new Position(2, 2, new Pawn(second == 1 ? Side.White : Side.Black))
            };

            var teammateCount = rules.Positions.Count(x => x?.Piece.Side == Side.White) - 1;

            var field = new Field(rules);

            var moves = field[4, 4].GetAvailableMoves();
            Assert.AreEqual(6 + 4 - teammateCount, moves.Count);
        }
    }
}