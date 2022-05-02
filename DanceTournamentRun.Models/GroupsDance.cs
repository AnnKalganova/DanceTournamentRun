using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class GroupsDance
    {
        public long Id { get; set; }
        public long GroupId { get; set; }
        public long DanceId { get; set; }

        public virtual Dance Dance { get; set; }
        public virtual Group Group { get; set; }
    }
}
