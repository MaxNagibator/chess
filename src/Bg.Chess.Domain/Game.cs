namespace Bg.Chess.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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

        public List<AvailableMove> AvailableMoves()
        {
            var pieces = _field.GetPieces(StepSide);
            var availableMoves = new List<AvailableMove>();
            foreach (var piece in pieces)
            {
                var moves = piece.GetAvailableMoves(MoveMode.NotRules);
                var to = moves.Select(move => new AvailableMove.Position(move.X, move.Y)).ToList();
                availableMoves.Add(new AvailableMove(new AvailableMove.Position(piece.CurrentPosition.X, piece.CurrentPosition.Y), to));
            }

            return availableMoves;
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
        /// https://ru.wikipedia.org/wiki/%D0%9D%D0%BE%D1%82%D0%B0%D1%86%D0%B8%D1%8F_%D0%A4%D0%BE%D1%80%D1%81%D0%B0%D0%B9%D1%82%D0%B0_%E2%80%94_%D0%AD%D0%B4%D0%B2%D0%B0%D1%80%D0%B4%D1%81%D0%B0
        /// </remarks>
        /// <returns>Растановка фигур на доске.</returns>
        public string GetForsythEdwardsNotation()
        {
            var notationSb = new StringBuilder();
            for (var i = _field.FieldHeight - 1; i >= 0; i--)
            {
                if (i < _field.FieldHeight - 1)
                {
                    notationSb.Append("/");
                }
                var lineEmptyCount = 0;
                for (var j = 0; j < _field.FieldWidth; j++)
                {
                    var posPiece = _field[j, i].Piece;
                    if (posPiece != null)
                    {
                        if (lineEmptyCount > 0)
                        {
                            notationSb.Append(lineEmptyCount);
                            lineEmptyCount = 0;
                        }

                        var pieceName = posPiece.Type.ShortName;
                        if (posPiece.Side == Side.White)
                        {
                            notationSb.Append(pieceName.ToString().ToUpper());
                        }
                        else
                        {
                            notationSb.Append(pieceName);
                        }
                    }
                    else
                    {
                        lineEmptyCount++;
                    }
                }

                if (lineEmptyCount > 0)
                {
                    notationSb.Append(lineEmptyCount);
                }
            }

            notationSb.Append(" ");
            if (StepSide == Side.White)
            {
                notationSb.Append("w");
            }
            else
            {
                notationSb.Append("b");
            }

            var whiteKing = _field.GetPieces(Side.White).Where(x => x.Type is King).First();
            var whiteRooks = _field.GetPieces(Side.White).Where(x => x.Type is Rook).ToList();

            var castlingString = string.Empty;
            if (whiteKing.IsInStartPosition)
            {
                var rightRookInStart = whiteRooks.Any(x => x.CurrentPosition.X == 7 && x.IsInStartPosition);
                if (rightRookInStart)
                {
                    castlingString += "K";
                }
                var leftRookInStart = whiteRooks.Any(x => x.CurrentPosition.X == 0 && x.IsInStartPosition);
                if (leftRookInStart)
                {
                    castlingString += "Q";
                }
            }
            var blackKing = _field.GetPieces(Side.Black).Where(x => x.Type is King).First();
            var blackRooks = _field.GetPieces(Side.Black).Where(x => x.Type is Rook).ToList();
            if (blackKing.IsInStartPosition)
            {
                var rightRookInStart = blackRooks.Any(x => x.CurrentPosition.X == 7 && x.IsInStartPosition);
                if (rightRookInStart)
                {
                    castlingString += "k";
                }
                var leftRookInStart = blackRooks.Any(x => x.CurrentPosition.X == 0 && x.IsInStartPosition);
                if (leftRookInStart)
                {
                    castlingString += "q";
                }
            }
            if (string.IsNullOrEmpty(castlingString))
            {
                castlingString = "-";
            }
            notationSb.Append(" " + castlingString);

            var lastMove = _field.Moves.LastOrDefault();
            if (lastMove != null && lastMove.Runner.Type is Pawn && Math.Abs(lastMove.To.Y - lastMove.From.Y) == 2)
            {
                var toX = widthSymbols[lastMove.To.X];
                int toY;
                if (lastMove.To.Y > lastMove.From.Y)
                {
                    toY = lastMove.To.Y - 1;
                }
                else
                {
                    toY = lastMove.To.Y + 1;
                }
                notationSb.Append(" " + toX + heightSymbols[toY]);
            }
            else
            {
                notationSb.Append(" -");
            }
            var withoutKillAndPawnMoveCount = 0;
            for (var i = _field.Moves.Count - 1; i >= 0; i--)
            {
                if(_field.Moves[i].Runner.Type is Pawn || _field.Moves[i].KillEnemy != null)
                {
                    break;
                }
                else
                {
                    withoutKillAndPawnMoveCount++;
                }
            }
            notationSb.Append(" " + withoutKillAndPawnMoveCount);
            notationSb.Append(" " + ((_field.Moves.Count / 2) + 1));
            return notationSb.ToString();
        }
    }
}
