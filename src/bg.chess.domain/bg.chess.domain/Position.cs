namespace Bg.Chess.Domain
{
    /// <summary>
    /// Позиция.
    /// </summary>
    /// <remarks>
    /// Координаты на поле и фигура.
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

        /// <summary>
        /// Позиция пустая или там вражеская фигура.
        /// </summary>
        /// <param name="side">Сторона фигуры вызывающей проверку.</param>
        /// <returns>true - если позиция без фигуры или вражеская фигура присутствует.</returns>
        public bool IsEmptyOrEnemy(Side side)
        {
            return Piece == null || Piece.Side != side;
        }

        /// <summary>
        /// Позиция с вражеской фигурой.
        /// </summary>
        /// <param name="side">Сторона фигуры вызывающей проверку.</param>
        /// <returns>true - если вражеская фигура присутствует.</returns>
        public bool IsEnemy(Side side)
        {
            return Piece != null && Piece.Side != side;
        }

        /// <summary>
        /// Позиция с дружественной фигурой.
        /// </summary>
        /// <param name="side">Сторона фигуры вызывающей проверку.</param>
        /// <returns>true - если дружественная фигура присутствует.</returns>
        public bool IsTeammate(Side side)
        {
            return Piece != null && Piece.Side == side;
        }

        public override string ToString()
        {
            return X + "/" + Y + (Piece != null ? "/" + Piece.ToString() : null);
        }
    }
}