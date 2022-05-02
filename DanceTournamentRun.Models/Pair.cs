using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Pair
    {
        public Pair()
        {
            Heats = new HashSet<Heat>();
            Points = new HashSet<Point>();
            Results = new HashSet<Result>();
        }

        public long Id { get; set; }
        public long GroupId { get; set; }
        public string Partner1FirstName { get; set; }
        public string Partner1LastName { get; set; }
        public string Partner2FirstName { get; set; }
        public string Partner2LastName { get; set; }
        public int? Number { get; set; }

        public virtual Group Group { get; set; }
        public virtual ICollection<Heat> Heats { get; set; }
        public virtual ICollection<Point> Points { get; set; }
        public virtual ICollection<Result> Results { get; set; }
    }
}
