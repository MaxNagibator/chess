using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Фигура "Пешка"
    /// </summary>
    public class Pawn : Piece
    {
        /// </inheritdoc>
        public override bool IsPawnTransformAvailable => false;

        /// </inheritdoc>
        public override string Name => "pawn";

        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Pawn(Side side) : base(side)
        {
        }

        /// </inheritdoc>
        protected override List<FieldPosition> GetBaseMoves(FieldPosition position, MoveMode moveMode)
        {
            var width = position.Field.FieldWidth;
            var height = position.Field.PawnTransforms;

            var availablePositions = new List<FieldPosition>();
            availablePositions.Add(position.Field.GetPositionOrEmpty(position.X, position.Y + MoveMult));

            // если пешка ранее не ходила, то имеет право сходить на две клетки
            if (IsInStartPosition)
            {
                availablePositions.Add(position.Field.GetPositionOrEmpty(position.X, position.Y + MoveMult + MoveMult));
            }

            CheckEnemyKill(position, 1, availablePositions);
            CheckEnemyKill(position, -1, availablePositions);

            //todo если пешка прекрывает фигуру, которая угрожает королю, то королю пофиг и он срубит!
            // учеть тут MOVEMODE! написать тестик

            // todo взятие на проходе https://ru.wikipedia.org/wiki/%D0%92%D0%B7%D1%8F%D1%82%D0%B8%D0%B5_%D0%BD%D0%B0_%D0%BF%D1%80%D0%BE%D1%85%D0%BE%D0%B4%D0%B5

            return availablePositions.Where(x => x != null).ToList();
        }

        /// <summary>
        /// Проверка на возможность убить врага
        /// </summary>
        private void CheckEnemyKill(FieldPosition position, int shiftX, List<FieldPosition> availablePositions)
        {
            var pos1 = position.Field.GetPositionOrEmpty(position.X + shiftX, position.Y + MoveMult);
            if (pos1?.IsEnemy(Side) == true)
            {
                availablePositions.Add(pos1);
            }
        }

        public override string ToString()
        {
            return "P";
        }
    }
}
