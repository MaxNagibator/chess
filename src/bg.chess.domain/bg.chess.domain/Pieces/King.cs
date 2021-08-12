using System.Collections.Generic;

namespace Bg.Chess.Domain
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

        /// </inheritdoc>
        protected override List<FieldPosition> GetBaseMoves(FieldPosition position)
        {
            var availablePositions = new List<FieldPosition>();

            AddAvailableDiagonalMoves(position, availablePositions, 1);
            AddAvailableLineMoves(position, availablePositions, 1);

            // todo добавить обсчёт возможности рокировки

            return availablePositions;
        }

        public override string ToString()
        {
            return "K";
        }
    }
}
