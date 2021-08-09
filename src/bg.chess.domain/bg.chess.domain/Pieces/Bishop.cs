using System.Collections.Generic;

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

        /// </inheritdoc>
        public override List<FieldPosition> GetMoves(FieldPosition position)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "B";
        }
    }
}
