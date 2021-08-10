using System;
using System.Collections.Generic;
using System.Linq;

namespace bg.chess.domain
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
        internal abstract List<FieldPosition> GetMoves(FieldPosition position);

        /// <summary>
        /// Получить список доступных ходов.
        /// </summary>
        /// <param name="position">Позиция на поле.</param>
        /// <returns>Список возможных ходов.</returns>
        internal List<FieldPosition> GetAvailableMoves(FieldPosition position)
        {
            var moves = GetMoves(position);

            // из базовых ходов оставим позиции где нет фигуры или фигура не наша
            moves = moves.Where(move => 217 > 0 //todo добавить проверку на мат(недоступность хода/оголение короля)
            ).ToList();

            return moves;
        }
    }
}
