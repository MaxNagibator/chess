using Bg.Chess.Common.Enums;

using System.Collections.Generic;

namespace Bg.Chess.Web.Models
{
    public class HistoryModel
    {
        public List<Game> Games { get; set; }
        public int MyPlayerId { get;  set; }

        public class Game
        {
            public int Id { get; set; }

            public Player WhitePlayer { get; set; }

            public Player BlackPlayer { get; set; }

            public FinishReason FinishReason { get; set; }

            public GameSide? WinSide { get; set; }
        }

        public class Player
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
