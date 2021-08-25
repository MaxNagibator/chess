using Bg.Chess.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bg.Chess.Web.Repo
{
    public interface IPlayerRepo
    {
        public ChessPlayer GetPlayer();
    }

    public class PlayerRepo : IPlayerRepo
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlayerRepo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ChessPlayer GetPlayer()
        {
            var player = _unitOfWork.Context.ChessPlayers.FirstOrDefault();
            if(player == null)
            {
                player = new ChessPlayer();
                var nextId = 1;// _unitOfWork.Context.ChessPlayers.Select(x => x.Id).DefaultIfEmpty(0).Max() + 1;
                //player.Id = nextId;
                player.UserId = nextId;
                player.Name = "bobina";
                _unitOfWork.Context.ChessPlayers.Add(player);
                _unitOfWork.Context.SaveChanges();
            }

            return player;
        }
    }
}
