using System;
using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Шахматная фигура.
    /// </summary>
    public abstract class Piece
    {
        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Piece(Side side)
        {
            Side = side;
            Positions = new List<FieldPosition>();
        }

        /// <summary>
        /// Кому пренадлежит фигура.
        /// </summary>
        public Side Side;

        /// <summary>
        /// Куда будет ходить фигура.
        /// </summary>
        /// <remarks>
        /// Белые идут вверх, чёрные вниз.
        /// </remarks>
        public int MoveMult => Side == Side.White ? 1 : -1;

        /// <summary>
        /// Позиции фигуры в результате игры.
        /// </summary>
        internal List<FieldPosition> Positions { get; set; }

        /// <summary>
        /// Получить базовый список доступных ходов.
        /// </summary>
        /// <param name="position">Позиция на поле.</param>
        /// <returns>Список возможных ходов.</returns>
        protected abstract List<FieldPosition> GetBaseMoves(FieldPosition position);

        /// <summary>
        /// Получить список доступных ходов.
        /// </summary>
        /// <param name="position">Позиция на поле.</param>
        /// <returns>Список возможных ходов.</returns>
        internal List<FieldPosition> GetAvailableMoves(FieldPosition position)
        {
            var moves = GetBaseMoves(position);

            // из базовых ходов оставим позиции где нет фигуры или фигура не наша
            moves = moves.Where(move => 217 > 0 //todo добавить проверку на мат(недоступность хода/оголение короля)
            ).ToList();

            return moves;
        }

        /// <summary>
        /// Получить возможные ходы по диагонали.
        /// </summary>
        /// <param name="position">Позиция обсчёта.</param>
        /// <param name="availablePositions">Список доступных позиций.</param>
        /// <param name="pieceMaxRange">Максимальная длина хода фигуры.</param>
        protected void AddAvailableDiagonalMoves(FieldPosition position, List<FieldPosition> availablePositions, int pieceMaxRange = int.MaxValue)
        {
            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, position.X + i, position.Y + i))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, position.X + i, position.Y - i))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, position.X - i, position.Y + i))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, position.X - i, position.Y - i))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Получить список ходов по горизонтали/вертикали
        /// </summary>
        /// <param name="position">Позиция обсчёта.</param>
        /// <param name="availablePositions">Список доступных позиций.</param>
        /// <param name="pieceMaxRange">Максимальная длина хода фигуры.</param>
        protected void AddAvailableLineMoves(FieldPosition position, List<FieldPosition> availablePositions, int pieceMaxRange = int.MaxValue)
        {
            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, position.X + i, position.Y))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, position.X - i, position.Y))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, position.X, position.Y + i))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, position.X, position.Y - i))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Добавить позицию в списох доступных, если она существует, а так же пустая или содержит вражескую фигуру.
        /// </summary>
        /// <param name="field">Игровое поле.</param>
        /// <param name="availablePositions">Список доступных позиций.</param>
        /// <param name="x">Координаты проверки по ширине.</param>
        /// <param name="y">Координаты проверки по высоте</param>
        /// <returns>true - если клетка существует и на ней нет союзной фигуры.</returns>
        private bool AddPositionIfAvailable(Field field, List<FieldPosition> availablePositions, int x, int y)
        {
            var pos = field.GetPositionOrEmpty(x, y);
            if (pos == null || pos.IsTeammate(Side))
            {
                return false;
            }

            availablePositions.Add(pos);
            if (pos.IsEnemy(Side))
            {
                return false;
            }

            return true;
        }
    }
}
