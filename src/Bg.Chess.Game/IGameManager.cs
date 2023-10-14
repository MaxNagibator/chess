namespace Bg.Chess.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bg.Chess.Common.Enums;

    using Microsoft.Extensions.Logging;

    public interface IGameManager
    {
        bool IsInit { get; }
        void Init(List<IGameInfo> games);
        void StartSearch(Player player, GameMode gameMode);
        void StopSearch(int playerId);
        RatingSearchStatus Check(int playerId);
        RatingSearchStatus Confirm(int playerId);
        IGameInfo FindMyPlayingGame(int playerId);

        void StartSearchTargetGame(Player player, Player targetPlayer, GameMode gameMode);
        void StopSearchTargetGame(int playerId);
        TargetGameConfirmStatus CheckTargetGame(int playerId);
        TargetGameConfirmStatus ConfirmTargetGame(int playerId);
    }

    public class GameManager : IGameManager
    {
        private ILogger _logger;
        private PieceTypes _pieceTypes;
        private List<IGameInfo> _games;

        public GameManager(PieceTypes pieceTypes, ILoggerFactory loggerFactory)
        {
            _pieceTypes = pieceTypes;
            _logger = loggerFactory.CreateLogger("chess");
        }

        private class Search
        {
            public Player Player { get; set; }
            public RatingSearchStatus Status { get; set; }
            public string GameId { get; set; }
            public DateTime? GameStart { get; set; }
            public GameMode GameMode { get; set; }
        }

        private class TargetGame
        {
            public Player Player { get; set; }
            public Player TargetPlayer { get; set; }
            public TargetGameConfirmStatus Status { get; set; }
            public string GameId { get; set; }
            public DateTime? GameStart { get; set; }
            public GameMode GameMode { get; set; }
        }

        private object lockSearchList = new object();

        private List<Search> ratingSearchList = new List<Search>();
        private List<TargetGame> targetGameList = new List<TargetGame>();

        public bool IsInit { get; private set; }

        public void Init(List<IGameInfo> games)
        {
            _games = games;
            IsInit = true;
        }


        public void StartSearch(Player player, GameMode gameMode)
        {
            _logger.LogInformation("Search Start [player=" + player.Id + "]");

            lock (lockSearchList)
            {
                var search = ratingSearchList.FirstOrDefault(x => x.Player.Id == player.Id && x.Status != RatingSearchStatus.Finish);
                if (search != null)
                {
                    _logger.LogInformation("Search in process");
                    return;
                }

                var gameInProcess = _games.Any(x => x.IsMyGame(player.Id) && !x.IsFinish);
                if (gameInProcess)
                {
                    throw new BusinessException("Существует незаконценная игра, поиск невозможен");
                }

                _logger.LogInformation("Search Start Success");

                search = new Search();
                search.Player = player;
                search.Status = RatingSearchStatus.InProcess;
                search.GameMode = gameMode;
                ratingSearchList.Insert(0, search);

                CheckPairPlayer();
            }
        }

        private void CheckPairPlayer()
        {
            var searchs = ratingSearchList.Where(x => x.Status == RatingSearchStatus.InProcess).GroupBy(x => x.GameMode).First();
            if (searchs.Count() == 2)
            {
                _logger.LogInformation("Search Finish");
                var gameId = DateTime.Now.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N");
                ratingSearchList[0].GameId = gameId;
                ratingSearchList[0].Status = RatingSearchStatus.NeedConfirm;
                ratingSearchList[1].GameId = gameId;
                ratingSearchList[1].Status = RatingSearchStatus.NeedConfirm;
            }
        }

        public RatingSearchStatus Check(int playerId)
        {
            _logger.LogInformation("Search Check [player=" + playerId + "]");
            var search = ratingSearchList.FirstOrDefault(x => x.Player.Id == playerId);
            if (search == null)
            {
                if (_games.Any(x => x.GameType == GameType.Ranked && x.IsFinish == false && x.IsMyGame(playerId)))
                {
                    return RatingSearchStatus.Finish;
                }
                return RatingSearchStatus.NotFound;
            }

            return search.Status;
        }

        public RatingSearchStatus Confirm(int playerId)
        {
            lock (lockSearchList)
            {
                _logger.LogInformation("Search Confirm [player=" + playerId + "]");
                var search = ratingSearchList.FirstOrDefault(x => x.Player.Id == playerId);
                if (search == null)
                {
                    throw new BusinessException("Поиск игры отсутствует");
                }

                if (search.Status == RatingSearchStatus.NeedConfirm)
                {
                    var twoSearch = ratingSearchList.First(x => x.GameId == search.GameId && x.Player.Id != search.Player.Id);
                    if (twoSearch.Status == RatingSearchStatus.NeedConfirmOpponent)
                    {
                        _logger.LogInformation("Two Player Confirm Game Start");
                        var gameStartDate = DateTime.Now;
                        search.GameStart = gameStartDate;
                        search.Status = RatingSearchStatus.Finish;
                        twoSearch.GameStart = gameStartDate;
                        twoSearch.Status = RatingSearchStatus.Finish;

                        GetWhiteBlackPlayer(ratingSearchList[0].Player, ratingSearchList[1].Player, out Player whitePlayer, out Player blackPlayer);

                        IGameInfo game = new GameInfo(_pieceTypes, search.GameId, GameType.Ranked, search.GameMode, whitePlayer, blackPlayer);
                        _games.Add(game);
                    }
                    else
                    {
                        _logger.LogInformation("Wait Game Opponent Confirm");
                        search.Status = RatingSearchStatus.NeedConfirmOpponent;
                    }
                }

                return search.Status;
            }
        }

        private void GetWhiteBlackPlayer(Player player1, Player player2, out Player whitePlayer, out Player blackPlayer)
        {
            if (DateTime.Now.Millisecond % 2 == 0)
            {
                whitePlayer = player2;
                blackPlayer = player1;
            }
            else
            {
                blackPlayer = player2;
                whitePlayer = player1;
            }
        }

        public void StopSearch(int playerId)
        {
            _logger.LogInformation("Search Stop [player=" + playerId + "]");
            var search = ratingSearchList.FirstOrDefault(x => x.Player.Id == playerId);
            if (search == null)
            {
                return;
            }
            if (search.Status == RatingSearchStatus.Finish)
            {
                throw new Exception("this status " + search.Status + " bad");
            }

            lock (lockSearchList)
            {
                var twoSearch = ratingSearchList.FirstOrDefault(x => x.GameId == search.GameId && x.Player.Id != search.Player.Id);
                if (twoSearch != null)
                {
                    twoSearch.Status = RatingSearchStatus.InProcess;
                }
                ratingSearchList.Remove(search);
            }
        }

        public IGameInfo FindMyPlayingGame(int playerId)
        {
            var gameInProcess = _games.LastOrDefault(x => x.IsMyGame(playerId));
            return gameInProcess;
        }

        public void StartSearchTargetGame(Player player, Player targetPlayer, GameMode gameMode)
        {
            lock (lockSearchList)
            {
                var search = ratingSearchList.FirstOrDefault(x => x.Player.Id == player.Id && x.Status != RatingSearchStatus.Finish);
                if (search != null)
                {
                    throw new BusinessException("Запущен рейтинговый поиск. Новый вызов невозможен.");
                }
                search = ratingSearchList.FirstOrDefault(x => x.Player.Id == targetPlayer.Id && x.Status != RatingSearchStatus.Finish);
                if (search != null)
                {
                    throw new BusinessException("У оппонента запущен рейтинговый поиск. Его вызов невозможен.");
                }

                var targetSearch = targetGameList.FirstOrDefault(x => x.Player.Id == player.Id && x.Status != TargetGameConfirmStatus.Finish);
                if (targetSearch != null)
                {
                    throw new BusinessException("Вызов уже брошен. Новый вызов невозможен.");
                }
                targetSearch = targetGameList.FirstOrDefault(x => x.Player.Id == targetPlayer.Id && x.Status != TargetGameConfirmStatus.Finish);
                if (targetSearch != null)
                {
                    throw new BusinessException("У оппонента брошен вызов. Его вызов невозможен.");
                }

                var gameInProcess = _games.Any(x => x.IsMyGame(player.Id) && !x.IsFinish);
                if (gameInProcess)
                {
                    throw new BusinessException("Существует незаконценная игра, ПВП невозможно");
                }
                gameInProcess = _games.Any(x => x.IsMyGame(targetPlayer.Id) && !x.IsFinish);
                if (gameInProcess)
                {
                    throw new BusinessException("Существует незаконценная игра, ПВП невозможно");
                }

                var target = new TargetGame();
                target.Player = player;
                target.TargetPlayer = targetPlayer;
                target.GameMode = gameMode;
                target.Status = TargetGameConfirmStatus.NeedConfirmOpponent;
                targetGameList.Add(target);
            }
        }

        public void StopSearchTargetGame(int playerId)
        {
            var search = targetGameList.FirstOrDefault(x => x.Player.Id == playerId || x.TargetPlayer.Id == playerId);
            if (search == null)
            {
                return;
            }
            if (search.Status == TargetGameConfirmStatus.Finish)
            {
                throw new Exception("this status " + search.Status + " bad");
            }

            lock (lockSearchList)
            {
                targetGameList.Remove(search);
            }
        }


        public TargetGameConfirmStatus ConfirmTargetGame(int playerId)
        {
            lock (lockSearchList)
            {
                var search = targetGameList.FirstOrDefault(x => x.Player.Id == playerId || x.TargetPlayer.Id == playerId);
                if (search == null)
                {
                    throw new BusinessException("Вызов не найден");
                }

                // если это мой вызов
                if (search.Player.Id == playerId)
                {
                    if (search.Status == TargetGameConfirmStatus.NeedConfirm)
                    {
                        search.Status = TargetGameConfirmStatus.Finish;
                        GetWhiteBlackPlayer(search.Player, search.TargetPlayer, out Player whitePlayer, out Player blackPlayer);
                        IGameInfo game = new GameInfo(_pieceTypes, search.GameId, GameType.Ranked, search.GameMode, whitePlayer, blackPlayer);
                        // если никто не сходил, то игра пропадает после перезапуска пула.
                        //_gameService.SaveGame(game);
                        _games.Add(game);
                    }
                    else
                    {
                        throw new Exception("this status " + search.Status + " bad");
                    }
                }
                else
                {
                    if (search.Status == TargetGameConfirmStatus.NeedConfirmOpponent)
                    {
                        search.Status = TargetGameConfirmStatus.NeedConfirm;
                    }
                    else
                    {
                        throw new Exception("this status " + search.Status + " bad");
                    }
                }

                return search.Status;
            }
        }

        public TargetGameConfirmStatus CheckTargetGame(int playerId)
        {
            var search = targetGameList.FirstOrDefault(x => x.Player.Id == playerId || x.TargetPlayer.Id == playerId);
            if (search == null)
            {
                if (_games.Any(x => x.GameType == GameType.Target && x.IsFinish == false && x.IsMyGame(playerId)))
                {
                    return TargetGameConfirmStatus.Finish;
                }
                return TargetGameConfirmStatus.NotFound;
            }

            if (search.TargetPlayer.Id == playerId)
            {
                // если это второй игрок справшивает статус вызова, то инвертируем ему статусы подтвержедния
                if (search.Status == TargetGameConfirmStatus.NeedConfirmOpponent)
                {
                    return TargetGameConfirmStatus.NeedConfirm;
                }
                if (search.Status == TargetGameConfirmStatus.NeedConfirm)
                {
                    return TargetGameConfirmStatus.NeedConfirmOpponent;
                }
            }

            return search.Status;
        }
    }
}
