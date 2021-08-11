﻿using System.Collections.Generic;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Фигура "Ферзь"
    /// </summary>
    public class Queen : Piece
    {
        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Queen(Side side) : base(side)
        {
        }

        /// </inheritdoc>
        internal override List<FieldPosition> GetMoves(FieldPosition position)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "Q";
        }
    }
}
