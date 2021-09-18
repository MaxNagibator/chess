namespace Bg.Chess.Game.Tests
{
    using Bg.Chess.Data.Repo;

    using Moq;

    using NUnit.Framework;

    public class PlayTests
    {
        private IGameManager _manager;
        private IGameService _gameService;
        private int _player1 = 217;
        private int _player2 = 999;

        [SetUp]
        public void SetUp()
        {
            _manager = new GameManager(new TestLoggerFactory());
            
            var mock = new Mock<IGameRepo>();
            mock.Setup(a => a.SaveGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));
            _gameService = new GameService(null, mock.Object, new TestLoggerFactory());
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
            _gameService.SaveGame(game);
        }

        /// <summary>
        /// ѕри попытке сохранить игру, где была трансформаци€ пешки, падала ошибка.
        /// </summary>
        /// <remarks>
        /// “ак как у новой фигуры нет было заполнено "From" (ну она же с улицы пришла, так то логично)
        /// </remarks>
        [Test]
        public void SaveGameAfterPawnTransformTest()
        {
            _manager.StartSearch(_player1);
            _manager.StartSearch(_player2);
            _manager.Confirm(_player1);
            _manager.Confirm(_player2);

            var game = _manager.FindMyPlayingGame(_player2);
            Assert.AreEqual(true, game.IsMyGame(_player1));
            Assert.AreEqual(true, game.IsMyGame(_player2));

            game.Move(game.WhitePlayerId, 7, 1, 7, 3);
            game.Move(game.BlackPlayerId, 6, 6, 6, 4);
            game.Move(game.WhitePlayerId, 7, 3, 6, 4);
            game.Move(game.BlackPlayerId, 6, 7, 5, 5);
            game.Move(game.WhitePlayerId, 6, 4, 6, 5);
            game.Move(game.BlackPlayerId, 7, 6, 7, 4);
            game.Move(game.WhitePlayerId, 2, 1, 2, 2);
            game.Move(game.BlackPlayerId, 7, 7, 7, 6);
            game.Move(game.WhitePlayerId, 1, 1, 1, 2);
            game.Move(game.BlackPlayerId, 5, 7, 7, 5);
            game.Move(game.WhitePlayerId, 6, 5, 6, 6);
            game.Move(game.BlackPlayerId, 7, 4, 7, 3);
            game.Move(game.WhitePlayerId, 6, 6, 6, 7, "queen");

            _gameService.SaveGame(game);
        }
    }
}