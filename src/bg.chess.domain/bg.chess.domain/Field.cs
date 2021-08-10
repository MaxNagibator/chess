namespace bg.chess.domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Игровое поле.
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Конструктор поля
        /// </summary>
        public Field()
        {

        }

        /// <summary>
        /// Конструктор поля
        /// </summary>
        /// <param name="rules">Правила игры.</param>
        /// <remarks>Фигуры из правил игры не клонируются, будь бдителен.</remarks>
        public Field(Rules rules)
        {
            SetRules(rules);
        }

        /// <summary>
        /// Установить базовые правила
        /// </summary>
        /// <returns></returns>
        public Field WithDefaultRules()
        {
            SetRules(new ClassicRules());
            return this;
        }

        private void SetRules(Rules rules)
        {
            var duplicatePos = rules.Positions.GroupBy(p => new { p.X, p.Y }).FirstOrDefault(pg => pg.Count() > 1);
            if (duplicatePos != null)
            {
                throw new Exception("dublicate position " + duplicatePos.Key.X + "/" + duplicatePos.Key.Y);
            }

            Positions = new List<FieldPosition>();
            for (var x = 0; x < rules.FieldWidth; x++)
            {
                for (var y = 0; y < rules.FieldHeight; y++)
                {
                    var positionFromRules = rules.Positions.FirstOrDefault(rulPos => rulPos.X == x && rulPos.Y == y);

                    var pos = new FieldPosition(this, x, y, positionFromRules?.Piece);
                    Positions.Add(pos);
                    positionFromRules?.Piece.Positions.Add(pos);
                }
            }

            FieldWidth = rules.FieldWidth;
            FieldHeight = rules.FieldHeight;
        }

        /// <summary>
        /// Позиции на поле.
        /// </summary>
        public List<FieldPosition> Positions { get; private set; }

        /// <summary>
        /// Ширина поля
        /// </summary>
        public int FieldWidth { get; private set; }

        /// <summary>
        /// Высота поля
        /// </summary>
        public int FieldHeight { get; private set; }

        /// <summary>
        /// Взять позицию по координатам.
        /// </summary>
        /// <param name="x">По ширине.</param>
        /// <param name="y">По высоте.</param>
        /// <returns>Позиция.</returns>
        public FieldPosition this[int x, int y]
        {
            get
            {
                return Positions.First(p => p.X == x && p.Y == y);
            }
        }

        /// <summary>
        /// Получить кусочек позиции, или пустоту
        /// </summary>
        /// <param name="x">По ширине.</param>
        /// <param name="y">По высоте.</param>
        /// <returns>Позиция.</returns>
        public FieldPosition GetPositionOrEmpty(int x, int y)
        {
            return Positions.FirstOrDefault(p => p.X == x && p.Y == y);
        }

        public void Move(Side side, int fromX, int fromY, int toX, int toY)
        {
            var currentPosition = this[fromX, fromY];
            var piece = currentPosition.Piece;
            if (piece == null)
            {
                throw new Exception("piece not found by " + fromX + "/" + fromY);
            }
            if (piece.Side != side)
            {
                throw new Exception("piece not this side");
            }

            piece.GetMoves(currentPosition);
        }
    }
}
