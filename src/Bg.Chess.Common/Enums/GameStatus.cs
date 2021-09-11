namespace Bg.Chess.Common.Enums
{
    /// <summary>
    /// Статус игры.
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// В процессе.
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Белые победили.
        /// </summary>
        WinWhite = 2,

        /// <summary>
        /// Чёрные победили.
        /// </summary>
        WinBlack = 3,

        /// <summary>
        /// Ничья (куча матчасти и приёдсят обрабатывать каждый случай позже)
        /// </summary>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Draw_(chess)
        /// https://ru.wikipedia.org/wiki/%D0%9D%D0%B8%D1%87%D1%8C%D1%8F_(%D1%88%D0%B0%D1%85%D0%BC%D0%B0%D1%82%D1%8B)
        /// </remarks>
        Draw = 4,
    }
}
