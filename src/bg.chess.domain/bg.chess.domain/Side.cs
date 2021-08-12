namespace Bg.Chess.Domain
{
    /// <summary>
    /// Игровые стороны.
    /// </summary>
    public enum Side
    {
        /// <summary>
        /// Белые.
        /// </summary>
        White = 0,

        /// <summary>
        /// Чёрные.
        /// </summary>
        Black = 1,
    }

    public static class SideHelper
    {
        public static Side Invert(this Side side)
        {
           return side == Side.White ? Side.Black : Side.White;
        }
    }
}
