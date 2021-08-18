namespace Bg.Chess.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Тип шахматной фигуры.
    /// </summary>
    /// <remarks>
    /// Поведение фигуры на поле.
    /// </remarks>
    public abstract class PieceType
    {
        /// <summary>
        /// Может ли пешка превратится в эту фигуру, если достигнет конца поля.
        /// </summary>
        public abstract bool IsPawnTransformAvailable { get; }

        /// <summary>
        /// Имя фигуры.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Имя фигуры из одного символа.
        /// </summary>
        public abstract char ShortName { get; }

        /// <summary>
        /// Получить базовый список доступных ходов.
        /// </summary>
        /// <param name="piece">Фигура.</param>
        /// <param name="moveMode">Режим обсчёта ходов.</param>
        /// <returns>Список возможных ходов.</returns>
        protected abstract List<Position> GetBaseMoves(Piece piece, MoveMode moveMode);

        /// <summary>
        /// Получить список доступных ходов.
        /// </summary>
        /// <param name="position">Позиция на поле.</param>
        /// <param name="moveMode">Режим обсчёта ходов.</param>
        /// <returns>Список возможных ходов.</returns>
        internal List<Position> GetAvailableMoves(Piece piece, MoveMode moveMode)
        {
            var myKing = piece.Field.GetPieces(piece.Side).FirstOrDefault(x => x.Type is King);
            if (myKing == null)
            {
                throw new Exception("my king death");
            }

            var moves = GetBaseMoves(piece, moveMode);
            List<Position> availableMoves;
            if (moveMode.HasFlag(MoveMode.IndifferentKingDeath))
            {
                availableMoves = moves;
            }
            else
            {
                availableMoves = new List<Position>();
                foreach (var move in moves)
                {
                    piece.CurrentPosition.Move(move, false);

                    // проверим, что после моего хода, враг имеет возможность убить нашего короля
                    if (!piece.CurrentPosition.Field.CheckKingAlert(piece.Side.Invert()))
                    {
                        availableMoves.Add(move);
                    }

                    piece.CurrentPosition.RevertLastMove();
                }
            }

            return availableMoves;
        }

        /// <summary>
        /// Получить возможные ходы по диагонали.
        /// </summary>
        /// <param name="piece">Фигура.</param>
        /// <param name="availablePositions">Список доступных позиций.</param>
        /// <param name="moveMode">Режим обсчёта ходов.</param>
        /// <param name="pieceMaxRange">Максимальная длина хода фигуры.</param>
        protected void AddAvailableDiagonalMoves(Piece piece, List<Position> availablePositions, MoveMode moveMode, int pieceMaxRange = int.MaxValue)
        {
            var position = piece.CurrentPosition;
            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(piece, availablePositions, moveMode, position.X + i, position.Y + i))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(piece, availablePositions, moveMode, position.X + i, position.Y - i))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(piece, availablePositions, moveMode, position.X - i, position.Y + i))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(piece, availablePositions, moveMode, position.X - i, position.Y - i))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Получить список ходов по горизонтали/вертикали
        /// </summary>
        /// <param name="piece">Фигура.</param>
        /// <param name="availablePositions">Список доступных позиций.</param>
        /// <param name="moveMode">Режим обсчёта ходов.</param>
        /// <param name="pieceMaxRange">Максимальная длина хода фигуры.</param>
        protected void AddAvailableLineMoves(Piece piece, List<Position> availablePositions, MoveMode moveMode, int pieceMaxRange = int.MaxValue)
        {
            var position = piece.CurrentPosition;
            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(piece, availablePositions, moveMode, position.X + i, position.Y))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(piece, availablePositions, moveMode, position.X - i, position.Y))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(piece, availablePositions, moveMode, position.X, position.Y + i))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(piece, availablePositions, moveMode, position.X, position.Y - i))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Добавить позицию в списох доступных, если она существует, а так же пустая или содержит вражескую(или свою по требованию) фигуру.
        /// </summary>
        /// <param name="piece">Фигура.</param>
        /// <param name="availablePositions">Список доступных позиций.</param>
        /// <param name="moveMode">Режим обсчёта ходов.</param>
        /// <param name="x">Координаты проверки по ширине.</param>
        /// <param name="y">Координаты проверки по высоте</param>
        /// <returns>true - если клетка существует и на ней нет союзной(или своей по требованию) фигуры.</returns>
        protected bool AddPositionIfAvailable(Piece piece, List<Position> availablePositions, MoveMode moveMode, int x, int y)
        {
            var pos = piece.Field.GetPositionOrEmpty(x, y);
            if (pos == null)
            {
                return false;
            }

            if (moveMode.HasFlag(MoveMode.WithoutKillTeammates) && pos.IsTeammate(piece.Side))
            {
                return false;
            }

            availablePositions.Add(pos);
            if (moveMode.HasFlag(MoveMode.NotRules))
            {
                if (pos.Piece != null)
                {
                    return false;
                }

                return true;
            }

            if (pos.IsEnemy(piece.Side))
            {
                return false;
            }

            return true;
        }
    }
}
