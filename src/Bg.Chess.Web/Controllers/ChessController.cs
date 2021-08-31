namespace Bg.Chess.Web.Controllers
{
    using Bg.Chess.Domain;
    using Bg.Chess.Game;
    using Bg.Chess.Web.Models;
    using Bg.Chess.Web.Repo;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class ChessController : Controller
    {
        private readonly ILogger<ChessController> _logger;
        private ISearchManager _searchManager;
        private IGameHolder _gameHolder;
        private IPlayerService _playerService;

        public ChessController(ILogger<ChessController> logger, ISearchManager searchManager, IGameHolder gameHolder, IPlayerService playerService)
        {
            _logger = logger;
            _searchManager = searchManager;
            _gameHolder = gameHolder;
            _playerService = playerService;
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
            var player = _playerService.GetOrCreatePlayer(userId, userName);
            _searchManager.Start(player.Id);
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
            _searchManager.Stop(playerId);
            return Json(new { error = false });
        }

        [HttpPost]
        public JsonResult GetGame()
        {
            var playerId = GetPlayerId();
            var game = _gameHolder.GetMyPlayingGame(playerId);
            return InitFieldResponse(playerId, game);
        }

        [HttpPost]
        public JsonResult Move(int fromX, int fromY, int toX, int toY)
        {
            var playerId = GetPlayerId();
            var game = _gameHolder.GetMyPlayingGame(playerId);
            game.Move(playerId, fromX, fromY, toX, toY);
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
            var player = _playerService.GetPlayer(userId);
            return player.Id;
        }

    }
}
