using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Variel.Web.Common;

namespace YearInPixels.Models.Data
{
    public class Account : IAccount
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public ICollection<Calendar> Calendars { get; set; }
    }
}
