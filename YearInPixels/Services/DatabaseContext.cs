using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Variel.Web.Authentication;
using Variel.Web.Common;
using YearInPixels.Models.Data;

namespace YearInPixels.Services
{
    public class DatabaseContext : AuthenticationDatabaseContext<Account>, ISettingsDatabaseContext
    {
        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<DayLog> DayLogs { get; set; }

        public DatabaseContext() { }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Credential<Account>>()
                .HasKey(c => new {c.Provider, c.ProviderId});

            modelBuilder.Entity<DayLog>()
                .HasKey(d => new {d.CalendarId, d.Month, d.Day});

            modelBuilder.Entity<Calendar>()
                .HasMany(c => c.DayLogs)
                .WithOne(d => d.Calendar)
                .HasForeignKey(d => d.CalendarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Calendar>()
                .Property<string>("OptionsStr")
                .HasField("_options");
        }
    }
}
