using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerTv.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFixtureStatusAndCompetitionSortOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "competition_sort_order", table: "fixtures");

            migrationBuilder.DropColumn(name: "status", table: "fixtures");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "competition_sort_order",
                table: "fixtures",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "fixtures",
                type: "text",
                nullable: false,
                defaultValue: ""
            );
        }
    }
}
