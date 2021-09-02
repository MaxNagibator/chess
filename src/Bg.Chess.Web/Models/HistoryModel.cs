
namespace Bg.Chess.Web.Models
{
    using Bg.Chess.Common.Enums;
    using System;
    using System.Collections.Generic;

    public class HistoryModel
    {
        public List<Game> Games { get; set; }
        public int MyPlayerId { get;  set; }

        public class Game
        {
            public int Id { get; set; }

            public int WhitePlayerId { get; set; }

            public int BlackPlayerId { get; set; }

            public GameStatus Status { get; set; }
        }
    }
}
