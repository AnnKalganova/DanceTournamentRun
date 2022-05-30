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

        //GET: api/Referee/{token}/pairs/{grId}
        //get pairs by group
        [HttpGet("pairs/{grId}")]
        public async Task<ActionResult<HeatInfo>> GetPairsByGroup(string token, long grId)
        {
            int access;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                access = db.IsAccessToGroupGranted(grId, token);
            }
            if (access == 0)
                return NotFound();
            var dances = _context.GetDances(grId);
            var userId = _context.Users.First(u => u.SecurityToken == token).Id;
            HeatInfo heatInfo = new HeatInfo() { RefProgressId = 0, PairsInfo = new List<HeatPairInfo>()};
            foreach(var dance in dances)
            {
                RefereeProgress progress = _context.RefereeProgresses.Where(r => r.DanceId == dance.Id && r.UserId == userId && r.IsCompleted == false)
                    .OrderBy(r => r.Heat).FirstOrDefault();
                if(progress != null)
                {
                    heatInfo.RefProgressId = progress.Id;
                    heatInfo.Dance = dance.Name;
                    heatInfo.Heat = progress.Heat;
                    break;
                }
            }
            if (heatInfo.RefProgressId == 0)
                return BadRequest("Судейство данной группы завершено!");
            var scores = _context.Scores.Where(s => s.ProgressId == heatInfo.RefProgressId).ToList();
            foreach(var score in scores)
            {
                Pair pair = _context.Pairs.Find(score.PairId);
                HeatPairInfo pairInfo = new HeatPairInfo() { ScoreId = score.Id, Score = (int)score.Score1, PairNumber = (int)pair.Number };
                heatInfo.PairsInfo.Add(pairInfo);
            }
            heatInfo.PairsInfo = heatInfo.PairsInfo.OrderBy(p => p.PairNumber).ToList();
            return heatInfo;
        }

        //POST: api/Referee/{token}/setScore
        //set score 
        [HttpPost("setScore")]
        public async Task<ActionResult> SetScore(string token, [FromBody] RefScoreSet scoreSet)
        {
            if (!isThisRefereesProgress(token, scoreSet.RefProgressId))
                return NotFound();
            var score = _context.Scores.Find(scoreSet.ScoreId);
            score.Score1 = scoreSet.Score;
            await _context.SaveChangesAsync();
            return Ok();
        }

        //POST: api/Referee/{token}/complete
        //set complete state on referee progress
        [HttpPost("complete")]
        public async Task<ActionResult> CompleteRefereeing (string token, [FromBody] RefScoreSet scoreSet)
        {
            if (!isThisRefereesProgress(token, scoreSet.RefProgressId))
                return NotFound();
            var progress = _context.RefereeProgresses.Find(scoreSet.RefProgressId);
            progress.IsCompleted = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool isThisRefereesProgress( string token, long refProgId)
        {
            var userId = _context.Users.FirstOrDefault(u => u.SecurityToken == token).Id;
            var progress = _context.RefereeProgresses.FirstOrDefault(r => r.Id == refProgId && r.UserId == userId);
            if (progress == null)
                return false;
            return true;
        }

    }
}
