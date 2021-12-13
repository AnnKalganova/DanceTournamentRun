using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Pair
    {
        public Pair()
        {
            DoublePairPair1s = new HashSet<DoublePair>();
            DoublePairPair2s = new HashSet<DoublePair>();
        }

        public long Id { get; set; }
        public long GroupId { get; set; }
        public long? CoachId { get; set; }
        public string Partner1FirstName { get; set; }
        public string Partner1LastName { get; set; }
        public string Partner2FirstName { get; set; }
        public string Partner2LastName { get; set; }
        public int? Number { get; set; }

        public virtual Coach Coach { get; set; }
        public virtual Group Group { get; set; }
        public virtual ICollection<DoublePair> DoublePairPair1s { get; set; }
        public virtual ICollection<DoublePair> DoublePairPair2s { get; set; }
    }
}
