using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefereeingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RefereeingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{lastname}/{groupId}")]
        public async Task<ActionResult<IEnumerable<Pair>>> GetPairsForReferee(string lastname, long groupId)
        {
            IQueryable<GroupsReferee> groupsReferee = _context.GroupsReferees.Where(k => k.GroupId == groupId).Include(x=>x.Referee);
            if (lastname != null)
            {
                groupsReferee = groupsReferee.Where(g => g.Referee.Lastname == lastname);
            }
            if (groupsReferee.Count() == 0 )
            {
                return NotFound();
            }

            IQueryable<Pair> pairs =  _context.Pairs.Where(s => s.GroupId == groupId);
            pairs = pairs.OrderBy(p => p.Number);
            if (pairs.Count() == 0)
                return NotFound();
            return await pairs.AsNoTracking().ToListAsync();

        }
    }
}
