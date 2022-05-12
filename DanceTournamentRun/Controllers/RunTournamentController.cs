using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DanceTournamentRun.Controllers
{
    [Authorize(Roles = "admin")]
    public class RunTournamentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RunTournamentController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            long userId = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Tournament tournament = _context.Tournaments.FirstOrDefault(p => p.UserId == userId);
            if (tournament == null)
            {
                return RedirectToAction("CreateTournament");
            }
            if (!(bool)tournament.IsTournamentRun)
            {
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.Login = _context.Users.Where(u => u.Id == tournament.UserId).Select(p => p.Login).FirstOrDefault();
            return View(tournament);
        }

        public ActionResult GetData(long? tournId)
        {
            if (tournId != null)
            {
                var tourn = _context.Tournaments.Find(tournId);
                if (!(bool)tourn.IsFirstStepOver)
                {
                    return RedirectToAction("GetFirstView", tourn);
                }
                else if (!(bool)tourn.IsSecondStepOver)
                {
                    return RedirectToAction("GetSecondView");
                }
                else
                {
                    return RedirectToAction("GetThirdView");
                }

            }
            return PartialView("Error");
        }

        public PartialViewResult GetFirstView(Tournament tournament)
        {
            if(tournament !=null)
            {
                return PartialView("FirstStep", tournament);
            }
            return PartialView("Error");
        }

        public PartialViewResult GetSecondView(Tournament tournament)
        {
            if (tournament != null)
            {
                return PartialView("SecondStep");
            }
            return PartialView("Error");
        }

        public PartialViewResult GetThirdView(Tournament tournament)
        {
            if (tournament != null)
            {
                return PartialView("EndStep");
            }
            return PartialView("Error");
        }

        public async Task<ActionResult> GetRegQR(long? tournId)
        {
            if(tournId != null)
            {
                //сформировать qr +вывести
                List<User> registrators = new List<User>();
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    registrators = db.GetRegistratorsByTournId((long)tournId);
                }

                List<RegViewModel> regViews = new List<RegViewModel>();
                foreach (var reg in registrators)
                {
                    QRRegData data = new QRRegData() { Login = reg.Login, LastName = reg.LastName, Firstname = reg.FirstName };
                    var link = JsonConvert.SerializeObject(data);
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);

                    RegViewModel regView = new RegViewModel() { LastName = reg.LastName, Firstname = reg.FirstName, QR = BitmapToBytesCode(qrCodeImage) };
                    regViews.Add(regView);
                }
                ViewBag.regViews = regViews;
                return View("RegQRs");
                // строка прогресса регистрации ViewComponent??
            }
            return View("Error");
        }

        public async Task<ActionResult> GetRefereeQR(long? tournId)
        {
            if (tournId != null)
            {
                List<User> referees = new List<User>();
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    referees = db.GetRefereesByTourn((long)tournId);
                }

                List<RegViewModel> refViews = new List<RegViewModel>();
                foreach (var referee in referees)
                {
                    QRRegData data = new QRRegData() { Login = referee.Login, LastName = referee.LastName, Firstname = referee.FirstName };
                    var link = JsonConvert.SerializeObject(data);
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);

                    RegViewModel refView = new RegViewModel() { LastName = referee.LastName, Firstname = referee.FirstName, QR = BitmapToBytesCode(qrCodeImage) };
                    refViews.Add(refView);
                }
                ViewBag.regViews = refViews;
                return View("RefereeQRs");
            }
            return View("Error");
        }

        private static Byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

    }
}


       
