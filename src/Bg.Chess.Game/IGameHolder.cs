using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Game
{
    public interface IGameHolder
    {
        void AddGame(string gameId, int whitePlayerId, int blackPlayerId);
        GameState StartGame(int playerId);
        GameState StopGame(int playerId);
        IGameInfo GetMyPlayingGame(int playerId);
    }

    public class GameHolder : IGameHolder
    {
        private List<IGameInfo> games = new List<IGameInfo>();

        public void AddGame(string gameId, int whitePlayerId, int blackPlayerId)
        {
            var game = InitObj();
            game.Init(gameId, whitePlayerId, blackPlayerId);
            games.Add(game);
        }

        private IGameInfo InitObj()
        {
            return new GameInfo();
        }

        public GameState StartGame(int playerId)
        {
            var game = games.First(x => x.State == GameState.WaitStart && x.IsMyGame(playerId));
            game.ConfirmStart(playerId);
            return game.State;
        }

        public GameState StopGame(int playerId)
        {
            var game = games.First(x => x.State == GameState.WaitStart && x.IsMyGame(playerId));
            game.StopStart(playerId);
            return game.State;
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
