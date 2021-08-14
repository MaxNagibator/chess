using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain.PieceMoves
{
    /// <summary>
    /// ����� ������ �� ������������ �����.
    /// </summary>
    public class PawnTests
    {
        /// <summary>
        /// ������� ���� �� �������� ������ ����� ��� ���� � ���� ����
        /// </summary>
        [Test]
        public void PawnMoveWithAtackTest()
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(1, 1, new Pawn(Side.White)),
                new Position(2, 5, new Knight(Side.Black))
            };

            var field = new Field(rules);

            field.Move(Side.White, 1, 1, 1, 2);
            field.Move(Side.White, 1, 2, 1, 3);
            field.Move(Side.White, 1, 3, 1, 4);
            field.Move(Side.White, 1, 4, 2, 5);

            Assert.AreEqual(Side.White, field[2, 5].Piece.Side);
        }

        /// <summary>
        /// �����, ����� ���� ��� �������, �� ����� ������ �� ��� ������ �����.
        /// </summary>
        [Test]
        public void PawnAfterMoveCanNotTwoFieldMoveTest()
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(1, 1, new Pawn(Side.White)),
            };

            var field = new Field(rules);

            field.Move(Side.White, 1, 1, 1, 2);
            var moves = field[1, 2].GetAvailableMoves();

            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(1, moves[0].X);
            Assert.AreEqual(3, moves[0].Y);
        }

        /// <summary>
        /// ����� �� ������ ������ �� ��� ������, ���� ����� ��� ������.
        /// </summary>
        [Test]
        public void PawnThroughKnightCanNotTwoFieldMoveTest()
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(1, 1, new Pawn(Side.White)),
                new Position(1, 2, new Knight(Side.White)),
            };

            var field = new Field(rules);
            var moves = field[1, 1].GetAvailableMoves();

            Assert.AreEqual(0, moves.Count);
        }

        /// <summary>
        /// ����� ������ � �������� ������ ������
        /// </summary>
        [Test]
        [TestCase("queen", typeof(Queen))]
        [TestCase("rook", typeof(Rook))]
        [TestCase("knight", typeof(Knight))]
        [TestCase("bishop", typeof(Bishop))]
        public void PawnTransformToQueen(string name, Type pieceType)
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(1, 6, new Pawn(Side.White))
            };

            var field = new Field(rules);

            field.Move(Side.White, 1, 6, 1, 7, name);

            Assert.AreEqual(Side.White, field[1, 7].Piece.Side);
            Assert.AreEqual(pieceType, field[1, 7].Piece.GetType());
        }

        /// <summary>
        /// ������ ��������� ����� �� �������.
        /// </summary>
        /// <remarks>
        /// 1 - ����� ��������� ������ �������
        /// -1 - ����� ��������� ����� �������
        /// </remarks>
        [Test]
        [TestCase(1)]
        [TestCase(-1)]
        public void PawnEnPassantMoveTest(int shift)
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(4, 6, new Pawn(Side.Black)),
                new Position(4 + shift, 4, new Pawn(Side.White)),
                new Position(4 + shift, 5, new Knight(Side.White)),
            };

            var field = new Field(rules);

            field.Move(Side.Black, 4, 6, 4, 4);
            var moves = field[4 + shift, 4].GetAvailableMoves();

            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(4, moves[0].X);
            Assert.AreEqual(5, moves[0].Y);

            field.Move(Side.White, 4 + shift, 4, 4, 5);
            Assert.IsNull(field[4, 4].Piece, "��������� ������ �������� �� ����");
        }
    }
}