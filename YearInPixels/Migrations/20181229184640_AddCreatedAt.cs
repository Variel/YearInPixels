using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace YearInPixels.Migrations
{
    public partial class AddCreatedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calendars_Accounts_OwnerId",
                table: "Calendars");

            migrationBuilder.AlterColumn<long>(
                name: "OwnerId",
                table: "Calendars",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Calendars",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Accounts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 150, nullable: false),
                    Content = table.Column<string>(nullable: true),
                    IsJson = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Key);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Calendars_Accounts_OwnerId",
                table: "Calendars",
                column: "OwnerId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calendars_Accounts_OwnerId",
                table: "Calendars");

            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Calendars");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Accounts");

            migrationBuilder.AlterColumn<long>(
                name: "OwnerId",
                table: "Calendars",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Calendars_Accounts_OwnerId",
                table: "Calendars",
                column: "OwnerId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
