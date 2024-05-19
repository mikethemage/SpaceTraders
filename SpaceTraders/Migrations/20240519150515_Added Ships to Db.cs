using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceTraders.Migrations
{
    /// <inheritdoc />
    public partial class AddedShipstoDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cooldown",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShipSymbol = table.Column<string>(type: "TEXT", nullable: false),
                    TotalSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    RemainingSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    Expiration = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cooldown", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipCargo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    Units = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipCargo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipCrew",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Current = table.Column<int>(type: "INTEGER", nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    Required = table.Column<int>(type: "INTEGER", nullable: false),
                    Rotation = table.Column<string>(type: "TEXT", nullable: false),
                    Morale = table.Column<int>(type: "INTEGER", nullable: false),
                    Wages = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipCrew", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipFuelConsumed",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipFuelConsumed", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipNavRouteWaypoint",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    SystemSymbol = table.Column<string>(type: "TEXT", nullable: false),
                    X = table.Column<int>(type: "INTEGER", nullable: false),
                    Y = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipNavRouteWaypoint", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipRegistration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    FactionSymbol = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipRegistration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShipEngineId = table.Column<int>(type: "INTEGER", nullable: true),
                    ShipFrameId = table.Column<int>(type: "INTEGER", nullable: true),
                    ShipModuleId = table.Column<int>(type: "INTEGER", nullable: true),
                    ShipMountId = table.Column<int>(type: "INTEGER", nullable: true),
                    ShipReactorId = table.Column<int>(type: "INTEGER", nullable: true),
                    Power = table.Column<int>(type: "INTEGER", nullable: true),
                    Crew = table.Column<int>(type: "INTEGER", nullable: true),
                    Slots = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipRequirements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipCargoItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Units = table.Column<int>(type: "INTEGER", nullable: false),
                    ShipCargoId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipCargoItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipCargoItem_ShipCargo_ShipCargoId",
                        column: x => x.ShipCargoId,
                        principalTable: "ShipCargo",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShipFuel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Current = table.Column<int>(type: "INTEGER", nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    ConsumedId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipFuel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipFuel_ShipFuelConsumed_ConsumedId",
                        column: x => x.ConsumedId,
                        principalTable: "ShipFuelConsumed",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShipNavRoute",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OriginId = table.Column<int>(type: "INTEGER", nullable: false),
                    DestinationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Arrival = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipNavRoute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipNavRoute_ShipNavRouteWaypoint_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "ShipNavRouteWaypoint",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipNavRoute_ShipNavRouteWaypoint_OriginId",
                        column: x => x.OriginId,
                        principalTable: "ShipNavRouteWaypoint",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipEngine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Condition = table.Column<double>(type: "REAL", nullable: false),
                    Integrity = table.Column<double>(type: "REAL", nullable: false),
                    Speed = table.Column<int>(type: "INTEGER", nullable: false),
                    ShipEngineId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipEngine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipEngine_ShipRequirements_ShipEngineId",
                        column: x => x.ShipEngineId,
                        principalTable: "ShipRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipFrame",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ModuleSlots = table.Column<int>(type: "INTEGER", nullable: false),
                    MountingPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelCapacity = table.Column<int>(type: "INTEGER", nullable: false),
                    Condition = table.Column<double>(type: "REAL", nullable: false),
                    Integrity = table.Column<double>(type: "REAL", nullable: false),
                    ShipFrameId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipFrame", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipFrame_ShipRequirements_ShipFrameId",
                        column: x => x.ShipFrameId,
                        principalTable: "ShipRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipReactor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Condition = table.Column<double>(type: "REAL", nullable: false),
                    Integrity = table.Column<double>(type: "REAL", nullable: false),
                    PowerOutput = table.Column<int>(type: "INTEGER", nullable: false),
                    ShipReactorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipReactor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipReactor_ShipRequirements_ShipReactorId",
                        column: x => x.ShipReactorId,
                        principalTable: "ShipRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipNav",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SystemSymbol = table.Column<string>(type: "TEXT", nullable: false),
                    WaypointSymbol = table.Column<string>(type: "TEXT", nullable: false),
                    RouteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    FlightMode = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipNav", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipNav_ShipNavRoute_RouteId",
                        column: x => x.RouteId,
                        principalTable: "ShipNavRoute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    NavId = table.Column<int>(type: "INTEGER", nullable: false),
                    CrewId = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelId = table.Column<int>(type: "INTEGER", nullable: false),
                    CooldownId = table.Column<int>(type: "INTEGER", nullable: false),
                    FrameId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReactorId = table.Column<int>(type: "INTEGER", nullable: false),
                    EngineId = table.Column<int>(type: "INTEGER", nullable: false),
                    RegistrationId = table.Column<int>(type: "INTEGER", nullable: false),
                    CargoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ships_Cooldown_CooldownId",
                        column: x => x.CooldownId,
                        principalTable: "Cooldown",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ships_ShipCargo_CargoId",
                        column: x => x.CargoId,
                        principalTable: "ShipCargo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ships_ShipCrew_CrewId",
                        column: x => x.CrewId,
                        principalTable: "ShipCrew",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ships_ShipEngine_EngineId",
                        column: x => x.EngineId,
                        principalTable: "ShipEngine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ships_ShipFrame_FrameId",
                        column: x => x.FrameId,
                        principalTable: "ShipFrame",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ships_ShipFuel_FuelId",
                        column: x => x.FuelId,
                        principalTable: "ShipFuel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ships_ShipNav_NavId",
                        column: x => x.NavId,
                        principalTable: "ShipNav",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ships_ShipReactor_ReactorId",
                        column: x => x.ReactorId,
                        principalTable: "ShipReactor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ships_ShipRegistration_RegistrationId",
                        column: x => x.RegistrationId,
                        principalTable: "ShipRegistration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipModule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: true),
                    Range = table.Column<int>(type: "INTEGER", nullable: true),
                    ShipModuleId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShipId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipModule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipModule_ShipRequirements_ShipModuleId",
                        column: x => x.ShipModuleId,
                        principalTable: "ShipRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipModule_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShipMount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Strength = table.Column<int>(type: "INTEGER", nullable: true),
                    Deposits = table.Column<string>(type: "TEXT", nullable: true),
                    ShipMountId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShipId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipMount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipMount_ShipRequirements_ShipMountId",
                        column: x => x.ShipMountId,
                        principalTable: "ShipRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipMount_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipCargoItem_ShipCargoId",
                table: "ShipCargoItem",
                column: "ShipCargoId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipEngine_ShipEngineId",
                table: "ShipEngine",
                column: "ShipEngineId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipFrame_ShipFrameId",
                table: "ShipFrame",
                column: "ShipFrameId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipFuel_ConsumedId",
                table: "ShipFuel",
                column: "ConsumedId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipModule_ShipId",
                table: "ShipModule",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipModule_ShipModuleId",
                table: "ShipModule",
                column: "ShipModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipMount_ShipId",
                table: "ShipMount",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipMount_ShipMountId",
                table: "ShipMount",
                column: "ShipMountId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipNav_RouteId",
                table: "ShipNav",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipNavRoute_DestinationId",
                table: "ShipNavRoute",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipNavRoute_OriginId",
                table: "ShipNavRoute",
                column: "OriginId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipReactor_ShipReactorId",
                table: "ShipReactor",
                column: "ShipReactorId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_CargoId",
                table: "Ships",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_CooldownId",
                table: "Ships",
                column: "CooldownId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_CrewId",
                table: "Ships",
                column: "CrewId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_EngineId",
                table: "Ships",
                column: "EngineId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_FrameId",
                table: "Ships",
                column: "FrameId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_FuelId",
                table: "Ships",
                column: "FuelId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_NavId",
                table: "Ships",
                column: "NavId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_ReactorId",
                table: "Ships",
                column: "ReactorId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_RegistrationId",
                table: "Ships",
                column: "RegistrationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipCargoItem");

            migrationBuilder.DropTable(
                name: "ShipModule");

            migrationBuilder.DropTable(
                name: "ShipMount");

            migrationBuilder.DropTable(
                name: "Ships");

            migrationBuilder.DropTable(
                name: "Cooldown");

            migrationBuilder.DropTable(
                name: "ShipCargo");

            migrationBuilder.DropTable(
                name: "ShipCrew");

            migrationBuilder.DropTable(
                name: "ShipEngine");

            migrationBuilder.DropTable(
                name: "ShipFrame");

            migrationBuilder.DropTable(
                name: "ShipFuel");

            migrationBuilder.DropTable(
                name: "ShipNav");

            migrationBuilder.DropTable(
                name: "ShipReactor");

            migrationBuilder.DropTable(
                name: "ShipRegistration");

            migrationBuilder.DropTable(
                name: "ShipFuelConsumed");

            migrationBuilder.DropTable(
                name: "ShipNavRoute");

            migrationBuilder.DropTable(
                name: "ShipRequirements");

            migrationBuilder.DropTable(
                name: "ShipNavRouteWaypoint");
        }
    }
}
