namespace Bg.Chess.Domain.Tests.GameTests
{
    using NUnit.Framework;

    public class NotationTests
    {
        /// <summary>
        /// Проверка формирования номации для классической стартовой расстановки.
        /// </summary>
        [Test]
        public void GameWithDefaultRulesTest()
        {
            var game = new Game();
            game.Init();

            Assert.AreEqual("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", game.GetForsythEdwardsNotation());

            game.Move(Side.White, "e2", "e4");
            Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", game.GetForsythEdwardsNotation());

            game.Move(Side.Black, "d7", "d5");
            Assert.AreEqual("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 2", game.GetForsythEdwardsNotation());

            game.Move(Side.White, "g1", "f3");
            Assert.AreEqual("rnbqkbnr/ppp1pppp/8/3p4/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2", game.GetForsythEdwardsNotation());

            game.Move(Side.Black, "e8", "d7");
            Assert.AreEqual("rnbq1bnr/pppkpppp/8/3p4/4P3/5N2/PPPP1PPP/RNBQKB1R w KQ - 2 3", game.GetForsythEdwardsNotation());
        }
    }
}