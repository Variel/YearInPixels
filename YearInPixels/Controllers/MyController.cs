using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Variel.Web.Session;
using YearInPixels.Models.Data;
using YearInPixels.Models.Response;
using YearInPixels.Services;

namespace YearInPixels.Controllers
{
    [Route("/api/my")]
    public class MyController : Controller
    {
        private readonly SessionService<Account> _session;
        private readonly DatabaseContext _database;

        public MyController(DatabaseContext database, SessionService<Account> session)
        {
            _database = database;
            _session = session;
        }

        [Route("calendars")]
        public async Task<IActionResult> GetMyCalendars()
        {
            var user = await _session.GetUserAsync();
            if (user == null)
                return StatusCode(401, new ErrorResponseModel("로그인이 필요합니다"));

            return Ok(await _database.Calendars.Where(c => c.OwnerId == user.Id).OrderByDescending(c => c.CreatedAt).ToArrayAsync());
        }
    }
}
