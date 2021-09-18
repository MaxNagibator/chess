namespace Bg.Chess.Domain.Tests.GameTests
{
    using NUnit.Framework;

    public class ClassicGameTests
    {
        /// <summary>
        /// Простой тест, что мы умеем ходить.
        /// </summary>
        /// <remarks>
        /// Slav Defense
        /// Славянская защита относится к закрытым началам 
        /// и возникает на доске после ходов 
        /// 1.d2->d4 d7->d5 
        /// 2.c2->c4 c7->c6.
        /// Находит применение за черных в ответ на Ферзевый гамбит, 
        /// среди всех построений которого является самым часто встречающимся. 
        /// Стоит на прочном позиционном, но не лишенном тактического потенциала, 
        /// фундаменте и входит в дебютный репертуар преобладающего 
        /// большинства современных шахматистов высшего класса.</remarks>
        [Test]
        public void GameWithDefaultRulesTest()
        {
            var game = new Game();
            game.Init();
            game.Move(Side.White, "d2", "d4");
            game.Move(Side.Black, "d7", "d5");
            game.Move(Side.White, "c2", "c4");
            game.Move(Side.Black, "c7", "c6");
        }

        /// <summary>
        /// Детский мат.
        /// </summary>
        /// <remarks>
        /// это мат, который получается, например, после ходов белых 1. e4 2. Фh5 3. Сс4 4. Ф:f7×. 
        /// 1. e2->e4 e7->e5
        /// 2. d1->h5 b8->c6
        /// 3. f1->c4 g8->f6
        /// 4. h5->f7 mate!
        /// Фигуры могут двигаться в разной последовательности, но основная идея — ферзь и слон атакуют поле f7. 
        /// Классический детский мат ставится чёрным, но может ставиться и белым на поле f2[1].
        /// </remarks>
        [Test]
        public void MateTest()
        {
            var game = new Game();
            game.Init();
            game.Move(Side.White, "e2", "e4");
            game.Move(Side.Black, "e7", "e5");
            game.Move(Side.White, "d1", "h5");
            game.Move(Side.Black, "b8", "c6");
            game.Move(Side.White, "f1", "c4");
            game.Move(Side.Black, "g8", "f6");
            game.Move(Side.White, "h5", "f7");
            Assert.AreEqual(GameState.Finish, game.State);
            Assert.AreEqual(Side.White, game.WinSide);
            Assert.AreEqual(FinishReason.Mate, game.FinishReason);
        }
    }
}