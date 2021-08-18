namespace Bg.Chess.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Фигура "Король"
    /// </summary>
    public class King : PieceType
    {
        /// </inheritdoc>
        public override bool IsPawnTransformAvailable => false;

        /// </inheritdoc>
        public override string Name => "king";

        /// </inheritdoc>
        public override char ShortName => 'k';

        /// </inheritdoc>
        protected override List<Position> GetBaseMoves(Piece piece, MoveMode moveMode)
        {
            var king = piece;
            var availablePositions = new List<Position>();

            AddAvailableDiagonalMoves(piece, availablePositions, moveMode, 1);
            AddAvailableLineMoves(piece, availablePositions, moveMode, 1);

            var enemyPieces = piece.CurrentPosition.Field.GetPieces(piece.Side.Invert());

            AddAvailableCastling(king, enemyPieces, availablePositions);

            return availablePositions;
        }

        /// <summary>
        /// Обсчёт возможности рокировки.
        /// </summary>
        /// <remarks>
        /// https://ru.wikipedia.org/wiki/%D0%A0%D0%BE%D0%BA%D0%B8%D1%80%D0%BE%D0%B2%D0%BA%D0%B0
        /// </remarks>
        private void AddAvailableCastling(Piece king, List<Piece> enemyPieces, List<Position> availablePositions)
        {
            var field = king.Field;
            var position = king.CurrentPosition;
            if (king.IsInStartPosition)
            {
                // обсчитать, что король не под шахом
                var hasShah = false;

                // optimization: было бы красиво yeald возвращать и останавливая перебор тормозим всё
                foreach (var enemyPiece in enemyPieces.Where(x => x.Type is King == false))
                {
                    var attackPositions = enemyPiece.GetAvailableMoves(MoveMode.WithoutKillTeammates | MoveMode.IndifferentKingDeath);
                    if (attackPositions.Any(x => x.X == position.X && x.Y == position.Y))
                    {
                        hasShah = true;
                        break;
                    }
                }

                if (!hasShah)
                {
                    // получили союзные ладьи, с нами на одной линии, которые не двигались
                    var rooks = field.GetPieces(king.Side).Where(x => x.Type is Rook
                        && x.IsInStartPosition
                        // проверим, что они находятся на одной линии (ну у нас может быть произвольная расстановка и мало ли)
                        && x.CurrentPosition.Y == position.Y).ToList();
                    foreach (var rook in rooks)
                    {
                        var sectorClear = СastlingSectorClear(king, position, field, rook.CurrentPosition);
                        if (sectorClear)
                        {
                            var checkPositions = new List<Position>();
                            if (rook.CurrentPosition.X > position.X)
                            {
                                checkPositions.Add(field[position.X + 1, position.Y]);
                                checkPositions.Add(field[position.X + 2, position.Y]);
                            }
                            else
                            {
                                checkPositions.Add(field[position.X - 1, position.Y]);
                                checkPositions.Add(field[position.X - 2, position.Y]);
                            }

                            var moveSafety = KingMoveNotAttack(checkPositions, enemyPieces, MoveMode.WithoutKillTeammates);
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
        private bool KingMoveNotAttack(List<Position> kingMovePositions, List<Piece> enemyPieces, MoveMode moveMode)
        {
            foreach (var enemyPiece in enemyPieces)
            {
                // особая обработка взаимодействия королей, так как зацикливается метод "доступных ходов"
                if(enemyPiece.Type is King)
                {
                    foreach (var kingMovePosition in kingMovePositions)
                    {
                        if (Math.Abs(kingMovePosition.X - enemyPiece.CurrentPosition.X) < 2 
                            && Math.Abs(kingMovePosition.Y - enemyPiece.CurrentPosition.Y) < 2)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    var attackPositions = enemyPiece.GetAvailableMoves(moveMode | MoveMode.IndifferentKingDeath);
                    foreach (var kingMovePosition in kingMovePositions)
                    {
                        if (attackPositions.Any(x => x.X == kingMovePosition.X && x.Y == kingMovePosition.Y))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Проверка свободного места для рокировки
        /// </summary>
        /// <returns>false - если между королём и ладьей, предназначенными для рокировки, находится другая фигура.</returns>
        private bool СastlingSectorClear(Piece king, Position position, Field field, Position rookPosition)
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
    }
}
