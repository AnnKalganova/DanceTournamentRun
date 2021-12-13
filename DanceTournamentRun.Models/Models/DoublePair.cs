using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class DoublePair
    {
        public long Id { get; set; }
        public long Pair1Id { get; set; }
        public long Pair2Id { get; set; }

        public virtual Pair Pair1 { get; set; }
        public virtual Pair Pair2 { get; set; }
    }
}
