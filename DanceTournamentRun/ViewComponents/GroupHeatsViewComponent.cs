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
            var danceId = db.GroupsDances.First(d=>d.GroupId == groupId).Id;
            var refereeId = db.GetRefereesByGroupId(groupId)[0].Id;
            var refProgresses = db.GetHeats(refereeId, danceId);
            Dictionary<int, List<int>> heats = new Dictionary<int, List<int>>();
            foreach(var progress in refProgresses)
            {
                heats.Add(progress.Heat, new List<int>());
                var pairsInHeat = db.GetPairsByRefProgress(progress.Id);
                foreach(var pair in pairsInHeat)
                {
                    heats[progress.Heat].Add((int)pair.Number);
                }
            }
            ViewBag.groupId = groupId;
            return View(heats);
        }
    }
}
