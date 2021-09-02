namespace Bg.Chess.Web.Service
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bg.Chess.Common.Enums;
    using Bg.Chess.Game;
    using Bg.Chess.Web.Repo;

    public interface IGameService
    {
        public void SaveGame(IGameInfo game);
        public IGameInfo GetGame(int gameId);
        public IEnumerable<IHistoryGame> GetGames(int playerId);
    }

    //todo сервисы перевезти в сборку Game
    //todo репозитории в сборку Data
    public class GameService : IGameService
    {
        private IGameRepo _gameRepo;

        public GameService(IGameRepo gameRepo)
        {
            _gameRepo = gameRepo;
        }

        public void SaveGame(IGameInfo game)
        {
            var gameDto = new SaveGameDtoV1();
            FillDtoV1(gameDto, game);

            string data = JsonConvert.SerializeObject(gameDto);
            _gameRepo.SaveGame(0, game.WhitePlayerId, game.BlackPlayerId, (int)game.Status, data);
        }

        public IGameInfo GetGame(int gameId)
        {
            var game = _gameRepo.GetGame(gameId);
            var gameDto = JsonConvert.DeserializeObject<SaveGameDtoV1>(game.Data);
            var gameInfo = new GameInfo();
            FillGameFromDtoV1(gameInfo, gameDto);
            return gameInfo;
        }

        public IEnumerable<IHistoryGame> GetGames(int playerId)
        {
            var games = _gameRepo.GetGames(playerId);

            return games.Select(x => new HistoryGame
            {
                Id = x.Id,
                BlackPlayerId = x.BlackPlayerId,
                WhitePlayerId = x.WhitePlayerId,
                Status = (GameStatus)x.Status,
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
                    dto.KillEnemy = FillDtoPiece(x.KillEnemy);

                }
                dto.Runner = FillDtoPiece(x.Runner);
                return dto;
            }).ToList();
            dto.Positions = game.GetPositions().Select(x =>
            {
                var pos = new SaveGameDtoV1.Position();
                pos.X = x.X;
                pos.Y = x.Y;
                pos.Piece = FillDtoPiece(x.Piece);
                return pos;
            }).ToList();
        }

        private static SaveGameDtoV1.Piece FillDtoPiece(Domain.Piece piece)
        {
            if(piece == null)
            {
                return null;
            }

            string side;
            switch (piece.Side)
            {
                case Domain.Side.White:
                    side = "white";
                    break;
                case Domain.Side.Black:
                    side = "black";
                    break;
                default:
                    throw new ArgumentException("side not recognized");
            }

            var dtoPiece = new SaveGameDtoV1.Piece
            {
                Side = side,
                Type = piece.Type.Name
            };
            return dtoPiece;
        }

        private static SaveGameDtoV1.Move FillDtoMove(Domain.Move x)
        {
            if (x == null)
            {
                return null;
            }

            return new SaveGameDtoV1.Move()
            {
                From = new SaveGameDtoV1.Position
                {
                    X = x.From.X,
                    Y = x.From.Y,
                },
                To = new SaveGameDtoV1.Position
                {
                    X = x.To.X,
                    Y = x.To.Y,
                },

            };
        }

        public void FillGameFromDtoV1(IGameInfo game, SaveGameDtoV1 dto)
        {

        }
    }
}
