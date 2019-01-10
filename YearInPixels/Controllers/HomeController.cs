using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Variel.Web.Session;
using YearInPixels.Helpers;
using YearInPixels.Models;
using YearInPixels.Models.Data;
using YearInPixels.Services;

namespace YearInPixels.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _database;
        private readonly SessionService<Account> _session;

        public HomeController(DatabaseContext database, SessionService<Account> session)
        {
            _database = database;
            _session = session;
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
            var deviceId = Request.Cookies["deviceId"];
            if (String.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = RandomIdentityGenerator.Generate();
                Response.Cookies.Append("deviceId", deviceId, new CookieOptions { Expires = DateTimeOffset.Now.AddYears(10) });
            }

            var user = await _session.GetUserAsync();

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
                Year = year??DateTimeOffset.UtcNow.AddHours(9).Year,
                OwnerDeviceId = deviceId,
                IsPrivate = true,
                Owner = user
            };

            _database.Calendars.Add(calendar);
            await _database.SaveChangesAsync();

            return RedirectToAction("Calendar", new { id = calendar.Id });
        }

        [HttpPost]
        [Route("api/calendars")]
        public async Task<IActionResult> CreateApi(string title, int? year)
        {
            var deviceId = Request.Cookies["deviceId"];
            if (String.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = RandomIdentityGenerator.Generate();
                Response.Cookies.Append("deviceId", deviceId, new CookieOptions { Expires = DateTimeOffset.Now.AddYears(10) });
            }

            var user = await _session.GetUserAsync();

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
                Title = title ?? "나의 기분 달력",
                Year = year ?? DateTimeOffset.UtcNow.AddHours(9).Year,
                OwnerDeviceId = deviceId,
                IsPrivate = true,
                Owner = user
            };

            _database.Calendars.Add(calendar);
            await _database.SaveChangesAsync();

            return Created(Url.Action("GetCalendar", "Calendar", new {calendarId = calendar.Id}),
                new {id = calendar.Id});
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

            var user = await _session.GetUserAsync();

            ViewBag.HasCollaborateAuthority = false;
            if (calendar.OwnerId != null)
            {
                if (user?.Id != calendar.OwnerId)
                {
                    if (calendar.SharingOption == CalendarSharingOption.None)
                    {
                        ViewBag.HasViewAuthority = false;
                        return View(calendar);
                    }
                }
                else
                {
                    ViewBag.HasCollaborateAuthority = true;
                }

                ViewBag.HasViewAuthority = true;
                return View(calendar);
            }

            if (calendar.OwnerDeviceId != null)
                if (Request.Cookies["deviceId"] != calendar.OwnerDeviceId)
                    if (calendar.SharingOption == CalendarSharingOption.None)
                    {
                        ViewBag.HasViewAuthority = false;
                        return View(calendar);
                    }

            Response.Cookies.Append("lastCalendarId", calendar.Id);

            ViewBag.HasViewAuthority = true;

            return View(calendar);
        }
    }
}
