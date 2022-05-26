using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Group
    {
        public Group()
        {
            GroupsDances = new HashSet<GroupsDance>();
            Pairs = new HashSet<Pair>();
            UsersGroups = new HashSet<UsersGroup>();
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public bool? IsRegistrationOn { get; set; }
        public long TournamentId { get; set; }
        public int? CompetitionState { get; set; }

        public virtual Tournament Tournament { get; set; }
        public virtual ICollection<GroupsDance> GroupsDances { get; set; }
        public virtual ICollection<Pair> Pairs { get; set; }
        public virtual ICollection<UsersGroup> UsersGroups { get; set; }
    }
}
