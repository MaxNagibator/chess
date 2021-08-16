namespace Bg.Chess.Domain.Tests.PieceAvailableMoves
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            rules.Positions = new List<Position> 
            { 
                new Position(x, y, PieceBuilder.Knight(Side.White)),
                new Position(x.Shift(7), y.Shift(7), PieceBuilder.King(Side.White)),
                new Position(x.Shift(5), y.Shift(7), PieceBuilder.King(Side.Black)),
            };

            var field = new Field(rules);

            var moves = field[x, y].GetAvailableMoves();
            Assert.AreEqual(movesCount, moves.Count);
        }

        /// <summary>
        /// Конь в центре, но на клетках куда он может сходить, есть две фигуры
        /// </summary>
        [Test]
        [TestCase(1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, 1)]
        [TestCase(-1, -1)]
        public void KnightWithTeamMateTest(int first, int second)
        {
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            rules.Positions = new List<Position>
            {
                new Position(4, 5, PieceBuilder.Knight(Side.White)),
                new Position(6, 6, PieceBuilder.Rook(first == 1 ? Side.White : Side.Black)),
                new Position(2, 4, PieceBuilder.Rook(second == 1 ? Side.White : Side.Black)),
                new Position(0, 7, PieceBuilder.King(Side.Black)),
                new Position(7, 7, PieceBuilder.King(Side.White)),
            };

            var teammateCount = rules.Positions.Count(x => x?.Piece.Side == Side.White && x?.Piece.Type is Rook);

            var field = new Field(rules);

            var moves = field[4, 5].GetAvailableMoves();
            Assert.AreEqual(8 - teammateCount, moves.Count);
        }
    }
}