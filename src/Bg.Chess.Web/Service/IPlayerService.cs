namespace Bg.Chess.Web.Service
{
    using Bg.Chess.Game;
    using Bg.Chess.Web.Data;
    using Bg.Chess.Web.Repo;

    public interface IPlayerService
    {
        public Player GetPlayerByUserId(string userId);
        public Player GetOrCreatePlayerByUserId(string userId, string name);
        public Player GetPlayer(int id);
    }

    public class PlayerService : IPlayerService
    {
        private IPlayerRepo _playerRepo;

        public PlayerService(IPlayerRepo playerRepo)
        {
            _playerRepo = playerRepo;
        }

        public Player GetPlayerByUserId(string userId)
        {
            var dbPlayer = _playerRepo.FindPlayerByUserId(userId);
            return FillPlayerDto(dbPlayer);
        }

        public Player GetPlayer(int id)
        {
            var dbPlayer = _playerRepo.GetPlayer(id);
            return FillPlayerDto(dbPlayer);
        }

        public Player GetOrCreatePlayerByUserId(string userId, string name)
        {
            var dbPlayer = _playerRepo.FindPlayerByUserId(userId);
            if (dbPlayer == null)
            {
                dbPlayer = _playerRepo.CreatePlayer(userId, name);
            }

            return FillPlayerDto(dbPlayer);
        }

        private static Player FillPlayerDto(ChessPlayer dbPlayer)
        {
            var playerDto = new Player();
            playerDto.Id = dbPlayer.Id;
            playerDto.UserId = dbPlayer.UserId;
            playerDto.Name = dbPlayer.Name;
            return playerDto;
        }
    }
}
