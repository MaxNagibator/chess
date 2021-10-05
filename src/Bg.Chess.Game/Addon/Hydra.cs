using System.Collections.Generic;

using Bg.Chess.Domain;

namespace Bg.Chess.Game.Addon
{
    /// <summary>
    /// Фигура "Гидра"
    /// </summary>
    public class Hydra : PieceType
    {
        /// </inheritdoc>
        public override bool IsPawnTransformAvailable => true;

        /// </inheritdoc>
        public override string Name => "hydra";

        /// </inheritdoc>
        public override char ShortName => 'h';

        /// </inheritdoc>
        /// 
        protected override List<Domain.Position> GetBaseMoves(Domain.Piece piece, MoveMode moveMode)
        {
            var availablePositions = new List<Domain.Position>();

            AddAvailableDiagonalMoves(piece, availablePositions, moveMode, 2);
            AddAvailableLineMoves(piece, availablePositions, moveMode, 3);

            return availablePositions;
        }
    }
}
