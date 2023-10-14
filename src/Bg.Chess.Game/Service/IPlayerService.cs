namespace Bg.Chess.Game
{
    using Bg.Chess.Data;
    using Bg.Chess.Data.Repo;

    public interface IPlayerService
    {
        public Player FindPlayerByUserId(string userId);
        public Player FindPlayerByName(string name);
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

        public Player FindPlayerByUserId(string userId)
        {
            var dbPlayer = _playerRepo.FindPlayerByUserId(userId);
            if(dbPlayer == null)
            {
                return null;
            }
            return FillPlayerDto(dbPlayer);
        }

        public Player FindPlayerByName(string name)
        {
            var dbPlayer = _playerRepo.FindPlayerByName(name);
            if (dbPlayer == null)
            {
                return null;
            }
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

        private Player FillPlayerDto(ChessPlayer dbPlayer)
        {
            var playerDto = new Player();
            playerDto.Id = dbPlayer.Id;
            playerDto.UserId = dbPlayer.UserId;
            playerDto.Name = dbPlayer.Name;
            return playerDto;
        }
    }
}
