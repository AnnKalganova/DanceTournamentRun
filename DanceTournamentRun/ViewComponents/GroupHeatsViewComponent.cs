using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.ViewComponents
{
    public class GroupHeatsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;

        public GroupHeatsViewComponent(ApplicationDbContext context) => db = context;

        public async Task<IViewComponentResult> InvokeAsync(long groupId)
        {
            var danceId = db.GroupsDances.First(d=>d.GroupId == groupId).DanceId;
            var refereeId = db.GetRefereesByGroupId(groupId)[0].Id;
            var refProgresses = db.GetHeats(refereeId, danceId);
            Dictionary<int, List<int>> heats = new Dictionary<int, List<int>>();
            foreach(var progress in refProgresses)
            {
                if(!heats.ContainsKey(progress.Heat))
                    heats.Add(progress.Heat, new List<int>());
                var pairsInHeat = db.GetPairsByRefProgress(progress.Id);
                foreach(var pair in pairsInHeat)
                {
                    heats[progress.Heat].Add((int)pair.Number);
                }
            }
            var keys = heats.Keys.ToList();
            for (var i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                heats[key] = heats[key].OrderBy(v => v).ToList();
            }

            ViewBag.groupId = groupId;
            return View(heats);
        }
    }
}
