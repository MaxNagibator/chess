using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain.PieceAvailableMoves
{
    /// <summary>
    /// Набор тестов на доступные ходы коня.
    /// </summary>
    public class KnightTests
    {
        /// <summary>
        /// Проверка коня на пустой доске.
        /// </summary>
        /// <remarks>
        /// Конь в углах имеет 4 хода, 
        /// рядом с углом смещённый на 1 1 имеет 4
        /// У края, но не в углу 4 хода
        /// Рядом с краем, но не в углу 6 ходов
        /// В "центре" 8 ходов
        /// </remarks>
        [Test]
        [TestCase(0, 0, 2)]
        [TestCase(0, 7, 2)]
        [TestCase(7, 0, 2)]
        [TestCase(7, 7, 2)]

        [TestCase(1, 1, 4)]
        [TestCase(1, 6, 4)]
        [TestCase(6, 1, 4)]
        [TestCase(6, 6, 4)]

        [TestCase(4, 0, 4)]
        [TestCase(4, 7, 4)]
        [TestCase(0, 4, 4)]
        [TestCase(7, 4, 4)]

        [TestCase(4, 6, 6)]
        [TestCase(4, 1, 6)]
        [TestCase(6, 4, 6)]
        [TestCase(1, 4, 6)]

        [TestCase(2, 2, 8)]
        [TestCase(4, 4, 8)]
        [TestCase(5, 5, 8)]
        [TestCase(4, 5, 8)]
        public void KnightDefaultTest(int x, int y, int movesCount)
        {
            var rules = new Rules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            var piece = new Knight(Side.White);
            rules.Positions = new List<Position> { new Position(x, y, piece) };

            var field = new Field(rules);

            var moves = field[x, y].GetAvailableMoves();
            Assert.AreEqual(movesCount, moves.Count);
        }

        /// <summary>
        /// Конь в центре, но на клетках куда он может сходить, есть две фигуры
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
                new Position(4, 5, new Knight(Side.White)),
                new Position(6, 6, new Rook(first == 1 ? Side.White : Side.Black)),
                new Position(2, 4, new Rook(second == 1 ? Side.White : Side.Black))
            };

            var teammateCount = rules.Positions.Count(x => x?.Piece.Side == Side.White) - 1;

            var field = new Field(rules);

            var moves = field[4, 5].GetAvailableMoves();
            Assert.AreEqual(8 - teammateCount, moves.Count);
        }
    }
}