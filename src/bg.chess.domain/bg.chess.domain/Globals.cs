namespace bg.chess.domain
{
    using System;

    /// <summary>
    /// Хранитель синглтончиков.
    /// </summary>
    public static class Globals
    {
        /// <summary>
        /// Строитель поля
        /// </summary>
        public static FieldBuilder FieldBuilder { get; } = new FieldBuilder();
    }
}
