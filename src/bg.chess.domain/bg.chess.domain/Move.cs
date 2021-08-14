using System;
using System.Collections.Generic;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Ход.
    /// </summary>
    public class Move
    {
        /// <summary>
        /// Откуда.
        /// </summary>
        public Position From { get; set; }

        /// <summary>
        /// Куда.
        /// </summary>
        public Position To { get; set; }

        /// <summary>
        /// Конструтор хода
        /// </summary>
        /// <param name="from">Откуда.</param>
        /// <param name="to">Куда.</param>
        public Move(Position from, Position to)
        {
            From = from;
            To = to;
        }
    }
}