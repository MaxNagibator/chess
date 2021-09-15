namespace Bg.Chess.Game
{
    /// <summary>
    /// Пользователь.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Почта подтверждена.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
    }
}
