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
        public DragonRules() : base()
        {
            FieldWidth = 10;
            FieldHeight = 8;

            // todo ну чёто такое себе выглядит, надо будет придумать чтото покруче
            var types = PieceBuilder.PieceTypes;
            var hidra = new Hydra();
            var soldier = new Soldier();
            var dragon = new Dragon();
            if(!types.ContainsKey(hidra.Name))
            {
                types.Add(hidra.Name, hidra);
                types.Add(soldier.Name, soldier);
                types.Add(dragon.Name, dragon);
            }

            Positions = new List<DomainPosition>
            {
                new DomainPosition(0, 0, new DomainPiece(DomainSide.White, PieceBuilder.PieceTypes[hidra.Name])),
                new DomainPosition(1, 0, PieceBuilder.Rook(DomainSide.White)),
                new DomainPosition(2, 0, PieceBuilder.Knight(DomainSide.White)),
                new DomainPosition(3, 0, PieceBuilder.Bishop(DomainSide.White)),
                new DomainPosition(4, 0, PieceBuilder.Queen(DomainSide.White)),
                new DomainPosition(5, 0, PieceBuilder.King(DomainSide.White)),
                new DomainPosition(6, 0, PieceBuilder.Bishop(DomainSide.White)),
                new DomainPosition(7, 0, PieceBuilder.Knight(DomainSide.White)),
                new DomainPosition(8, 0, PieceBuilder.Rook(DomainSide.White)),
                new DomainPosition(9, 0, new DomainPiece(DomainSide.White, PieceBuilder.PieceTypes[dragon.Name])),

                new DomainPosition(0, 1, PieceBuilder.Pawn(DomainSide.White)),
                new DomainPosition(1, 1, PieceBuilder.Pawn(DomainSide.White)),
                new DomainPosition(2, 1, PieceBuilder.Pawn(DomainSide.White)),
                new DomainPosition(3, 1, new DomainPiece(DomainSide.White, PieceBuilder.PieceTypes[soldier.Name])),
                new DomainPosition(4, 1, PieceBuilder.Pawn(DomainSide.White)),
                new DomainPosition(5, 1, PieceBuilder.Pawn(DomainSide.White)),
                new DomainPosition(6, 1, new DomainPiece(DomainSide.White, PieceBuilder.PieceTypes[soldier.Name])),
                new DomainPosition(7, 1, PieceBuilder.Pawn(DomainSide.White)),
                new DomainPosition(8, 1, PieceBuilder.Pawn(DomainSide.White)),
                new DomainPosition(9, 1, PieceBuilder.Pawn(DomainSide.White)),

                new DomainPosition(0, 6, PieceBuilder.Pawn(DomainSide.Black)),
                new DomainPosition(1, 6, PieceBuilder.Pawn(DomainSide.Black)),
                new DomainPosition(2, 6, PieceBuilder.Pawn(DomainSide.Black)),
                new DomainPosition(3, 6, new DomainPiece(DomainSide.Black, PieceBuilder.PieceTypes[soldier.Name])),
                new DomainPosition(4, 6, PieceBuilder.Pawn(DomainSide.Black)),
                new DomainPosition(5, 6, PieceBuilder.Pawn(DomainSide.Black)),
                new DomainPosition(6, 6, new DomainPiece(DomainSide.Black, PieceBuilder.PieceTypes[soldier.Name])),
                new DomainPosition(7, 6, PieceBuilder.Pawn(DomainSide.Black)),
                new DomainPosition(8, 6, PieceBuilder.Pawn(DomainSide.Black)),
                new DomainPosition(9, 6, PieceBuilder.Pawn(DomainSide.Black)),

                new DomainPosition(0, 7, new DomainPiece(DomainSide.Black, PieceBuilder.PieceTypes[dragon.Name])),
                new DomainPosition(1, 7, PieceBuilder.Rook(DomainSide.Black)),
                new DomainPosition(2, 7, PieceBuilder.Knight(DomainSide.Black)),
                new DomainPosition(3, 7, PieceBuilder.Bishop(DomainSide.Black)),
                new DomainPosition(4, 7, PieceBuilder.Queen(DomainSide.Black)),
                new DomainPosition(5, 7, PieceBuilder.King(DomainSide.Black)),
                new DomainPosition(6, 7, PieceBuilder.Bishop(DomainSide.Black)),
                new DomainPosition(7, 7, PieceBuilder.Knight(DomainSide.Black)),
                new DomainPosition(8, 7, PieceBuilder.Rook(DomainSide.Black)),
                new DomainPosition(9, 7, new DomainPiece(DomainSide.Black, PieceBuilder.PieceTypes[hidra.Name])),
            };

            PawnTransforms = new Dictionary<string, Func<DomainSide, DomainPiece>>();
            foreach (var itm in PieceBuilder.PieceTypes)
            {
                if (itm.Value.IsPawnTransformAvailable)
                {
                    PawnTransforms.Add(itm.Key, (DomainSide side) => { return new DomainPiece(side, itm.Value); });
                }
            }
        }
    }
}
