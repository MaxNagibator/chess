using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Game
{
    public interface IGameHolder
    {
        void AddGame(string gameId, int whitePlayerId, int blackPlayerId);
        GameStatus StartGame(int playerId);
        GameStatus StopGame(int playerId);
        IGameInfo GetMyPlayingGame(int playerId);
    }

    public class GameHolder : IGameHolder
    {
        private List<IGameInfo> games = new List<IGameInfo>();

        public void AddGame(string gameId, int whitePlayerId, int blackPlayerId)
        {
            var whiteGame = games.FirstOrDefault(x => x.IsMyGame(whitePlayerId));
            if(whiteGame != null)
            {
                games.Remove(whiteGame);
            }
            var blackGame = games.FirstOrDefault(x => x.IsMyGame(blackPlayerId));
            if (blackGame != null)
            {
                games.Remove(blackGame);
            }

            var game = InitObj();
            game.Init(gameId, whitePlayerId, blackPlayerId);
            games.Add(game);
        }

        private IGameInfo InitObj()
        {
            return new GameInfo();
        }

        public GameStatus StartGame(int playerId)
        {
            var game = games.First(x => x.Status == GameStatus.WaitStart && x.IsMyGame(playerId));
            game.ConfirmStart(playerId);
            return game.Status;
        }

        public GameStatus StopGame(int playerId)
        {
            var game = games.First(x => x.Status == GameStatus.WaitStart && x.IsMyGame(playerId));
            game.StopStart(playerId);
            return game.Status;
        }

        public IGameInfo GetMyPlayingGame(int playerId)
        {
            var game = games.FirstOrDefault(x => x.IsMyGame(playerId));// && x.State == GameState.InProgress);
            if (game == null)
            {
                return null;// throw new BusinessException("Не найдено игр в процессе");
            }

            return game;
        }
    }
}
