using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain.Tests.PieceAvailableMoves
{
    /// <summary>
    /// ����� ������ �� ��������� ���� ������.
    /// </summary>
    public class KingTests
    {
        /// <summary>
        /// �������� ������ �� ������ �����.
        /// </summary>
        /// <remarks>
        /// � ����� ������ ����� 3 ����.
        /// ������� ������ �� ���� ���� 5 �����.
        /// </remarks>
        [Test]
        [TestCase(0, 0, 3)]
        [TestCase(0, 7, 3)]
        [TestCase(7, 0, 3)]
        [TestCase(7, 7, 3)]

        [TestCase(0, 1, 5)]
        [TestCase(1, 0, 5)]
        [TestCase(7, 1, 5)]
        [TestCase(1, 7, 5)]

        [TestCase(2, 2, 8)]
        [TestCase(4, 4, 8)]
        [TestCase(5, 5, 8)]
        [TestCase(4, 5, 8)]
        public void KingDefaultTest(int x, int y, int movesCount)
        {
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            var piece = PieceBuilder.King(Side.White);
            rules.Positions = new List<Position> { new Position(x, y, piece) };

            var field = new Field(rules);

            var moves = field[x, y].GetAvailableMoves();
            Assert.AreEqual(movesCount, moves.Count);
        }

        /// <summary>
        /// ������ � ������, �� �� ������� ���� �� ����� �������, ���� ��� ������
        /// </summary>
        /// <remarks>
        /// ��������� ���� ������ ��������� ��� ������ ��� ���� ����.
        /// ��������� ���� ����� � ���� ��������� ������ ���� �����.
        /// </remarks>
        [Test]
        [TestCase(1, 1, 6)]
        [TestCase(1, -1, 6)]
        [TestCase(-1, 1, 6)]
        [TestCase(-1, -1, 4)]
        public void KingWithTeamMateTest(int right, int leftDown, int availableMoves)
        {
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            rules.Positions = new List<Position>
            {
                new Position(4, 5, PieceBuilder.King(Side.White)),
                new Position(4, 6, PieceBuilder.Knight(right == 1 ? Side.White : Side.Black)),
                new Position(3, 4, PieceBuilder.Knight(leftDown == 1 ? Side.White : Side.Black)),
                new Position(0, 0, PieceBuilder.King(Side.Black)),
            };

            var field = new Field(rules);
            var moves = field[4, 5].GetAvailableMoves();
            Assert.AreEqual(availableMoves, moves.Count);
        }

        /// <summary>
        /// ������ ������ �� �������� � �������� ��� ���������.
        /// </summary>
        /// <remarks>
        /// ������ � ��� ����� �� ����� ������, ����� �� ��������.
        /// � ������ ��� �� �������� 5 �����, ������ ���������.
        /// </remarks>
        [Test]
        [TestCase(Side.White)]
        [TestCase(Side.Black)]
        public void KingCastlingTest(Side side)
        {
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            var lineIndex = side == Side.White ? 0 : 7;
            rules.Positions = new List<Position>
            {                
                new Position(0, lineIndex, PieceBuilder.Rook(side)),
                new Position(4, lineIndex, PieceBuilder.King(side)),
                new Position(7, lineIndex, PieceBuilder.Rook(side)),
                new Position(4, lineIndex.Invert(), PieceBuilder.King(side.Invert())),
            };

            var field = new Field(rules);

            var moves = field[4, lineIndex].GetAvailableMoves();
            Assert.AreEqual(5 + 2, moves.Count);
            Assert.AreEqual(true, moves.Any(x => x.X == 2));
            Assert.AreEqual(true, moves.Any(x => x.X == 6));
        }

        /// <summary>
        /// ������ ����� ��������� ���� � ����� �� ������ ���������.
        /// </summary>
        /// <remarks>
        /// ������ � ��� ����� �� ����� ������, ����� �� ��������.
        /// � ������ ��� �� �������� 3 �����, ������ 1 ���������.
        /// ��������� � ����� ������� ����� ��� ������ � �� ��������.
        /// </remarks>
        [Test]
        [TestCase(Side.White, 3)]
        [TestCase(Side.Black, 3)]
        [TestCase(Side.White, 5)]
        [TestCase(Side.Black, 5)]
        public void KingCastlingWithAttackTest(Side side, int attackVerticalLine)
        {
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            var lineIndex = side == Side.White ? 0 : 7;
            var attackLineIndex = side == Side.White ? 7 : 0;
            rules.Positions = new List<Position>
            {
                new Position(0, lineIndex, PieceBuilder.Rook(side)),
                new Position(4, lineIndex, PieceBuilder.King(side)),
                new Position(7, lineIndex, PieceBuilder.Rook(side)),

                new Position(4, attackLineIndex, PieceBuilder.King(side.Invert())),
                new Position(attackVerticalLine, attackLineIndex, PieceBuilder.Rook(side.Invert())),
            };

            var field = new Field(rules);

            var moves = field[4, lineIndex].GetAvailableMoves();
            Assert.AreEqual(3 + 1, moves.Count);
            if (attackVerticalLine > 4)
            { 
                Assert.AreEqual(true, moves.Any(x => x.X == 2));
            }
            else
            {
                Assert.AreEqual(true, moves.Any(x => x.X == 6));
            }
        }


        /// <summary>
        /// ������ ��� ����� �� ����� ������������.
        /// </summary>
        /// <remarks>
        /// ������ � ��� ����� �� ����� ������, ����� �� ��������.
        /// ������ ������� ������ ���� �� ���� 4 ���� � �������.
        /// </remarks>
        [Test]
        [TestCase(Side.White)]
        [TestCase(Side.Black)]
        public void KingCastlingWithKingAlertTest(Side side)
        {
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            var lineIndex = side == Side.White ? 0 : 7;
            var attackLineIndex = side == Side.White ? 7 : 0;
            rules.Positions = new List<Position>
            {
                new Position(0, lineIndex, PieceBuilder.Rook(side)),
                new Position(4, lineIndex, PieceBuilder.King(side)),
                new Position(7, lineIndex, PieceBuilder.Rook(side)),

                new Position(5, attackLineIndex, PieceBuilder.King(side.Invert())),
                new Position(4, attackLineIndex, PieceBuilder.Rook(side.Invert())),
            };

            var field = new Field(rules);

            var moves = field[4, lineIndex].GetAvailableMoves();
            Assert.AreEqual(4, moves.Count);
        }

        /// <summary>
        /// ���� ������ ������ ���������
        /// </summary>
        [Test]
        [TestCase(Side.White)]
        [TestCase(Side.Black)]
        public void KingCastlingBlockedByTeammateTest(Side side)
        {
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            var lineIndex = side == Side.White ? 0 : 7;
            rules.Positions = new List<Position>
            {
                new Position(0, lineIndex, PieceBuilder.Rook(side)),
                new Position(1, lineIndex, PieceBuilder.Knight(side)),
                new Position(4, lineIndex, PieceBuilder.King(side)),
                new Position(6, lineIndex, PieceBuilder.Knight(side)),
                new Position(7, lineIndex, PieceBuilder.Rook(side)),
            };

            var field = new Field(rules);

            var moves = field[4, lineIndex].GetAvailableMoves();
            Assert.AreEqual(5, moves.Count);
        }

        /// <summary>
        /// ������ ������ ������ ��������� ������� ������.
        /// </summary>
        /// <param name="side">������ �� 4, ������ ������ �� 6 � �� ������ ����/����, ����� ������� �� 7.</param>
        [Test]
        [TestCase(Side.White)]
        [TestCase(Side.Black)]
        public void KingCastlingBlockedByKingTest(Side side)
        {
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            var lineIndex = side == Side.White ? 0 : 7;
            rules.Positions = new List<Position>
            {
                new Position(4, lineIndex, PieceBuilder.King(side)),
                new Position(7, lineIndex, PieceBuilder.Rook(side)),
                new Position(6, lineIndex, PieceBuilder.King(side.Invert())),
            };

            var field = new Field(rules);

            var moves = field[4, lineIndex].GetAvailableMoves();
            Assert.AreEqual(3, moves.Count);
        }

        /// <summary>
        /// ������ ����� ������� �����, �� � ���������� ������ �����, ������� �� �� �����.
        /// </summary>
        /// <remarks>
        /// ������ �� ����� ������� ����� �� ���������, �� ����� ���� ����� ��� ������.
        /// </remarks>
        [Test]
        public void KingDontKillPawnTest()
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(0, 0, PieceBuilder.King(Side.White)),
                new Position(1, 1, PieceBuilder.Pawn(Side.Black)),
                new Position(2, 2, PieceBuilder.Pawn(Side.Black)),
                new Position(7, 7, PieceBuilder.King(Side.Black)),
            };

            var field = new Field(rules);

            var moves = field[0, 0].GetAvailableMoves();
            Assert.AreEqual(2, moves.Count);
            Assert.AreEqual(true, moves.Any(x=>x.X == 0 && x.Y == 1));
            Assert.AreEqual(true, moves.Any(x=>x.X == 1 && x.Y == 0));
        }

        /// <summary>
        /// ������ ����� ������� ����, �� � ���������� �����, ������� �� �� �����.
        /// </summary>
        /// <remarks>
        /// ������ �� ����� ������� ���� ������, �� ����� ���� ����� ��� ������.
        /// </remarks>
        [Test]
        public void KingDontKillKnightTest()
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(0, 1, PieceBuilder.King(Side.White)),
                new Position(1, 1, PieceBuilder.Knight(Side.Black)),
                new Position(2, 2, PieceBuilder.Pawn(Side.Black)),
                new Position(7, 7, PieceBuilder.King(Side.Black)),
            };

            var field = new Field(rules);

            var moves = field[0, 1].GetAvailableMoves();
            Assert.AreEqual(4, moves.Count);
            Assert.AreEqual(true, moves.Any(x => x.X == 0 && x.Y == 0));
            Assert.AreEqual(true, moves.Any(x => x.X == 1 && x.Y == 0));
            Assert.AreEqual(true, moves.Any(x => x.X == 0 && x.Y == 2));
            Assert.AreEqual(true, moves.Any(x => x.X == 1 && x.Y == 2));
        }
    }
}