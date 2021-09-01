using Bg.Chess.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bg.Chess.Web.Repo
{
    public interface IGameRepo
    {
        public void SaveGame(int id, int whitePlayerId, int blackPlayerId, string data);
    }

    public class GameRepo : IGameRepo
    {
        private readonly IUnitOfWork _unitOfWork;

        public GameRepo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void SaveGame(int id, int whitePlayerId, int blackPlayerId, string data)
        {
            var dbGame = _unitOfWork.Context.ChessGames.FirstOrDefault(x => x.Id == id);
            if (dbGame == null)
            {
                dbGame = new ChessGame();
                _unitOfWork.Context.ChessGames.Add(dbGame);
            }

            dbGame.WhitePlayerId = whitePlayerId;
            dbGame.BlackPlayerId = blackPlayerId;
            dbGame.Data = data;
            _unitOfWork.Context.SaveChanges();
        }
    }
}
