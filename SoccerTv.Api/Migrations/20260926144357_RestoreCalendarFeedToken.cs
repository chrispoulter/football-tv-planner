using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerTv.Api.Migrations
{
    /// <inheritdoc />
    public partial class RestoreCalendarFeedToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "calendar_feed_token",
                table: "users",
                type: "text",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_calendar_feed_token",
                table: "users",
                column: "calendar_feed_token",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_users_calendar_feed_token", table: "users");

            migrationBuilder.DropColumn(name: "calendar_feed_token", table: "users");
        }
    }
}
