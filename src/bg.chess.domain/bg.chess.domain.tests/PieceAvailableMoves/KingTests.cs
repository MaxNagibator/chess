using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Bg.Chess.Domain.PieceAvailableMoves
{
    /// <summary>
    /// Набор тестов на доступные ходы короля.
    /// </summary>
    public class KingTests
    {
        /// <summary>
        /// Проверка короля на пустой доске.
        /// </summary>
        /// <remarks>
        /// В углах король имеет 3 хода.
        /// Касаясь одного из краёв поля 5 ходов.
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
            var piece = new King(Side.White);
            rules.Positions = new List<Position> { new Position(x, y, piece) };

            var field = new Field(rules);

            var moves = field[x, y].GetAvailableMoves();
            Assert.AreEqual(movesCount, moves.Count);
        }

        /// <summary>
        /// Король в центре, но на клетках куда он может сходить, есть две фигуры
        /// </summary>
        /// <remarks>
        /// Вражеский конь сверху блокирует две клетки для хода вниз.
        /// Вражеский конь левее и ниже блокирует клетку хода вверх.
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
                new Position(4, 5, new King(Side.White)),
                new Position(4, 6, new Knight(right == 1 ? Side.White : Side.Black)),
                new Position(3, 4, new Knight(leftDown == 1 ? Side.White : Side.Black))
            };

            var field = new Field(rules);
            var moves = field[4, 5].GetAvailableMoves();
            Assert.AreEqual(availableMoves, moves.Count);
        }

        /// <summary>
        /// Королю ничего не угрожает и доступны обе рокировки.
        /// </summary>
        /// <remarks>
        /// Король и две ладьи на своих местах, никто не двигался.
        /// У короля так же доступно 5 ходов, помимо рокировки.
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
                new Position(0, lineIndex, new Rook(side)),
                new Position(4, lineIndex, new King(side)),
                new Position(7, lineIndex, new Rook(side)),
            };

            var field = new Field(rules);

            var moves = field[4, lineIndex].GetAvailableMoves();
            Assert.AreEqual(5 + 2, moves.Count);
            Assert.AreEqual(true, moves.Any(x => x.X == 2));
            Assert.AreEqual(true, moves.Any(x => x.X == 6));
        }

        /// <summary>
        /// Король имеет атакуемое поле с одной из сторон рокировки.
        /// </summary>
        /// <remarks>
        /// Король и две ладьи на своих местах, никто не двигался.
        /// У короля так же доступно 3 ходов, помимо 1 рокировки.
        /// Рокировка с одной стороны будет под атакой и не доступна.
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
                new Position(0, lineIndex, new Rook(side)),
                new Position(4, lineIndex, new King(side)),
                new Position(7, lineIndex, new Rook(side)),

                new Position(attackVerticalLine, attackLineIndex, new Rook(side.Invert())),
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
        /// Король под шахом не может рокироваться.
        /// </summary>
        /// <remarks>
        /// Король и две ладьи на своих местах, никто не двигался.
        /// Королю остаётся только уйти от шаха 4 хода в стороны.
        /// </remarks>
        [Test]
        [TestCase(Side.White)]
        [TestCase(Side.Black)]
        public void KingCastlingWithShahTest(Side side)
        {
            var rules = new ClassicRules();
            rules.FieldWidth = 8;
            rules.FieldHeight = 8;
            var lineIndex = side == Side.White ? 0 : 7;
            var attackLineIndex = side == Side.White ? 7 : 0;
            rules.Positions = new List<Position>
            {
                new Position(0, lineIndex, new Rook(side)),
                new Position(4, lineIndex, new King(side)),
                new Position(7, lineIndex, new Rook(side)),

                new Position(4, attackLineIndex, new Rook(side.Invert())),
            };

            var field = new Field(rules);

            var moves = field[4, lineIndex].GetAvailableMoves();
            Assert.AreEqual(4, moves.Count);
        }

        /// <summary>
        /// Кони мешают делать рокировки
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
                new Position(0, lineIndex, new Rook(side)),
                new Position(1, lineIndex, new Knight(side)),
                new Position(4, lineIndex, new King(side)),
                new Position(6, lineIndex, new Knight(side)),
                new Position(7, lineIndex, new Rook(side)),
            };

            var field = new Field(rules);

            var moves = field[4, lineIndex].GetAvailableMoves();
            Assert.AreEqual(5, moves.Count);
        }

        /// <summary>
        /// Король хочет срубить пешку, но её прикрывает другая пешка, поэтому он не может.
        /// </summary>
        /// <remarks>
        /// Король не может срубить пешку по диагонали, но может уйти вверх или вправо.
        /// </remarks>
        [Test]
        public void KingDontKillPawnTest()
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(0, 0, new King(Side.White)),
                new Position(1, 1, new Pawn(Side.Black)),
                new Position(2, 2, new Pawn(Side.Black))
            };

            var field = new Field(rules);

            var moves = field[0, 0].GetAvailableMoves();
            Assert.AreEqual(2, moves.Count);
            Assert.AreEqual(true, moves.Any(x=>x.X == 0 && x.Y == 1));
            Assert.AreEqual(true, moves.Any(x=>x.X == 1 && x.Y == 0));
        }

        /// <summary>
        /// Король хочет срубить коня, но её прикрывает пешка, поэтому он не может.
        /// </summary>
        /// <remarks>
        /// Король не может срубить коня справа, но может уйти вверх или вправо.
        /// </remarks>
        [Test]
        public void KingDontKillKnightTest()
        {
            var rules = new ClassicRules();
            rules.Positions = new List<Position>
            {
                new Position(0, 1, new King(Side.White)),
                new Position(1, 1, new Knight(Side.Black)),
                new Position(2, 2, new Pawn(Side.Black))
            };

            var field = new Field(rules);

            var moves = field[0, 0].GetAvailableMoves();
            Assert.AreEqual(4, moves.Count);
            Assert.AreEqual(true, moves.Any(x => x.X == 0 && x.Y == 0));
            Assert.AreEqual(true, moves.Any(x => x.X == 1 && x.Y == 0));
            Assert.AreEqual(true, moves.Any(x => x.X == 0 && x.Y == 2));
            Assert.AreEqual(true, moves.Any(x => x.X == 1 && x.Y == 2));
        }
    }
}