namespace Bg.Chess.Domain
{
    /// <summary>
    /// Причина окончания игры.
    /// </summary>
    public enum FinishReason
    {
        /// <summary>
        /// Победа матом.
        /// </summary>
        Mate = 0,

        /// <summary>
        /// Один из игроков сдался.
        /// </summary>
        Surrender = 1,

        /// <summary>
        /// Истекло время.
        /// </summary>
        TimeOver = 2,

        /// <summary>
        /// Ничья.
        /// </summary>
        Draw = 3,
    }
}
