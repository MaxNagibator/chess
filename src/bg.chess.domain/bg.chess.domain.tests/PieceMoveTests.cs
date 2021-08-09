using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace bg.chess.domain.tests
{
    public class PieceMoveTests
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

            var moves = piece.GetAvailableMoves(field[x, y]);
            Assert.AreEqual(movesCount, moves.Count);
        }
    }
}