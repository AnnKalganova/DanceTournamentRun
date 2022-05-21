using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.ApiControllers
{
    [EnableCors("AllowMyOrigin")]
    [Route("api/[controller]/{token}")]
    [ApiController]
    public class RefereeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RefereeController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/Referee/{token}
        //get user name
        [HttpGet]
        public ActionResult<UserInfoModel> GetUserInfo(string token)
        {
            User user = _context.Users.FirstOrDefault(u => u.SecurityToken == token);
            if (user != null)
            {
                return new UserInfoModel() { LastName = user.LastName, FirstName = user.FirstName };
            }
            return NotFound();
        }

        //GET: api/Referee/{token}/groups
        //get groups by token
        [HttpGet("groups")]
        public ActionResult<List<Group>> GetGroups(string token)
        {

            List<Group> groups = new List<Group>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                groups = db.GetGroupsByToken(token);
            }
            if (groups.Count() == 0)
                return NotFound();
            return groups;
        }

        [HttpGet("pairs/{grId}")]
        public async Task<ActionResult<IEnumerable<Pair>>> GetPairsByGroup(string token, long grId)
        {
            int access;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                access = db.isUserHasAccessToGroup(grId, token);
            }
            if (access != 0)
            {
                var pairs = await _context.Pairs.Where(x => x.GroupId == grId).ToListAsync();
                return pairs;
            }
            return NotFound();
        }

    }
}
