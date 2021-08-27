namespace Bg.Chess.Web.Controllers
{
    using Bg.Chess.Web.Models;
    using Bg.Chess.Web.Repo;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    public class ChessController : Controller
    {
        private readonly ILogger<ChessController> _logger;

        public ChessController(ILogger<HomeController> logger)
        {
        }

        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        private static string searchId { get; set; }
        private static int currentCheck { get; set; }

        public JsonResult SearchGame()
        {
            searchId = Guid.NewGuid().ToString("N");
            game = null;
            currentCheck = 0;
            return Json(new { searchId = searchId });
        }

        public JsonResult CheckSearch(string searchId)
        {
            if (currentCheck > 5)
            {
                var gameId = Guid.NewGuid().ToString("N");
                game = new Bg.Chess.Domain.Game();
                game.Init();
                return Json(new { gameId = gameId });
            }
            else
            {
                currentCheck++;
            }

            return Json(new { waitpliz = true });
        }

        private static Bg.Chess.Domain.Game game;
        private static Bg.Chess.Domain.Game Game
        {
            get
            {
                if (game == null)
                {
                    game = new Bg.Chess.Domain.Game();
                    game.Init();
                }
                return game;
            }
        }

        [HttpGet]
        [Route("Chess/Game/{gameId}")]
        public JsonResult GetGame(string gameId)
        {
            var notation = Game.GetForsythEdwardsNotation();
            var moves = Game.AvailableMove();

            return Json(new { Notation = notation, AvailableMoves = moves });
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

        //[HttpPost]
        //public JsonResult Move(int fromX, int fromY, int toX, int toY)
        //{
        //    Game.Move(Game.StepSide, fromX, fromY, toX, toY);
        //    var notation = Game.GetForsythEdwardsNotation();
        //    var moves = Game.AvailableMove();
        //    return Json(new { Notation = notation, AvailableMoves = moves });
        //}
    }
}
