using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace SoccerTv.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    type = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
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
                    name = table.Column<string>(type: "text", nullable: false),
                    short_name = table.Column<string>(type: "text", nullable: false),
                    country = table.Column<string>(type: "text", nullable: false),
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
                    name = table.Column<string>(type: "text", nullable: false),
                    short_name = table.Column<string>(type: "text", nullable: false),
                    badge_url = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teams", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    email_address = table.Column<string>(type: "text", nullable: false),
                    normalized_email_address = table.Column<string>(
                        type: "text",
                        nullable: false,
                        computedColumnSql: "lower(email_address)",
                        stored: true
                    ),
                    password = table.Column<string>(type: "text", nullable: true),
                    password_reset_token = table.Column<string>(type: "text", nullable: true),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    is_locked_out = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        defaultValue: false
                    ),
                    roles = table.Column<string[]>(type: "text[]", nullable: true),
                    calendar_feed_token = table.Column<string>(type: "text", nullable: true),
                    reminder_minutes_before = table.Column<int>(
                        type: "integer",
                        nullable: false,
                        defaultValue: 30
                    ),
                    search_vector = table
                        .Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation(
                            "Npgsql:TsVectorProperties",
                            new[] { "first_name", "last_name", "email_address" }
                        ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "fixtures",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    source = table.Column<string>(type: "text", nullable: false),
                    external_id = table.Column<string>(type: "text", nullable: false),
                    competition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    home_team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    away_team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kickoff_utc = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    venue = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fixtures", x => x.id);
                    table.ForeignKey(
                        name: "fk_fixtures_competitions_competition_id",
                        column: x => x.competition_id,
                        principalTable: "competitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_fixtures_teams_away_team_id",
                        column: x => x.away_team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_fixtures_teams_home_team_id",
                        column: x => x.home_team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
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

            migrationBuilder.CreateTable(
                name: "user_fixtures",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fixture_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_fixtures", x => new { x.user_id, x.fixture_id });
                    table.ForeignKey(
                        name: "fk_user_fixtures_fixtures_fixture_id",
                        column: x => x.fixture_id,
                        principalTable: "fixtures",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_user_fixtures_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
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
                name: "ix_fixtures_kickoff_utc",
                table: "fixtures",
                column: "kickoff_utc"
            );

            migrationBuilder.CreateIndex(
                name: "ix_fixtures_source_external_id",
                table: "fixtures",
                columns: new[] { "source", "external_id" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_teams_name",
                table: "teams",
                column: "name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_fixtures_fixture_id",
                table: "user_fixtures",
                column: "fixture_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_calendar_feed_token",
                table: "users",
                column: "calendar_feed_token",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_normalized_email_address",
                table: "users",
                column: "normalized_email_address",
                unique: true
            );

            migrationBuilder
                .CreateIndex(
                    name: "ix_users_search_vector",
                    table: "users",
                    column: "search_vector"
                )
                .Annotation("Npgsql:IndexMethod", "gin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "fixture_broadcasts");

            migrationBuilder.DropTable(name: "user_fixtures");

            migrationBuilder.DropTable(name: "channels");

            migrationBuilder.DropTable(name: "fixtures");

            migrationBuilder.DropTable(name: "users");

            migrationBuilder.DropTable(name: "competitions");

            migrationBuilder.DropTable(name: "teams");
        }
    }
}
