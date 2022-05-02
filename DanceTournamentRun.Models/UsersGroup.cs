using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class UsersGroup
    {
        public long Id { get; set; }
        public long GroupId { get; set; }
        public long UserId { get; set; }

        public virtual Group Group { get; set; }
        public virtual User User { get; set; }
    }
}
