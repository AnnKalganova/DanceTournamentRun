using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class GroupsReferee
    {
        public long Id { get; set; }
        public long GroupId { get; set; }
        public long RefereeId { get; set; }

        public virtual Group Group { get; set; }
        public virtual Referee Referee { get; set; }
    }
}
