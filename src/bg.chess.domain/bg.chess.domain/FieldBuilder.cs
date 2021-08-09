namespace bg.chess.domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Строитель поля.
    /// </summary>
    public class FieldBuilder
    {
        /// <summary>
        /// Получить поле.
        /// </summary>
        /// <param name="rules">Правила игры.</param>
        /// <remarks>Фигуры из правил игры не клонируются, будь бдителен.</remarks>
        /// <returns>Поле.</returns>
        public Field GetField(Rules rules)
        {
            var duplicatePos = rules.positions.GroupBy(p => new { p.X, p.Y }).FirstOrDefault(pg => pg.Count() > 1);
            if (duplicatePos != null)
            {
                throw new Exception("dublicate position " + duplicatePos.Key.X + "/" + duplicatePos.Key.Y);
            }

            var field = new Field();
            field.Positions = new List<Position>();
            for (var x = 0; x < rules.FieldWidth; x++)
            {
                for (var y = 0; y < rules.FieldHeight; y++)
                {
                    var positionFromRules = rules.positions.FirstOrDefault(rulPos => rulPos.X == x && rulPos.Y == y);

                    field.Positions.Add(new Position(x, y, positionFromRules?.Piece));
                }
            }

            field.FieldWidth = rules.FieldWidth;
            field.FieldHeight = rules.FieldHeight;

            return field;
        }
    }
}
