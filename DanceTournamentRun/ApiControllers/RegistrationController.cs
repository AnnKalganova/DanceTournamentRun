using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DanceTournamentRun.ApiControllers
{
    [EnableCors("AllowMyOrigin")]
    [Route("api/[controller]/{token}")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegistrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/Registration/{token}
        //get user name
        [HttpGet]
        public ActionResult<UserInfoModel> GetUserInfo(string token)
        {
            User user = _context.Users.FirstOrDefault(u => u.SecurityToken == token);
            if (user != null)
            {
                return new UserInfoModel() { LastName = user.LastName, FirstName = user.FirstName};
            }
            return NotFound();
        }

        //GET: api/Registration/{token}/groups
        //get groups by token
        [HttpGet("groups")]
        public ActionResult<List<GroupForRegModel>> GetGroups(string token) {

            List<Group> groups = new List<Group>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                groups = db.GetGroupsByToken(token);
            }
            if (groups.Count() == 0)
                return NotFound();
            List<GroupForRegModel> regGroups = new List<GroupForRegModel>();
            if (groups.FirstOrDefault(g => g.IsRegistrationOn == false) != null)
                return regGroups;
            
            foreach(var group in groups)
            {
                GroupForRegModel model = new GroupForRegModel() { Id = group.Id, Name = group.Name.Replace("_"," "), CompletedState = false };
                var pairWhithNoNumber = _context.Pairs.FirstOrDefault(p => p.GroupId == group.Id && p.Number == null);
                if(pairWhithNoNumber == null )
                {
                    model.CompletedState = true;
                }
                regGroups.Add(model);
            }
            return regGroups;
        }

        //GET: api/Registration/{token}/pairs/{grId}
        //get pairs by group with token verification
        [HttpGet("pairs/{grId}")]
        public async Task<ActionResult<List<Pair>>> GetPairsByGroup(string token, long grId)
        {
            int access;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                access = db.IsAccessToGroupGranted(grId, token);
            }
            if (access != 0)
            {
                var pairs = await _context.Pairs.Where(x => x.GroupId == grId).ToListAsync();
                return pairs.OrderBy(p => p.Partner1LastName).ToList();
            }
            return NotFound();
        }

        //POST: api/Registration/{token}/updatePair/{pair}
        //update pair by pair object with token verification
        [HttpPost("updatePair")]
        public async Task<StatusCodeResult> UpdatePair(string token,[FromBody] Pair pair)
        {
            int access;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                access = db.IsAccessToGroupGranted(pair.GroupId, token);
            }
            if ( access == 0)
                return NotFound();
            
            int countSimilarP1, countSimilarP2;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                countSimilarP1 = db.FindSimilarPartner(pair.GroupId, pair.Id ,pair.Partner1LastName, pair.Partner1FirstName);
                countSimilarP2 = db.FindSimilarPartner(pair.GroupId, pair.Id, pair.Partner2LastName, pair.Partner2FirstName);
            }
            var simularNumber = pair.Number == null ? null : _context.Pairs.FirstOrDefault(p => p.Id != pair.Id && p.Number == pair.Number && p.GroupId == pair.GroupId);
            if (countSimilarP1 != 0)
                return StatusCode(1);
            else if (countSimilarP2 != 0)
                return StatusCode(2);
            if (simularNumber != null)
                return StatusCode(3);
             _context.Entry(pair).State = EntityState.Modified;
             await _context.SaveChangesAsync();
            return Ok();
        }

        //POST:api/Registration/{token}/createPair
        // create pair by pair object from body with token verification
        [HttpPost("createPair")]
        public async Task<ActionResult<string>> CreatePair(string token,[FromBody] CreatePairModel pair)
        {
            int access;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                access = db.IsAccessToGroupGranted(pair.GroupId, token);
            }
            if (access == 0)
            {
                return NotFound();
            }
            int countSimilarP1, countSimilarP2;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                countSimilarP1 = db.FindSimilarPartner(pair.GroupId, 0, pair.Partner1LastName, pair.Partner1FirstName);
                countSimilarP2 = db.FindSimilarPartner(pair.GroupId, 0, pair.Partner2LastName, pair.Partner2FirstName);
            }
            var simularNumber = pair.Number == null ? null : _context.Pairs.FirstOrDefault(p => p.GroupId == pair.GroupId && p.Number == pair.Number);
            if (countSimilarP1 != 0)
                return BadRequest("Такой партнер уже участвует в группе");
            else if (countSimilarP2 != 0)
                return BadRequest("Такая партнерша уже учавствует в группе");
            if (simularNumber != null)
                return BadRequest("Такой номер присвоен другой паре");
            Pair newPair = new Pair()
            {
                GroupId = pair.GroupId,
                Partner1LastName = pair.Partner1LastName,
                Partner1FirstName = pair.Partner1FirstName,
                Partner2LastName = pair.Partner2LastName,
                Partner2FirstName = pair.Partner2FirstName,
                Number = pair.Number
            };
            _context.Pairs.Add(newPair);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST: api/Registration/{token}/deletePair/{pairId}/{groupId}
        //delete pair by id with token verification
        [HttpPost("deletePair/{pairId}/{groupId}")]
        public async Task<IActionResult> DeletePair(string token, long pairId, long groupId)
        {
            int access;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                access = db.IsAccessToGroupGranted(groupId, token);
            }
            var pair = _context.Pairs.Find(pairId);
            if (pair == null || access == 0)
            {
                return NotFound();
            }
            _context.Pairs.Remove(pair);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //POST: api/Referee/{token}/setScore
        //set registration complete state for all registrator's groups
        [HttpGet("complete")]
        public async Task<ActionResult> CompleteRegistration(string token)
        {
            List<Group> groups = new List<Group>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                groups = db.GetGroupsByToken(token);
            }
            if (groups.Count() == 0)
                return NotFound();
            foreach(var gr in groups)
            {
                var currGr = _context.Groups.Find(gr.Id);
                currGr.IsRegistrationOn = false;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        // GET: api/Registration/2
        [HttpGet("pair/{Id}")]
        public async Task<ActionResult<Pair>> GetPair(long Id)
        {
            var pair = await _context.Pairs.FindAsync(Id);
            if (pair == null)
            {
                return NotFound();
            }
            return pair;
        }

    }
}