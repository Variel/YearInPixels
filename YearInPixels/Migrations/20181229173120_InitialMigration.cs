using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace YearInPixels.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Calendars",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 32, nullable: false),
                    Title = table.Column<string>(maxLength: 64, nullable: true),
                    Year = table.Column<int>(nullable: false),
                    OwnerId = table.Column<long>(nullable: false),
                    OptionsStr = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Calendars_Accounts_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Credentials",
                columns: table => new
                {
                    Provider = table.Column<int>(nullable: false),
                    ProviderId = table.Column<string>(maxLength: 128, nullable: false),
                    Key = table.Column<string>(nullable: true),
                    AccountId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credentials", x => new { x.Provider, x.ProviderId });
                    table.ForeignKey(
                        name: "FK_Credentials_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DayLogs",
                columns: table => new
                {
                    CalendarId = table.Column<string>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    Day = table.Column<int>(nullable: false),
                    Option = table.Column<int>(nullable: true),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayLogs", x => new { x.CalendarId, x.Month, x.Day });
                    table.ForeignKey(
                        name: "FK_DayLogs_Calendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calendars_OwnerId",
                table: "Calendars",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Credentials_AccountId",
                table: "Credentials",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Credentials");

            migrationBuilder.DropTable(
                name: "DayLogs");

            migrationBuilder.DropTable(
                name: "Calendars");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
