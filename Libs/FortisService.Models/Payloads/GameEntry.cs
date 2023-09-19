using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortisService.Models.Payloads
{
    public  class GameEntry
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public IList<int> PlayerIds { get; set; }
    }
}
