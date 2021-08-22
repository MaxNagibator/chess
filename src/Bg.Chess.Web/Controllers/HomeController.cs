using Bg.Chess.Domain;
using Bg.Chess.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Bg.Chess.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Field()
        {
            return View("Field");
        }

        [HttpGet]
        public JsonResult GetField()
        {
            var game = new Game();
            game.Init();
            var notation = game.GetForsythEdwardsNotation();
            return Json(new { Notation = notation});
        }

        public IActionResult Index()
        {
            // todo 1) прикрутить сборку с логикой шахматишек 
            // todo 2) сделать стартовую бомж страницу с отрисовкой поля
            // todo 3) фигуры могут ходить
            // todo 4) добавить игроков
            // todo 5) добавить "поиск игры"
            // todo 6) реализовать "живое обновление", в первой версии на Ф5 пусть жмут (signalR/websocket)
            // todo 7) удалить jquery из проэкта
            // todo 8)
            // todo 9) 
            // todo 10) табличку с результатами игры и "рейтинг"
            // todo 11) логи
            // todo 12) разобраться с регистрацией и авторизаций и прочим
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
