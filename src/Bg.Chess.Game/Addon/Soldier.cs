using System.Collections.Generic;
using System.Linq;

using Bg.Chess.Domain;

namespace Bg.Chess.Game.Addon
{
    /// <summary>
    /// Фигура "Гидра"
    /// </summary>
    public class Soldier : Pawn
    {
        /// </inheritdoc>
        public override bool IsPawnTransformAvailable => false;

        /// </inheritdoc>
        public override string Name => "soldier";

        /// </inheritdoc>
        public override char ShortName => 's';

        /// </inheritdoc>
        protected override List<Domain.Position> GetBaseMoves(Domain.Piece piece, MoveMode moveMode)
        {
            var pawnMoves = base.GetBaseMoves(piece, moveMode);
            var position = piece.CurrentPosition;

            // удалим ходы по вертикали и просчитаем заново
            var availablePositions = pawnMoves.Where(x => x.X != position.X).ToList();

            var pos = piece.Field.GetPositionOrEmpty(position.X, position.Y + piece.MoveMult);
            if (pos != null)
            {
                if(pos.Piece != null)
                {
                    if (pos.IsTeammate(piece.Side))
                    {
                        if (!moveMode.HasFlag(MoveMode.WithoutKillTeammates))
                        {
                            availablePositions.Add(pos);
                        }
                    }
                    else
                    {
                        // солдаты не может рубить вражеского солдата в лоб, только атака с боку (по диагонали)
                        if (pos.Piece.Type.ShortName != piece.Type.ShortName)
                        {
                            availablePositions.Add(pos);
                        }
                    }
                }
                else
                {
                    availablePositions.Add(pos);
                }
            }

            return availablePositions;
        }
    }
}
