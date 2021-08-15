using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain.PieceAvailableMoves
{
    /// <summary>
    /// Ќабор тестов на доступные ходы пешки.
    /// </summary>
    public class PawnTests
    {
        /// <summary>
        /// ѕроверка пешки на пустой доске.
        /// </summary>
        /// <remarks>
        /// ѕешка находитс€ на стартовой позиции и имеет право на два хода
        /// </remarks>
        [Test]
        [TestCase(1)]
        [TestCase(-1)]
        public void PawnDefaultTest(int side)
        {
            for (var i = 0; i < 8; i++)
            {
                var y = side == 1 ? 1 : 7;
                var rules = new ClassicRules();
                var piece = PieceBuilder.Pawn(side == 1 ? Side.White : Side.Black);
                rules.Positions = new List<Position> { new Position(i, y, piece) };

                var field = new Field(rules);

                var moves = field[i, y].GetAvailableMoves();
                Assert.AreEqual(2, moves.Count);
                Assert.AreEqual(true, moves.Any(move => move.X == i && move.Y == y + side));
                Assert.AreEqual(true, moves.Any(move => move.X == i && move.Y == y + side + side));
            }
        }

        /// <summary>
        /// ѕешка может сходить пр€мо или срубить любую из двух пешек
        /// </summary>
        /// <remarks>
        /// ј враги могут сходить пр€мо или срубить нашу пешку.
        /// пешки хоть и в центре, но ниразу не ходили, поэтому могут на две клетки сходить вперЄд
        /// 
        /// </remarks>
        [Test]
        public void PawnWithEnemyTest()
        {
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            var piece = PieceBuilder.Pawn(Side.White);
            var pieceEnemyLeft = PieceBuilder.Pawn(Side.Black);
            var pieceEnemyRight = PieceBuilder.Pawn(Side.Black);
            rules.Positions = new List<Position>
            {
                new Position(5, 5, piece),
                new Position(4, 6, pieceEnemyLeft),
                new Position(6, 6, pieceEnemyRight)
            };

            var field = new Field(rules);

            var moves = field[5, 5].GetAvailableMoves();
            Assert.AreEqual(4, moves.Count);
            Assert.AreEqual(true, moves.Any(move => move.X == 4 && move.Y == 6));
            Assert.AreEqual(true, moves.Any(move => move.X == 5 && move.Y == 6));
            Assert.AreEqual(true, moves.Any(move => move.X == 5 && move.Y == 7));
            Assert.AreEqual(true, moves.Any(move => move.X == 6 && move.Y == 6));

            var movesEnemyLeft = field[4, 6].GetAvailableMoves();
            Assert.AreEqual(3, movesEnemyLeft.Count);
            Assert.AreEqual(true, movesEnemyLeft.Any(move => move.X == 4 && move.Y == 5));
            Assert.AreEqual(true, movesEnemyLeft.Any(move => move.X == 4 && move.Y == 4));
            Assert.AreEqual(true, movesEnemyLeft.Any(move => move.X == 5 && move.Y == 5));

            var movesEnemyRight = field[6, 6].GetAvailableMoves();
            Assert.AreEqual(3, movesEnemyRight.Count);
            Assert.AreEqual(true, movesEnemyRight.Any(move => move.X == 6 && move.Y == 5));
            Assert.AreEqual(true, movesEnemyRight.Any(move => move.X == 6 && move.Y == 4));
            Assert.AreEqual(true, movesEnemyRight.Any(move => move.X == 5 && move.Y == 5));
        }
    }
}