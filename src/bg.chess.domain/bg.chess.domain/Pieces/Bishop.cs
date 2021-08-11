﻿using System.Collections.Generic;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Фигура "Слон"
    /// </summary>
    public class Bishop : Piece
    {
        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Bishop(Side side) : base(side)
        {
        }

        /// </inheritdoc>
        internal override List<FieldPosition> GetMoves(FieldPosition position)
        {
            var availablePositions = new List<FieldPosition>();

            AddAvailableDiagonalMoves(position, availablePositions);

            return availablePositions;
        }

        public override string ToString()
        {
            return "B";
        }
    }
}
