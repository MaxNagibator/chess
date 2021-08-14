using System;
using System.Collections.Generic;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Позиция.
    /// </summary>
    /// <remarks>
    /// Кусочек игрового поля.
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
        /// Конструктор позиции.
        /// </summary>
        /// <param name="x">По горизонтали.</param>
        /// <param name="y">По вертикали.</param>
        public Position(Field field, int x, int y) : this(x, y)
        {
            Field = field;
        }

        /// <summary>
        /// Конструктор позиции.
        /// </summary>
        /// <param name="x">По горизонтали.</param>
        /// <param name="y">По вертикали.</param>
        /// <param name="piece">Фигура.</param>
        public Position(Field field, int x, int y, Piece piece) : this(x, y, piece)
        {
            Field = field;
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
        public Piece Piece { get; protected set; }

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

        public override string ToString()
        {
            return X + "/" + Y + (Piece != null ? "/" + Piece.ToString() : null);
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
        public List<Position> GetAvailableMoves(MoveMode moveMode = MoveMode.WithoutKillTeammates)
        {
            if (Piece == null)
            {
                return new List<Position>();
            }

            return Piece.GetAvailableMoves(this, moveMode);
        }

        /// <summary>
        /// Передвинуть фигуру с текущего поля на новую позицию.
        /// </summary>
        /// <param name="newPosition">Новая позиция.</param>
        /// <param name="pawnTransformPiece">Имя фигуры для трансформации пешки.</param>
        internal void Move(Position newPosition, string pawnTransformPiece = null)
        {
            if (Piece == null)
            {
                throw new Exception("piece not found");
            }

            if (newPosition.Piece != null)
            {
                if (newPosition.IsTeammate(Piece.Side))
                {
                    throw new Exception("it's teammate attack!!!");
                }
            }

            var isTransform = false;
            if (Piece is Pawn)
            {
                // если дошли до края поля, то выбираем новую фигуру
                if (Piece.MoveMult == 1 && newPosition.Y == Field.FieldHeight - 1
                    || Piece.MoveMult == -1 && newPosition.Y == 0)
                {
                    if (pawnTransformPiece == null)
                    {
                        throw new Exception("need transform piece");
                    }

                    Func<Side, Piece> transformAction;
                    if (!Field.PawnTransforms.TryGetValue(pawnTransformPiece, out transformAction))
                    {
                        throw new Exception("transform for " + pawnTransformPiece + " not available");
                    }

                    var newPiece = transformAction(Piece.Side);
                    // возможно стоит перекинуть историю на новую фигуру
                    isTransform = true;
                    newPosition.Piece = newPiece;
                }

                var pawn = Piece as Pawn;
                if (pawn.EnPassant(this, 1) != null)
                {
                    Field[this.X + 1, this.Y].Piece = null;
                }
                if (pawn.EnPassant(this, -1) != null)
                {
                    Field[this.X - 1, this.Y].Piece = null;
                }
            }

            if (isTransform == false)
            {
                newPosition.Piece = Piece;
            }

            Piece.AddPosition(newPosition);
            Piece = null;
        }
    }
}