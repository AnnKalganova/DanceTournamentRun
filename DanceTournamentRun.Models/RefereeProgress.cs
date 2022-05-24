using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class RefereeProgress
    {
        public RefereeProgress()
        {
            Score = new HashSet<Score>();
        }
        public long Id { get; set; }
        public long UserId { get; set; }
        public long DanceId { get; set; }
        public int Heat { get; set; }
        public bool? IsCompleted { get; set; }

        public virtual User User { get; set; }
        public virtual Dance Dance { get; set; }

        public virtual ICollection<Score> Score { get; set; }

    }
}
