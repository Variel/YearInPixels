using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YearInPixels.Models.Data;
using YearInPixels.Services;

namespace YearInPixels.Controllers
{
    [Route("api/calendars/{calendarId}")]
    public class CalendarController : Controller
    {
        private readonly DatabaseContext _database;

        public CalendarController(DatabaseContext database)
        {
            _database = database;
        }

        [Route("")]
        public async Task<IActionResult> GetCalendar(string calendarId)
        {
            var calendar = await _database.Calendars
                .Include(c => c.DayLogs)
                .SingleOrDefaultAsync(c => c.Id == calendarId);

            return Ok(calendar);
        }

        [HttpPost]
        [Route("{month}/{day}/option")]
        public async Task<IActionResult> SelectOption(string calendarId, int month, int day, int? optionId)
        {
            var log = await _database.DayLogs.FindAsync(calendarId, month, day);
            if (log == null)
            {
                log = new DayLog {CalendarId = calendarId, Month = month, Day = day};
                _database.DayLogs.Add(log);
            }

            log.Option = optionId;

            if (String.IsNullOrWhiteSpace(log.Note) && log.Option == null)
                _database.DayLogs.Remove(log);

            await _database.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("{month}/{day}/note")]
        public async Task<IActionResult> EditNote(string calendarId, int month, int day, string note)
        {
            var log = await _database.DayLogs.FindAsync(calendarId, month, day);
            if (log == null)
            {
                log = new DayLog {CalendarId = calendarId, Month = month, Day = day};
                _database.DayLogs.Add(log);
            }

            log.Note = note;

            if (String.IsNullOrWhiteSpace(log.Note) && log.Option == null)
                _database.DayLogs.Remove(log);

            await _database.SaveChangesAsync();

            return Ok();
        }
    }
}
