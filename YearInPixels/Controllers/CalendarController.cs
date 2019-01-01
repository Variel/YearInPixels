using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Variel.Web.Session;
using YearInPixels.Models.Data;
using YearInPixels.Models.Response;
using YearInPixels.Services;

namespace YearInPixels.Controllers
{
    [Route("api/calendars/{calendarId}")]
    public class CalendarController : Controller
    {
        private readonly DatabaseContext _database;
        private readonly SessionService<Account> _session;

        public CalendarController(DatabaseContext database, SessionService<Account> session)
        {
            _database = database;
            _session = session;
        }

        [Route("")]
        public async Task<IActionResult> GetCalendar(string calendarId)
        {
            if (!await HasAuthorityAsync(calendarId, CalendarSharingOption.View))
                return StatusCode(403, new ErrorResponseModel("볼 권한이 없습니다"));

            var calendar = await _database.Calendars
                .Include(c => c.DayLogs)
                .SingleOrDefaultAsync(c => c.Id == calendarId);

            return Ok(calendar);
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> DeleteCalendar(string calendarId)
        {
            var user = await _session.GetUserAsync();
            if (user == null)
                return StatusCode(401, new ErrorResponseModel("로그인이 필요합니다"));

            var calendar = await _database.Calendars.FindAsync(calendarId);
            if (calendar.OwnerId != user.Id)
                return StatusCode(403, new ErrorResponseModel("본인의 달력만 삭제할 수 있습니다"));

            _database.Calendars.Remove(calendar);

            await _database.SaveChangesAsync();

            return Ok(new { });
        }

        [HttpPost]
        [Route("title")]
        public async Task<IActionResult> UpdateTitle(string calendarId, string title)
        {
            if (!await HasAuthorityAsync(calendarId, CalendarSharingOption.Collaborate))
                return StatusCode(403, new ErrorResponseModel("편집할 권한이 없습니다"));

            var calendar = await _database.Calendars
                .SingleOrDefaultAsync(c => c.Id == calendarId);

            calendar.Title = title;

            await _database.SaveChangesAsync();

            return Ok(new { });
        }

        [HttpPost]
        [Route("options")]
        public async Task<IActionResult> UpdateOptions(string calendarId, string optionsString)
        {
            if (!await HasAuthorityAsync(calendarId, CalendarSharingOption.Collaborate))
                return StatusCode(403, new ErrorResponseModel("편집할 권한이 없습니다"));

            var calendar = await _database.Calendars.FindAsync(calendarId);

            var options = JsonConvert.DeserializeObject<Option[]>(optionsString);
            calendar.Options = options;

            await _database.SaveChangesAsync();

            return Ok(new { });
        }

        [HttpPost]
        [Route("{month}/{day}/option")]
        public async Task<IActionResult> SelectOption(string calendarId, int month, int day, int? optionId)
        {
            if (!await HasAuthorityAsync(calendarId, CalendarSharingOption.Collaborate))
                return StatusCode(403, new ErrorResponseModel("편집할 권한이 없습니다"));

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

            return Ok(new { });
        }

        [HttpPost]
        [Route("{month}/{day}/note")]
        public async Task<IActionResult> EditNote(string calendarId, int month, int day, string note)
        {
            if (!await HasAuthorityAsync(calendarId, CalendarSharingOption.Collaborate))
                return StatusCode(403, new ErrorResponseModel("편집할 권한이 없습니다"));

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

            return Ok(new { });
        }

        [HttpGet]
        [Route("claimOwnership")]
        public async Task<IActionResult> ClaimOwnership(string calendarId)
        {
            var user = await _session.GetUserAsync();
            if (user == null)
                return StatusCode(401, new ErrorResponseModel("로그인이 필요합니다"));

            var calendar = await _database.Calendars.FindAsync(calendarId);

            if (calendar.OwnerId != null)
                return StatusCode(403, new ErrorResponseModel("이미 소유자가 있는 달력입니다"));

            var deviceId = Request.Cookies["deviceId"];
            if (calendar.OwnerDeviceId != null && calendar.OwnerDeviceId != deviceId)
                return StatusCode(403, new ErrorResponseModel("소유할 권한이 없습니다"));

            calendar.Owner = user;

            await _database.SaveChangesAsync();

            return Ok(new { });
        }

        [HttpPost]
        [Route("authorities/view")]
        public async Task<IActionResult> UpdateViewAuthority(string calendarId, bool value)
        {
            var user = await _session.GetUserAsync();
            if (user == null)
                return StatusCode(401, new ErrorResponseModel("로그인이 필요합니다"));

            var calendar = await _database.Calendars.FindAsync(calendarId);
            if (calendar.OwnerId != user.Id)
                return StatusCode(403, new ErrorResponseModel("본인의 달력만 권한을 편집할 수 있습니다"));

            if (value)
                calendar.SharingOption |= CalendarSharingOption.View;
            else
                calendar.SharingOption &= ~CalendarSharingOption.View;

            await _database.SaveChangesAsync();

            return Ok(new { });
        }

        private async Task<bool> HasAuthorityAsync(string calendarId, CalendarSharingOption authority)
        {
            var calendar = await _database.Calendars.FindAsync(calendarId);

            if (calendar == null)
            {
                Response.Cookies.Delete("lastCalendarId");
                return false;
            }

            var user = await _session.GetUserAsync();

            if (calendar.OwnerId != null)
            {
                if (user?.Id != calendar.OwnerId)
                {
                    if ((calendar.SharingOption & authority) != authority)
                    {
                        return false;
                    }
                }

                return true;
            }

            if (calendar.OwnerDeviceId != null)
                if (Request.Cookies["deviceId"] != calendar.OwnerDeviceId)
                    if ((calendar.SharingOption & authority) != authority)
                    {
                        return false;
                    }

            return true;
        }
    }
}
