namespace bg.chess.domain
{
    /// <summary>
    /// Позиция.
    /// </summary>
    /// <remarks>
    /// Кусочек игрового поля.
    /// </remarks>
    public class FieldPosition : Position
    {
        /// <summary>
        /// Конструктор позиции.
        /// </summary>
        /// <param name="x">По горизонтали.</param>
        /// <param name="y">По вертикали.</param>
        public FieldPosition(Field field, int x, int y) : base(x, y)
        {
            Field = field;
        }

        /// <summary>
        /// Конструктор позиции.
        /// </summary>
        /// <param name="x">По горизонтали.</param>
        /// <param name="y">По вертикали.</param>
        /// <param name="piece">Фигура.</param>
        public FieldPosition(Field field, int x, int y, Piece piece) : base(x, y, piece)
        {
            Field = field;
        }

        /// <summary>
        /// Поле, к которому пренадлежит этот кусочек.
        /// </summary>
        public Field Field { get; }
    }
}