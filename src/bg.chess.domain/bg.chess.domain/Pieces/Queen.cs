﻿namespace bg.chess.domain
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

        public override string ToString()
        {
            return "Q";
        }
    }
}
