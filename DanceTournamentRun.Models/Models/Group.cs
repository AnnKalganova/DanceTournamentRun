using System;
using System.Collections.Generic;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class Group
    {
        public Group()
        {
            GroupsReferees = new HashSet<GroupsReferee>();
            Pairs = new HashSet<Pair>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Dances { get; set; }
        public long DepartmentId { get; set; }
        public int Number { get; set; }
        public bool RegistrationFinished { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<GroupsReferee> GroupsReferees { get; set; }
        public virtual ICollection<Pair> Pairs { get; set; }
    }
}
