using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Result
    {
        public long Id { get; set; }
        public long PairId { get; set; }
        public int Position { get; set; }

        public virtual Pair Pair { get; set; }
    }
}
