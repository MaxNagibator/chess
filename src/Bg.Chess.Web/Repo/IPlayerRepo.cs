using Bg.Chess.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bg.Chess.Web.Repo
{
    public interface IPlayerRepo
    {
        public ChessPlayer GetPlayer(string userId);
        public ChessPlayer CreatePlayer(string userId, string name);
    }

    public class PlayerRepo : IPlayerRepo
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlayerRepo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ChessPlayer GetPlayer(string userId)
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
