namespace Bg.Chess.Game
{
    using System.Collections.Generic;
    using System.Linq;

    using Bg.Chess.Common.Enums;
    using Bg.Chess.Data.Repo;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    public interface IGameService
    {
        public void SaveGame(IGameInfo game);
        public HistoryGame GetGame(int gameId);
        public IEnumerable<HistoryGame> GetGames(int playerId);
    }

    public class GameService : IGameService
    {
        private IGameRepo _gameRepo;
        private IPlayerService _playerService;
        private ILogger _logger;

        public GameService(IPlayerService playerService, IGameRepo gameRepo, ILoggerFactory loggerFactory)
        {
            _gameRepo = gameRepo;
            _playerService = playerService;
            // todo сделать LogSource набор констант
            _logger = loggerFactory.CreateLogger("chess");
        }

        public void SaveGame(IGameInfo game)
        {
            var gameDto = new SaveGameDtoV1();
            FillDtoV1(gameDto, game);

            string data = JsonConvert.SerializeObject(gameDto);
            _gameRepo.SaveGame(0, game.WhitePlayerId, game.BlackPlayerId, game.FinishReason, game.WinSide, data);
        }

        public HistoryGame GetGame(int gameId)
        {
            var game = _gameRepo.GetGame(gameId);
            var gameDto = JsonConvert.DeserializeObject<SaveGameDtoV1>(game.Data);
            var gameInfo = new HistoryGame();
            gameInfo.Id = gameId;
            gameInfo.WhitePlayer = _playerService.GetPlayer(game.WhitePlayerId);
            gameInfo.BlackPlayer = _playerService.GetPlayer(game.BlackPlayerId);

            gameInfo.FinishReason = (FinishReason)game.FinishReason;
            gameInfo.WinSide = (GameSide)game.WinSide;

            FillGameFromDtoV1(gameInfo, gameDto);
            return gameInfo;
        }

        public IEnumerable<HistoryGame> GetGames(int playerId)
        {
            var games = _gameRepo.GetGames(playerId);

            return games.Select(x => new HistoryGame
            {
                Id = x.Id,
                BlackPlayer = new Player { Id = x.BlackPlayerId },
                WhitePlayer = new Player { Id = x.WhitePlayerId },
                FinishReason = (FinishReason)x.FinishReason,
                WinSide = (GameSide)x.WinSide,
            }).ToList();
        }

        public void FillDtoV1(SaveGameDtoV1 dto, IGameInfo game)
        {
            dto.Moves = game.GetMoves().Select(x =>
            {
                var dto = FillDtoMove(x);
                dto.AdditionalMove = FillDtoMove(x.AdditionalMove);
                if (x.KillEnemy != null)
                {
                    dto.KillEnemy =x.KillEnemy;

                }
                dto.Runner = x.Runner;
                return dto;
            }).ToList();

            var positionsStr = game.GetForsythEdwardsNotation(true);
            dto.Positions = positionsStr;
        }

        private SaveGameDtoV1.Move FillDtoMove(Move move)
        {
            if (move == null)
            {
                return null;
            }

            return new SaveGameDtoV1.Move()
            {
                From = move.From == null ? null : new SaveGameDtoV1.Position
                {
                    X = move.From.X,
                    Y = move.From.Y,
                },
                To = new SaveGameDtoV1.Position
                {
                    X = move.To.X,
                    Y = move.To.Y,
                },

            };
        }

        private void FillGameFromDtoV1(HistoryGame game, SaveGameDtoV1 dto)
        {
            game.Moves = dto.Moves.Select(x =>
            {
                var dto = FillMove(x);
                dto.AdditionalMove = FillMove(x.AdditionalMove);
                if (x.KillEnemy != null)
                {
                    dto.KillEnemy = x.KillEnemy;

                }
                dto.Runner = x.Runner;
                return dto;
            }).ToList();

            game.Positions = dto.Positions;
        }

        private Move FillMove(SaveGameDtoV1.Move move)
        {
            if (move == null)
            {
                return null;
            }

            return new Move()
            {
                From = move.From == null ? null : new Position
                {
                    X = move.From.X,
                    Y = move.From.Y,
                },
                To = new Position
                {
                    X = move.To.X,
                    Y = move.To.Y,
                },

            };
        }
    }
}
