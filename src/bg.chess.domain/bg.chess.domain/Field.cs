namespace bg.chess.domain
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Игровое поле.
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Кто ходит первый.
        /// </summary>
        public Side FirstStep => Side.White;

        /// <summary>
        /// Позиции на поле.
        /// </summary>
        public List<Position> Positions { get; set; }

        /// <summary>
        /// Взять позицию по координатам.
        /// </summary>
        /// <param name="x">По ширине.</param>
        /// <param name="y">По высоте.</param>
        /// <returns>Позиция.</returns>
        public Position this[int x, int y]
        {
            get
            {
                return Positions.First(p => p.X == x && p.Y == y);
            }
        }
    }
}
