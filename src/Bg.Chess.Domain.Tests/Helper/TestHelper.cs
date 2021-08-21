namespace Bg.Chess.Domain.Tests
{
    using System;

    public static class TestHelper
    {
        /// <summary>
        /// Сдвинуть в лево.
        /// </summary>
        public static int Shift(this int value, int shiftValue)
        {
            var pos = value - shiftValue;
            if (pos < 0)
            {
                pos = 8 + pos;
            }

            return pos;
        }

        /// <summary>
        /// Отзеркалить.
        /// </summary>
        public static int Invert(this int value)
        {
            return 7 - value;
        }
    }
}
