using Microsoft.EntityFrameworkCore.Migrations;

namespace YearInPixels.Migrations
{
    public partial class AddCalendarAuthority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Calendars",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OwnerDeviceId",
                table: "Calendars",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SharingOption",
                table: "Calendars",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Calendars");

            migrationBuilder.DropColumn(
                name: "OwnerDeviceId",
                table: "Calendars");

            migrationBuilder.DropColumn(
                name: "SharingOption",
                table: "Calendars");
        }
    }
}
