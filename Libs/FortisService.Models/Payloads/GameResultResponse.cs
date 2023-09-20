using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortisService.Models.Payloads
{
    public class GameResultResponse
    {
        public bool IsTie { get => Players.All(p => !p.IsWinner);  }

        public IList<PlayerHandResponse> Players { get; set; }

        public GameResultResponse(IList<PlayerHandResponse> players)
        {
            Players = players;
        }
    }
}
