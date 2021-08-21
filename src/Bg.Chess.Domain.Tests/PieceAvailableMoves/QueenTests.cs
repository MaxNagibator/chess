using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain.Tests.PieceAvailableMoves
{
    /// <summary>
    /// Набор тестов на доступные ходы ферзя.
    /// </summary>
    public class QueenTests
    {
        /// <summary>
        /// Ферзь на первой и последней линии всегда имеет 21 ход.
        /// </summary>
        [Test]
        [TestCase(0)]
        [TestCase(7)]
        public void QueenFirstLineTest(int x)
        {
            for (var y = 0; y < 8; y++)
            {
                var rules = new ClassicRules();
                rules.FieldWidth = 8;
                rules.FieldHeight = 8;
                var piece = PieceBuilder.Queen(Side.White);
                rules.Positions = new List<Position>
                {
                    new Position(x, y, piece),
                    new Position(x.Shift(6), y.Invert(), PieceBuilder.King(Side.White)),
                    new Position(x.Shift(4), y.Invert(), PieceBuilder.King(Side.Black)),
                };

                var field = new Field(rules);

                var moves = field[x, y].GetAvailableMoves();
                Assert.AreEqual(21, moves.Count);
            }
        }

        /// <summary>
        /// В разных местах доски ферзь имеет разное количество ходов.
        /// </summary>
        /// <remarks>
        /// По диагоналям разное, а по горизонтали и вертикали всегда 7 + 7
        /// </remarks>
        [Test]
        [TestCase(1, 1, 9 + 7 + 7)]
        [TestCase(1, 4, 9 + 7 + 7)]
        [TestCase(3, 3, 13 + 7 + 7)]
        [TestCase(5, 5, 11 + 7 + 7)]
        public void QueenCustomPositionTest(int x, int y, int movesCount)
        {
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            var piece = PieceBuilder.Queen(Side.White);
            rules.Positions = new List<Position>
            { 
                new Position(x, y, piece) ,
                new Position(4, 0, PieceBuilder.King(Side.White)),
                new Position(4, 7, PieceBuilder.King(Side.Black)),
            };

            var field = new Field(rules);

            var moves = field[x, y].GetAvailableMoves();
            Assert.AreEqual(movesCount, moves.Count);
        }

        /// <summary>
        /// Ферзь в центре поля, но на клетках куда она может сходить, есть две фигуры по горизонтали
        /// </summary>
        /// <remarks>
        /// 3 + 3 хода у ферзя по незанятым диагональным направлениям. и 14 по горизонтали/вертикали
        /// Фигуры на расстоянии 2 клеток по другим направлениям. 
        /// Если они союзные, то один ход, если вражеские то два возможных хода.
        /// </remarks>
        [Test]
        [TestCase(1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, 1)]
        [TestCase(-1, -1)]
        public void QueenWithTeamMateTest(int first, int second)
        {
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            rules.Positions = new List<Position>
            {
                new Position(4, 4, PieceBuilder.Queen(Side.White)),
                new Position(6, 6, PieceBuilder.Pawn(first == 1 ? Side.White : Side.Black)),
                new Position(2, 2, PieceBuilder.Pawn(second == 1 ? Side.White : Side.Black)),
                new Position(4, 0, PieceBuilder.King(Side.White)),
                new Position(0, 1, PieceBuilder.King(Side.Black)),
            };

            var teammateCount = rules.Positions.Count(x => x?.Piece.Side == Side.White) - 1;

            var field = new Field(rules);

            var moves = field[4, 4].GetAvailableMoves();
            Assert.AreEqual(14 + 6 + 4 - teammateCount, moves.Count);
        }
    }
}