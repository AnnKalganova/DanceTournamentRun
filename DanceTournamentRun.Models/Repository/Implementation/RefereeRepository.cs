
using DanceTournamentRun.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models.Repository.Implementation
{
    public class RefereeRepository : GenericRepository<Referee>, IRefereeRepository
    {
        /// <summary>
        /// ClubRepository Constructor.
        /// </summary>
        /// <param name="context"></param>
        public RefereeRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
