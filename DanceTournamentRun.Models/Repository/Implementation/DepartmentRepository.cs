
using DanceTournamentRun.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models.Repository.Implementation
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        /// <summary>
        /// ClubRepository Constructor.
        /// </summary>
        /// <param name="context"></param>
        public DepartmentRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
