using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Miastro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase5PersonFunctionalSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicalProbes");

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 260, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: true),
                    PrivateNote = table.Column<string>(type: "TEXT", maxLength: 10000, nullable: true),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifiedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    LastConsultationAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BirthData",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LocalDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    TimePrecision = table.Column<int>(type: "INTEGER", nullable: false),
                    LocalTime = table.Column<TimeOnly>(type: "TEXT", nullable: true),
                    RangeStart = table.Column<TimeOnly>(type: "TEXT", nullable: true),
                    RangeEnd = table.Column<TimeOnly>(type: "TEXT", nullable: true),
                    DayPeriod = table.Column<int>(type: "INTEGER", nullable: true),
                    GeoNameId = table.Column<long>(type: "INTEGER", nullable: false),
                    Locality = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Region = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    Subregion = table.Column<string>(type: "TEXT", maxLength: 160, nullable: true),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    IanaTimeZoneId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TzdbVersion = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    TemporalResolutionState = table.Column<int>(type: "INTEGER", nullable: false),
                    HistoricalOffsetSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    ResolvedInstantUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    AmbiguousEarlierOffsetSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    AmbiguousEarlierInstantUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    AmbiguousLaterOffsetSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    AmbiguousLaterInstantUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    AmbiguousSelectedCandidate = table.Column<int>(type: "INTEGER", nullable: true),
                    AmbiguousSelectionRecordedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ManualCoordinateOverride = table.Column<bool>(type: "INTEGER", nullable: false),
                    OriginalGeoNamesLatitude = table.Column<double>(type: "REAL", nullable: true),
                    OriginalGeoNamesLongitude = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthData", x => x.PersonId);
                    table.ForeignKey(
                        name: "FK_BirthData_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CurrentResidences",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Locality = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    GeoNameId = table.Column<long>(type: "INTEGER", nullable: true),
                    Region = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    IanaTimeZoneId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentResidences", x => x.PersonId);
                    table.ForeignKey(
                        name: "FK_CurrentResidences_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PersonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventType = table.Column<int>(type: "INTEGER", nullable: false),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 240, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonHistory_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BirthData_GeoNameId",
                table: "BirthData",
                column: "GeoNameId");

            migrationBuilder.CreateIndex(
                name: "IX_BirthData_IanaTimeZoneId",
                table: "BirthData",
                column: "IanaTimeZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentResidences_GeoNameId",
                table: "CurrentResidences",
                column: "GeoNameId");

            migrationBuilder.CreateIndex(
                name: "IX_People_FirstName",
                table: "People",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_People_IsFavorite",
                table: "People",
                column: "IsFavorite");

            migrationBuilder.CreateIndex(
                name: "IX_People_LastConsultationAtUtc",
                table: "People",
                column: "LastConsultationAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_People_LastName",
                table: "People",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_People_NormalizedName",
                table: "People",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_PersonHistory_PersonId_OccurredAtUtc",
                table: "PersonHistory",
                columns: new[] { "PersonId", "OccurredAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BirthData");

            migrationBuilder.DropTable(
                name: "CurrentResidences");

            migrationBuilder.DropTable(
                name: "PersonHistory");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.CreateTable(
                name: "TechnicalProbes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicalProbes", x => x.Id);
                });
        }
    }
}
