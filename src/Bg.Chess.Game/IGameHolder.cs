using System;
using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Game
{
    public interface IGameHolder
    {
        public void StartGame(int playerId);

        void AddGame(string gameId, int whitePlayerId, int blackPlayerId);
    }

    public class GameHolder : IGameHolder
    {
        private List<IGameInfo> games = new List<IGameInfo>();

        public void AddGame(string gameId, int whitePlayerId, int blackPlayerId)
        {
            var game = GetGame();
            game.Init(gameId, whitePlayerId, blackPlayerId);
            games.Add(game);
        }

        private IGameInfo GetGame()
        {
            return new GameInfo();
        }

        public GameState StartGame(string gameId, int playerId)
        {
            var game = games.First(x => x.Id == gameId);
            game.ConfirmStart(playerId);
            return game.GetState();
        }
    }
}
