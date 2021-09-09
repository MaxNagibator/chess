namespace Bg.Chess.Game.Tests
{
    using NUnit.Framework;

    using Bg.Chess.Common.Enums;

    public class SearchGameTests
    {
        private IGameManager _manager;
        private int _player1 = 217;
        private int _player2 = 999;

        [SetUp]
        public void SetUp()
        {
            InitEnv();
        }

        private void InitEnv()
        {
            _manager = new GameManager(new TestLoggerFactory());
        }

        [Test]
        public void CheckStateWithoutSearchTest()
        {
            var state = _manager.Check(_player1);
            Assert.AreEqual(SearchStatus.NotFound, state);
        }

        [Test]
        public void CheckStateAfterStartSearchTest()
        {
            _manager.StartSearch(_player1);
            var state = _manager.Check(_player1);
            Assert.AreEqual(SearchStatus.InProcess, state);
        }

        [Test]
        public void CheckStateAfterGameFoundTest()
        {
            _manager.StartSearch(_player1);
            _manager.StartSearch(_player2);
            var state1 = _manager.Check(_player1);
            var state2 = _manager.Check(_player2);
            Assert.AreEqual(SearchStatus.NeedConfirm, state1);
            Assert.AreEqual(SearchStatus.NeedConfirm, state2);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void CheckStateAfterOneConfirmTest(int confirmerNumber)
        {
            InitEnv();
            _manager.StartSearch(_player1);
            _manager.StartSearch(_player2);

            var confirmPlayer = confirmerNumber == 1 ? _player1 : _player2;
            var anotherPlayer = confirmerNumber == 1 ? _player2 : _player1;
            _manager.Confirm(confirmPlayer);
            var state1 = _manager.Check(confirmPlayer);
            var state2 = _manager.Check(anotherPlayer);
            Assert.AreEqual(SearchStatus.NeedConfirmOpponent, state1);
            Assert.AreEqual(SearchStatus.NeedConfirm, state2);
        }

        [Test]
        public void CheckStateAfterTwoConfirmTest()
        {
            _manager.StartSearch(_player1);
            _manager.StartSearch(_player2);

            var state1a = _manager.Confirm(_player1);
            var state2a = _manager.Confirm(_player2);
            var state1b = _manager.Check(_player1);
            var state2b = _manager.Check(_player2);
            Assert.AreEqual(SearchStatus.NeedConfirmOpponent, state1a);
            Assert.AreEqual(SearchStatus.Finish, state2a);
            Assert.AreEqual(SearchStatus.Finish, state1b);
            Assert.AreEqual(SearchStatus.Finish, state2b);
        }
    }
}