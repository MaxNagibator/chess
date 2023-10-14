namespace Bg.Chess.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    using Bg.Chess.Game;
    using Bg.Chess.Web.Models;
    using Bg.Chess.Common.Enums;
    using Microsoft.AspNetCore.Authorization;
    using Bg.Chess.Data.Repo;
    using Bg.Chess.Web.Models.Common;

    [Authorize]
    public class ChessController : BaseController
    {
        private readonly ILogger _logger;
        private IGameManager _gameManager;
        private IPlayerService _playerService;
        private IUserService _userService;
        private IGameService _gameService;
        private static object lockObject = new object();

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
            var winSide = game.WinSide == GameSide.White ? "White" : "Black";
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
