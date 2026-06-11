namespace Bg.Chess.Domain.Tests.GameTests
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            Assert.AreEqual(GameState.Finish, game.State);
            Assert.AreEqual(FinishReason.Draw, game.FinishReason);
            Assert.IsNull(game.WinSide);
        }

        // писал бот, не проверял
        [Test]
        public void DrawAfterThreefoldRepetitionTest()
        {
            var game = new Game();
            game.Init();

            game.Move(Side.White, "g1", "f3");
            game.Move(Side.Black, "g8", "f6");
            game.Move(Side.White, "f3", "g1");
            game.Move(Side.Black, "f6", "g8");
            game.Move(Side.White, "g1", "f3");
            game.Move(Side.Black, "g8", "f6");
            game.Move(Side.White, "f3", "g1");
            game.Move(Side.Black, "f6", "g8");

            Assert.AreEqual(GameState.Finish, game.State);
            Assert.AreEqual(FinishReason.Draw, game.FinishReason);
            Assert.IsNull(game.WinSide);
        }

        // писал бот, не проверял
        [Test]
        public void DrawAfterFiftyMovesWithoutPawnMoveOrCaptureTest()
        {
            var playedMoves = new List<PlayedMove>();
            var positionRepeatCounts = new Dictionary<string, int>();
            var game = CreateFiftyMoveGame();
            RegisterPosition(game, positionRepeatCounts);

            for (var i = 0; i < 99; i++)
            {
                var move = FindNextFiftyMoveRuleMove(playedMoves, positionRepeatCounts);
                playedMoves.Add(move);
                game.Move(move.Side, move.FromX, move.FromY, move.ToX, move.ToY);
                RegisterPosition(game, positionRepeatCounts);

                Assert.AreEqual(GameState.InProgress, game.State);
            }

            var finalMove = FindNextFiftyMoveRuleMove(playedMoves, positionRepeatCounts);
            game.Move(finalMove.Side, finalMove.FromX, finalMove.FromY, finalMove.ToX, finalMove.ToY);

            Assert.AreEqual(GameState.Finish, game.State);
            Assert.AreEqual(FinishReason.Draw, game.FinishReason);
            Assert.IsNull(game.WinSide);
        }

        // писал бот, не проверял
        [Test]
        public void PawnMoveResetsFiftyMoveRuleCounterTest()
        {
            var game = new Game();
            game.Init();

            game.Move(Side.White, "g1", "f3");
            game.Move(Side.Black, "g8", "f6");
            Assert.AreEqual("2", game.GetForsythEdwardsNotation().Split(' ')[4]);

            game.Move(Side.White, "e2", "e4");
            Assert.AreEqual("0", game.GetForsythEdwardsNotation().Split(' ')[4]);
            Assert.AreEqual(GameState.InProgress, game.State);
        }

        // писал бот, не проверял
        [Test]
        public void CaptureResetsFiftyMoveRuleCounterTest()
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(0, 0, PieceBuilder.King(Side.White)),
                new Position(7, 7, PieceBuilder.King(Side.Black)),
                new Position(1, 0, PieceBuilder.Rook(Side.White)),
                new Position(1, 7, PieceBuilder.Rook(Side.Black)),
                new Position(6, 7, PieceBuilder.Knight(Side.Black)),
            };

            var game = new Game();
            game.Init(new Field(rules));

            game.Move(Side.White, 0, 0, 0, 1);
            game.Move(Side.Black, 6, 7, 5, 5);
            Assert.AreEqual("2", game.GetForsythEdwardsNotation().Split(' ')[4]);

            game.Move(Side.White, 1, 0, 1, 7);
            Assert.AreEqual("0", game.GetForsythEdwardsNotation().Split(' ')[4]);
            Assert.AreEqual(GameState.InProgress, game.State);
        }

        private static Game CreateFiftyMoveGame()
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(0, 0, PieceBuilder.King(Side.White)),
                new Position(1, 0, PieceBuilder.Knight(Side.White)),
                new Position(6, 0, PieceBuilder.Knight(Side.White)),
                new Position(7, 7, PieceBuilder.King(Side.Black)),
                new Position(1, 7, PieceBuilder.Knight(Side.Black)),
                new Position(6, 7, PieceBuilder.Knight(Side.Black)),
            };

            var game = new Game();
            game.Init(new Field(rules));
            return game;
        }

        private static PlayedMove FindNextFiftyMoveRuleMove(List<PlayedMove> playedMoves, Dictionary<string, int> positionRepeatCounts)
        {
            var game = ReplayFiftyMoveGame(playedMoves);
            var side = game.StepSide;
            var availableMoves = game.AvailableMoves()
                .SelectMany(x => x.To.Select(to => new PlayedMove(side, x.From.X, x.From.Y, to.X, to.Y)))
                .ToList();

            foreach (var move in availableMoves)
            {
                var runner = game.GetPositions().First(x => x.X == move.FromX && x.Y == move.FromY).Piece;
                var target = game.GetPositions().First(x => x.X == move.ToX && x.Y == move.ToY).Piece;
                if (runner.Type is Pawn || target != null)
                {
                    continue;
                }

                var replayMoves = playedMoves.Concat(new[] { move }).ToList();
                var trialGame = ReplayFiftyMoveGame(replayMoves);
                var repeatKey = GetPositionRepeatKey(trialGame);
                if (!positionRepeatCounts.TryGetValue(repeatKey, out var repeatCount) || repeatCount < 2)
                {
                    return move;
                }
            }

            throw new Exception("move for fifty move rule test not found");
        }

        private static Game ReplayFiftyMoveGame(List<PlayedMove> playedMoves)
        {
            var game = CreateFiftyMoveGame();
            foreach (var move in playedMoves)
            {
                game.Move(move.Side, move.FromX, move.FromY, move.ToX, move.ToY);
            }

            return game;
        }

        private static void RegisterPosition(Game game, Dictionary<string, int> positionRepeatCounts)
        {
            var repeatKey = GetPositionRepeatKey(game);
            if (positionRepeatCounts.ContainsKey(repeatKey))
            {
                positionRepeatCounts[repeatKey]++;
            }
            else
            {
                positionRepeatCounts.Add(repeatKey, 1);
            }
        }

        private static string GetPositionRepeatKey(Game game)
        {
            return string.Join(" ", game.GetForsythEdwardsNotation().Split(' ').Take(4));
        }

        private class PlayedMove
        {
            public PlayedMove(Side side, int fromX, int fromY, int toX, int toY)
            {
                Side = side;
                FromX = fromX;
                FromY = fromY;
                ToX = toX;
                ToY = toY;
            }

            public Side Side { get; }
            public int FromX { get; }
            public int FromY { get; }
            public int ToX { get; }
            public int ToY { get; }
        }
    }
}
