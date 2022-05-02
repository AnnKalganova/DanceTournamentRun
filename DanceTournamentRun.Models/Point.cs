using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Point
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long PairId { get; set; }
        public long DanceId { get; set; }
        public int? Point1 { get; set; }

        public virtual Dance Dance { get; set; }
        public virtual Pair Pair { get; set; }
        public virtual User User { get; set; }
    }
}
