using Bg.Chess.Game;
using Bg.Chess.Web.Repo;
using Newtonsoft.Json;

namespace Bg.Chess.Web.Service
{
    public interface IGameService
    {
        public void SaveGame(IGameInfo game);
    }

    //todo сервисы перевезти в сборку Game
    //todo репозитории в сборку Data
    public class GameService : IGameService
    {
        private IGameRepo _gameRepo;

        public GameService(IGameRepo gameRepo)
        {
            _gameRepo = gameRepo;
        }

        public void SaveGame(IGameInfo game)
        {
            var gameDto = new SaveGameDtoV1();
            gameDto.Test = game.Status.ToString();
            // заполнить Dto из объекта
            string data = JsonConvert.SerializeObject(gameDto);
            _gameRepo.SaveGame(0, game.WhitePlayerId, game.BlackPlayerId, data);
        }
    }
}
