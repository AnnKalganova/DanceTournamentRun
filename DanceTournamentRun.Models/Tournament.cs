using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Tournament
    {
        public Tournament()
        {
            Groups = new HashSet<Group>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
    }
}
