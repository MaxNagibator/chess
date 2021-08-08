namespace bg.chess.domain
{
    using System.Collections.Generic;

    /// <summary>
    /// Правила игры.
    /// </summary>
    public class Rules
    {
        /// <summary>
        /// Ширина поля.
        /// </summary>
        public int FieldWidth = 8;

        /// <summary>
        /// Высота поля.
        /// </summary>
        public int FieldHeight = 8;

        /// <summary>
        /// Расположение фигур
        /// </summary>
        public List<Position> positions = new List<Position>
        {
            new Position(0, 0, new Rook(Side.White)),
            new Position(1, 0, new Knight(Side.White)),
            new Position(2, 0, new Bishop(Side.White)),
            new Position(3, 0, new Queen(Side.White)),
            new Position(4, 0, new King(Side.White)),
            new Position(5, 0, new Bishop(Side.White)),
            new Position(6, 0, new Knight(Side.White)),
            new Position(7, 0, new Rook(Side.White)),
            new Position(0, 1, new Pawn(Side.White)),
            new Position(1, 1, new Pawn(Side.White)),
            new Position(2, 1, new Pawn(Side.White)),
            new Position(3, 1, new Pawn(Side.White)),
            new Position(4, 1, new Pawn(Side.White)),
            new Position(5, 1, new Pawn(Side.White)),
            new Position(6, 1, new Pawn(Side.White)),
            new Position(7, 1, new Pawn(Side.White)),

            new Position(0, 7, new Rook(Side.Black)),
            new Position(1, 7, new Knight(Side.Black)),
            new Position(2, 7, new Bishop(Side.Black)),
            new Position(3, 7, new Queen(Side.Black)),
            new Position(4, 7, new King(Side.Black)),
            new Position(5, 7, new Bishop(Side.Black)),
            new Position(6, 7, new Knight(Side.Black)),
            new Position(7, 7, new Rook(Side.Black)),
            new Position(0, 6, new Pawn(Side.Black)),
            new Position(1, 6, new Pawn(Side.Black)),
            new Position(2, 6, new Pawn(Side.Black)),
            new Position(3, 6, new Pawn(Side.Black)),
            new Position(4, 6, new Pawn(Side.Black)),
            new Position(5, 6, new Pawn(Side.Black)),
            new Position(6, 6, new Pawn(Side.Black)),
            new Position(7, 6, new Pawn(Side.Black)),
        };
    }
}
