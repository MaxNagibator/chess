using System.Collections.Generic;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Фигура "Ферзь"
    /// </summary>
    public class Queen : Piece
    {
        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Queen(Side side) : base(side)
        {
        }

        /// </inheritdoc>
        protected override List<FieldPosition> GetBaseMoves(FieldPosition position, MoveMode moveMode)
        {
            var availablePositions = new List<FieldPosition>();

            AddAvailableDiagonalMoves(position, availablePositions, moveMode);
            AddAvailableLineMoves(position, availablePositions, moveMode);
            
            return availablePositions;
        }

        public override string ToString()
        {
            return "Q";
        }
    }
}
