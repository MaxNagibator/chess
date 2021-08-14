using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain.PieceAvailableMoves
{
    /// <summary>
    /// Набор тестов на доступные ходы ладьи.
    /// </summary>
    public class RookTests
    {
        /// <summary>
        /// В любой точке поле ладья имеет 7 + 7 ходов
        /// </summary>
        [TestCase(0, 0)]
        [TestCase(0, 4)]
        [TestCase(0, 7)]
        [TestCase(4, 7)]
        [TestCase(7, 7)]
        [TestCase(7, 4)]
        [TestCase(7, 0)]
        [TestCase(4, 0)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        public void RookDefaultTest(int x, int y)
        {
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            var piece = new Rook(Side.White);
            rules.Positions = new List<Position> { new Position(x, y, piece) };

            var field = new Field(rules);

            var moves = field[x, y].GetAvailableMoves();
            Assert.AreEqual(14, moves.Count);
        }

        /// <summary>
        /// Ладья в углу, но на клетках куда она может сходить, есть две фигуры
        /// </summary>
        /// <remarks>
        /// Если никого не рубить, то у ладьи 4 + 4 ходов
        /// </remarks>
        [TestCase(1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, 1)]
        [TestCase(-1, -1)]
        public void RookWithTeamMateTest(int first, int second)
        {
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            rules.Positions = new List<Position>
            {
                new Position(0, 0, new Rook(Side.White)),
                new Position(0, 5, new Rook(first == 1 ? Side.White : Side.Black)),
                new Position(5, 0, new Rook(second == 1 ? Side.White : Side.Black))
            };

            var teammateCount = rules.Positions.Count(x => x?.Piece.Side == Side.White) - 1;

            var field = new Field(rules);

            var moves = field[0, 0].GetAvailableMoves();
            Assert.AreEqual(10 - teammateCount, moves.Count);
        }
    }
}