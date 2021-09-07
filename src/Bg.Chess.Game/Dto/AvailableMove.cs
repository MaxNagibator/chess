namespace Bg.Chess.Game
{
    using System.Collections.Generic;

    /// <summary>
    /// Доступные ходы из указанной позиции.
    /// </summary>
    public class AvailableMove
    {
        /// <summary>
        /// Откуда ходим.
        /// </summary>
        public Position From { get; set; }

        /// <summary>
        /// Куда ходим.
        /// </summary>
        public List<Position> To { get; set; }
    }
}