using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Department
    {
        public Department()
        {
            Groups = new HashSet<Group>();
        }

        public long Id { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime RegistrationTime { get; set; }
        public long TournamentId { get; set; }

        public virtual Tournament Tournament { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
    }
}
