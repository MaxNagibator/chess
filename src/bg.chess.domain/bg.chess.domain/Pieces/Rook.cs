using System.Collections.Generic;

namespace Bg.Chess.Domain
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
        internal override List<FieldPosition> GetMoves(FieldPosition position)
        {
            var availablePositions = new List<FieldPosition>();

            AddAvailableLineMoves(position, availablePositions);

            return availablePositions;
        }

        public override string ToString()
        {
            return "R";
        }
    }
}
