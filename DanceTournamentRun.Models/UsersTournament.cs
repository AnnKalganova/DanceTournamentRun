using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class UsersTournament
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long TournamentId { get; set; }

        public virtual User User { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}
