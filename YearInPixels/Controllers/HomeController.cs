using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YearInPixels.Models;
using YearInPixels.Models.Data;
using YearInPixels.Services;

namespace YearInPixels.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _database;

        public HomeController(DatabaseContext database)
        {
            _database = database;
        }


        public async Task<IActionResult> Index()
        {
            var calendar = new Calendar();

            _database.Calendars.Add(calendar);
            await _database.SaveChangesAsync();

            return RedirectToAction("Calendar", new {id = calendar.Id});
        }

        [Route("{id}")]
        public async Task<IActionResult> Calendar(string id)
        {
            var calendar = await _database.Calendars.FindAsync(id);

            if (calendar == null)
                return RedirectToAction("Index");

            return View(calendar);
        }
    }
}
