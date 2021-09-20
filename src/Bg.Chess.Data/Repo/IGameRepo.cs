namespace Bg.Chess.Data.Repo
{
    using System.Collections.Generic;
    using System.Linq;

    using Chess.Common.Enums;

    public interface IGameRepo
    {
        ChessGame GetGame(int gameId);
        void SaveGame(int id, int whitePlayerId, int blackPlayerId, FinishReason finishReason, GameSide? winSide, string data);
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
                .Where(x => x.BlackPlayerId == playerId || x.WhitePlayerId == playerId).OrderByDescending(x => x.Id).ToList();
            return dbGames;
        }

        public void SaveGame(int id, int whitePlayerId, int blackPlayerId, FinishReason finishReason, GameSide? winSide, string data)
        {
            var dbGame = _unitOfWork.Context.ChessGames.FirstOrDefault(x => x.Id == id);
            if (dbGame == null)
            {
                dbGame = new ChessGame();
                _unitOfWork.Context.ChessGames.Add(dbGame);
            }

            dbGame.WhitePlayerId = whitePlayerId;
            dbGame.BlackPlayerId = blackPlayerId;
            dbGame.FinishReason = (int)finishReason;
            dbGame.WinSide = (int)winSide;
            dbGame.Data = data;
            _unitOfWork.Context.SaveChanges();
        }
    }
}
