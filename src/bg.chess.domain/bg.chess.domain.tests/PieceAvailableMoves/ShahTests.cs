using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain.Tests.PieceAvailableMoves
{
    /// <summary>
    /// Набор тестов на доступные ходы слона.
    /// </summary>
    public class ShahTests
    {
        /// <summary>
        /// Если король под шахом, то нельзя походить пешкой.
        /// </summary>
        [Test]
        public void PawnDontMoveIfKingShahTest()
        {
            for (var y = 0; y < 8; y++)
            {
                var rules = new ClassicRules();
                rules.FieldWidth = 8;
                rules.FieldHeight = 8;
                rules.Positions = new List<Position>
                {
                    new Position(4, 0, PieceBuilder.King(Side.White)),
                    new Position(0, 1, PieceBuilder.Pawn(Side.White)),
                    new Position(4, 7, PieceBuilder.Rook(Side.Black)),
                    new Position(5, 7, PieceBuilder.King(Side.Black)),
                };

                var field = new Field(rules);

                var moves = field[0, 1].GetAvailableMoves();
                Assert.AreEqual(0, moves.Count);
            }
        }

        /// <summary>
        /// Если король под шахом, то пешкой можно его прикрыть.
        /// </summary>
        /// <remarks>
        /// Пешка ранее не ходила, но ход на 2 клетки не доступен, так как нужно прикрыть короля.
        /// </remarks>
        [Test]
        public void PawnMoveForKingShahDisableTest()
        {
            for (var y = 0; y < 8; y++)
            {
                var rules = new ClassicRules();
                rules.FieldWidth = 8;
                rules.FieldHeight = 8;
                rules.Positions = new List<Position>
                {
                    new Position(3, 0, PieceBuilder.King(Side.White)),
                    new Position(5, 1, PieceBuilder.Pawn(Side.White)),
                    new Position(7, 4, PieceBuilder.Bishop(Side.Black)),
                    new Position(4, 7, PieceBuilder.King(Side.Black)),
                };

                var field = new Field(rules);

                var moves = field[5, 1].GetAvailableMoves();
                Assert.AreEqual(1, moves.Count);
                Assert.AreEqual(5, moves[0].X);
                Assert.AreEqual(3, moves[0].Y);
            }
        }

        /// <summary>
        /// Если король под шахом, то можно срубить угрожающего.
        /// </summary>
        [Test]
        public void RookMoveForSaveKingTest()
        {
            for (var y = 0; y < 8; y++)
            {
                var rules = new ClassicRules();
                rules.FieldWidth = 8;
                rules.FieldHeight = 8;
                rules.Positions = new List<Position>
                {
                    new Position(3, 0, PieceBuilder.King(Side.White)),
                    new Position(7, 0, PieceBuilder.Rook(Side.White)),
                    new Position(7, 4, PieceBuilder.Bishop(Side.Black)),
                    new Position(4, 7, PieceBuilder.King(Side.Black)),
                };

                var field = new Field(rules);

                var moves = field[7, 0].GetAvailableMoves();
                Assert.AreEqual(1, moves.Count);
                Assert.AreEqual(7, moves[0].X);
                Assert.AreEqual(4, moves[0].Y);
            }
        }
    }
}