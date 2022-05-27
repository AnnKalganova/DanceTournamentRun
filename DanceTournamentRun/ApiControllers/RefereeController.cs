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
        public ActionResult<List<GroupForRefModel>> GetGroups(string token)
        {
            var tournId = _context.UsersTournaments.FirstOrDefault(u => u.User.SecurityToken == token).TournamentId;
            List<Group> allGroups = new List<Group>();
            allGroups = _context.Groups.Where(g => g.TournamentId == tournId).ToList();
            if (allGroups == null)
                return NotFound();
            List<GroupForRefModel> groupsModels = new List<GroupForRefModel>();
            allGroups = allGroups.OrderBy(g => g.Number).ToList();
            foreach(var group in allGroups)
            {
                var access = _context.IsAccessToGroupGranted(group.Id, token);
                GroupForRefModel model = new GroupForRefModel() 
                {   
                    Id = group.Id,
                    Name = group.Name.Replace("_", " "), 
                    State = (int)group.CompetitionState, 
                    isAccessGranted = access != 0 ? true : false 
                };
                groupsModels.Add(model);
            }
            return groupsModels;
        }

        [HttpGet("pairs/{grId}")]
        public async Task<ActionResult<IEnumerable<Pair>>> GetPairsByGroup(string token, long grId)
        {
            int access;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                access = db.IsAccessToGroupGranted(grId, token);
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
