using System;
using System.Collections.Generic;

using Bg.Chess.Domain;
using DomainPosition = Bg.Chess.Domain.Position;
using DomainSide = Bg.Chess.Domain.Side;
using DomainPiece = Bg.Chess.Domain.Piece;


namespace Bg.Chess.Game.Addon
{
    /// <summary>
    /// Правила игры.
    /// </summary>
    public class DragonRules : Rules
    {
        public DragonRules(PieceTypes pieceTypes) : base()
        {
            FieldWidth = 10;
            FieldHeight = 8;

            Positions = new List<DomainPosition>
            {
                new DomainPosition(0, 0, pieceTypes.Hydra(DomainSide.White)),
                new DomainPosition(1, 0, pieceTypes.Rook(DomainSide.White)),
                new DomainPosition(2, 0, pieceTypes.Knight(DomainSide.White)),
                new DomainPosition(3, 0, pieceTypes.Bishop(DomainSide.White)),
                new DomainPosition(4, 0, pieceTypes.Queen(DomainSide.White)),
                new DomainPosition(5, 0, pieceTypes.King(DomainSide.White)),
                new DomainPosition(6, 0, pieceTypes.Bishop(DomainSide.White)),
                new DomainPosition(7, 0, pieceTypes.Knight(DomainSide.White)),
                new DomainPosition(8, 0, pieceTypes.Rook(DomainSide.White)),
                new DomainPosition(9, 0, pieceTypes.Dragon(DomainSide.White)),

                new DomainPosition(0, 1, pieceTypes.Pawn(DomainSide.White)),
                new DomainPosition(1, 1, pieceTypes.Pawn(DomainSide.White)),
                new DomainPosition(2, 1, pieceTypes.Pawn(DomainSide.White)),
                new DomainPosition(3, 1, pieceTypes.Soldier(DomainSide.White)),
                new DomainPosition(4, 1, pieceTypes.Pawn(DomainSide.White)),
                new DomainPosition(5, 1, pieceTypes.Pawn(DomainSide.White)),
                new DomainPosition(6, 1, pieceTypes.Soldier(DomainSide.White)),
                new DomainPosition(7, 1, pieceTypes.Pawn(DomainSide.White)),
                new DomainPosition(8, 1, pieceTypes.Pawn(DomainSide.White)),
                new DomainPosition(9, 1, pieceTypes.Pawn(DomainSide.White)),

                new DomainPosition(0, 6, pieceTypes.Pawn(DomainSide.Black)),
                new DomainPosition(1, 6, pieceTypes.Pawn(DomainSide.Black)),
                new DomainPosition(2, 6, pieceTypes.Pawn(DomainSide.Black)),
                new DomainPosition(3, 6, pieceTypes.Soldier(DomainSide.Black)),
                new DomainPosition(4, 6, pieceTypes.Pawn(DomainSide.Black)),
                new DomainPosition(5, 6, pieceTypes.Pawn(DomainSide.Black)),
                new DomainPosition(6, 6, pieceTypes.Soldier(DomainSide.Black)),
                new DomainPosition(7, 6, pieceTypes.Pawn(DomainSide.Black)),
                new DomainPosition(8, 6, pieceTypes.Pawn(DomainSide.Black)),
                new DomainPosition(9, 6, pieceTypes.Pawn(DomainSide.Black)),

                new DomainPosition(0, 7, pieceTypes.Dragon(DomainSide.Black)),
                new DomainPosition(1, 7, pieceTypes.Rook(DomainSide.Black)),
                new DomainPosition(2, 7, pieceTypes.Knight(DomainSide.Black)),
                new DomainPosition(3, 7, pieceTypes.Bishop(DomainSide.Black)),
                new DomainPosition(4, 7, pieceTypes.Queen(DomainSide.Black)),
                new DomainPosition(5, 7, pieceTypes.King(DomainSide.Black)),
                new DomainPosition(6, 7, pieceTypes.Bishop(DomainSide.Black)),
                new DomainPosition(7, 7, pieceTypes.Knight(DomainSide.Black)),
                new DomainPosition(8, 7, pieceTypes.Rook(DomainSide.Black)),
                new DomainPosition(9, 7, pieceTypes.Hydra(DomainSide.Black)),
            };

            PawnTransforms = new Dictionary<string, Func<DomainSide, DomainPiece>>();
            foreach (var itm in pieceTypes.Value)
            {
                if (itm.Value.IsPawnTransformAvailable)
                {
                    PawnTransforms.Add(itm.Key, (DomainSide side) => { return new DomainPiece(side, itm.Value); });
                }
            }
        }
    }
}
