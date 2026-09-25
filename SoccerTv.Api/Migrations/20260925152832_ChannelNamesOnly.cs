using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerTv.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChannelNamesOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // jsonb can't be cast to text[] in place, so copy the names (keeping their
            // order) into a new column and swap it in.
            migrationBuilder.AddColumn<List<string>>(
                name: "channel_names",
                table: "fixtures",
                type: "text[]",
                nullable: false,
                defaultValueSql: "'{}'"
            );

            migrationBuilder.Sql(
                """
                UPDATE fixtures
                SET channel_names = ARRAY(
                    SELECT c.value ->> 'name'
                    FROM jsonb_array_elements(COALESCE(channels, '[]'::jsonb)) WITH ORDINALITY AS c(value, position)
                    ORDER BY c.position
                );
                """
            );

            migrationBuilder.DropColumn(name: "channels", table: "fixtures");

            migrationBuilder.RenameColumn(
                name: "channel_names",
                table: "fixtures",
                newName: "channels"
            );

            migrationBuilder.Sql("ALTER TABLE fixtures ALTER COLUMN channels DROP DEFAULT;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "channel_objects",
                table: "fixtures",
                type: "jsonb",
                nullable: true
            );

            // Provider and type aren't recoverable; the next sync fills them back in.
            migrationBuilder.Sql(
                """
                UPDATE fixtures
                SET channel_objects = COALESCE(
                    (
                        SELECT jsonb_agg(jsonb_build_object('name', c.name) ORDER BY c.position)
                        FROM unnest(channels) WITH ORDINALITY AS c(name, position)
                    ),
                    '[]'::jsonb
                );
                """
            );

            migrationBuilder.DropColumn(name: "channels", table: "fixtures");

            migrationBuilder.RenameColumn(
                name: "channel_objects",
                table: "fixtures",
                newName: "channels"
            );
        }
    }
}
