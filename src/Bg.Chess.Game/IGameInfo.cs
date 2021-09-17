namespace Bg.Chess.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bg.Chess.Common.Enums;
    using Bg.Chess.Domain;

    public interface IGameInfo
    {
        int WhitePlayerId { get; }
        int BlackPlayerId { get; }
        GameSide StepSide { get; }
        bool IsMyGame(int playerId);

        GameStatus Status { get; }
        bool IsFinish { get; }
        void Move(int playerId, int fromX, int fromY, int toX, int toY, string pawnTransformPiece = null);

        string GetForsythEdwardsNotation(bool onlyPositions = false);

        List<AvailableMove> AvailableMoves();

        List<Move> GetMoves();
    }

    public class GameInfo : IGameInfo
    {
        public int WhitePlayerId { get; private set; }
        public int BlackPlayerId { get; private set; }

        public bool whiteConfirm;
        public bool blackConfirm;

        private Game game;

        public GameInfo(int whitePlayerId, int blackPlayerId)
        {
            WhitePlayerId = whitePlayerId;
            BlackPlayerId = blackPlayerId;
            game = new Game();
            game.Init();

            // оставлю для отладки особых случаев
            if (false)
            {
                var rules = new ClassicRules();
                rules.FieldHeight = 8;
                rules.FieldWidth = 8;
                rules.Positions.Remove(rules.Positions.First(x => x.X == 4 && x.Y == 6));
                rules.Positions.Remove(rules.Positions.First(x => x.X == 7 && x.Y == 7));
                rules.Positions.Remove(rules.Positions.First(x => x.X == 0 && x.Y == 0));
                rules.Positions.Add(new Domain.Position(4, 6, PieceBuilder.Pawn(Side.White)));
                rules.Positions.Add(new Domain.Position(7, 7, PieceBuilder.King(Side.Black)));
                rules.Positions.Add(new Domain.Position(0, 0, PieceBuilder.King(Side.White)));
                rules.Positions.Add(new Domain.Position(4, 6, PieceBuilder.Pawn(Side.White)));
                rules.Positions.Add(new Domain.Position(7, 7, PieceBuilder.King(Side.Black)));
                var field = new Field(rules);
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
            Side side = GetSide(playerId);

            game.Move(side, fromX, fromY, toX, toY, pawnTransformPiece);

        }

        private Side GetSide(int playerId)
        {
            Side side;
            if (WhitePlayerId == playerId)
            {
                side = Side.White;
            }
            else if (BlackPlayerId == playerId)
            {
                side = Side.Black;
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
                From = new Position { X = move.From.X, Y = move.From.Y },
                To = new Position { X = move.To.X, Y = move.To.Y },

                AdditionalMove = Init(move.AdditionalMove),
                KillEnemy = FillDtoPiece(move.KillEnemy),
                Runner = FillDtoPiece(move.Runner),
            };

            return dto;
        }

        private string FillDtoPiece(Piece piece)
        {
            if (piece == null)
            {
                return null;
            }

            var pieceName = piece.Type.ShortName;
            if (piece.Side == Side.White)
            {
                return pieceName.ToString().ToUpper();
            }
            else
            {
                return pieceName.ToString();
            }
        }

        public GameStatus Status
        {
            get
            {
                switch (game.State)
                {
                    case Domain.GameState.InProgress:
                        return GameStatus.InProgress;
                    case Domain.GameState.WinWhite:
                        return GameStatus.WinWhite;
                    case Domain.GameState.WinBlack:
                        return GameStatus.WinBlack;
                    case Domain.GameState.Draw:
                        return GameStatus.Draw;
                    default:
                        throw new Exception("state unrecognized " + game.State);
                }
            }
        }

        public GameSide StepSide => game.StepSide == Domain.Side.White ? GameSide.White : GameSide.Black;

        public bool IsFinish => Status == GameStatus.WinBlack 
                || Status == GameStatus.WinWhite
                || Status == GameStatus.Draw;
    }
}
