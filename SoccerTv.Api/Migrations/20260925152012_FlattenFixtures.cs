using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerTv.Api.Migrations
{
    /// <inheritdoc />
    public partial class FlattenFixtures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "competition",
                table: "fixtures",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<int>(
                name: "competition_sort_order",
                table: "fixtures",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<string>(
                name: "home_team",
                table: "fixtures",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "away_team",
                table: "fixtures",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "channels",
                table: "fixtures",
                type: "jsonb",
                nullable: true
            );

            // Copy existing data across so fixtures (and bookmarks to them) stay intact
            // before the lookup tables are dropped.
            migrationBuilder.Sql(
                """
                UPDATE fixtures f
                SET competition = c.name,
                    competition_sort_order = c.sort_order,
                    home_team = h.name,
                    away_team = a.name,
                    channels = COALESCE(
                        (
                            SELECT jsonb_agg(
                                jsonb_build_object('name', ch.name, 'provider', ch.provider, 'type', ch.type)
                                ORDER BY ch.sort_order, ch.name
                            )
                            FROM fixture_broadcasts b
                            JOIN channels ch ON ch.id = b.channel_id
                            WHERE b.fixture_id = f.id
                        ),
                        '[]'::jsonb
                    )
                FROM competitions c, teams h, teams a
                WHERE c.id = f.competition_id
                  AND h.id = f.home_team_id
                  AND a.id = f.away_team_id;
                """
            );

            migrationBuilder.DropForeignKey(
                name: "fk_fixtures_competitions_competition_id",
                table: "fixtures"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_fixtures_teams_away_team_id",
                table: "fixtures"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_fixtures_teams_home_team_id",
                table: "fixtures"
            );

            migrationBuilder.DropTable(name: "competitions");

            migrationBuilder.DropTable(name: "fixture_broadcasts");

            migrationBuilder.DropTable(name: "teams");

            migrationBuilder.DropTable(name: "channels");

            migrationBuilder.DropIndex(name: "IX_fixtures_away_team_id", table: "fixtures");

            migrationBuilder.DropIndex(name: "IX_fixtures_competition_id", table: "fixtures");

            migrationBuilder.DropIndex(name: "IX_fixtures_home_team_id", table: "fixtures");

            migrationBuilder.DropColumn(name: "away_team_id", table: "fixtures");

            migrationBuilder.DropColumn(name: "competition_id", table: "fixtures");

            migrationBuilder.DropColumn(name: "home_team_id", table: "fixtures");

            migrationBuilder.DropColumn(name: "updated_at", table: "fixtures");

            migrationBuilder.DropColumn(name: "venue", table: "fixtures");

            migrationBuilder.CreateIndex(
                name: "ix_fixtures_competition",
                table: "fixtures",
                column: "competition"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_fixtures_competition", table: "fixtures");

            migrationBuilder.DropColumn(name: "away_team", table: "fixtures");

            migrationBuilder.DropColumn(name: "channels", table: "fixtures");

            migrationBuilder.DropColumn(name: "competition", table: "fixtures");

            migrationBuilder.DropColumn(name: "competition_sort_order", table: "fixtures");

            migrationBuilder.DropColumn(name: "home_team", table: "fixtures");

            migrationBuilder.AddColumn<Guid>(
                name: "away_team_id",
                table: "fixtures",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.AddColumn<Guid>(
                name: "competition_id",
                table: "fixtures",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.AddColumn<Guid>(
                name: "home_team_id",
                table: "fixtures",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "fixtures",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(
                    new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    new TimeSpan(0, 0, 0, 0, 0)
                )
            );

            migrationBuilder.AddColumn<string>(
                name: "venue",
                table: "fixtures",
                type: "text",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "channels",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    name = table.Column<string>(type: "text", nullable: false),
                    provider = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_channels", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "competitions",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    country = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    short_name = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_competitions", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    badge_url = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    short_name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teams", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "fixture_broadcasts",
                columns: table => new
                {
                    fixture_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "pk_fixture_broadcasts",
                        x => new { x.fixture_id, x.channel_id }
                    );
                    table.ForeignKey(
                        name: "fk_fixture_broadcasts_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_fixture_broadcasts_fixtures_fixture_id",
                        column: x => x.fixture_id,
                        principalTable: "fixtures",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_fixtures_away_team_id",
                table: "fixtures",
                column: "away_team_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_fixtures_competition_id",
                table: "fixtures",
                column: "competition_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_fixtures_home_team_id",
                table: "fixtures",
                column: "home_team_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_channels_name",
                table: "channels",
                column: "name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_competitions_name",
                table: "competitions",
                column: "name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_fixture_broadcasts_channel_id",
                table: "fixture_broadcasts",
                column: "channel_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_teams_name",
                table: "teams",
                column: "name",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "fk_fixtures_competitions_competition_id",
                table: "fixtures",
                column: "competition_id",
                principalTable: "competitions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "fk_fixtures_teams_away_team_id",
                table: "fixtures",
                column: "away_team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "fk_fixtures_teams_home_team_id",
                table: "fixtures",
                column: "home_team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );
        }
    }
}
