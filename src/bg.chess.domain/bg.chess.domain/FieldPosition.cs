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
    //// todo возможно стоит упразднить этот класс, а логику перенести в "фигуру". будет более правильно наверн или нет, подумать
    //// но это надо удалить, вообще не очевидно, где используется какой класс!
    //// из фигуры можно выкинуть часть логики связанной со стороной
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

        /// <summary>
        /// Передвинуть фигуру с текущего поля на новую позицию.
        /// </summary>
        /// <param name="newPosition">Новая позиция.</param>
        /// <param name="pawnTransformPiece">Имя фигуры для трансформации пешки.</param>
        internal void Move(FieldPosition newPosition, string pawnTransformPiece = null)
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

            // todo записать в историю ходов новую позицию фигуры (начать с тестика, что пешка может сделать два раза двойной шаг)
            //Piece.Positions.Add(newPosition);

            //todo нужен метод "смерти фигуры" или не нужен?
            //newPosition.Piece.Kill();

            var isTransform = false;
            if (Piece is Pawn)
            {
                if(Piece.MoveMult == 1 && newPosition.Y == Field.FieldHeight - 1
                    || Piece.MoveMult == -1 && newPosition.Y == 0)
                {
                    if(pawnTransformPiece == null)
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
            }

            if (isTransform == false)
            {
                newPosition.Piece = Piece;
            }

            Piece = null;
        }
    }
}