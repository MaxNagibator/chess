using Bg.Chess.Web.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Diagnostics;

namespace Bg.Chess.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // todo 10) сделать игру комфортной, чтоб не надо было жать F5 ниразу
            // todo 15) играть бесконечное количество игр и не жать F5 и не рестартить пул
            // todo 20) логи
            // todo 30) разобраться с регистрацией и авторизаций и прочим
            // todo 40) реализовать "живое обновление", в первой версии таймер на js пойдёт (signalR/websocket)
            // todo 50) удалить jquery из проэкта
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
