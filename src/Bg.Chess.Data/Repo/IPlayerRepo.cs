namespace Bg.Chess.Data.Repo
{
    using System;
    using System.Linq;

    public interface IPlayerRepo
    {
        public ChessPlayer FindPlayerByUserId(string userId);
        public ChessPlayer CreatePlayer(string userId, string name);
        public ChessPlayer GetPlayer(int id);
    }

    public class PlayerRepo : IPlayerRepo
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlayerRepo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ChessPlayer GetPlayer(int id)
        {
            var player = _unitOfWork.Context.ChessPlayers
                .First(x => x.Id == id);
            return player;
        }

        public ChessPlayer FindPlayerByUserId(string userId)
        {
            var player = _unitOfWork.Context.ChessPlayers
                .FirstOrDefault(x => x.UserId == userId);
            return player;
        }

        public ChessPlayer CreatePlayer(string userId, string name)
        {
            var player = _unitOfWork.Context.ChessPlayers
                .FirstOrDefault(x => x.UserId == userId);
            if (player != null)
            {
                throw new Exception("user has player");
            }

            player = new ChessPlayer();
            player.UserId = userId;
            player.Name = name;
            _unitOfWork.Context.ChessPlayers.Add(player);
            _unitOfWork.Context.SaveChanges();

            return player;
        }
    }
}
