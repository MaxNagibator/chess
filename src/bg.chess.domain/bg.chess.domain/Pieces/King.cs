using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Фигура "Король"
    /// </summary>
    public class King : Piece
    {
        /// </inheritdoc>
        public override bool IsPawnTransformAvailable => false;

        /// </inheritdoc>
        public override string Name => "king";

        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public King(Side side) : base(side)
        {
        }

        /// </inheritdoc>
        protected override List<Position> GetBaseMoves(Position position, MoveMode moveMode)
        {
            var availablePositions = new List<Position>();

            AddAvailableDiagonalMoves(position, availablePositions, moveMode, 1);
            AddAvailableLineMoves(position, availablePositions, moveMode, 1);

            var enemyPositions = position.Field.GetPositionsWithPiece(Side.Invert());
            var notAttackedPositions = new List<Position>();
            foreach (var pos in availablePositions)
            {
                var isSafetyMove = KingMoveNotAttack(new List<Position> { pos }, enemyPositions, MoveMode.NotRules);
                if (isSafetyMove)
                {
                    notAttackedPositions.Add(pos);
                }
            }

            AddAvailableCastling(position, enemyPositions, notAttackedPositions);

            return notAttackedPositions;
        }

        /// <summary>
        /// Обсчёт возможности рокировки.
        /// </summary>
        /// <remarks>
        /// https://ru.wikipedia.org/wiki/%D0%A0%D0%BE%D0%BA%D0%B8%D1%80%D0%BE%D0%B2%D0%BA%D0%B0
        /// </remarks>
        private void AddAvailableCastling(Position position, List<Position> enemyPositions, List<Position> availablePositions)
        {
            var field = position.Field;
            if (IsInStartPosition)
            {
                // обсчитать, что король не под шахом
                var hasShah = false;

                // optimization: было бы красиво yeald возвращать и останавливая перебор тормозим всё
                foreach (var enemyPosition in enemyPositions)
                {
                    var attackPositions = enemyPosition.GetAvailableMoves();
                    if (attackPositions.Any(x => x.X == position.X && x.Y == position.Y))
                    {
                        hasShah = true;
                        break;
                    }
                }

                if (!hasShah)
                {
                    // получили союзные ладьи, с нами на одной линии, которые не двигались
                    var rookPositions = field.GetPositionsWithPiece(Side).Where(x => x.Piece is Rook
                        && x.Piece.IsInStartPosition
                        // проверим, что они находятся на одной линии (ну у нас может быть произвольная расстановка и мало ли)
                        && x.Y == position.Y).ToList();
                    foreach (var rookPosition in rookPositions)
                    {
                        var sectorClear = СastlingSectorClear(position, field, rookPosition);
                        if (sectorClear)
                        {
                            var checkPositions = new List<Position>();
                            if (rookPosition.X > position.X)
                            {
                                checkPositions.Add(field[position.X + 1, position.Y]);
                                checkPositions.Add(field[position.X + 2, position.Y]);
                            }
                            else
                            {
                                checkPositions.Add(field[position.X - 1, position.Y]);
                                checkPositions.Add(field[position.X - 2, position.Y]);
                            }

                            var moveSafety = KingMoveNotAttack(checkPositions, enemyPositions, MoveMode.WithoutKillTeammates);
                            if (moveSafety)
                            {
                                availablePositions.Add(checkPositions[1]);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Проверка безопасного прохода короля.
        /// </summary>
        /// <returns>false - в результате рокировки пройдёт через битое поле или встанет под шах.</returns>
        //// optimisation: поидеи мы уже обсчитали, может ли король ходить влево и в право, 
        //// поэтому можно метод переоборудовать чтоб принимал, только вторую позицию, а первую обсчитывать ранее
        private bool KingMoveNotAttack(List<Position> kingMovePositions, List<Position> enemyPositions, MoveMode moveMode)
        {
            foreach (var enemyPosition in enemyPositions)
            {
                // optimization: мы уже получали шаги и можно повторно не расчитывать
                // возможно даже обсчёт ходов "кэшировать", если на игровом поле ничего не поменялось возвращаем старый результат
                // и сбрасывать кэш при "шагах"
                var attackPositions = enemyPosition.GetAvailableMoves(moveMode);
                foreach (var kingMovePosition in kingMovePositions)
                {
                    if (attackPositions.Any(x => x.X == kingMovePosition.X && x.Y == kingMovePosition.Y))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Проверка свободного места для рокировки
        /// </summary>
        /// <returns>false - если между королём и ладьей, предназначенными для рокировки, находится другая фигура.</returns>
        private bool СastlingSectorClear(Position position, Field field, Position rookPosition)
        {
            int left;
            int right;
            if (rookPosition.X > position.X)
            {
                left = position.X + 1;
                right = rookPosition.X - 1;
            }
            else
            {
                left = rookPosition.X + 1;
                right = position.X - 1;
            }

            for (var i = left; i <= right; i++)
            {
                if (field[i, position.Y].Piece != null)
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return "K";
        }
    }
}
