using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models.Repository.Interfaces
{
    public interface ITournamentRepository : IRepository<Tournament>
    {
        /// <summary>
        /// Get Tournament by unique tournament code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Tournament GetTournamentByCode(string code);
    }
}
