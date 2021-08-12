namespace Bg.Chess.Domain
{
    using System;

    /// <summary>
    /// Игра.
    /// </summary>
    public class Game
    {
        private Field _field;
        private GameState state = GameState.WaitStart;

        // нужны буковки и цифорки, чтоб кормить на вход можно было e2 e4 :)
        private string widthSymbols = "abcdefgh";
        private string heightSymbols = "12345678";

        /// <summary>
        /// Чей ход.
        /// </summary>
        public Side StepSide { get; private set; } = Side.White;

        /// <summary>
        /// Задать параметры игры.
        /// </summary>
        /// <param name="field">Поле.</param>
        public void Init(Field field = null)
        {
            if (state != GameState.WaitStart)
            {
                throw new Exception("game state != waitStart");
            }

            if (field == null)
            {
                field = new Field().WithDefaultRules();
            }

            _field = field;
            state = GameState.InProgress;
        }

        /// <summary>
        /// Сделать ход.
        /// </summary>
        /// <param name="side">Кто ходит.</param>
        /// <param name="fromX">Откуда X</param>
        /// <param name="fromY">Откуда Y</param>
        /// <param name="toX">Куда X</param>
        /// <param name="toY">Куда Y</param>
        public void Move(Side side, string from, string to)
        {
            if (from.Length != 2)
            {
                throw new Exception("need two symbols for from");
            }

            if (to.Length != 2)
            {
                throw new Exception("need two symbols for to");
            }

            var fromX = widthSymbols.IndexOf(from[0]);
            var fromY = heightSymbols.IndexOf(from[1]);
            var toX = widthSymbols.IndexOf(to[0]);
            var toY = heightSymbols.IndexOf(to[1]);
            Move(side, fromX, fromY, toX, toY);
        }

        /// <summary>
        /// Сделать ход.
        /// </summary>
        /// <param name="side">Кто ходит.</param>
        /// <param name="fromX">Откуда X</param>
        /// <param name="fromY">Откуда Y</param>
        /// <param name="toX">Куда X</param>
        /// <param name="toY">Куда Y</param>
        public void Move(Side side, int fromX, int fromY, int toX, int toY)
        {
            if (state != GameState.InProgress)
            {
                throw new Exception("game state != InProgress");
            }

            if (side != StepSide)
            {
                throw new Exception("now move " + StepSide);
            }

            _field.Move(side, fromX, fromY, toX, toY);

            StepSide = StepSide.Invert();
        }
    }
}
