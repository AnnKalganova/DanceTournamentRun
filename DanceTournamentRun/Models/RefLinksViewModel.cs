using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models
{
    public class RefLinksViewModel
    {
        public RefLinksViewModel() {}

        public string refLastname { get; set; }

        public List<long> groupsId { get; set; } = new List<long>();

    }
}
