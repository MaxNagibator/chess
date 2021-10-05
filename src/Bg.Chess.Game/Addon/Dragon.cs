using System;
using System.Collections.Generic;

using Bg.Chess.Domain;

namespace Bg.Chess.Game.Addon
{
    /// <summary>
    /// Фигура "Дракон"
    /// </summary>
    public class Dragon : PieceType
    {
        /// </inheritdoc>
        public override bool IsPawnTransformAvailable => true;

        /// </inheritdoc>
        public override string Name => "dragon";

        /// </inheritdoc>
        public override char ShortName => 'd';

        /// </inheritdoc>
        /// 
        protected override List<Domain.Position> GetBaseMoves(Domain.Piece piece, MoveMode moveMode)
        {
            var availablePositions = new List<Domain.Position>();

            for (var i = -2; i <= 2; i++)
            {
                for (var j = -2; j <= 2; j++)
                {
                    if (Math.Abs(i) + Math.Abs(j) <= 2)
                    {
                        AddPositionIfAvailable(piece, availablePositions, moveMode, piece.CurrentPosition.X + i, piece.CurrentPosition.Y + j);
                    }
                }
            }

            return availablePositions;
        }
    }
}
