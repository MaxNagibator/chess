namespace bg.chess.domain
{
    /// <summary>
    /// Фигура "Король"
    /// </summary>
    public class King : Piece
    {
        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public King(Side side) : base(side)
        {
        }

        public override string ToString()
        {
            return "K";
        }
    }
}
