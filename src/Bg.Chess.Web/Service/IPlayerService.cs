namespace Bg.Chess.Web.Service
{
    using Bg.Chess.Web.Data;
    using Bg.Chess.Web.Repo;

    public interface IPlayerService
    {
        public Player GetPlayer(string userId);
        public Player GetOrCreatePlayer(string userId, string name);
    }

    public class PlayerService : IPlayerService
    {
        private IPlayerRepo _playerRepo;

        public PlayerService(IPlayerRepo playerRepo)
        {
            _playerRepo = playerRepo;
        }

        public Player GetPlayer(string userId)
        {
            var dbPlayer = _playerRepo.GetPlayer(userId);
            return FillPlayerDto(dbPlayer);
        }

        public Player GetOrCreatePlayer(string userId, string name)
        {
            var dbPlayer = _playerRepo.GetPlayer(userId);
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
