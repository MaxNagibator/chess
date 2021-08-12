using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace Bg.Chess.Domain.Tests
{
    public class GameTests
    {
        /// <summary>
        /// Простой тест, что мы умеем ходить
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

            // ну славянский дебют готов, можно и курнуть, скор вернусь
            //покурили щас дальше попрём

        }
    }
}