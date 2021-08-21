namespace Bg.Chess.Domain.Tests.PieceMoves
{
    using NUnit.Framework;
    using System.Collections.Generic;

    /// <summary>
    /// Ќабор тестов на передвижение корол€.
    /// </summary>
    public class KiengTests
    {
        /// <summary>
        ///  оролю ничего не угрожает и доступны обе рокировки.
        /// </summary>
        /// <remarks>
        ///  ороль и две ладьи на своих местах, никто не двигалс€.
        /// ѕосле движени€ корол€ на две клетки, проверим, что ладь€ переместилась за него.
        /// </remarks>
        [Test]
        [TestCase(Side.White, 2)]
        [TestCase(Side.White, -2)]
        [TestCase(Side.Black, 2)]
        [TestCase(Side.Black, -2)]
        public void KingCastlingTest(Side side, int shift)
        {
            var rules = new ClassicRules();
            var lineIndex = side == Side.White ? 0 : 7;
            rules.Positions = new List<Position>
            {
                new Position(0, lineIndex, PieceBuilder.Rook(side)),
                new Position(4, lineIndex, PieceBuilder.King(side)),
                new Position(7, lineIndex, PieceBuilder.Rook(side)),
                new Position(4, lineIndex.Invert(), PieceBuilder.King(side.Invert())),
            };

            var field = new Field(rules);
            field.Move(side, 4, lineIndex, 4 + shift, lineIndex);
            var expectedRookX = shift == 2 ? 5 : 3;
            Assert.AreEqual(true, field[expectedRookX, lineIndex].Piece?.Type is Rook);
        }
    }
}