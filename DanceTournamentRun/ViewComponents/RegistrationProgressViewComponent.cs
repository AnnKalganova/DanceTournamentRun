using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.ViewComponents
{
    public class RegistrationProgressViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;

        public RegistrationProgressViewComponent(ApplicationDbContext context) => db = context;

        public async Task<IViewComponentResult> InvokeAsync(long tournId)
        {
            var countGr = await db.GetCompletedGroupsCount(tournId);
            var maxNumber = GetMaxNumber(tournId);
            ViewBag.maxNumber = maxNumber;
            return View(countGr);
        }
        
        private int GetMaxNumber(long tournId)
        {
            var maxNumber = db.Groups.Where(g => g.TournamentId == tournId).Count();
            return maxNumber;
        }
    }
}
