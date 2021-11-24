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
        /// Описание для системы координат по горизонтали.
        /// </summary>
        /// <remarks>
        /// Длина строки должна ровнятся <see cref="FieldWidth"/>
        /// Например "abcdefghij"
        /// </remarks>
        public string WidthSymbols { get; set; }

        /// <summary>
        /// Описание для системы координат по вертикали.
        /// </summary>
        /// <remarks>
        /// Длина строки должна ровнятся <see cref="FieldHeight"/>
        /// Например "12345678"
        /// </remarks>
        public string HeightSymbols { get; set; }

        /// <summary>
        /// Методы получения фигуры для превращения пешки.
        /// </summary>
        public Dictionary<string, Func<Side, Piece>> PawnTransforms { get; set; }
    }
}
