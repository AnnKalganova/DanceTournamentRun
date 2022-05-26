using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.ViewComponents
{
    public class RefereeProcessViewComponent :ViewComponent
    {
        private readonly ApplicationDbContext db;

        public RefereeProcessViewComponent(ApplicationDbContext context) => db = context;

        public async Task<IViewComponentResult> InvokeAsync(long groupId)
        {
            var dances = db.GetDances(groupId);
            var refCount = db.GetRefereesByGroupId(groupId).Count;
            var completedCount = 0;
            foreach (var dance in dances)
            {
                completedCount += db.RefereeProgresses.Where(r => r.DanceId == dance.Id && r.IsCompleted == true).ToList().Count;
            }
            ViewBag.maxNumber = dances.Count * refCount;
            return View(completedCount);
        }

    }
}
