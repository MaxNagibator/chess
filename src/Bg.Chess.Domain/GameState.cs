namespace Bg.Chess.Domain
{
    /// <summary>
    /// Статус игры.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// Ждёт начала.
        /// </summary>
        WaitStart = 0,

        /// <summary>
        /// В процессе.
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Игра оконцена.
        /// </summary>
        Finish = 2,
    }
}
