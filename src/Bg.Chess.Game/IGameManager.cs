namespace Bg.Chess.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bg.Chess.Common.Enums;
    using Bg.Chess.Data.Repo;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    public interface IGameManager
    {
        bool IsInit { get; }
        void Init(List<IGameInfo> games);
        void StartSearch(int playerId);
        void StopSearch(int playerId);
        SearchStatus Check(int playerId);
        SearchStatus Confirm(int playerId);
        IGameInfo FindMyPlayingGame(int playerId);
    }

    public class GameManager : IGameManager
    {
        private ILogger _logger;
        private IGameRepo _gameRepo;
        private List<IGameInfo> _games;

        public GameManager(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("chess");
        }

        private class Search
        {
            public int PlayerId { get; set; }
            public SearchStatus Status { get; set; }

            /// <summary>
            /// В случае когда два игрока нашлись для игры, заполняется это поле
            /// </summary>
            public string GameId { get; set; }
            public DateTime? GameStart { get; set; }
        }

        private object lockSearchList = new object();

        private List<Search> searchList = new List<Search>();

        public bool IsInit { get; private set; }

        public void Init(List<IGameInfo> games)
        {
            _games = games;
            IsInit = true;
        }


        public void StartSearch(int playerId)
        {
            _logger.LogInformation("Search Start [player=" + playerId + "]");

            lock (lockSearchList)
            {
                var search = searchList.FirstOrDefault(x => x.PlayerId == playerId && x.Status != SearchStatus.Finish);
                if (search != null)
                {
                    _logger.LogInformation("Search in process");
                    return;
                }

                var gameInProcess = _games.Any(x => x.IsMyGame(playerId) && !x.IsFinish);
                if (gameInProcess)
                {
                    throw new BusinessException("Существует незаконценная игра, поиск невозможен");
                }

                _logger.LogInformation("Search Start Success");

                search = new Search();
                search.PlayerId = playerId;
                search.Status = SearchStatus.InProcess;
                searchList.Insert(0, search);

                CheckPairPlayer();
            }
        }

        private void CheckPairPlayer()
        {
            var searchs = searchList.Where(x => x.Status == SearchStatus.InProcess).Take(2);
            if (searchs.Count() == 2)
            {
                _logger.LogInformation("Search Finish");
                var gameId = DateTime.Now.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N");
                searchList[0].GameId = gameId;
                searchList[0].Status = SearchStatus.NeedConfirm;
                searchList[1].GameId = gameId;
                searchList[1].Status = SearchStatus.NeedConfirm;
            }
        }

        public SearchStatus Check(int playerId)
        {
            _logger.LogInformation("Search Check [player=" + playerId + "]");
            var search = searchList.FirstOrDefault(x => x.PlayerId == playerId);
            if (search == null)
            {
                return SearchStatus.NotFound;
            }

            return search.Status;
        }

        public SearchStatus Confirm(int playerId)
        {
            _logger.LogInformation("Search Confirm [player=" + playerId + "]");
            var search = searchList.FirstOrDefault(x => x.PlayerId == playerId);
            if (search == null)
            {
                throw new BusinessException("Поиск игры отсутствует");
            }

            if (search.Status == SearchStatus.NeedConfirm)
            {
                var twoSearch = searchList.First(x => x.GameId == search.GameId && x.PlayerId != search.PlayerId);
                if(twoSearch.Status == SearchStatus.NeedConfirmOpponent)
                {
                    _logger.LogInformation("Two Player Confirm Game Start");
                    var gameStartDate = DateTime.Now;
                    search.GameStart = gameStartDate;
                    search.Status = SearchStatus.Finish;
                    twoSearch.GameStart = gameStartDate;
                    twoSearch.Status = SearchStatus.Finish;

                    int whitePlayerId;
                    int blackPlayerId;
                    if (DateTime.Now.Millisecond % 2 == 0)
                    {
                        whitePlayerId = searchList[1].PlayerId;
                        blackPlayerId = searchList[0].PlayerId;
                    }
                    else
                    {
                        blackPlayerId = searchList[1].PlayerId;
                        whitePlayerId = searchList[0].PlayerId;
                    }

                    IGameInfo game = new GameInfo(search.GameId, whitePlayerId, blackPlayerId);
                    _games.Add(game);
                }
                else
                {
                    _logger.LogInformation("Wait Game Opponent Confirm");
                    search.Status = SearchStatus.NeedConfirmOpponent;
                }
            }

            return search.Status;
        }

        public void StopSearch(int playerId)
        {
            _logger.LogInformation("Search Stop [player=" + playerId + "]");
            var search = searchList.FirstOrDefault(x => x.PlayerId == playerId);
            if (search == null)
            {
                return;
            }
            if (search.Status == SearchStatus.Finish)
            {
                throw new Exception("this status " + search.Status + " bad");
            }

            lock (lockSearchList)
            {
                var twoSearch = searchList.FirstOrDefault(x => x.GameId == search.GameId && x.PlayerId != search.PlayerId);
                if (twoSearch != null)
                {
                    twoSearch.Status = SearchStatus.InProcess;
                }
                searchList.Remove(search);
            }
        }

        public IGameInfo FindMyPlayingGame(int playerId)
        {
            var gameInProcess = _games.LastOrDefault(x => x.IsMyGame(playerId));
            return gameInProcess;
        }

    }
}
