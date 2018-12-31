using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YearInPixels.Helpers;

namespace YearInPixels.Models.Data
{
    public class Calendar
    {
        [Key, MaxLength(32)]
        public string Id { get; set; } = RandomIdentityGenerator.Generate();

        [MaxLength(64)]
        public string Title { get; set; }
        public int Year { get; set; } = DateTimeOffset.Now.Year;

        private string _options;

        [NotMapped]
        public Option[] Options
        {
            get => JsonConvert.DeserializeObject<Option[]>(_options ?? "null");
            set => _options = JsonConvert.SerializeObject(value);
        }

        public ICollection<DayLog> DayLogs { get; set; } = new HashSet<DayLog>();

        public long? OwnerId { get; set; }
        [JsonIgnore]
        public Account Owner { get; set; }

        [MaxLength(32)]
        public string OwnerDeviceId { get; set; }

        public bool IsPrivate { get; set; }
        public CalendarSharingOption SharingOption { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    }

    [Flags]
    public enum CalendarSharingOption
    {
        None = 0,
        View = 1 << 0,
        Comment = 1 << 1,
        Collaborate = 1 << 2
    }
}
