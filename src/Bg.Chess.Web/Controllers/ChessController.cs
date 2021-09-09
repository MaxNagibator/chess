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

    public class ChessController : Controller
    {
        private readonly ILogger _logger;
        private IGameManager _searchManager;
        private IPlayerService _playerService;
        private IGameService _gameService;

        public ChessController(
            ILoggerFactory loggerFactory,
            IGameManager searchManager,
            IPlayerService playerService,
            IGameService gameService)
        {
            _logger = loggerFactory.CreateLogger("chess");
            _searchManager = searchManager;
            _playerService = playerService;
            _gameService = gameService;
        }

        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult StartSearch()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = this.User.FindFirstValue(ClaimTypes.Name);
            var player = _playerService.GetOrCreatePlayerByUserId(userId, userName);
            _searchManager.StartSearch(player.Id);
            return Json(new { error = false });
        }

        [HttpPost]
        public JsonResult CheckSearch()
        {
            var playerId = GetPlayerId();
            var status = _searchManager.Check(playerId);
            return Json(new { status = status.ToString() });
        }

        [HttpPost]
        public JsonResult ConfirmSearch()
        {
            var playerId = GetPlayerId();
            var status = _searchManager.Confirm(playerId);
            return Json(new { status = status.ToString() });
        }

        [HttpPost]
        public JsonResult StopSearch()
        {
            var playerId = GetPlayerId();
            _searchManager.StopSearch(playerId);
            return Json(new { error = false });
        }

        [HttpPost]
        public JsonResult GetGame()
        {
            var playerId = GetPlayerId();
            var game = _searchManager.FindMyPlayingGame(playerId);
            return InitFieldResponse(playerId, game);
        }

        [HttpPost]
        public JsonResult Move(int fromX, int fromY, int toX, int toY)
        {
            var playerId = GetPlayerId();
            var game = _searchManager.FindMyPlayingGame(playerId);
            game.Move(playerId, fromX, fromY, toX, toY);
            switch (game.Status)
            {
                case GameStatus.InProgress:
                    break;
                case GameStatus.WaitStart:
                    break;
                case GameStatus.Draw:
                case GameStatus.WinBlack:
                case GameStatus.WinWhite:
                    _gameService.SaveGame(game);
                    break;
            }
            return InitFieldResponse(playerId, game);
        }

        private JsonResult InitFieldResponse(int playerId, IGameInfo game)
        {
            if (game == null)
            {
                return Json(new { error = true, message = "Game not found" });
            }

            string notation = null;
            List<AvailableMove> moves = null;
            if (game.Status == GameStatus.InProgress)
            {
                notation = game.GetForsythEdwardsNotation();
                moves = game.AvailableMoves();
            }
            var side = game.WhitePlayerId == playerId ? "White" : "Black";
            var stepSide = game.StepSide == GameSide.White ? "White" : "Black";
            var status = "";
            switch (game.Status)
            {
                case GameStatus.Draw:
                    status = "Draw";
                    break;
                case GameStatus.InProgress:
                    status = "InProgress";
                    break;
                case GameStatus.WaitStart:
                    status = "WaitStart";
                    break;
                case GameStatus.WinBlack:
                    status = "WinBlack";
                    break;
                case GameStatus.WinWhite:
                    status = "WinWhite";
                    break;
            }


            return Json(new
            {
                Notation = notation,
                AvailableMoves = moves,
                Side = side,
                StepSide = stepSide,
                Status = status
            });
        }

        private int GetPlayerId()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var player = _playerService.GetPlayerByUserId(userId);
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
                    Status = x.Status,
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
        public ActionResult HistoryGame(int gameId)
        {
            var game = _gameService.GetGame(gameId);

            var model = new HistoryGameModel
            {
                Id = game.Id,
                BlackPlayer = FillPlayer(game.BlackPlayer),
                WhitePlayer = FillPlayer(game.WhitePlayer),
                Status = game.Status,
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
                    dto.KillEnemy = x.KillEnemy;

                }
                dto.Runner = x.Runner;
                return dto;
            }).ToList();

            model.Positions = game.Positions;
            return View("HistoryGame", model);
        }

        private HistoryGameModel.Move FillMove(Bg.Chess.Game.Move x)
        {
            if(x == null)
            {
                return null;
            }
            return new HistoryGameModel.Move()
            {
                From = new HistoryGameModel.Position
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
