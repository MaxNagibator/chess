﻿using System.Collections.Generic;
using System.Linq;

namespace bg.chess.domain
{
    /// <summary>
    /// Фигура "Пешка"
    /// </summary>
    public class Pawn : Piece
    {
        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Pawn(Side side) : base(side)
        {
        }

        /// </inheritdoc>
        internal override List<FieldPosition> GetMoves(FieldPosition position)
        {
            var width = position.Field.FieldWidth;
            var height = position.Field.FieldHeight;

            var availablePositions = new List<FieldPosition>();
            availablePositions.Add(position.Field.GetPositionOrEmpty(position.X, position.Y + MoveMult));

            // если пешка ранее не ходила, то имеет право сходить на две клетки
            if (Positions.Count == 1)
            {
                availablePositions.Add(position.Field.GetPositionOrEmpty(position.X, position.Y + MoveMult + MoveMult));
            }

            CheckEnemyKill(position, 1, availablePositions);
            CheckEnemyKill(position, -1, availablePositions);

            // todo если пешка достигра конца поля, то может породить событие "выбор фигуры"

            return availablePositions.Where(x => x != null).ToList();
        }

        // проверка на возможность убить врага
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
