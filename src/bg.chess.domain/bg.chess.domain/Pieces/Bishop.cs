using System.Collections.Generic;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Фигура "Слон"
    /// </summary>
    public class Bishop : Piece
    {
        /// </inheritdoc>
        public override bool IsPawnTransformAvailable => true;

        /// </inheritdoc>
        public override string Name => "bishop";

        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Bishop(Side side) : base(side)
        {
        }

        /// </inheritdoc>
        protected override List<Position> GetBaseMoves(Position position, MoveMode moveMode)
        {
            var availablePositions = new List<Position>();

            AddAvailableDiagonalMoves(position, availablePositions, moveMode);

            return availablePositions;
        }

        public override string ToString()
        {
            return "B";
        }
    }
}
