using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Фигура "Конь"
    /// </summary>
    public class Knight : Piece
    {
        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Knight(Side side) : base(side)
        {
        }

        /// </inheritdoc>
        protected override List<FieldPosition> GetBaseMoves(FieldPosition position, MoveMode moveMode)
        {
            var availablePositions = new List<FieldPosition>();

            AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X - 1, position.Y + 2);
            AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X + 1, position.Y + 2);
            AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X + 2, position.Y - 1);
            AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X + 2, position.Y + 1);
            AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X - 1, position.Y - 2);
            AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X + 1, position.Y - 2);
            AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X - 2, position.Y - 1);
            AddPositionIfAvailable(position.Field, availablePositions, moveMode, position.X - 2, position.Y + 1);

            return availablePositions;
        }

        public override string ToString()
        {
            return "N";
        }
    }
}
