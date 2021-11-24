namespace Bg.Chess.Data.Repo
{
    using System.Collections.Generic;
    using System.Linq;

    using Chess.Common.Enums;

    public interface IGameRepo
    {
        ChessGame GetGame(string gameId);
        void SaveGame(string id, int whitePlayerId, int blackPlayerId, FinishReason? finishReason, GameSide? winSide, GameMode gameMode, string data);
        List<ChessGame> GetGames(int playerId);
        List<ChessGame> GetNotFinishGames();
    }

    public class GameRepo : IGameRepo
    {
        private readonly IUnitOfWork _unitOfWork;

        public GameRepo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ChessGame GetGame(string gameId)
        {
            var dbGame = _unitOfWork.Context.ChessGames.FirstOrDefault(x => x.LogicalName == gameId);
            return dbGame;
        }

        public List<ChessGame> GetGames(int playerId)
        {
            var dbGames = _unitOfWork.Context.ChessGames
                .Where(x => x.BlackPlayerId == playerId || x.WhitePlayerId == playerId).OrderByDescending(x => x.Id).ToList();
            return dbGames;
        }

        public List<ChessGame> GetNotFinishGames()
        {
            return _unitOfWork.Context.ChessGames.Where(x => x.FinishReason == null).ToList();
        }

        public void SaveGame(string id, int whitePlayerId, int blackPlayerId, FinishReason? finishReason, GameSide? winSide, GameMode gameMode, string data)
        {
            var dbGame = _unitOfWork.Context.ChessGames.FirstOrDefault(x => x.LogicalName == id);
            if (dbGame == null)
            {
                dbGame = new ChessGame();
                _unitOfWork.Context.ChessGames.Add(dbGame);
            }

            dbGame.GameMode = (int)gameMode;
            dbGame.LogicalName = id;
            dbGame.WhitePlayerId = whitePlayerId;
            dbGame.BlackPlayerId = blackPlayerId;
            dbGame.FinishReason = (int?)finishReason;
            dbGame.WinSide = (int?)winSide;
            dbGame.Data = data;
            _unitOfWork.Context.SaveChanges();
        }
    }
}
