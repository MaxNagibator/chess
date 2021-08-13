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
            var width = position.Field.FieldWidth;
            var height = position.Field.FieldHeight;

            var availablePositions = new List<FieldPosition>();

            availablePositions.Add(position.Field.GetPositionOrEmpty(position.X - 1, position.Y + 2));
            availablePositions.Add(position.Field.GetPositionOrEmpty(position.X + 1, position.Y + 2));

            availablePositions.Add(position.Field.GetPositionOrEmpty(position.X + 2, position.Y - 1));
            availablePositions.Add(position.Field.GetPositionOrEmpty(position.X + 2, position.Y + 1));

            availablePositions.Add(position.Field.GetPositionOrEmpty(position.X - 1, position.Y - 2));
            availablePositions.Add(position.Field.GetPositionOrEmpty(position.X + 1, position.Y - 2));

            availablePositions.Add(position.Field.GetPositionOrEmpty(position.X - 2, position.Y - 1));
            availablePositions.Add(position.Field.GetPositionOrEmpty(position.X - 2, position.Y + 1));

            return availablePositions.Where(x => x != null && position.Field[x.X, x.Y].IsEmptyOrEnemy(Side)).ToList();
        }

        public override string ToString()
        {
            return "N";
        }
    }
}
