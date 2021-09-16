namespace Bg.Chess.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System.Security.Claims;

    using Bg.Chess.Game;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System;

    public class BaseController : Controller
    {
        private readonly ILogger _logger;
        private IUserService _userService;

        public BaseController(
            ILoggerFactory loggerFactory,
            IUserService userService)
        {
            _logger = loggerFactory.CreateLogger("base");
            _userService = userService;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                try
                {
                    var user = _userService.GetUser(userId);
                    var model = new Models.Common.User();
                    model.Id = user.Id;
                    model.IsEmailConfirmed = user.IsEmailConfirmed;
                    ViewBag.User = model;
                }catch(Exception ex)
                {
                    _logger.LogError(ex, "Ошибка получения пользователя");
                }
            }

            base.OnActionExecuted(context);
        }
    }
}
