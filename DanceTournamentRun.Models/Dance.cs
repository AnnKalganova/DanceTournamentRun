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
            Points = new HashSet<Point>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<GroupsDance> GroupsDances { get; set; }
        public virtual ICollection<Point> Points { get; set; }
    }
}
