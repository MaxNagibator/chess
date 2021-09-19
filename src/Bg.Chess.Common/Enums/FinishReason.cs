namespace Bg.Chess.Common.Enums
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
        /// Ничья по ситуации на поле.
        /// </summary>
        Draw = 3,

        /// <summary>
        /// Ничья по договорённости.
        /// </summary>
        DrawByAgreement = 4,

        /// <summary>
        /// Ничья по времени.
        /// </summary>
        DrawByTime = 5,
    }
}
