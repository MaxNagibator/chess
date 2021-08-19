namespace Bg.Chess.Domain
{
    using System;
    using System.Linq;

    /// <summary>
    /// Игра.
    /// </summary>
    public class Game
    {
        private Field _field;

        /// <summary>
        /// Состояние игры.
        /// </summary>
        public GameState State { get; private set; } = GameState.WaitStart;

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
            if (State != GameState.WaitStart)
            {
                throw new Exception("game state != waitStart");
            }

            if (field == null)
            {
                field = new Field().WithDefaultRules();
            }

            _field = field;
            var whiteKing = _field.Positions.Count(x => x.Piece != null && x.Piece.Type is King && x.Piece.Side == Side.White);
            var blackKing = _field.Positions.Count(x => x.Piece != null && x.Piece.Type is King && x.Piece.Side == Side.Black);
            if (whiteKing != 1)
            {
                throw new Exception("need one white king");
            }
            if (blackKing != 1)
            {
                throw new Exception("need one black king");
            }
            State = GameState.InProgress;
        }

        /// <summary>
        /// Сделать ход.
        /// </summary>
        /// <param name="side">Кто ходит.</param>
        /// <param name="from">Откуда. пример A1.</param>
        /// <param name="to">Куда. пример A7.</param>
        /// <param name="pawnTransformPiece">Название фигуры, для превращения пешки в другую фигуру в конце поля.</param>
        public void Move(Side side, string from, string to, string pawnTransformPiece = null)
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
            Move(side, fromX, fromY, toX, toY, pawnTransformPiece);
        }

        /// <summary>
        /// Сделать ход.
        /// </summary>
        /// <param name="side">Кто ходит.</param>
        /// <param name="fromX">Откуда X.</param>
        /// <param name="fromY">Откуда Y.</param>
        /// <param name="toX">Куда X.</param>
        /// <param name="toY">Куда Y.</param>
        /// <param name="pawnTransformPiece">Название фигуры, для превращения пешки в другую фигуру в конце поля.</param>
        public void Move(Side side, int fromX, int fromY, int toX, int toY, string pawnTransformPiece = null)
        {
            if (State != GameState.InProgress)
            {
                throw new Exception("game state != InProgress");
            }

            if (side != StepSide)
            {
                throw new Exception("now move " + StepSide);
            }

            _field.Move(side, fromX, fromY, toX, toY, pawnTransformPiece);

            // над теперь реализовать какойнить бомжатский MVP для окончания игры
            var mate = _field.CheckMate(StepSide);
            if (mate == CheckMateResult.Mate)
            {
                if (StepSide == Side.White)
                {
                    State = GameState.WinWhite;
                }
                else
                {
                    State = GameState.WinBlack;
                }
                return;
            }
            else if (mate == CheckMateResult.None)
            {
                StepSide = StepSide.Invert();
            }
            else
            {
                State = GameState.Draw;
            }
        }

        /// <summary>
        /// Нотация Форсайта — Эдвардса / Forsyth–Edwards Notation.
        /// </summary>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation
        /// </remarks>
        /// <returns>Растановка фигур на доске.</returns>
        public string GetNotation()
        {
            // денёк слишком короткий остался, поэтому сайтец мы начнём набрасывать как нить в другой раз)
            throw new NotImplementedException("сделаем скоро");
            //todo нам понадобится этот метод, чтоб выплёвывать это на фронт и там уже по ней можно будет рисовать доску
            //изначально планировался обычный json (сериализованная dto-шка какаянить. но потом я наткнулся на это)

            // Нотация Форсайта — Эдвардса
            ////Пример
            ////rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
            ////Начальная позиция
            ////Запись позиций с помощью нотации Форсайта—Эдвардса(FEN)

            ////Начальная позиция шахматной партии:
            ////rnbqkbnr / pppppppp / 8 / 8 / 8 / 8 / PPPPPPPP / RNBQKBNR w KQkq -0 1

            ////rnbqkbnr — расположение фигур на 8 - й горизонтали слева направо,
            ///// — разделитель,
            ////pppppppp — расположение фигур на 7 - й горизонтали,
            ////8 / 8 / 8 / 8 — пустые 6 - 5 - 4 - 3 - я горизонтали,
            ////PPPPPPPP — расположение фигур на 2 - й горизонтали,
            ////RNBQKBNR — расположение фигур на 1 - й горизонтали,
            ////w — предстоит ход белых,
            ////KQkq — возможны короткие и длинные рокировки белых и чёрных,
            ////- — не было предыдущего хода пешкой на два поля,
            ////0 — последних ходов без взятий или движения пешек не было,
            ////1 — предстоит первый ход.

            ////Позиция после хода 1.e4: rnbqkbnr / pppppppp / 8 / 8 / 4P3 / 8 / PPPP1PPP / RNBQKBNR b KQkq e3 0 1

            ////После хода 1. ... d5: rnbqkbnr / ppp1pppp / 8 / 3p4 / 4P3 / 8 / PPPP1PPP / RNBQKBNR w KQkq d6 0 2

            ////После хода 2.Nf3: rnbqkbnr / ppp1pppp / 8 / 3p4 / 4P3 / 5N2 / PPPP1PPP / RNBQKB1R b KQkq -1 2

            ////После хода 2. ... Kd7: rnbq1bnr / pppkpppp / 8 / 3p4 / 4P3 / 5N2 / PPPP1PPP / RNBQKB1R w KQ -2 3
            return "";
        }
    }
}
