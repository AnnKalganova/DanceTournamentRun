using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Club
    {
        public Club()
        {
            Coaches = new HashSet<Coach>();
            Tournaments = new HashSet<Tournament>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? CityId { get; set; }

        public virtual City City { get; set; }
        public virtual ICollection<Coach> Coaches { get; set; }
        public virtual ICollection<Tournament> Tournaments { get; set; }
    }
}
