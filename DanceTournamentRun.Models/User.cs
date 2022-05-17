using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class User
    {
        public User()
        {
            Points = new HashSet<Point>();
            Tournaments = new HashSet<Tournament>();
            UsersGroups = new HashSet<UsersGroup>();
            UsersTournaments = new HashSet<UsersTournament>();
        }
        public long Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public long RoleId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string SecurityToken { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Point> Points { get; set; }
        public virtual ICollection<Tournament> Tournaments { get; set; }
        public virtual ICollection<UsersGroup> UsersGroups { get; set; }
        public virtual ICollection<UsersTournament> UsersTournaments { get; set; }
    }
}
