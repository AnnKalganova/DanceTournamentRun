using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models
{
    public class GroupViewModal
    {
        public long GroupId { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public ICollection<Dance> Dances { get; set; }

    }
}
