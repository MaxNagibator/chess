namespace Bg.Chess.Domain.Tests.GameTests
{
    using NUnit.Framework;
    using System.Collections.Generic;

    public class CustomPositionGameTests
    {
        /// <summary>
        /// На поле будет две ладьи, которые не оставят королю ходов и будет пат
        /// </summary>
        /// <remarks>
        /// Ладья сделает ход и не оставит врагу ходов.
        /// </remarks>
        [Test]
        public void DrawAfterBlackKingDontHasMovesTest()
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
                {
                    new Position(0, 0, PieceBuilder.King(Side.White)),
                    new Position(0, 7, PieceBuilder.King(Side.Black)),
                    new Position(1, 0, PieceBuilder.Rook(Side.White)),
                    new Position(7, 5, PieceBuilder.Rook(Side.White)),
                };

            var field = new Field(rules);
            var game = new Game();
            game.Init(field);
            game.Move(Side.White, 7, 5, 7, 6);
            Assert.AreEqual(GameState.Draw, game.State);
        }
    }
}