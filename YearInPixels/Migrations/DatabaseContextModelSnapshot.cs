﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using YearInPixels.Services;

namespace YearInPixels.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Variel.Web.Authentication.Credential<YearInPixels.Models.Data.Account>", b =>
                {
                    b.Property<int>("Provider");

                    b.Property<string>("ProviderId")
                        .HasMaxLength(128);

                    b.Property<long>("AccountId");

                    b.Property<string>("Key");

                    b.HasKey("Provider", "ProviderId");

                    b.HasIndex("AccountId");

                    b.ToTable("Credentials");
                });

            modelBuilder.Entity("Variel.Web.Common.AppSetting", b =>
                {
                    b.Property<string>("Key")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(150);

                    b.Property<string>("Content");

                    b.Property<bool>("IsJson");

                    b.HasKey("Key");

                    b.ToTable("AppSettings");
                });

            modelBuilder.Entity("YearInPixels.Models.Data.Account", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("YearInPixels.Models.Data.Calendar", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(32);

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<string>("OptionsStr");

                    b.Property<long?>("OwnerId");

                    b.Property<string>("Title")
                        .HasMaxLength(64);

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Calendars");
                });

            modelBuilder.Entity("YearInPixels.Models.Data.DayLog", b =>
                {
                    b.Property<string>("CalendarId");

                    b.Property<int>("Month");

                    b.Property<int>("Day");

                    b.Property<string>("Note");

                    b.Property<int?>("Option");

                    b.HasKey("CalendarId", "Month", "Day");

                    b.ToTable("DayLogs");
                });

            modelBuilder.Entity("Variel.Web.Authentication.Credential<YearInPixels.Models.Data.Account>", b =>
                {
                    b.HasOne("YearInPixels.Models.Data.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("YearInPixels.Models.Data.Calendar", b =>
                {
                    b.HasOne("YearInPixels.Models.Data.Account", "Owner")
                        .WithMany("Calendars")
                        .HasForeignKey("OwnerId");
                });

            modelBuilder.Entity("YearInPixels.Models.Data.DayLog", b =>
                {
                    b.HasOne("YearInPixels.Models.Data.Calendar", "Calendar")
                        .WithMany("DayLogs")
                        .HasForeignKey("CalendarId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
