namespace Bg.Chess.Domain
{
    /// <summary>
    /// Режим обсчёта возможных ходов.
    /// </summary>
    public enum MoveMode
    {
        /// <summary>
        /// Можем ходить, на любую клетку, если там нет союзника.
        /// </summary>
        WithoutKillTeammates = 0,

        /// <summary>
        /// Можем ходить, если такая клетка существует.
        /// </summary>
        /// <remarks>
        /// Режим нужен для проверки, что если вражеский король срубит нашу фигуру, 
        /// то мы его накажем и он не cможет её срубить в итоге.
        /// </remarks>
        NotRules = 1,
    }
}
