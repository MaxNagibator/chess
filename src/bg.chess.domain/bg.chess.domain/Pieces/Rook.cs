using System.Collections.Generic;

namespace bg.chess.domain
{
    /// <summary>
    /// Фигура "Ладья"
    /// </summary>
    public class Rook : Piece
    {
        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Rook(Side side) : base(side)
        {
        }

        /// </inheritdoc>
        public override List<FieldPosition> GetMoves(FieldPosition position)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "R";
        }
    }
}
