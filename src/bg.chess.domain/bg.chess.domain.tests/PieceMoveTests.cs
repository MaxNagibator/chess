using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bg.chess.domain.tests
{
    public class PieceMoveTests
    {
        /// <summary>
        /// ѕроверка кон€ на пустой доске.
        /// </summary>
        /// <remarks>
        ///  онь в углах имеет 4 хода, 
        /// р€дом с углом смещЄнный на 1 1 имеет 4
        /// ” кра€, но не в углу 4 хода
        /// –€дом с краем, но не в углу 6 ходов
        /// ¬ "центре" 8 ходов
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
        ///  онь в центре, но на клетках куда он может сходить, есть две фигуры
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
                var rules = new Rules();
                rules.FieldWidth = 8;
                rules.FieldHeight = 8;
                var piece = new Pawn(side == 1 ? Side.White : Side.Black);
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
            var rules = new Rules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            var piece = new Pawn(Side.White);
            var pieceEnemyLeft = new Pawn(Side.Black);
            var pieceEnemyRight = new Pawn(Side.Black);
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