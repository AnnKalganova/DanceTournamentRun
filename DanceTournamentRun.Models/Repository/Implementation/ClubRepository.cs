
using DanceTournamentRun.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models.Repository.Implementation
{
    public class ClubRepository : GenericRepository<Club>, IClubRepository
    {
        /// <summary>
        /// ClubRepository Constructor.
        /// </summary>
        /// <param name="context"></param>
        public ClubRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
