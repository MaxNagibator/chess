using System;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Позиция.
    /// </summary>
    /// <remarks>
    /// Координаты на поле и фигура.
    /// </remarks>
    public class Position
    {
        /// <summary>
        /// Конструктор позиции.
        /// </summary>
        /// <param name="x">По горизонтали.</param>
        /// <param name="y">По вертикали.</param>
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Конструктор позиции.
        /// </summary>
        /// <param name="x">По горизонтали.</param>
        /// <param name="y">По вертикали.</param>
        /// <param name="piece">Фигура.</param>
        public Position(int x, int y, Piece piece) : this(x, y)
        {
            Piece = piece;
        }

        /// <summary>
        /// По горизонтали.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// По вертикали.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Шахматная фигура.
        /// </summary>
        public Piece Piece { get; private set; }

        /// <summary>
        /// Позиция пустая или там вражеская фигура.
        /// </summary>
        /// <param name="side">Сторона фигуры вызывающей проверку.</param>
        /// <returns>true - если позиция без фигуры или вражеская фигура присутствует.</returns>
        public bool IsEmptyOrEnemy(Side side)
        {
            return Piece == null || Piece.Side != side;
        }

        /// <summary>
        /// Позиция с вражеской фигурой.
        /// </summary>
        /// <param name="side">Сторона фигуры вызывающей проверку.</param>
        /// <returns>true - если вражеская фигура присутствует.</returns>
        public bool IsEnemy(Side side)
        {
            return Piece != null && Piece.Side != side;
        }

        /// <summary>
        /// Позиция с дружественной фигурой.
        /// </summary>
        /// <param name="side">Сторона фигуры вызывающей проверку.</param>
        /// <returns>true - если дружественная фигура присутствует.</returns>
        public bool IsTeammate(Side side)
        {
            return Piece != null && Piece.Side == side;
        }

        /// <summary>
        /// Передвинуть фигуру с текущего поля на новую позицию.
        /// </summary>
        /// <param name="newPosition">Новая позиция.</param>
        internal void Move(Position newPosition)
        {
            if(Piece == null)
            {
                throw new Exception("piece not found");
            }

            if(newPosition.Piece != null)
            {
                if (newPosition.IsTeammate(Piece.Side))
                {
                    // при правильной логики мы не увидим эти ошибки никогда, но добавить стоит
                    throw new Exception("it's teammate attack!!!");
                }

                //todo нужен метод "смерти фигуры" или не нужен?
                //newPosition.Piece.Kill();

                //так я вернулся)) убедился что музыку слышно)) мотивация)
                    //мотивированно пью пивко)
            }

            newPosition.Piece = Piece;
            Piece = null;
        }

        public override string ToString()
        {
            return X + "/" + Y + (Piece != null ? "/" + Piece.ToString() : null);
        }
    }
}