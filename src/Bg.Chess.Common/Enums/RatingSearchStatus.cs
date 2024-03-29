﻿namespace Bg.Chess.Common.Enums
{
    /// <summary>
    /// Статус поиска рейтинговой игры.
    /// </summary>
    public enum RatingSearchStatus
    {
        /// <summary>
        /// Не существует.
        /// </summary>
        NotFound = 0,

        /// <summary>
        /// В процессе.
        /// </summary>
        InProcess = 1,

        /// <summary>
        /// Ожидается подтверждение.
        /// </summary>
        NeedConfirm = 2,

        /// <summary>
        /// Ожидается подтверждение противника.
        /// </summary>
        NeedConfirmOpponent = 3,

        /// <summary>
        /// Окончен.
        /// </summary>
        Finish = 4,
    }
}
