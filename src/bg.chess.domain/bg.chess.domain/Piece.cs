namespace Bg.Chess.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Фигура.
    /// </summary>
    public class Piece
    {
        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        /// <param name="type">Тип фигуры.</param>
        public Piece(Side side, PieceType type)
        {
            Side = side;
            Type = type;
            Positions = new List<Position>();
        }

        /// <summary>
        /// Тип фигуры.
        /// </summary>
        public PieceType Type { get; }

        /// <summary>
        /// Кому пренадлежит фигура.
        /// </summary>
        public Side Side;

        /// <summary>
        /// Куда будет ходить фигура.
        /// </summary>
        /// <remarks>
        /// Белые идут вверх, чёрные вниз.
        /// </remarks>
        public int MoveMult => Side == Side.White ? 1 : -1;

        /// <summary>
        /// Фигура в результате игры не двигалась.
        /// </summary>
        internal bool IsInStartPosition => Positions.Count == 1;

        /// <summary>
        /// Текущая позиция фигуры
        /// </summary>
        public Position CurrentPosition { get; private set; }

        /// <summary>
        /// Поле, на котором стоит фигура
        /// </summary>
        public Field Field => CurrentPosition?.Field;

        /// <summary>
        /// Позиции фигуры в результате игры.
        /// </summary>
        private List<Position> Positions { get; set; }

        /// <summary>
        /// Добавить историю движения фигуры.
        /// </summary>
        internal void AddPosition(Position position)
        {
            CurrentPosition = position;
            Positions.Add(position);
        }

        /// <summary>
        /// Удалить последний элемент истории движения фигуры.
        /// </summary>
        internal void RemoveLastPosition()
        {
            Positions.RemoveAt(Positions.Count - 1);
            CurrentPosition = Positions.LastOrDefault();
        }

        private Dictionary<MoveMode, List<Position>> _availableMoves = new Dictionary<MoveMode, List<Position>>();
        private Dictionary<MoveMode, int> _fieldMoveNumber = new Dictionary<MoveMode, int>();
        
        /// <summary>
        /// Получить список доступных ходов.
        /// </summary>
        /// <param name="moveMode">Режим обсчёта.</param>
        /// <returns>Доступные ходы.</returns>
        /// <remarks>Использовать аккуратно, может привести к зациклевания при обсчёте королей.</remarks>
        internal List<Position> GetAvailableMoves(MoveMode moveMode)
        {
            var cacheNotWorkedAfterRevertMoveAndDisable = true;
            if (cacheNotWorkedAfterRevertMoveAndDisable == false)
            {
                if (_fieldMoveNumber.ContainsKey(moveMode))
                {
                    if (_fieldMoveNumber[moveMode] == Field.MoveNumber)
                    {
                        return _availableMoves[moveMode];
                    }
                }
            }

            var availableMoves = Type.GetAvailableMoves(this, moveMode);
            if (cacheNotWorkedAfterRevertMoveAndDisable == false)
            {
                _availableMoves[moveMode] = availableMoves;
                _fieldMoveNumber[moveMode] = Field.MoveNumber;
            }
            return availableMoves;
        }

        public override string ToString()
        {
            return Type.ShortName.ToString();
        }
    }
}
