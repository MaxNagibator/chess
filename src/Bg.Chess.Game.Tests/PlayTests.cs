namespace Bg.Chess.Game.Tests
{
    using System.Collections.Generic;

    using Bg.Chess.Common.Enums;
    using Bg.Chess.Data.Repo;

    using Moq;

    using NUnit.Framework;

    public class PlayTests
    {
        private IGameManager _manager;
        private IGameService _gameService;
        private Player _player1 = new Player { Id = 217 };
        private Player _player2 = new Player { Id = 999 };

        [SetUp]
        public void SetUp()
        {
            var pieceTypes = new PieceTypes();
            _manager = new GameManager(pieceTypes, new TestLoggerFactory());
            _manager.Init(new List<IGameInfo>());

            var mock = new Mock<IGameRepo>();
            mock.Setup(a => a.SaveGame(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<FinishReason>(), It.IsAny<GameSide?>(), It.IsAny<GameMode>(), It.IsAny<string>()));
            _gameService = new GameService(null, null, mock.Object, pieceTypes, new TestLoggerFactory());
        }

        [Test]
        public void CheckStateAfterTwoConfirmTest()
        {
            _manager.StartSearch(_player1, GameMode.Classic);
            _manager.StartSearch(_player2, GameMode.Classic);
            _manager.Confirm(_player1.Id);
            _manager.Confirm(_player2.Id);

            var game = _manager.FindMyPlayingGame(_player2.Id);
            Assert.AreEqual(true, game.IsMyGame(_player1.Id));
            Assert.AreEqual(true, game.IsMyGame(_player2.Id));

            game.Move(game.WhitePlayer.Id, 5, 1, 5, 3);
            game.Move(game.BlackPlayer.Id, 5, 6, 5, 4);
            _gameService.SaveGame(game);
        }

        [Test]
        public void CheckStateAfterTwoConfirmTargetGameTest()
        {
            _manager.StartSearchTargetGame(_player1, _player2, GameMode.Classic);
            _manager.ConfirmTargetGame(_player2.Id);
            _manager.ConfirmTargetGame(_player1.Id);

            var game = _manager.FindMyPlayingGame(_player2.Id);
            Assert.AreEqual(true, game.IsMyGame(_player1.Id));
            Assert.AreEqual(true, game.IsMyGame(_player2.Id));

            game.Move(game.WhitePlayer.Id, 5, 1, 5, 3);
            game.Move(game.BlackPlayer.Id, 5, 6, 5, 4);
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
            _manager.StartSearch(_player1, GameMode.Classic);
            _manager.StartSearch(_player2, GameMode.Classic);
            _manager.Confirm(_player1.Id);
            _manager.Confirm(_player2.Id);

            var game = _manager.FindMyPlayingGame(_player2.Id);
            Assert.AreEqual(true, game.IsMyGame(_player1.Id));
            Assert.AreEqual(true, game.IsMyGame(_player2.Id));

            game.Move(game.WhitePlayer.Id, 7, 1, 7, 3);
            game.Move(game.BlackPlayer.Id, 6, 6, 6, 4);
            game.Move(game.WhitePlayer.Id, 7, 3, 6, 4);
            game.Move(game.BlackPlayer.Id, 6, 7, 5, 5);
            game.Move(game.WhitePlayer.Id, 6, 4, 6, 5);
            game.Move(game.BlackPlayer.Id, 7, 6, 7, 4);
            game.Move(game.WhitePlayer.Id, 2, 1, 2, 2);
            game.Move(game.BlackPlayer.Id, 7, 7, 7, 6);
            game.Move(game.WhitePlayer.Id, 1, 1, 1, 2);
            game.Move(game.BlackPlayer.Id, 5, 7, 7, 5);
            game.Move(game.WhitePlayer.Id, 6, 5, 6, 6);
            game.Move(game.BlackPlayer.Id, 7, 4, 7, 3);
            game.Move(game.WhitePlayer.Id, 6, 6, 6, 7, "queen");

            _gameService.SaveGame(game);
        }
    }
}