using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Heat
    {
        public long Id { get; set; }
        public long PairId { get; set; }
        public int Heat1 { get; set; }

        public virtual Pair Pair { get; set; }
    }
}
