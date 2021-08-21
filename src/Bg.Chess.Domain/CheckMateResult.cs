namespace Bg.Chess.Domain
{
    /// <summary>
    /// Результат проверки на мат.
    /// </summary>
    public enum CheckMateResult
    {
        /// <summary>
        /// Игра продолжается.
        /// </summary>
        None = 0,

        /// <summary>
        /// Мат. Победа проверяющей стороны.
        /// </summary>
        Mate = 1,

        /// <summary>
        /// Ничья. У проитивника нет ходов.
        /// </summary>
        DrawByEnemyDontHasMoves = 2,
    }
}
