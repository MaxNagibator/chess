namespace bg.chess.domain
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

        public override string ToString()
        {
            return "B";
        }
    }
}
