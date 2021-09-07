namespace Bg.Chess.Game
{
    public class Position
    {
        /// <summary>
        /// По горизонтали.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// По вертикали.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Фигура.
        /// </summary>
        public string Piece { get; set; }
    }
}
