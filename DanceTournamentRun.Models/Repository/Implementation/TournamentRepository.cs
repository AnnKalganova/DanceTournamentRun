
using DanceTournamentRun.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models.Repository.Implementation
{
    public class TournamentRepository : GenericRepository<Tournament>, ITournamentRepository
    {
        /// <summary>
        /// ClubRepository Constructor.
        /// </summary>
        /// <param name="context"></param>
        public TournamentRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public Tournament GetTournamentByCode(string code)
        {
            return Context.Set<Tournament>().FirstOrDefault(obj => obj.Code == code);
        }
    }
}
