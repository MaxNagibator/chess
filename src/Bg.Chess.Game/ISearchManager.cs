namespace Bg.Chess.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ISearchManager
    {
        void Start(int playerId);
        void Stop(int playerId);
        SearchStatus Check(int playerId);
        SearchStatus Confirm(int playerId);
    }

    public class SearchManager : ISearchManager
    {
        private IGameHolder _gameHolder;
        public SearchManager(IGameHolder gameHolder)
        {
            _gameHolder = gameHolder;
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

        public void Start(int playerId)
        {
            lock (lockSearchList)
            {
                var search = searchList.FirstOrDefault(x => x.PlayerId == playerId);
                if (search != null)
                {
                    return;
                }

                search = new Search();
                search.PlayerId = playerId;
                search.Status = SearchStatus.InProcess;
                searchList.Add(search);

                CheckPairPlayer();
            }
        }

        private void CheckPairPlayer()
        {
            var searchs = searchList.Where(x => x.Status == SearchStatus.InProcess).Take(2);
            if (searchs.Count() == 2)
            {
                var gameId = DateTime.Now.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N");
                searchList[0].GameId = gameId;
                searchList[0].Status = SearchStatus.NeedConfirm;
                searchList[1].GameId = gameId;
                searchList[1].Status = SearchStatus.NeedConfirm;

                if (DateTime.Now.Millisecond % 2 == 0)
                {
                    _gameHolder.AddGame(gameId, searchList[1].PlayerId, searchList[0].PlayerId);
                }
                else
                {
                    _gameHolder.AddGame(gameId, searchList[0].PlayerId, searchList[1].PlayerId);
                }
            }
        }

        public SearchStatus Check(int playerId)
        {
            var search = searchList.FirstOrDefault(x => x.PlayerId == playerId);
            if (search == null)
            {
                return SearchStatus.NotFound;
            }

            return search.Status;
        }

        public SearchStatus Confirm(int playerId)
        {
            var search = searchList.FirstOrDefault(x => x.PlayerId == playerId);
            if (search == null)
            {
                throw new BusinessException("Поиск игры отсутствует");
            }

            if (search.Status == SearchStatus.NeedConfirm)
            {
                var status = _gameHolder.StartGame(search.PlayerId);
                if (status == GameState.InProgress)
                {
                    var twoSearch = searchList.First(x => x.GameId == search.GameId && x.PlayerId != search.PlayerId);
                    var gameStartDate = DateTime.Now;
                    search.GameStart = gameStartDate;
                    search.Status = SearchStatus.Finish;
                    twoSearch.GameStart = gameStartDate;
                    twoSearch.Status = SearchStatus.Finish;
                }
                else if (status == GameState.WaitStart)
                {
                    search.Status = SearchStatus.NeedConfirmOpponent;
                }
                else
                {
                    throw new Exception("this status " + status + " bad");
                }
            }

            return search.Status;
        }

        public void Stop(int playerId)
        {
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
                _gameHolder.StopGame(search.PlayerId);
                var twoSearch = searchList.FirstOrDefault(x => x.GameId == search.GameId && x.PlayerId != search.PlayerId);
                if (twoSearch != null)
                {
                    twoSearch.Status = SearchStatus.InProcess;
                }
                searchList.Remove(search);
            }
        }
    }
}
