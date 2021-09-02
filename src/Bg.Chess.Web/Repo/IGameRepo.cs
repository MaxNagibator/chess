using Bg.Chess.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bg.Chess.Web.Repo
{
    public interface IGameRepo
    {
        ChessGame GetGame(int gameId);
        public void SaveGame(int id, int whitePlayerId, int blackPlayerId, int status, string data);
        List<ChessGame> GetGames(int playerId);
    }

    public class GameRepo : IGameRepo
    {
        private readonly IUnitOfWork _unitOfWork;

        public GameRepo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ChessGame GetGame(int gameId)
        {
            var dbGame = _unitOfWork.Context.ChessGames.FirstOrDefault(x => x.Id == gameId);
            return dbGame;
        }

        public List<ChessGame> GetGames(int playerId)
        {
            var dbGames = _unitOfWork.Context.ChessGames
                .Where(x => x.BlackPlayerId == playerId || x.WhitePlayerId == playerId).ToList();
            return dbGames;
        }

        public void SaveGame(int id, int whitePlayerId, int blackPlayerId, int status, string data)
        {
            var dbGame = _unitOfWork.Context.ChessGames.FirstOrDefault(x => x.Id == id);
            if (dbGame == null)
            {
                dbGame = new ChessGame();
                _unitOfWork.Context.ChessGames.Add(dbGame);
            }

            dbGame.WhitePlayerId = whitePlayerId;
            dbGame.BlackPlayerId = blackPlayerId;
            dbGame.Status = status;
            dbGame.Data = data;
            _unitOfWork.Context.SaveChanges();
        }
    }
}
