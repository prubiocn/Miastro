using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Miastro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase6BirthDataSnapshotIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AmbiguousSelection",
                table: "NatalCharts",
                type: "TEXT",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BirthDataHash",
                table: "NatalCharts",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BirthDataVersion",
                table: "NatalCharts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BirthTimePrecision",
                table: "NatalCharts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "GeoNameId",
                table: "NatalCharts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "HistoricalOffsetSeconds",
                table: "NatalCharts",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmbiguousSelection",
                table: "NatalCharts");

            migrationBuilder.DropColumn(
                name: "BirthDataHash",
                table: "NatalCharts");

            migrationBuilder.DropColumn(
                name: "BirthDataVersion",
                table: "NatalCharts");

            migrationBuilder.DropColumn(
                name: "BirthTimePrecision",
                table: "NatalCharts");

            migrationBuilder.DropColumn(
                name: "GeoNameId",
                table: "NatalCharts");

            migrationBuilder.DropColumn(
                name: "HistoricalOffsetSeconds",
                table: "NatalCharts");
        }
    }
}
