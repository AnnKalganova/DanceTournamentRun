using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models
{
    public class GroupForRefModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool isGroupRun { get; set; }
        public bool isAccessGranted { get; set; }
    }
}
