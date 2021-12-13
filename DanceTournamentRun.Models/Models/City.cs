using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class City
    {
        public City()
        {
            Clubs = new HashSet<Club>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Club> Clubs { get; set; }
    }
}
