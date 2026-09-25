using System;
using System.Collections.Generic;
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
                    competition = table.Column<string>(type: "text", nullable: false),
                    home_team = table.Column<string>(type: "text", nullable: false),
                    away_team = table.Column<string>(type: "text", nullable: false),
                    kickoff_utc = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    channels = table.Column<List<string>>(type: "text[]", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fixtures", x => x.id);
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
                name: "ix_fixtures_competition",
                table: "fixtures",
                column: "competition"
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
                name: "IX_user_fixtures_fixture_id",
                table: "user_fixtures",
                column: "fixture_id"
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
            migrationBuilder.DropTable(name: "user_fixtures");

            migrationBuilder.DropTable(name: "fixtures");

            migrationBuilder.DropTable(name: "users");
        }
    }
}
