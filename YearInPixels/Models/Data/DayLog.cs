using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YearInPixels.Models.Data
{
    public class DayLog
    {
        [JsonIgnore]
        public Calendar Calendar { get; set; }
        [JsonIgnore]
        public string CalendarId { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public int? Option { get; set; }
        public string Note { get; set; }
    }
}
