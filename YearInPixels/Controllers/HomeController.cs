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
            var id = Request.Cookies["lastCalendarId"];

            if (!String.IsNullOrWhiteSpace(id))
                return RedirectToAction("Calendar", new {id});

            return RedirectToAction("Create");
        }

        public async Task<IActionResult> Create(string title, int? year)
        {
            var calendar = new Calendar
            {
                Options = new[]
                {
                    new Option
                    {
                        Label = "너무 행복한 날",
                        Color = "#0fbcf9"
                    },
                    new Option
                    {
                        Label = "상쾌한 날",
                        Color = "#05c46b"
                    },
                    new Option
                    {
                        Label = "평범한 날",
                        Color = "#ffd32a"
                    },
                    new Option
                    {
                        Label = "피곤한 날",
                        Color = "#f53b57"
                    },
                    new Option
                    {
                        Label = "상처 받은 날",
                        Color = "#3c40c6"
                    },
                    new Option
                    {
                        Label = "끔찍한 날",
                        Color = "#1e272e"
                    }
                },
                Title = title??"나의 기분 달력",
                Year = year??DateTimeOffset.UtcNow.AddHours(9).Year
            };



            _database.Calendars.Add(calendar);
            await _database.SaveChangesAsync();

            return RedirectToAction("Calendar", new { id = calendar.Id });
        }

        [Route("{id}")]
        public async Task<IActionResult> Calendar(string id)
        {
            var calendar = await _database.Calendars.FindAsync(id);

            if (calendar == null)
            {
                Response.Cookies.Delete("lastCalendarId");
                return RedirectToAction("Index");
            }

            Response.Cookies.Append("lastCalendarId", calendar.Id);

            return View(calendar);
        }
    }
}
