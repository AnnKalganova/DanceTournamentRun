using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models
{
    public class HeatInfo
    {
        public int Heat { get; set; }
        public string Dance { get; set; }
        public long RefProgressId { get; set; }
        public List<HeatPairInfo> PairsInfo { get; set; }
    }

    public class HeatPairInfo
    {
        public long ScoreId { get; set; }
        public int PairNumber { get; set; }
        public int Score { get; set; }
    } 
}
