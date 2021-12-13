
using DanceTournamentRun.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models.Repository.Implementation
{
    public class DoublePairRepository : GenericRepository<DoublePair>, IDoublePairRepository
    {
        /// <summary>
        /// ClubRepository Constructor.
        /// </summary>
        /// <param name="context"></param>
        public DoublePairRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
