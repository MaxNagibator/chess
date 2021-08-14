namespace Bg.Chess.Domain
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
            PawnTransforms = rules.PawnTransforms;
        }

        /// <summary>
        /// Позиции на поле.
        /// </summary>
        public List<FieldPosition> Positions { get; private set; }

        /// <summary>
        /// Получить позиции на поле, где находятся фигуры игрока.
        /// </summary>
        /// <param name="side">Игровая сторона, чьи фигуры мы хотим получить.</param>
        /// <returns>Список позиций.</returns>
        public List<FieldPosition> GetPositionsWithPiece(Side side)
        {
            return Positions.Where(x => x.Piece != null && x.Piece.Side == side).ToList();
        }

        /// <summary>
        /// Ширина поля
        /// </summary>
        public int FieldWidth { get; private set; }

        /// <summary>
        /// Высота поля
        /// </summary>
        public int FieldHeight { get; private set; }

        /// <summary>
        /// Методы получения фигуры для превращения пешки.
        /// </summary>
        public Dictionary<string, Func<Side, Piece>> PawnTransforms { get; private set; }

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
        /// Проверка на мат.
        /// </summary>
        /// <param name="checkSide">Проверяющая сторона.</param>
        /// <remarks>
        /// Если белые проверяют на мат, то нам надо проверить вражеского короля.
        /// </remarks>
        /// <returns>true - если мат.</returns>
        internal bool CheckMate(Side checkSide)
        {
            var shah = false;
            var enemyKing = Positions.First(x => x.Piece != null && x.Piece is King && x.Piece.Side == checkSide.Invert());
            foreach (var pos in Positions)
            {
                if (pos.Piece != null && pos.Piece.Side == checkSide)
                {
                    var moves = pos.GetAvailableMoves();
                    if (moves.Any(x => x.X == enemyKing.X && x.Y == enemyKing.Y))
                    {
                        //шах!
                        shah = true;

                    }
                }
            }
            if (shah)
            {
                var kingMoves = enemyKing.GetAvailableMoves();
                if (kingMoves.Count == 0)
                {
                    return true;
                }

                foreach (var pos in Positions)
                {
                    if (pos.Piece != null && pos.Piece.Side == checkSide)
                    {
                        var moves = pos.GetAvailableMoves(MoveMode.NotRules);
                        var kingMovesCount = kingMoves.Count;
                        for (int i = 0; i < kingMovesCount; i++)
                        {
                            FieldPosition kingMove = kingMoves[i];
                            var kingBlockedMove = moves.FirstOrDefault(x => x.X == kingMove.X && x.Y == kingMove.Y);
                            if (kingBlockedMove != null)
                            {
                                kingMoves.Remove(kingBlockedMove);
                                i--;
                                kingMovesCount--;
                                if (kingMoves.Count == 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
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

        public void Move(Side side, int fromX, int fromY, int toX, int toY, string pawnTransformPiece = null)
        {
            var currentPosition = this[fromX, fromY];
            var nextPosition = this[toX, toY];
            if (currentPosition == null)
            {
                throw new Exception("position not found by " + fromX + "/" + fromY);
            }
            if (nextPosition == null)
            {
                throw new Exception("position not found by " + toX + "/" + toY);
            }

            var piece = currentPosition.Piece;
            if (piece == null)
            {
                throw new Exception("piece not found by " + fromX + "/" + fromY);
            }
            if (piece.Side != side)
            {
                throw new Exception("piece not this side");
            }

            var moves = piece.GetAvailableMoves(currentPosition, MoveMode.WithoutKillTeammates);

            if (moves.Any(move => move.X == toX && move.Y == toY))
            {
                currentPosition.Move(this[toX, toY], pawnTransformPiece);
            }
            else
            {
                throw new Exception("move not available " + toX + "/" + toY);
            }
        }
    }
}
