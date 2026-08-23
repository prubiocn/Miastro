using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Miastro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase6NatalSnapshotSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NatalCharts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PersonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    InputHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    IsApproximateBirthTime = table.Column<bool>(type: "INTEGER", nullable: false),
                    BirthLocalDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    BirthLocalTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    InstantUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Locality = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    IanaTimeZoneId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TzdbVersion = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    HouseSystem = table.Column<int>(type: "INTEGER", nullable: false),
                    CalculationProfileId = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    MiastroVersion = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Engine = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    EngineVersion = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    AdapterVersion = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    EphemerisVersion = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    CalculatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    InvalidatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    SupersededByChartId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NatalCharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NatalCharts_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NatalAspects",
                columns: table => new
                {
                    ChartId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstObject = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondObject = table.Column<int>(type: "INTEGER", nullable: false),
                    Kind = table.Column<int>(type: "INTEGER", nullable: false),
                    SeparationDegrees = table.Column<double>(type: "REAL", nullable: false),
                    ExactAngleDegrees = table.Column<double>(type: "REAL", nullable: false),
                    DeviationDegrees = table.Column<double>(type: "REAL", nullable: false),
                    AllowedOrbDegrees = table.Column<double>(type: "REAL", nullable: false),
                    UsedOrbDegrees = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NatalAspects", x => new { x.ChartId, x.FirstObject, x.SecondObject });
                    table.ForeignKey(
                        name: "FK_NatalAspects_NatalCharts_ChartId",
                        column: x => x.ChartId,
                        principalTable: "NatalCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NatalHouseCusps",
                columns: table => new
                {
                    ChartId = table.Column<Guid>(type: "TEXT", nullable: false),
                    HouseNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    LongitudeDegrees = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NatalHouseCusps", x => new { x.ChartId, x.HouseNumber });
                    table.ForeignKey(
                        name: "FK_NatalHouseCusps_NatalCharts_ChartId",
                        column: x => x.ChartId,
                        principalTable: "NatalCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NatalPlacements",
                columns: table => new
                {
                    ChartId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ObjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    LongitudeDegrees = table.Column<double>(type: "REAL", nullable: false),
                    LatitudeDegrees = table.Column<double>(type: "REAL", nullable: true),
                    DistanceAu = table.Column<double>(type: "REAL", nullable: true),
                    LongitudeSpeedDegreesPerDay = table.Column<double>(type: "REAL", nullable: true),
                    LatitudeSpeedDegreesPerDay = table.Column<double>(type: "REAL", nullable: true),
                    DistanceSpeedAuPerDay = table.Column<double>(type: "REAL", nullable: true),
                    Motion = table.Column<int>(type: "INTEGER", nullable: true),
                    ZodiacSign = table.Column<int>(type: "INTEGER", nullable: false),
                    DegreeInSign = table.Column<double>(type: "REAL", nullable: false),
                    HouseNumber = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NatalPlacements", x => new { x.ChartId, x.ObjectId });
                    table.ForeignKey(
                        name: "FK_NatalPlacements_NatalCharts_ChartId",
                        column: x => x.ChartId,
                        principalTable: "NatalCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NatalAspects_Kind",
                table: "NatalAspects",
                column: "Kind");

            migrationBuilder.CreateIndex(
                name: "IX_NatalCharts_CalculatedAtUtc",
                table: "NatalCharts",
                column: "CalculatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_NatalCharts_PersonId_InputHash",
                table: "NatalCharts",
                columns: new[] { "PersonId", "InputHash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NatalCharts_PersonId_Status",
                table: "NatalCharts",
                columns: new[] { "PersonId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_NatalPlacements_ObjectId",
                table: "NatalPlacements",
                column: "ObjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NatalAspects");

            migrationBuilder.DropTable(
                name: "NatalHouseCusps");

            migrationBuilder.DropTable(
                name: "NatalPlacements");

            migrationBuilder.DropTable(
                name: "NatalCharts");
        }
    }
}
