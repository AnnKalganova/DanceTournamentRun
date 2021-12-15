using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models
{
    public class RegLinksViewModel
    {
        public RegLinksViewModel() { }
        public List<long> GroupsId { get; set; } = new List<long>();

        public string QRCode { get; set; } 
    }
}
