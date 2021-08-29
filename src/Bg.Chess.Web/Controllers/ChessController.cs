namespace Bg.Chess.Web.Controllers
{
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
            var notation = game.GetForsythEdwardsNotation();
            var moves = game.AvailableMove();
            var side = game.WhitePlayerId == playerId ? "white" : "black";

            return Json(new { Notation = notation, AvailableMoves = moves, side = side });
        }

        //[HttpGet]
        //public JsonResult GetField()
        //{
        //    var player = _playerRepo.GetPlayer();
        //    var notation = Game.GetForsythEdwardsNotation();
        //    var moves = Game.AvailableMove();
        //    // todo сделать dto
        //    return Json(new { Notation = notation, AvailableMoves = moves, Player = player.Name });
        //}

        [HttpPost]
        public JsonResult Move(int fromX, int fromY, int toX, int toY)
        {
            var playerId = GetPlayerId();
            var game = _gameHolder.GetMyPlayingGame(playerId);
            game.Move(playerId, fromX, fromY, toX, toY);
            var notation = game.GetForsythEdwardsNotation();
            var moves = game.AvailableMove();
            return Json(new { Notation = notation, AvailableMoves = moves });
        }

        private int GetPlayerId()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var player = _playerService.GetPlayer(userId);
            return player.Id;
        }

    }
}
