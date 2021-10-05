namespace Bg.Chess.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Bg.Chess.Domain;

    public class PieceTypes
    {
        public PieceTypes()
        {
            Value = Bg.Chess.Domain.PieceBuilder.PieceTypes;

            var _pieceTypes = new Dictionary<string, PieceType>();
            var ourtype = typeof(PieceType);

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type itm in a.GetTypes().Where(type => type.IsSubclassOf(ourtype)))
                {
                    var type = (PieceType)Activator.CreateInstance(itm);
                    _pieceTypes.Add(type.Name, type);
                }
            }

            Value = _pieceTypes;
        }

        public Bg.Chess.Domain.Piece King(Side side)
        {
            return new Bg.Chess.Domain.Piece(side, Value["king"]);
        }
        public Bg.Chess.Domain.Piece Bishop(Side side)
        {
            return new Bg.Chess.Domain.Piece(side, Value["bishop"]);
        }
        public Bg.Chess.Domain.Piece Knight(Side side)
        {
            return new Bg.Chess.Domain.Piece(side, Value["knight"]);
        }
        public Bg.Chess.Domain.Piece Pawn(Side side)
        {
            return new Bg.Chess.Domain.Piece(side, Value["pawn"]);
        }
        public Bg.Chess.Domain.Piece Queen(Side side)
        {
            return new Bg.Chess.Domain.Piece(side, Value["queen"]);
        }
        public Bg.Chess.Domain.Piece Rook(Side side)
        {
            return new Bg.Chess.Domain.Piece(side, Value["rook"]);
        }
        public Bg.Chess.Domain.Piece Dragon(Side side)
        {
            return new Bg.Chess.Domain.Piece(side, Value["dragon"]);
        }
        public Bg.Chess.Domain.Piece Soldier(Side side)
        {
            return new Bg.Chess.Domain.Piece(side, Value["soldier"]);
        }
        public Bg.Chess.Domain.Piece Hydra(Side side)
        {
            return new Bg.Chess.Domain.Piece(side, Value["hydra"]);
        }

        public Dictionary<string, Bg.Chess.Domain.PieceType> Value { get; }
    }
}
