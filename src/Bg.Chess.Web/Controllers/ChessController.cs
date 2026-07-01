namespace Bg.Chess.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    using Bg.Chess.Game;
    using Bg.Chess.Web.Models;
    using Bg.Chess.Common.Enums;

    [Authorize]
    public class ChessController : BaseController
    {
        private readonly ILogger _logger;
        private IGameManager _gameManager;
        private IPlayerService _playerService;
        private IUserService _userService;
        private IGameService _gameService;
        private static object lockObject = new object();
        private static readonly System.Random BotRandom = new System.Random();

        public ChessController(
            ILoggerFactory loggerFactory,
            IGameManager gameManager,
            IPlayerService playerService,
            IUserService userService,
            IGameService gameService) : base(loggerFactory, userService)
        {
            _logger = loggerFactory.CreateLogger("chess");
            _gameManager = gameManager;
            _playerService = playerService;
            _userService = userService;
            _gameService = gameService;

            // todo костылища страшный
            if (!gameManager.IsInit)
            {
                lock (lockObject)
                {
                    if (!gameManager.IsInit)
                    {
                        var games = _gameService.GetNotFinishGames();
                        gameManager.Init(games);
                    }
                }
            }
        }

        [HttpGet]
        public ViewResult Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userService.GetUser(userId);

            return View();
        }

        [HttpPost]
        public JsonResult StartSearch(string mode)
        {
            var gameMode = GetMode(mode);

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = this.User.FindFirstValue(ClaimTypes.Name);

            var user = _userService.GetUser(userId);
            if (!user.IsEmailConfirmed)
            {
                return Json(new { error = true, message = "Подтвердите почту, перед началом игры" });
            }

            var player = _playerService.GetOrCreatePlayerByUserId(userId, userName);
            _gameManager.StartSearch(player, gameMode);
            return Json(new { error = false });
        }

        private static GameMode GetMode(string mode)
        {
            GameMode gameMode;
            if (mode == "classic")
            {
                gameMode = GameMode.Classic;
            }
            else if (mode == "dragon")
            {
                gameMode = GameMode.Dragon;
            }
            else
            {
                throw new System.Exception("unrecognized mode " + mode);
            }

            return gameMode;
        }

        [HttpPost]
        public JsonResult CheckSearch()
        {
            var playerId = GetPlayerId();
            var status = _gameManager.Check(playerId);
            return Json(new { status = status.ToString() });
        }

        [HttpPost]
        public JsonResult ConfirmSearch()
        {
            var playerId = GetPlayerId();
            var status = _gameManager.Confirm(playerId);
            return Json(new { status = status.ToString() });
        }

        [HttpPost]
        public JsonResult StopSearch()
        {
            var playerId = GetPlayerId();
            _gameManager.StopSearch(playerId);
            return Json(new { error = false });
        }

        [HttpPost]
        public JsonResult StartBotGame(string mode, string difficulty)
        {
            var gameMode = GetMode(mode);
            var botDifficulty = GetBotDifficulty(difficulty);

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = this.User.FindFirstValue(ClaimTypes.Name);

            var user = _userService.GetUser(userId);
            if (!user.IsEmailConfirmed)
            {
                return Json(new { error = true, message = "Подтвердите почту, перед началом игры" });
            }

            var player = _playerService.GetOrCreatePlayerByUserId(userId, userName);
            var botPlayer = _playerService.GetOrCreatePlayerByUserId("bot:" + botDifficulty, GetBotName(botDifficulty));
            var game = _gameManager.StartBotGame(player, botPlayer, gameMode);
            _gameService.SaveGame(game);

            return InitFieldResponse(player.Id, game);
        }

        public JsonResult StartSearchTargetGame(string mode, string playerName)
        {
            var gameMode = GetMode(mode);

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = this.User.FindFirstValue(ClaimTypes.Name);

            var user = _userService.GetUser(userId);
            if (!user.IsEmailConfirmed)
            {
                return Json(new { error = true, message = "Подтвердите почту, перед началом игры" });
            }
            var player = _playerService.GetOrCreatePlayerByUserId(userId, userName);

            var targetPlayer = _playerService.FindPlayerByName(playerName);
            if (targetPlayer == null)
            {
                throw new BusinessException("Противник " + playerName + " не найден");
            }
            _gameManager.StartSearchTargetGame(player, targetPlayer, gameMode);
            return Json(new { error = false });
        }

        [HttpPost]
        public JsonResult CheckSearchTargetGame()
        {
            var playerId = GetPlayerId();
            var status = _gameManager.CheckTargetGame(playerId);
            return Json(new { status = status.Status.ToString(), opponentName = status.OpponentPlayer?.Name });
        }

        [HttpPost]
        public JsonResult ConfirmSearchTargetGame()
        {
            var playerId = GetPlayerId();
            var status = _gameManager.ConfirmTargetGame(playerId);
            return Json(new { status = status.Status.ToString(), opponentName = status.OpponentPlayer?.Name });
        }

        [HttpPost]
        public JsonResult StopSearchTargetGame()
        {
            var playerId = GetPlayerId();
            _gameManager.StopSearchTargetGame(playerId);
            return Json(new { error = false });
        }

        [HttpPost]
        public JsonResult GetGame()
        {
            var playerId = GetPlayerId();
            var game = _gameManager.FindMyPlayingGame(playerId);
            return InitFieldResponse(playerId, game);
        }

        [HttpPost]
        public JsonResult Move(int fromX, int fromY, int toX, int toY, string pawnTransformPiece)
        {
            var playerId = GetPlayerId();
            var game = _gameManager.FindMyPlayingGame(playerId);
            game.Move(playerId, fromX, fromY, toX, toY, pawnTransformPiece);
            if (!game.IsFinish && game.GameType == GameType.Bot)
            {
                MoveBot(game);
            }
            _gameService.SaveGame(game);

            return InitFieldResponse(playerId, game);
        }

        [HttpPost]
        public JsonResult Surrender()
        {
            var playerId = GetPlayerId();
            var game = _gameManager.FindMyPlayingGame(playerId);
            game.Surrender(playerId);
            _gameService.SaveGame(game);
            return InitFieldResponse(playerId, game);
        }

        private JsonResult InitFieldResponse(int playerId, IGameInfo game)
        {
            if (game == null)
            {
                return Json(new { error = true, message = "Game not found" });
            }

            string notation;
            List<AvailableMove> moves = null;
            if (game.IsFinish)
            {
                notation = game.GetForsythEdwardsNotation(true);
            }
            else
            {
                notation = game.GetForsythEdwardsNotation();
                moves = game.AvailableMoves();
            }

            var historyMoves = game.GetMoves();

            var side = game.WhitePlayer.Id == playerId ? "White" : "Black";
            var stepSide = game.StepSide == GameSide.White ? "White" : "Black";
            string winSide = null;
            if (game.WinSide != null)
            {
                winSide = game.WinSide == GameSide.White ? "White" : "Black";
            }
            var finishReason = "";
            if (game.IsFinish)
            {
                switch (game.FinishReason)
                {
                    case FinishReason.Mate:
                        finishReason = "Mate";
                        break;
                    case FinishReason.Surrender:
                        finishReason = "Surrender";
                        break;
                    case FinishReason.Draw:
                        finishReason = "Draw";
                        break;
                    case FinishReason.TimeOver:
                        finishReason = "TimeOver";
                        break;
                }
            }

            // todo сделать DTO для отправки на фронт
            return Json(new
            {
                Id = game.Id,
                EnemyName = game.WhitePlayer.Id == playerId ? game.BlackPlayer.Name : game.WhitePlayer.Name,
                GameType = game.GameType.ToString(),
                FieldWidth = game.FieldWidth,
                FieldHeight = game.FieldHeight,
                Notation = notation,
                AvailableMoves = moves,
                HistoryMoves = historyMoves,
                Side = side,
                StepSide = stepSide,
                IsFinish = game.IsFinish,
                FinishReason = finishReason,
                WinSide = winSide,
            });
        }

        private int GetPlayerId()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var player = _playerService.FindPlayerByUserId(userId);
            if (player == null)
            {
                throw new BusinessException("Ваш игровой профиль не готов");
            }
            return player.Id;
        }

        private void MoveBot(IGameInfo game)
        {
            var botPlayer = game.BlackPlayer.UserId.StartsWith("bot:") ? game.BlackPlayer : game.WhitePlayer;
            var difficulty = GetBotDifficulty(botPlayer.UserId.Replace("bot:", string.Empty));
            var move = ChooseBotMove(game, difficulty);
            if (move == null)
            {
                return;
            }

            var pawnTransformPiece = GetPawnTransformPiece(game, move);
            game.Move(botPlayer.Id, move.From.X, move.From.Y, move.To.X, move.To.Y, pawnTransformPiece);
        }

        private AvailableMoveCandidate ChooseBotMove(IGameInfo game, string difficulty)
        {
            var moves = game.AvailableMoves()
                .SelectMany(move => move.To.Select(to => new AvailableMoveCandidate { From = move.From, To = to }))
                .ToList();
            if (!moves.Any())
            {
                return null;
            }

            if (difficulty == "easy")
            {
                return moves[BotRandom.Next(moves.Count)];
            }

            var pieces = ReadPieces(game);
            var evaluatedMoves = moves.Select(move =>
            {
                var targetPiece = pieces.FirstOrDefault(x => x.X == move.To.X && x.Y == move.To.Y);
                var runnerPiece = pieces.FirstOrDefault(x => x.X == move.From.X && x.Y == move.From.Y);
                var captureScore = targetPiece == null ? 0 : GetPieceValue(targetPiece.Type);
                var centerScore = 4 - System.Math.Abs(3 - move.To.X) - System.Math.Abs(3 - move.To.Y);
                var promotionScore = IsPromotionMove(game, move, runnerPiece) ? 8 : 0;
                var score = captureScore * 10 + promotionScore;
                if (difficulty == "hard")
                {
                    score += centerScore;
                    score += GetPieceValue(runnerPiece?.Type) < captureScore ? 2 : 0;
                }

                return new { Move = move, Score = score };
            }).ToList();

            var bestScore = evaluatedMoves.Max(x => x.Score);
            var bestMoves = evaluatedMoves.Where(x => x.Score == bestScore).Select(x => x.Move).ToList();
            return bestMoves[BotRandom.Next(bestMoves.Count)];
        }

        private string GetPawnTransformPiece(IGameInfo game, AvailableMoveCandidate move)
        {
            var piece = ReadPieces(game).FirstOrDefault(x => x.X == move.From.X && x.Y == move.From.Y);
            if (IsPromotionMove(game, move, piece))
            {
                return "queen";
            }

            return null;
        }

        private bool IsPromotionMove(IGameInfo game, AvailableMoveCandidate move, BotPiece piece)
        {
            if (piece == null || (piece.Type != "pawn" && piece.Type != "soldier"))
            {
                return false;
            }

            return (piece.Side == "White" && move.To.Y == game.FieldHeight - 1)
                || (piece.Side == "Black" && move.To.Y == 0);
        }

        private List<BotPiece> ReadPieces(IGameInfo game)
        {
            var notation = game.GetForsythEdwardsNotation().Split(' ')[0];
            var lines = notation.Split('/');
            var result = new List<BotPiece>();
            for (var rowIndex = 0; rowIndex < lines.Length; rowIndex++)
            {
                var x = 0;
                var y = game.FieldHeight - 1 - rowIndex;
                for (var symbolIndex = 0; symbolIndex < lines[rowIndex].Length; symbolIndex++)
                {
                    var symbol = lines[rowIndex][symbolIndex];
                    if (char.IsDigit(symbol))
                    {
                        var emptyFieldsText = symbol.ToString();
                        while (symbolIndex + 1 < lines[rowIndex].Length && char.IsDigit(lines[rowIndex][symbolIndex + 1]))
                        {
                            symbolIndex++;
                            emptyFieldsText += lines[rowIndex][symbolIndex];
                        }

                        var emptyFields = int.Parse(emptyFieldsText);
                        x += emptyFields;
                        continue;
                    }

                    result.Add(new BotPiece
                    {
                        X = x,
                        Y = y,
                        Side = char.IsUpper(symbol) ? "White" : "Black",
                        Type = GetPieceTypeByNotation(char.ToLower(symbol)),
                    });
                    x++;
                }
            }

            return result;
        }

        private string GetPieceTypeByNotation(char symbol)
        {
            switch (symbol)
            {
                case 'p':
                    return "pawn";
                case 'r':
                    return "rook";
                case 'n':
                    return "knight";
                case 'b':
                    return "bishop";
                case 'q':
                    return "queen";
                case 'k':
                    return "king";
                case 'd':
                    return "dragon";
                case 's':
                    return "soldier";
                case 'h':
                    return "hydra";
                default:
                    return symbol.ToString();
            }
        }

        private int GetPieceValue(string type)
        {
            switch (type)
            {
                case "pawn":
                case "soldier":
                    return 1;
                case "knight":
                case "bishop":
                    return 3;
                case "rook":
                case "hydra":
                    return 5;
                case "queen":
                case "dragon":
                    return 9;
                case "king":
                    return 100;
                default:
                    return 0;
            }
        }

        private string GetBotDifficulty(string difficulty)
        {
            switch (difficulty)
            {
                case "easy":
                case "normal":
                case "hard":
                    return difficulty;
                default:
                    return "normal";
            }
        }

        private string GetBotName(string difficulty)
        {
            switch (difficulty)
            {
                case "easy":
                    return "Бот: легкий";
                case "hard":
                    return "Бот: сложный";
                default:
                    return "Бот: нормальный";
            }
        }

        private class AvailableMoveCandidate
        {
            public Position From { get; set; }
            public Position To { get; set; }
        }

        private class BotPiece
        {
            public int X { get; set; }
            public int Y { get; set; }
            public string Side { get; set; }
            public string Type { get; set; }
        }

        [HttpGet]
        public ActionResult History()
        {
            var playerId = GetPlayerId();
            var games = _gameService.GetGames(playerId);

            var model = new HistoryModel();
            model.Games = games.Select(x =>
                new HistoryModel.Game
                {
                    Id = x.Id,
                    BlackPlayer = FillPlayer(x.BlackPlayer),
                    WhitePlayer = FillPlayer(x.WhitePlayer),
                    FinishReason = x.FinishReason,
                    WinSide = x.WinSide,
                }).ToList();
            model.MyPlayerId = playerId;

            HistoryModel.Player FillPlayer(Player player)
            {
                return new HistoryModel.Player
                {
                    Id = player.Id,
                    Name = player.Name,
                };
            }

            return View("History", model);
        }

        [HttpGet]
        [Route("/History/{gameId}")]
        [AllowAnonymous]
        public ActionResult HistoryGame(string gameId)
        {
            var game = _gameService.GetGame(gameId);
            var model = new HistoryGameModel
            {
                Id = game.Id,
                BlackPlayer = FillPlayer(game.BlackPlayer),
                WhitePlayer = FillPlayer(game.WhitePlayer),
                FinishReason = game.FinishReason,
                WinSide = game.WinSide,
            };

            HistoryGameModel.Player FillPlayer(Player player)
            {
                return new HistoryGameModel.Player
                {
                    Id = player.Id,
                    Name = player.Name,
                };
            }

            model.Moves = game.Moves.Select(x =>
            {
                var dto = FillMove(x);
                dto.AdditionalMove = FillMove(x.AdditionalMove);
                if (x.KillEnemy != null)
                {
                    dto.KillEnemy = x.KillEnemy.GetNotation();

                }
                return dto;
            }).ToList();

            model.Positions = game.Positions;
            return View("HistoryGame", model);
        }

        [HttpGet]
        [Route("/HistoryData/{gameId}")]
        [AllowAnonymous]
        public JsonResult GetHistoryData(string gameId)
        {
            var game = _gameService.GetGame(gameId);
           
            return Json(new
            {
                notation = game.Positions,
                fieldWidth = game.Positions.Split('/')[0].Length,
                fieldHeight = game.Positions.Split('/').Length,
            });
        }

        private HistoryGameModel.Move FillMove(Bg.Chess.Game.Move x)
        {
            if (x == null)
            {
                return null;
            }
            return new HistoryGameModel.Move()
            {
                Runner = x.Runner.GetNotation(),
                From = x.From == null ? null : new HistoryGameModel.Position
                {
                    X = x.From.X,
                    Y = x.From.Y,
                },
                To = new HistoryGameModel.Position
                {
                    X = x.To.X,
                    Y = x.To.Y,
                },

            };
        }
    }
}
