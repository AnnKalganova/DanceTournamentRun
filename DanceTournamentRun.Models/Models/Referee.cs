using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Referee
    {
        public Referee()
        {
            GroupsReferees = new HashSet<GroupsReferee>();
        }

        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public virtual ICollection<GroupsReferee> GroupsReferees { get; set; }
    }
}
