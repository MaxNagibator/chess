namespace Bg.Chess.Domain
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Правила игры.
    /// </summary>
    public class ClassicRules : Rules
    {
        public ClassicRules() : base()
        {
            FieldWidth = 8;
            FieldHeight = 8;
            Positions = new List<Position>
            {
                new Position(0, 0, PieceBuilder.Rook(Side.White)),
                new Position(1, 0, PieceBuilder.Knight(Side.White)),
                new Position(2, 0, PieceBuilder.Bishop(Side.White)),
                new Position(3, 0, PieceBuilder.Queen(Side.White)),
                new Position(4, 0, PieceBuilder.King(Side.White)),
                new Position(5, 0, PieceBuilder.Bishop(Side.White)),
                new Position(6, 0, PieceBuilder.Knight(Side.White)),
                new Position(7, 0, PieceBuilder.Rook(Side.White)),
                new Position(0, 1, PieceBuilder.Pawn(Side.White)),
                new Position(1, 1, PieceBuilder.Pawn(Side.White)),
                new Position(2, 1, PieceBuilder.Pawn(Side.White)),
                new Position(3, 1, PieceBuilder.Pawn(Side.White)),
                new Position(4, 1, PieceBuilder.Pawn(Side.White)),
                new Position(5, 1, PieceBuilder.Pawn(Side.White)),
                new Position(6, 1, PieceBuilder.Pawn(Side.White)),
                new Position(7, 1, PieceBuilder.Pawn(Side.White)),

                new Position(0, 7, PieceBuilder.Rook(Side.Black)),
                new Position(1, 7, PieceBuilder.Knight(Side.Black)),
                new Position(2, 7, PieceBuilder.Bishop(Side.Black)),
                new Position(3, 7, PieceBuilder.Queen(Side.Black)),
                new Position(4, 7, PieceBuilder.King(Side.Black)),
                new Position(5, 7, PieceBuilder.Bishop(Side.Black)),
                new Position(6, 7, PieceBuilder.Knight(Side.Black)),
                new Position(7, 7, PieceBuilder.Rook(Side.Black)),
                new Position(0, 6, PieceBuilder.Pawn(Side.Black)),
                new Position(1, 6, PieceBuilder.Pawn(Side.Black)),
                new Position(2, 6, PieceBuilder.Pawn(Side.Black)),
                new Position(3, 6, PieceBuilder.Pawn(Side.Black)),
                new Position(4, 6, PieceBuilder.Pawn(Side.Black)),
                new Position(5, 6, PieceBuilder.Pawn(Side.Black)),
                new Position(6, 6, PieceBuilder.Pawn(Side.Black)),
                new Position(7, 6, PieceBuilder.Pawn(Side.Black)),
            };

            PawnTransforms = new Dictionary<string, Func<Side, Piece>>();
            foreach (var itm in PieceBuilder.PieceTypes)
            {
                if (itm.Value.IsPawnTransformAvailable)
                {
                    PawnTransforms.Add(itm.Key, (Side side) => { return new Piece(side, itm.Value); });
                }
            }
        }
    }
}
