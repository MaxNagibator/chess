namespace bg.chess.domain
{
    /// <summary>
    /// Позиция.
    /// </summary>
    /// <remarks>
    /// Кусочек игрового поля.
    /// </remarks>
    public class Position
    {
        /// <summary>
        /// Конструктор позиции.
        /// </summary>
        /// <param name="x">По горизонтали.</param>
        /// <param name="y">По вертикали.</param>
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Конструктор позиции.
        /// </summary>
        /// <param name="x">По горизонтали.</param>
        /// <param name="y">По вертикали.</param>
        /// <param name="piece">Фигура.</param>
        public Position(int x, int y, Piece piece) : this(x, y)
        {
            Piece = piece;
        }

        /// <summary>
        /// По горизонтали.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// По вертикали.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Шахматная фигура.
        /// </summary>
        public Piece Piece { get; }

        public override string ToString()
        {
            return X + "/" + Y + (Piece != null ? "/" + Piece.ToString() : null);
        }
    }
}