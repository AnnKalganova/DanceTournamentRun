using DanceTournamentRun.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DanceTournamentRun.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("regLinks")]
        public async Task<ActionResult<RegLinksViewModel>> GetRegistrationLink()
        {
            var links = new RegLinksViewModel();

            var groups = await _context.Groups.ToListAsync();
            if (groups.Count() == 0)
                return NotFound();
            foreach (var group in groups)
            {
                links.GroupsId.Add(group.Id);
            }
            return links;
        }

        [HttpGet("regQR")]
        public IActionResult GetRegistrationQR(string link)
        { 
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            return File(BitmapToBytes(qrCodeImage), "image/jpeg");
        }

        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        // GET: api
        [HttpGet]
        [HttpGet("refLinks")]
        public ActionResult<RefLinksViewModel> GetRefereeLink(string lastname)
        {
            var refLinks = new RefLinksViewModel();

            IQueryable<GroupsReferee> groupsReferee = _context.GroupsReferees.Include(x => x.Referee);
            if (lastname != null)
            {
                groupsReferee = groupsReferee.Where(g => g.Referee.Lastname == lastname);
            }
            if (groupsReferee.Count() == 0 || lastname == null)
            {
                return null;
            }
            foreach (var gr in groupsReferee)
            {
                refLinks.groupsId.Add(gr.GroupId);
            }
            refLinks.refLastname = lastname;

            return refLinks;
        }



        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Pair>>> ShowRegistrationLink()
        //{
        //    //generateRegistrationUrl()
        //    //generateRegistrationQR()
        //    return await _context.Pairs.ToListAsync();
        //}

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Pair>>> GetPairsWithNumbers()
        //{
        //    //check if registraion is finished
        //    // show all pairs by groups
        //    return await _context.Pairs.ToListAsync();
        //}

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Pair>>> GetRefereeingResults()
        //{
        //    return await _context.Pairs.ToListAsync();
        //}
    }

    //public static class BitmapExtension
    //{
    //    public static byte[] BitmapToByteArray(this Bitmap bitmap)
    //    {
    //        using (MemoryStream ms = new MemoryStream())
    //        {
    //            bitmap.Save(ms, ImageFormat.Png);
    //            return ms.ToArray();
    //        }
    //    }
    //}
}
