namespace Bg.Chess.Domain
{
    using System.Collections.Generic;

    /// <summary>
    /// Фигура "Ферзь"
    /// </summary>
    public class Queen : PieceType
    {
        /// </inheritdoc>
        public override bool IsPawnTransformAvailable => true;

        /// </inheritdoc>
        public override string Name => "queen";

        /// </inheritdoc>
        public override char ShortName => 'q';

        /// </inheritdoc>
        protected override List<Position> GetBaseMoves(Piece piece, MoveMode moveMode)
        {
            var availablePositions = new List<Position>();

            AddAvailableDiagonalMoves(piece, availablePositions, moveMode);
            AddAvailableLineMoves(piece, availablePositions, moveMode);

            return availablePositions;
        }
    }
}
