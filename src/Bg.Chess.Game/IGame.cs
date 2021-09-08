namespace Bg.Chess.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bg.Chess.Common.Enums;
    using Bg.Chess.Domain;

    public interface IGameInfo
    {
        string Id { get; }
        int WhitePlayerId { get; }
        int BlackPlayerId { get; }
        GameSide StepSide { get; }
        bool IsMyGame(int playerId);
        void Init(string id, int whitePlayerId, int blackPlayerId);

        GameStatus Status { get; }
        void Move(int playerId, int fromX, int fromY, int toX, int toY, string pawnTransformPiece = null);
        void ConfirmStart(int playerId);
        void StopStart(int playerId);

        string GetForsythEdwardsNotation(bool onlyPositions = false);

        List<AvailableMove> AvailableMoves();

        List<Move> GetMoves();
    }

    public class GameInfo : IGameInfo
    {
        public string Id { get; private set; }
        public int WhitePlayerId { get; private set; }
        public int BlackPlayerId { get; private set; }

        public bool whiteConfirm;
        public bool blackConfirm;

        private Game game;

        public void Init(string id, int whitePlayerId, int blackPlayerId)
        {
            Id = id;
            WhitePlayerId = whitePlayerId;
            BlackPlayerId = blackPlayerId;
        }

        public bool IsMyGame(int playerId)
        {
            return BlackPlayerId == playerId || WhitePlayerId == playerId;
        }

        /// <summary>
        /// Подтвердить начало игры.
        /// </summary>
        /// <param name="player">Кто подтверждает.</param>
        public void ConfirmStart(int playerId)
        {
            Side side = GetSide(playerId);
            if (side == Side.White)
            {
                // 99921700565 зачем вообще знать, кто там подтвердил игру, пусть за это поисковик отвечает
                // удалить confirmStart/stopStart
                whiteConfirm = true;
            }
            if (side == Side.Black)
            {
                blackConfirm = true;
            }

            if (whiteConfirm && blackConfirm)
            {
                game = new Game();
                game.Init();
            }
        }
        public void StopStart(int playerId)
        {
            Side side = GetSide(playerId);
            if (side == Side.White)
            {
                whiteConfirm = false;
            }
            if (side == Side.Black)
            {
                blackConfirm = false;
            }
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
                if (game == null)
                {
                    return GameStatus.WaitStart;
                }

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
    }
}
