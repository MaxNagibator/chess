using NUnit.Framework;

namespace Bg.Chess.Game.Tests
{
    public class PlayTests
    {
        private ISearchManager _manager;
        private IGameHolder _gameHolder;
        private int _player1 = 217;
        private int _player2 = 999;

        [SetUp]
        public void SetUp()
        {
            _gameHolder = new GameHolder();
            _manager = new SearchManager(_gameHolder);
        }

        [Test]
        public void CheckStateAfterTwoConfirmTest()
        {
            _manager.Start(_player1);
            _manager.Start(_player2);
            _manager.Confirm(_player1);
            _manager.Confirm(_player2);

            var game = _gameHolder.GetMyPlayingGame(_player2);
            Assert.AreEqual(true, game.IsMyGame(_player1));
            Assert.AreEqual(true, game.IsMyGame(_player2));

            game.Move(game.WhitePlayerId, 5, 1, 5, 3);
            game.Move(game.BlackPlayerId, 5, 6, 5, 4);
        }
    }
}