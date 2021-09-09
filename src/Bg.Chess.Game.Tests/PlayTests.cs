using NUnit.Framework;

namespace Bg.Chess.Game.Tests
{
    public class PlayTests
    {
        private IGameManager _manager;
        private int _player1 = 217;
        private int _player2 = 999;

        [SetUp]
        public void SetUp()
        {
            _manager = new GameManager(new TestLoggerFactory());
        }

        [Test]
        public void CheckStateAfterTwoConfirmTest()
        {
            _manager.StartSearch(_player1);
            _manager.StartSearch(_player2);
            _manager.Confirm(_player1);
            _manager.Confirm(_player2);

            var game = _manager.FindMyPlayingGame(_player2);
            Assert.AreEqual(true, game.IsMyGame(_player1));
            Assert.AreEqual(true, game.IsMyGame(_player2));

            game.Move(game.WhitePlayerId, 5, 1, 5, 3);
            game.Move(game.BlackPlayerId, 5, 6, 5, 4);
        }
    }
}