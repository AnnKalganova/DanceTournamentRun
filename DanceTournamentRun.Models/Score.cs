using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Score
    {
        public long Id { get; set; }
        public long ProgressId { get; set; }
        public long PairId { get; set; }
        public int? Score1 { get; set; }

        public virtual RefereeProgress RefereeProgress { get; set; }
        public virtual Pair Pair { get; set; }
    }
}
