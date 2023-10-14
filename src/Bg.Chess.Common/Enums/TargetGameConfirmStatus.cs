namespace Bg.Chess.Common.Enums
{
    /// <summary>
    /// Статус вызова.
    /// </summary>
    public enum TargetGameConfirmStatus
    {
        /// <summary>
        /// Не существует.
        /// </summary>
        NotFound = 0,

        /// <summary>
        /// Ожидается подтверждение противника.
        /// </summary>
        NeedConfirmOpponent = 1,

        /// <summary>
        /// Ожидается подтверждение.
        /// </summary>
        NeedConfirm = 2,

        /// <summary>
        /// Окончен.
        /// </summary>
        Finish = 3,
    }
}
