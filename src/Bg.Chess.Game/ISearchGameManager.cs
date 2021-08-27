namespace Bg.Chess.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ISearchGameManager
    {
        public void AddSearch(int playerId);
        public SearchStatus CheckSearch(int playerId);
        public void Confirm(int playerId);
    }

    public class SearchGameManager : ISearchGameManager
    {
        private IGameHolder _gameHolder;
        public SearchGameManager(IGameHolder gameHolder)
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
        }

        private object lockSearchList = new object();

        private List<Search> searchList = new List<Search>();

        public void AddSearch(int playerId)
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
            var searchs = searchList.Where(x => x.GameId == null).Take(2);
            if (searchs.Count() == 2)
            {
                var gameId = DateTime.Now.ToString("yyyyMMddHHmmss")+Guid.NewGuid().ToString("N");
                searchList[0].GameId = gameId;
                searchList[0].Status = SearchStatus.NeedConfirm;
                searchList[1].GameId = gameId;
                searchList[1].Status = SearchStatus.NeedConfirm;

                if(DateTime.Now.Millisecond % 2 == 0)
                {
                    _gameHolder.AddGame(gameId, searchList[1].PlayerId, searchList[1].PlayerId);
                }
                else
                {
                    _gameHolder.AddGame(gameId, searchList[0].PlayerId, searchList[0].PlayerId);
                }
            }
        }

        public SearchStatus CheckSearch(int playerId)
        {
            var search = searchList.FirstOrDefault(x => x.PlayerId == playerId);
            if (search == null)
            {
                return SearchStatus.NotFound;
            }

            return search.Status;
        }

        public void Confirm(int playerId)
        {
            var search = searchList.FirstOrDefault(x => x.PlayerId == playerId);
            if (search == null)
            {
                throw new BusinessException("Поиск игры отсутствует");
            }

            if (search.Status == SearchStatus.NeedConfirm)
            {
                продолжить тут
                var status = _gameHolder.StartGame(search.GameId, search.PlayerId);
                search.GameId
                return;
            }

        }
    }
}
