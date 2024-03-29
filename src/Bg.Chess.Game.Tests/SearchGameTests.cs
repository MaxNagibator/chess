namespace Bg.Chess.Game.Tests
{
    using NUnit.Framework;

    using Bg.Chess.Common.Enums;
    using System.Collections.Generic;

    public class SearchGameTests
    {
        private IGameManager _manager;
        private Player _player1 = new Player { Id = 217 };
        private Player _player2 = new Player { Id = 999 };

        [SetUp]
        public void SetUp()
        {
            InitEnv();
        }

        private void InitEnv()
        {
            _manager = new GameManager(new PieceTypes(), new TestLoggerFactory());
            _manager.Init(new List<IGameInfo>());
        }

        [Test]
        public void CheckStateWithoutSearchTest()
        {
            var state = _manager.Check(_player1.Id);
            Assert.AreEqual(RatingSearchStatus.NotFound, state);
        }

        [Test]
        public void CheckStateAfterStartSearchTest()
        {
            _manager.StartSearch(_player1, GameMode.Classic);
            var state = _manager.Check(_player1.Id);
            Assert.AreEqual(RatingSearchStatus.InProcess, state);
        }

        [Test]
        public void CheckStateAfterGameFoundTest()
        {
            _manager.StartSearch(_player1, GameMode.Classic);
            _manager.StartSearch(_player2, GameMode.Classic);
            var state1 = _manager.Check(_player1.Id);
            var state2 = _manager.Check(_player2.Id);
            Assert.AreEqual(RatingSearchStatus.NeedConfirm, state1);
            Assert.AreEqual(RatingSearchStatus.NeedConfirm, state2);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void CheckStateAfterOneConfirmTest(int confirmerNumber)
        {
            InitEnv();
            _manager.StartSearch(_player1, GameMode.Classic);
            _manager.StartSearch(_player2, GameMode.Classic);

            var confirmPlayer = confirmerNumber == 1 ? _player1 : _player2;
            var anotherPlayer = confirmerNumber == 1 ? _player2 : _player1;
            _manager.Confirm(confirmPlayer.Id);
            var state1 = _manager.Check(confirmPlayer.Id);
            var state2 = _manager.Check(anotherPlayer.Id);
            Assert.AreEqual(RatingSearchStatus.NeedConfirmOpponent, state1);
            Assert.AreEqual(RatingSearchStatus.NeedConfirm, state2);
        }

        [Test]
        public void CheckStateAfterTwoConfirmTest()
        {
            _manager.StartSearch(_player1, GameMode.Classic);
            _manager.StartSearch(_player2, GameMode.Classic);

            var state1a = _manager.Confirm(_player1.Id);
            var state2a = _manager.Confirm(_player2.Id);
            var state1b = _manager.Check(_player1.Id);
            var state2b = _manager.Check(_player2.Id);
            Assert.AreEqual(RatingSearchStatus.NeedConfirmOpponent, state1a);
            Assert.AreEqual(RatingSearchStatus.Finish, state2a);
            Assert.AreEqual(RatingSearchStatus.Finish, state1b);
            Assert.AreEqual(RatingSearchStatus.Finish, state2b);
        }
    }
}