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

        /// <summary>
        /// Номер хода.
        /// </summary>
        public int MoveNumber => Moves.Count;

        /// <summary>
        /// Ходы фигур.
        /// </summary>
        public List<Move> Moves { get; internal set; }

        private void SetRules(Rules rules)
        {
            var duplicatePos = rules.Positions.GroupBy(p => new { p.X, p.Y }).FirstOrDefault(pg => pg.Count() > 1);
            if (duplicatePos != null)
            {
                throw new Exception("dublicate position " + duplicatePos.Key.X + "/" + duplicatePos.Key.Y);
            }

            Positions = new List<Position>();
            for (var x = 0; x < rules.FieldWidth; x++)
            {
                for (var y = 0; y < rules.FieldHeight; y++)
                {
                    var positionFromRules = rules.Positions.FirstOrDefault(rulPos => rulPos.X == x && rulPos.Y == y);

                    var pos = new Position(this, x, y, positionFromRules?.Piece);
                    Positions.Add(pos);
                    positionFromRules?.Piece.AddPosition(pos);
                }
            }

            FieldWidth = rules.FieldWidth;
            FieldHeight = rules.FieldHeight;
            PawnTransforms = rules.PawnTransforms;
            Moves = new List<Move>();
        }

        /// <summary>
        /// Позиции на поле.
        /// </summary>
        public List<Position> Positions { get; private set; }

        /// <summary>
        /// Получить позиции на поле, где находятся фигуры игрока.
        /// </summary>
        /// <param name="side">Игровая сторона, чьи фигуры мы хотим получить.</param>
        /// <returns>Список позиций.</returns>
        public List<Piece> GetPieces(Side side)
        {
            return Positions.Where(x => x.Piece != null && x.Piece.Side == side).Select(x=>x.Piece).ToList();
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
        public Position this[int x, int y]
        {
            get
            {
                return Positions.First(p => p.X == x && p.Y == y);
            }
        }

        /// <summary>
        /// Проверка на шах.
        /// </summary>
        /// <param name="checkSide">Проверяющая сторона.</param>
        /// <remarks>
        /// Если белые проверяют на шах, то нам надо проверить вражеского короля.
        /// </remarks>
        /// <returns>true - если шах.</returns>
        internal bool CheckKingAlert(Side checkSide)
        {
            var enemyKing = Positions.First(x => x.Piece != null && x.Piece.Type is King && x.Piece.Side == checkSide.Invert());
            foreach (var pos in Positions)
            {
                if (pos.Piece != null && pos.Piece.Side == checkSide)
                {
                    var moves = pos.GetAvailableMoves(MoveMode.IndifferentKingDeath);
                    //var moves = pos.GetAvailableMoves(MoveMode.WithoutKillTeammates | MoveMode.IndifferentKingDeath);
                    if (moves.Any(x => x.X == enemyKing.X && x.Y == enemyKing.Y))
                    {
                        //шах!
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Проверка на мат.
        /// </summary>
        /// <param name="checkSide">Проверяющая сторона.</param>
        /// <remarks>
        /// Если белые проверяют на мат, то нам надо проверить вражеского короля.
        /// </remarks>
        /// <returns>true - если мат.</returns>
        internal CheckMateResult CheckMate(Side checkSide)
        {
            var enemiesPositions = Positions.Where(x => x.Piece != null && x.Piece.Side == checkSide.Invert()).ToList();
            var kingAlertExists = CheckKingAlert(checkSide);
            foreach (var enemyPos in enemiesPositions)
            {
                var moves = enemyPos.GetAvailableMoves();

                foreach (var move in moves)
                {
                    enemyPos.Move(move, false);

                    // проверим, что после моего хода, враг имеет возможность убить нашего короля
                    if (!CheckKingAlert(checkSide))
                    {
                        enemyPos.RevertLastMove();
                        return CheckMateResult.None;
                    }

                    enemyPos.RevertLastMove();
                }
            }
            
            // если король не под шахом и мы выше не вышли из этого метода, 
            // означает, что ходов нет и это пат!
            if(kingAlertExists == false)
            {
                return CheckMateResult.DrawByEnemyDontHasMoves;
            }

            return CheckMateResult.Mate;
        }

        /// <summary>
        /// Получить кусочек позиции, или пустоту
        /// </summary>
        /// <param name="x">По ширине.</param>
        /// <param name="y">По высоте.</param>
        /// <returns>Позиция.</returns>
        public Position GetPositionOrEmpty(int x, int y)
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

            var moves = piece.GetAvailableMoves(MoveMode.WithoutKillTeammates);

            if (moves.Any(move => move.X == toX && move.Y == toY))
            {
                currentPosition.Move(this[toX, toY], true, pawnTransformPiece);
            }
            else
            {
                throw new Exception("move not available " + fromX + "/" + fromY + " -> " + toX + "/" + toY);
            }
        }
    }
}
