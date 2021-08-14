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
        //// todo между Position и Piece нужно чтото третие
        //// PieceType например, который будет описывать поведение фигуры.
        //// а Piece будет реальной фигурой на доске

        /// <summary>
        /// Может ли пешка превратится в эту фигуру, если достигнет конца поля.
        /// </summary>
        public abstract bool IsPawnTransformAvailable { get; }

        /// <summary>
        /// Имя фигуры.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Piece(Side side)
        {
            Side = side;
            Positions = new List<Position>();
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
        /// Фигура в результате игры не двигалась.
        /// </summary>
        internal bool IsInStartPosition => Positions.Count == 1;

        /// <summary>
        /// Позиции фигуры в результате игры.
        /// </summary>
        internal List<Position> Positions { get; set; }

        /// <summary>
        /// Получить базовый список доступных ходов.
        /// </summary>
        /// <param name="position">Позиция на поле.</param>
        /// <param name="moveMode">Режим обсчёта ходов.</param>
        /// <returns>Список возможных ходов.</returns>
        protected abstract List<Position> GetBaseMoves(Position position, MoveMode moveMode);

        /// <summary>
        /// Получить список доступных ходов.
        /// </summary>
        /// <param name="position">Позиция на поле.</param>
        /// <param name="moveMode">Режим обсчёта ходов.</param>
        /// <returns>Список возможных ходов.</returns>
        internal List<Position> GetAvailableMoves(Position position, MoveMode moveMode)
        {
            // todo если король под шахом, то нужно это будет учесть
            var moves = GetBaseMoves(position, moveMode);

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
        /// <param name="moveMode">Режим обсчёта ходов.</param>
        /// <param name="pieceMaxRange">Максимальная длина хода фигуры.</param>
        protected void AddAvailableDiagonalMoves(Position position, List<Position> availablePositions, MoveMode moveMode, int pieceMaxRange = int.MaxValue)
        {
            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X + i, position.Y + i))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X + i, position.Y - i))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X - i, position.Y + i))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X - i, position.Y - i))
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
        /// <param name="moveMode">Режим обсчёта ходов.</param>
        /// <param name="pieceMaxRange">Максимальная длина хода фигуры.</param>
        protected void AddAvailableLineMoves(Position position, List<Position> availablePositions, MoveMode moveMode, int pieceMaxRange = int.MaxValue)
        {
            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X + i, position.Y))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X - i, position.Y))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X, position.Y + i))
                {
                    break;
                }
            }

            for (var i = 1; i <= pieceMaxRange; i++)
            {
                if (!AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X, position.Y - i))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Добавить позицию в списох доступных, если она существует, а так же пустая или содержит вражескую(или свою по требованию) фигуру.
        /// </summary>
        /// <param name="field">Игровое поле.</param>
        /// <param name="availablePositions">Список доступных позиций.</param>
        /// <param name="moveMode">Режим обсчёта ходов.</param>
        /// <param name="x">Координаты проверки по ширине.</param>
        /// <param name="y">Координаты проверки по высоте</param>
        /// <returns>true - если клетка существует и на ней нет союзной(или своей по требованию) фигуры.</returns>
        protected bool AddPositionIfAvailable(Field field, List<Position> availablePositions, MoveMode moveMode, int x, int y)
        {
            var pos = field.GetPositionOrEmpty(x, y);
            if (pos == null)
            {
                return false;
            }

            if (moveMode == MoveMode.WithoutKillTeammates && pos.IsTeammate(Side))
            {
                return false;
            }

            availablePositions.Add(pos);
            if (moveMode == MoveMode.NotRules)
            {
                if (pos.Piece != null)
                {
                    return false;
                }

                return true;
            }

            if (pos.IsEnemy(Side))
            {
                return false;
            }

            return true;
        }
    }
}
