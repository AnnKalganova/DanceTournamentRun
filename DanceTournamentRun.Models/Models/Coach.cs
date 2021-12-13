using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Coach
    {
        public Coach()
        {
            Pairs = new HashSet<Pair>();
        }

        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public long ClubId { get; set; }

        public virtual Club Club { get; set; }
        public virtual ICollection<Pair> Pairs { get; set; }
    }
}
