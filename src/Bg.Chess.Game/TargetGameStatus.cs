namespace Bg.Chess.Game
{
    using Bg.Chess.Common.Enums;

    public class TargetGameStatusWithOpponent
    {
        public Player OpponentPlayer { get; set; }
        public TargetGameConfirmStatus Status { get; set; }
    }
}
