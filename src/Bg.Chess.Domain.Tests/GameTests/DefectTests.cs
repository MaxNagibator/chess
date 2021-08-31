namespace Bg.Chess.Domain.Tests.GameTests
{
    using NUnit.Framework;

    public class DefectTests
    {
        /// <summary>
        /// Была ошибка с расчётом движения ферзя, после указанного дебюта, но воспроизвести не удалось
        /// </summary>
        [Test]
        public void QueenTroubleTest()
        {
            var game = new Game();
            game.Init();

            game.Move(Side.White, 4, 1, 4, 2);
            game.Move(Side.Black, 3, 6, 3, 4);
            game.Move(Side.White, 4, 2, 4, 3);
            game.Move(Side.Black, 3, 4, 4, 3);
            var moves = game.AvailableMoves();
            Assert.IsNotNull(moves);
        }
    }
}