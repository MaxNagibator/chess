namespace Bg.Chess.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bg.Chess.Common.Enums;
    using Bg.Chess.Game.Addon;

    public interface IGameInfo
    {
        string Id { get; }
        Player WhitePlayer { get; }
        Player BlackPlayer { get; }
        GameSide StepSide { get; }
        bool IsMyGame(int playerId);

        bool IsFinish { get; }
        void Move(int playerId, int fromX, int fromY, int toX, int toY, string pawnTransformPiece = null);

        string GetForsythEdwardsNotation(bool onlyPositions = false);

        List<AvailableMove> AvailableMoves();

        List<Move> GetMoves();
        void Surrender(int playerId);
        FinishReason? FinishReason { get; }
        GameSide? WinSide { get; }
    }

    public class GameInfo : IGameInfo
    {
        public string Id { get; private set; }
        public Player WhitePlayer { get; private set; }
        public Player BlackPlayer { get; private set; }

        public bool whiteConfirm;
        public bool blackConfirm;

        private Domain.Game game;

        public GameInfo(string id, Player whitePlayer, Player blackPlayer)
        {
            Id = id;
            WhitePlayer = whitePlayer;
            BlackPlayer = blackPlayer;
            game = new Domain.Game();
            //game.Init();

            var rules = new DragonRules();
            var field = new Domain.Field(rules);
            game.Init(field);

            // оставлю для отладки особых случаев
            if (false)
            {
                var rules2 = new Domain.ClassicRules();
                rules2.Positions = new List<Domain.Position>
                {
                    new Domain.Position(4, 6, Domain.PieceBuilder.Pawn(Domain.Side.White)),
                    new Domain.Position(7, 7, Domain.PieceBuilder.King(Domain.Side.Black)),
                    new Domain.Position(0, 0, Domain.PieceBuilder.King(Domain.Side.White)),
                };
                var field2 = new Domain.Field(rules2);
                game.Init(field2);
            }
            else
            {
            }
        }

        public bool IsMyGame(int playerId)
        {
            return BlackPlayer.Id == playerId || WhitePlayer.Id == playerId;
        }

        /// <summary>
        /// Сделать ход.
        /// </summary>
        /// <param name="playerId">Кто ходит.</param>
        /// <param name="fromX">Откуда X.</param>
        /// <param name="fromY">Откуда Y.</param>
        /// <param name="toX">Куда X.</param>
        /// <param name="toY">Куда Y.</param>
        /// <param name="pawnTransformPiece">Название фигуры, для превращения пешки в другую фигуру в конце поля.</param>
        public void Move(int playerId, int fromX, int fromY, int toX, int toY, string pawnTransformPiece = null)
        {
            Domain.Side side = GetSide(playerId);

            game.Move(side, fromX, fromY, toX, toY, pawnTransformPiece);

        }

        private Domain.Side GetSide(int playerId)
        {
            Domain.Side side;
            if (WhitePlayer.Id == playerId)
            {
                side = Domain.Side.White;
            }
            else if (BlackPlayer.Id == playerId)
            {
                side = Domain.Side.Black;
            }
            else
            {
                throw new BusinessException("Это не ваша игра");
            }

            return side;
        }

        /// <summary>
        /// Нотация Форсайта — Эдвардса / Forsyth–Edwards Notation.
        /// </summary>
        /// <param name="onlyPositions">Только расстановка фигур.</param>
        /// <returns>Растановка фигур на доске.</returns>
        public string GetForsythEdwardsNotation(bool onlyPositions = false)
        {
            return game.GetForsythEdwardsNotation(onlyPositions);
        }

        public List<AvailableMove> AvailableMoves()
        {
            return game.AvailableMoves().Select(move =>
            {
                var dto = new AvailableMove();
                dto.From = new Position { X = move.From.X, Y = move.From.Y };
                dto.To = move.To.Select(to => new Position { X = to.X, Y = to.Y }).ToList();
                return dto;
            }).ToList();
        }

        public List<Move> GetMoves()
        {
            return game.GetMoves().Select(move =>
            {
                var dto = Init(move);
                return dto;
            }).ToList();
        }

        private Move Init(Domain.Move move)
        {
            if (move == null)
            {
                return null;
            }

            var dto = new Move
            {
                From = move.From == null ? null : new Position { X = move.From.X, Y = move.From.Y },
                To = new Position { X = move.To.X, Y = move.To.Y },

                AdditionalMove = Init(move.AdditionalMove),
                KillEnemy = FillDtoPiece(move.KillEnemy),
                Runner = FillDtoPiece(move.Runner),
            };

            return dto;
        }

        private Piece FillDtoPiece(Domain.Piece piece)
        {
            if (piece == null)
            {
                return null;
            }

            return new Piece
            {
                Side = piece.Side == Domain.Side.White ? GameSide.White : GameSide.Black,
                TypeName = piece.Type.Name,
                TypeShortName = piece.Type.ShortName.ToString(),
            };
        }

        public void Surrender(int playerId)
        {
            Domain.Side side = GetSide(playerId);
            game.Surrender(side);
        }

        public GameSide StepSide => game.StepSide == Domain.Side.White ? GameSide.White : GameSide.Black;

        public bool IsFinish => game.State == Domain.GameState.Finish;

        public GameSide? WinSide
        {
            get
            {
                if (game.WinSide == null)
                {
                    return null;
                }

                return game.WinSide == Domain.Side.White ? GameSide.White : GameSide.Black;
            }
        }

        public FinishReason? FinishReason
        {
            get
            {
                if (game.FinishReason == null)
                {
                    return null;
                }

                switch (game.FinishReason)
                {
                    case Domain.FinishReason.Draw:
                        return Bg.Chess.Common.Enums.FinishReason.Draw;
                    case Domain.FinishReason.Mate:
                        return Bg.Chess.Common.Enums.FinishReason.Mate;
                    case Domain.FinishReason.Surrender:
                        return Bg.Chess.Common.Enums.FinishReason.Surrender;
                    case Domain.FinishReason.TimeOver:
                        return Bg.Chess.Common.Enums.FinishReason.TimeOver;
                    default:
                        throw new Exception("finish reason unrecognized " + game.FinishReason);
                }
            }
        }
    }
}
