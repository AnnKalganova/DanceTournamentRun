using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.ViewComponents
{
    public class ResultsViewComponent :ViewComponent
    {
        private readonly ApplicationDbContext db;

        public ResultsViewComponent(ApplicationDbContext context) => db = context;

        public async Task<IViewComponentResult> InvokeAsync(Dictionary<long, int> orderedPairs)
        {
            List<ResultsViewModel> results = new List<ResultsViewModel>();
            foreach (var pairInfo in orderedPairs)
            {
                var pair = db.Pairs.Find(pairInfo.Key);
                var position = db.Results.First(r => r.PairId == pair.Id).Position;
                ResultsViewModel model = new ResultsViewModel()
                {
                    Position = position,
                    Partner1FullName = pair.Partner1LastName + " " + pair.Partner1FirstName,
                    Partner2Fullname = pair.Partner2LastName + " " + pair.Partner2FirstName,
                    Number = (int)pair.Number
                };
                results.Add(model);
            }
            return View(results);
        }
    }
}
