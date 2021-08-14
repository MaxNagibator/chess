using System.Collections.Generic;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Фигура "Ладья"
    /// </summary>
    public class Rook : Piece
    {
        /// </inheritdoc>
        public override bool IsPawnTransformAvailable => true;

        /// </inheritdoc>
        public override string Name => "rook";

        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Rook(Side side) : base(side)
        {
        }

        /// </inheritdoc>
        protected override List<Position> GetBaseMoves(Position position, MoveMode moveMode)
        {
            var availablePositions = new List<Position>();

            AddAvailableLineMoves(position, availablePositions, moveMode);

            return availablePositions;
        }

        public override string ToString()
        {
            return "R";
        }
    }
}
