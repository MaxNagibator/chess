namespace Bg.Chess.Game
{
    using System;
    using System.Collections.Generic;
    using Bg.Chess.Domain;
    using DomainGame = Bg.Chess.Domain.Game;

    public interface IGameInfo
    {
        string Id { get; }
        int WhitePlayerId { get; }
        int BlackPlayerId { get; }
        bool IsMyGame(int playerId);
        void Init(string id, int whitePlayerId, int blackPlayerId);

        //todo можно просто геттер сделать
        GameState State { get; }
        void Move(int playerId, int fromX, int fromY, int toX, int toY, string pawnTransformPiece = null);
        void ConfirmStart(int playerId);
        void StopStart(int playerId);
        string GetForsythEdwardsNotation();

        //todo AvailableMove это класс из другой сборки, переложить
        List<AvailableMove> AvailableMove();
    }

    public class GameInfo : IGameInfo
    {
        public string Id { get; private set; }
        public int WhitePlayerId { get; private set; }
        public int BlackPlayerId { get; private set; }

        public bool whiteConfirm;
        public bool blackConfirm;

        private DomainGame game;

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
                // todo зачем вообще знать, кто там подтвердил игру, пусть за это поисковик отвечает
                // удалить confirmStart/stopStart
                whiteConfirm = true;
            }
            if (side == Side.Black)
            {
                blackConfirm = true;
            }

            if (whiteConfirm && blackConfirm)
            {
                game = new DomainGame();
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
        /// <returns>Растановка фигур на доске.</returns>
        public string GetForsythEdwardsNotation()
        {
            return game.GetForsythEdwardsNotation();
        }

        public List<AvailableMove> AvailableMove()
        {
            return game.AvailableMove();
        }

        public GameState State
        {
            get
            {
                if (game == null)
                {
                    return GameState.WaitStart;
                }

                switch (game.State)
                {
                    case Domain.GameState.InProgress:
                        return GameState.InProgress;
                    case Domain.GameState.WinWhite:
                        return GameState.WinWhite;
                    case Domain.GameState.WinBlack:
                        return GameState.WinBlack;
                    case Domain.GameState.Draw:
                        return GameState.Draw;
                    default:
                        throw new Exception("state unrecognized " + game.State);
                }
            }
        }
    }

    /// <summary>
    /// Игровые стороны.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// Ждёт начала.
        /// </summary>
        WaitStart = 0,

        /// <summary>
        /// В процессе.
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Белые победили.
        /// </summary>
        WinWhite = 2,

        /// <summary>
        /// Чёрные победили.
        /// </summary>
        WinBlack = 3,

        /// <summary>
        /// Ничья (куча матчасти и приёдсят обрабатывать каждый случай позже)
        /// </summary>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Draw_(chess)
        /// https://ru.wikipedia.org/wiki/%D0%9D%D0%B8%D1%87%D1%8C%D1%8F_(%D1%88%D0%B0%D1%85%D0%BC%D0%B0%D1%82%D1%8B)
        /// </remarks>
        Draw = 4,
    }
}
