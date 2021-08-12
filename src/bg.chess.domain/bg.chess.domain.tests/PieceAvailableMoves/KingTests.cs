using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain.PieceAvailableMoves
{
    /// <summary>
    /// Набор тестов на доступные ходы короля.
    /// </summary>
    public class KingTests
    {
        /// <summary>
        /// Проверка коня на пустой доске.
        /// </summary>
        /// <remarks>
        /// В углах король имеет 3 хода.
        /// Касаясь одного из краёв поля 5 ходов.
        /// </remarks>
        [Test]
        [TestCase(0, 0, 3)]
        [TestCase(0, 7, 3)]
        [TestCase(7, 0, 3)]
        [TestCase(7, 7, 3)]

        [TestCase(0, 1, 5)]
        [TestCase(1, 0, 5)]
        [TestCase(7, 1, 5)]
        [TestCase(1, 7, 5)]

        [TestCase(2, 2, 8)]
        [TestCase(4, 4, 8)]
        [TestCase(5, 5, 8)]
        [TestCase(4, 5, 8)]
        public void KnightDefaultTest(int x, int y, int movesCount)
        {
            var rules = new Rules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            var piece = new King(Side.White);
            rules.Positions = new List<Position> { new Position(x, y, piece) };

            var field = new Field(rules);

            var moves = field[x, y].GetAvailableMoves();
            Assert.AreEqual(movesCount, moves.Count);
        }

        /// <summary>
        /// Король в центре, но на клетках куда он может сходить, есть две фигуры
        /// </summary>
        [TestCase(1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, 1)]
        [TestCase(-1, -1)]
        public void KnightWithTeamMateTest(int first, int second)
        {
            var rules = new Rules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            rules.Positions = new List<Position>
            {
                new Position(4, 5, new King(Side.White)),
                new Position(4, 6, new Knight(first == 1 ? Side.White : Side.Black)),
                new Position(3, 4, new Knight(second == 1 ? Side.White : Side.Black))
            };

            var teammateCount = rules.Positions.Count(x => x?.Piece.Side == Side.White) - 1;

            var field = new Field(rules);

            var moves = field[4, 5].GetAvailableMoves();
            Assert.AreEqual(8 - teammateCount, moves.Count);
        }
    }
}