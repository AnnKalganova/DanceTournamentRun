using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Dance
    {
        public Dance()
        {
            GroupsDances = new HashSet<GroupsDance>();
            RefereeProgress = new HashSet<RefereeProgress>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<GroupsDance> GroupsDances { get; set; }
        public virtual ICollection<RefereeProgress> RefereeProgress { get; set; }
    }
}
