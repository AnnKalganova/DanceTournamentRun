using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Tournament
    {
        public Tournament()
        {
            Departments = new HashSet<Department>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public long? ClubId { get; set; }
        public string Code { get; set; }

        public virtual Club Club { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
}
