using System.Collections.Generic;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Фигура "Ладья"
    /// </summary>
    public class Rook : Piece
    {
        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Rook(Side side) : base(side)
        {
        }

        /// </inheritdoc>
        internal override List<FieldPosition> GetMoves(FieldPosition position)
        {
            var width = position.Field.FieldWidth;
            var height = position.Field.FieldHeight;

            var availablePositions = new List<FieldPosition>();

            for (var i = position.X + 1; i < width; i++)
            {
                var pos = position.Field.GetPositionOrEmpty(i, position.Y);
                if (pos == null || pos.IsTeammate(Side))
                {
                    break;
                }

                availablePositions.Add(pos);
                if (pos.IsEnemy(Side))
                {
                    break;
                }
            }
            for (var i = position.X - 1; i >= 0; i--)
            {
                var pos = position.Field.GetPositionOrEmpty(i, position.Y);
                if (pos == null || pos.IsTeammate(Side))
                {
                    break;
                }

                availablePositions.Add(pos);
                if (pos.IsEnemy(Side))
                {
                    break;
                }
            }
            for (var i = position.Y + 1; i < height; i++)
            {
                var pos = position.Field.GetPositionOrEmpty(position.X, i);
                if (pos == null || pos.IsTeammate(Side))
                {
                    break;
                }

                availablePositions.Add(pos);
                if (pos.IsEnemy(Side))
                {
                    break;
                }
            }
            for (var i = position.Y - 1; i >= 0; i--)
            {
                var pos = position.Field.GetPositionOrEmpty(position.X, i);
                if (pos == null || pos.IsTeammate(Side))
                {
                    break;
                }

                availablePositions.Add(pos);
                if (pos.IsEnemy(Side))
                {
                    break;
                }
            }

            return availablePositions;
        }

        public override string ToString()
        {
            return "R";
        }
    }
}
