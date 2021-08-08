namespace bg.chess.domain
{
    /// <summary>
    /// Фигура "Конь"
    /// </summary>
    public class Knight : Piece
    {
        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Knight(Side side) : base(side)
        {
        }

        public override string ToString()
        {
            return "N";
        }
    }
}
