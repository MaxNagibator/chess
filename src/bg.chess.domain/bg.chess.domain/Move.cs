using System;
using System.Collections.Generic;

namespace Bg.Chess.Domain
{
    /// <summary>
    /// Ход.
    /// </summary>
    public class Move
    {
        /// <summary>
        /// Откуда.
        /// </summary>
        public Position From { get; set; }

        /// <summary>
        /// Куда.
        /// </summary>
        public Position To { get; set; }

        /// <summary>
        /// Кто ходил.
        /// </summary>
        public Piece Runner { get; set; }

        /// <summary>
        /// Если после хода умерла вражеская фигура.
        /// </summary>
        public Piece KillEnemy { get; set; }

        /// <summary>
        /// При ходе короля, двигалась и ладья, но это один.
        /// </summary>
        public Move AdditionalMove { get; set; }

        /// <summary>
        /// Конструтор хода
        /// </summary>
        /// <param name="runner">Кто.</param>
        /// <param name="from">Откуда.</param>
        /// <param name="to">Куда.</param>
        public Move(Piece runner, Position from, Position to)
        {
            Runner = runner;
            From = from;
            To = to;
        }
    }
}