using System.Collections.Generic;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Фигура "Слон"
    /// </summary>
    public class Bishop : PieceType
    {
        /// </inheritdoc>
        public override bool IsPawnTransformAvailable => true;

        /// </inheritdoc>
        public override string Name => "bishop";

        /// </inheritdoc>
        public override char ShortName => 'b';

        /// </inheritdoc>
        protected override List<Position> GetBaseMoves(Piece piece, MoveMode moveMode)
        {
            var availablePositions = new List<Position>();

            AddAvailableDiagonalMoves(piece, availablePositions, moveMode);

            return availablePositions;
        }
    }
}
