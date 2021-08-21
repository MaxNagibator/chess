namespace Bg.Chess.Domain
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Правила игры.
    /// </summary>
    public class Rules
    {
        /// <summary>
        /// Ширина поля.
        /// </summary>
        public int FieldWidth { get; set; }

        /// <summary>
        /// Высота поля.
        /// </summary>
        public int FieldHeight { get; set; }

        /// <summary>
        /// Расположение фигур
        /// </summary>
        public List<Position> Positions { get; set; }

        /// <summary>
        /// Методы получения фигуры для превращения пешки.
        /// </summary>
        public Dictionary<string, Func<Side, Piece>> PawnTransforms { get; set; }
    }
}
