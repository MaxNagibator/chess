using System.Collections.Generic;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Позиция.
    /// </summary>
    /// <remarks>
    /// Кусочек игрового поля.
    /// </remarks>
    //// todo возможно стоит упразднить этот класс, а логику перенести в "фигуру". будет более правильно наверн или нет, подумать
    //// но это надо удалить, вообще не очевидно, где используется какой класс!
    public class FieldPosition : Position
    {
        /// <summary>
        /// Конструктор позиции.
        /// </summary>
        /// <param name="x">По горизонтали.</param>
        /// <param name="y">По вертикали.</param>
        public FieldPosition(Field field, int x, int y) : base(x, y)
        {
            Field = field;
        }

        /// <summary>
        /// Конструктор позиции.
        /// </summary>
        /// <param name="x">По горизонтали.</param>
        /// <param name="y">По вертикали.</param>
        /// <param name="piece">Фигура.</param>
        public FieldPosition(Field field, int x, int y, Piece piece) : base(x, y, piece)
        {
            Field = field;
        }

        /// <summary>
        /// Поле, к которому пренадлежит этот кусочек.
        /// </summary>
        public Field Field { get; }

        /// <summary>
        /// Получить список доступных ходов в этой клетке.
        /// </summary>
        /// <param name="moveMode">Режим обсчёта ходов.</param>
        /// <returns>Список возможных ходов.</returns>
        public List<FieldPosition> GetAvailableMoves(MoveMode moveMode = MoveMode.WithoutKillTeammates)
        {
            if(Piece == null)
            {
                return new List<FieldPosition>();
            }

            return Piece.GetAvailableMoves(this, moveMode);
        }
    }
}