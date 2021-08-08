namespace bg.chess.domain
{
    /// <summary>
    /// Фигура "Пешка"
    /// </summary>
    public class Pawn : Piece
    {
        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Pawn(Side side) : base(side)
        {
        }

        public override string ToString()
        {
            return "P";
        }
    }
}
