﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Фигура "Пешка"
    /// </summary>
    public class Pawn : Piece
    {
        /// </inheritdoc>
        public override bool IsPawnTransformAvailable => false;

        /// </inheritdoc>
        public override string Name => "pawn";

        /// <summary>
        /// Конструктор фигуры.
        /// </summary>
        /// <param name="side">Кому пренадлежит фигура.</param>
        public Pawn(Side side) : base(side)
        {
        }

        /// </inheritdoc>
        protected override List<Position> GetBaseMoves(Position position, MoveMode moveMode)
        {
            var width = position.Field.FieldWidth;
            var height = position.Field.PawnTransforms;

            var availablePositions = new List<Position>();

            if (moveMode == MoveMode.WithoutKillTeammates)
            {
                var pos = position.Field.GetPositionOrEmpty(position.X, position.Y + MoveMult);
                if (pos?.Piece == null)
                {
                    availablePositions.Add(pos);

                    // если пешка ранее не ходила, то имеет право сходить на две клетки
                    if (IsInStartPosition)
                    {
                        var pos2 = position.Field.GetPositionOrEmpty(position.X, position.Y + MoveMult + MoveMult);
                        if (pos2?.Piece == null)
                        {
                            availablePositions.Add(pos2);
                        }
                    }
                }
            }

            CheckEnemyKill(position, 1, availablePositions, moveMode);
            CheckEnemyKill(position, -1, availablePositions, moveMode);

            EnPassant(position, 1, availablePositions);
            EnPassant(position, -1, availablePositions);

            return availablePositions.Where(x => x != null).ToList();
        }

        private void EnPassant(Position position, int shiftX, List<Position> availablePositions)
        {
            var pos = EnPassant(position, shiftX);
            if(pos != null)
            {
                availablePositions.Add(pos);
            }
        }

        /// <summary>
        /// Взятие пешки на проходе.
        /// </summary>
        /// <param name="position">Позиция ходящей пешки.</param>
        /// <param name="shiftX">Сдвиг по горизонтале, для проверки вражеской пешки.</param>
        /// <returns>Вернёт позицию доступную для хода, если пешку можно взять на проходе, иначе null.</returns>
        /// <remarks>
        /// https://ru.wikipedia.org/wiki/%D0%92%D0%B7%D1%8F%D1%82%D0%B8%D0%B5_%D0%BD%D0%B0_%D0%BF%D1%80%D0%BE%D1%85%D0%BE%D0%B4%D0%B5
        /// </remarks>
        public Position EnPassant(Position position, int shiftX)
        {
            var pawnPos = position.Field.GetPositionOrEmpty(position.X + shiftX, position.Y);

            if (pawnPos != null && pawnPos.Piece != null && pawnPos.Piece is Pawn)
            {
                var lastMove = position.Field.Moves.LastOrDefault();
                if (lastMove != null && lastMove.To.X == position.X + shiftX && lastMove.To.Y == position.Y &&
                    Math.Abs(lastMove.From.Y - lastMove.To.Y) == 2)
                {
                    var pos = position.Field.GetPositionOrEmpty(position.X + shiftX, position.Y + MoveMult);
                    return pos;
                }
            }

            return null;
        }

        /// <summary>
        /// Проверка на возможность убить врага
        /// </summary>
        private void CheckEnemyKill(Position position, int shiftX, List<Position> availablePositions, MoveMode moveMode)
        {
            var pos = position.Field.GetPositionOrEmpty(position.X + shiftX, position.Y + MoveMult);
            if (pos != null)
            {
                if (moveMode == MoveMode.WithoutKillTeammates)
                {
                    if (pos.IsEnemy(Side))
                    {
                        availablePositions.Add(pos);
                    }
                }
                else
                {
                    availablePositions.Add(pos);
                }
            }
        }

        public override string ToString()
        {
            return "P";
        }
    }
}

