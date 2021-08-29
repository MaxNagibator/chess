using Bg.Chess.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bg.Chess.Web.Repo
{
    public class Player
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }
    }
}
