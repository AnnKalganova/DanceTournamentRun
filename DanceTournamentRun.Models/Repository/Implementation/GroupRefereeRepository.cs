
using DanceTournamentRun.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models.Repository.Implementation
{
    public class GroupRefereeRepository : GenericRepository<GroupsReferee>, IGroupRefereeRepository
    {
        /// <summary>
        /// ClubRepository Constructor.
        /// </summary>
        /// <param name="context"></param>
        public GroupRefereeRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
