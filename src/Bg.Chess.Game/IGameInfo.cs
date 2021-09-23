namespace Bg.Chess.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bg.Chess.Common.Enums;

    public interface IGameInfo
    {
        string Id { get; }
        int WhitePlayerId { get; }
        int BlackPlayerId { get; }
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
        public int WhitePlayerId { get; private set; }
        public int BlackPlayerId { get; private set; }

        public bool whiteConfirm;
        public bool blackConfirm;

        private Domain.Game game;

        public GameInfo(string id, int whitePlayerId, int blackPlayerId)
        {
            Id = id;
            WhitePlayerId = whitePlayerId;
            BlackPlayerId = blackPlayerId;
            game = new Domain.Game();
            game.Init();

            // оставлю для отладки особых случаев
            if (false)
            {
                var rules = new Domain.ClassicRules();
                rules.FieldHeight = 8;
                rules.FieldWidth = 8;
                rules.Positions.Remove(rules.Positions.First(x => x.X == 4 && x.Y == 6));
                rules.Positions.Remove(rules.Positions.First(x => x.X == 7 && x.Y == 7));
                rules.Positions.Remove(rules.Positions.First(x => x.X == 0 && x.Y == 0));
                rules.Positions.Add(new Domain.Position(4, 6, Domain.PieceBuilder.Pawn(Domain.Side.White)));
                rules.Positions.Add(new Domain.Position(7, 7, Domain.PieceBuilder.King(Domain.Side.Black)));
                rules.Positions.Add(new Domain.Position(0, 0, Domain.PieceBuilder.King(Domain.Side.White)));
                rules.Positions.Add(new Domain.Position(4, 6, Domain.PieceBuilder.Pawn(Domain.Side.White)));
                rules.Positions.Add(new Domain.Position(7, 7, Domain.PieceBuilder.King(Domain.Side.Black)));
                var field = new Domain.Field(rules);
                game.Init(field);
            }
        }

        public bool IsMyGame(int playerId)
        {
            return BlackPlayerId == playerId || WhitePlayerId == playerId;
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
            if (WhitePlayerId == playerId)
            {
                side = Domain.Side.White;
            }
            else if (BlackPlayerId == playerId)
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

        private string FillDtoPiece(Domain.Piece piece)
        {
            if (piece == null)
            {
                return null;
            }

            var pieceName = piece.Type.ShortName;
            if (piece.Side == Domain.Side.White)
            {
                return pieceName.ToString().ToUpper();
            }
            else
            {
                return pieceName.ToString();
            }
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
