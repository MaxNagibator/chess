namespace Bg.Chess.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Доступные ходы из указанной позиции.
    /// </summary>
    public class AvailableMove
    {
        /// <summary>
        /// Откуда ходим.
        /// </summary>
        public Position From { get; }

        /// <summary>
        /// Куда ходим.
        /// </summary>
        public List<Position> To { get; }

        /// <summary>
        /// Доступные ходы из указанной позиции.
        /// </summary>
        /// <param name="from">Откуда ходим.</param>
        /// <param name="to">Куда ходим.</param>
        public AvailableMove(Position from, List<Position> to)
        {
            From = from;
            To = to;
        }

        /// <summary>
        /// Координаты поля.
        /// </summary>
        public class Position
        {
            /// <summary>
            /// По ширине.
            /// </summary>
            public int X { get; }

            /// <summary>
            /// По высоте.
            /// </summary>
            public int Y { get; }

            /// <summary>
            /// Координаты поля.
            /// </summary>
            /// <param name="x">По ширине.</param>
            /// <param name="y">По высоте.</param>
            public Position(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}