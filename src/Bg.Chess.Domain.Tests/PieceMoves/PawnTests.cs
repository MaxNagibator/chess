namespace Bg.Chess.Domain.Tests.PieceMoves
{
    using NUnit.Framework;
    using System.Collections.Generic;

    /// <summary>
    /// Набор тестов на передвижение пешки.
    /// </summary>
    public class PawnTests
    {
        /// <summary>
        /// Простой тест на движение пешкой вперёд три раза и сруб коня
        /// </summary>
        [Test]
        public void PawnMoveWithAtackTest()
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(1, 1, PieceBuilder.Pawn(Side.White)),
                new Position(2, 5, PieceBuilder.Knight(Side.Black)),
                new Position(4, 0, PieceBuilder.King(Side.White)),
                new Position(4, 7, PieceBuilder.King(Side.Black)),
            };

            var field = new Field(rules);

            field.Move(Side.White, 1, 1, 1, 2);
            field.Move(Side.White, 1, 2, 1, 3);
            field.Move(Side.White, 1, 3, 1, 4);
            field.Move(Side.White, 1, 4, 2, 5);

            Assert.AreEqual(Side.White, field[2, 5].Piece.Side);
        }

        /// <summary>
        /// Пешка, после того как сходила, не может ходить на две клетки вперёд.
        /// </summary>
        [Test]
        public void PawnAfterMoveCanNotTwoFieldMoveTest()
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(1, 1, PieceBuilder.Pawn(Side.White)),
                new Position(4, 0, PieceBuilder.King(Side.White)),
                new Position(4, 7, PieceBuilder.King(Side.Black)),
            };

            var field = new Field(rules);

            field.Move(Side.White, 1, 1, 1, 2);
            var moves = field[1, 2].GetAvailableMoves();

            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(1, moves[0].X);
            Assert.AreEqual(3, moves[0].Y);
        }

        /// <summary>
        /// Пешка не должна ходить на две клетки, если перед ней фигура.
        /// </summary>
        [Test]
        public void PawnThroughKnightCanNotTwoFieldMoveTest()
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(1, 1, PieceBuilder.Pawn(Side.White)),
                new Position(1, 2, PieceBuilder.Knight(Side.White)),
                new Position(4, 0, PieceBuilder.King(Side.White)),
                new Position(4, 7, PieceBuilder.King(Side.Black)),
            };

            var field = new Field(rules);
            var moves = field[1, 1].GetAvailableMoves();

            Assert.AreEqual(0, moves.Count);
        }

        /// <summary>
        /// Ходим пешкой и получаем другую фигуру
        /// </summary>
        [Test]
        [TestCase("queen")]
        [TestCase("rook")]
        [TestCase("knight")]
        [TestCase("bishop")]
        public void PawnTransformToQueen(string name)
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(1, 6, PieceBuilder.Pawn(Side.White)),
                new Position(4, 0, PieceBuilder.King(Side.White)),
                new Position(4, 7, PieceBuilder.King(Side.Black)),
            };

            var field = new Field(rules);

            field.Move(Side.White, 1, 6, 1, 7, name);

            Assert.AreEqual(Side.White, field[1, 7].Piece.Side);
            Assert.AreEqual(name, field[1, 7].Piece.Type.Name);
        }

        /// <summary>
        /// Взятие вражеской пешки на проходе.
        /// </summary>
        /// <remarks>
        /// 1 - белый находится правее чёрного
        /// -1 - белый находится левее чёрного
        /// </remarks>
        [Test]
        [TestCase(1)]
        [TestCase(-1)]
        public void PawnEnPassantMoveTest(int shift)
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(4, 6, PieceBuilder.Pawn(Side.Black)),
                new Position(4 + shift, 4, PieceBuilder.Pawn(Side.White)),
                // конь, чтоб нельзя было ходить вперёд и остался тока ход "взятие на проходе"
                new Position(4 + shift, 5, PieceBuilder.Knight(Side.White)),
                new Position(0, 0, PieceBuilder.King(Side.White)),
                new Position(0, 7, PieceBuilder.King(Side.Black)),
            };

            var field = new Field(rules);

            field.Move(Side.Black, 4, 6, 4, 4);
            var moves = field[4 + shift, 4].GetAvailableMoves();

            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(4, moves[0].X);
            Assert.AreEqual(5, moves[0].Y);

            field.Move(Side.White, 4 + shift, 4, 4, 5);
            Assert.IsNull(field[4, 4].Piece, "Вражеская фигура осталась на поле");
        }
    }
}