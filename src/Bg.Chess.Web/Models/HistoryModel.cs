
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

            public Player WhitePlayer { get; set; }

            public Player BlackPlayer { get; set; }

            public GameStatus Status { get; set; }
        }

        public class Player
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
