namespace Bg.Chess.Domain
{
    using System.Collections.Generic;

    /// <summary>
    /// Фигура "Конь"
    /// </summary>
    public class Knight : PieceType
    {
        /// </inheritdoc>
        public override bool IsPawnTransformAvailable => true;

        /// </inheritdoc>
        public override string Name => "knight";

        /// </inheritdoc>
        public override char ShortName => 'n';

        /// </inheritdoc>
        protected override List<Position> GetBaseMoves(Piece piece, MoveMode moveMode)
        {
            var availablePositions = new List<Position>();

            var pos = piece.CurrentPosition;
            AddPositionIfAvailable(piece, availablePositions, moveMode, pos.X - 1, pos.Y + 2);
            AddPositionIfAvailable(piece, availablePositions, moveMode, pos.X + 1, pos.Y + 2);
            AddPositionIfAvailable(piece, availablePositions, moveMode, pos.X + 2, pos.Y - 1);
            AddPositionIfAvailable(piece, availablePositions, moveMode, pos.X + 2, pos.Y + 1);
            AddPositionIfAvailable(piece, availablePositions, moveMode, pos.X - 1, pos.Y - 2);
            AddPositionIfAvailable(piece, availablePositions, moveMode, pos.X + 1, pos.Y - 2);
            AddPositionIfAvailable(piece, availablePositions, moveMode, pos.X - 2, pos.Y - 1);
            AddPositionIfAvailable(piece, availablePositions, moveMode, pos.X - 2, pos.Y + 1);

            return availablePositions;
        }
    }
}
