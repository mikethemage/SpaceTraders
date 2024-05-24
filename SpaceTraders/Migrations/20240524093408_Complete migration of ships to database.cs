using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceTraders.Migrations
{
    /// <inheritdoc />
    public partial class Completemigrationofshipstodatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketTradeGood_Markets_MarketId",
                table: "MarketTradeGood");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketTransaction_Markets_MarketId",
                table: "MarketTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipCargoItem_ShipCargo_ShipCargoId",
                table: "ShipCargoItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipFuel_ShipFuelConsumed_ConsumedId",
                table: "ShipFuel");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipModule_Ships_ShipId",
                table: "ShipModule");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipMount_Ships_ShipId",
                table: "ShipMount");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipNav_ShipNavRoute_RouteId",
                table: "ShipNav");

            migrationBuilder.DropForeignKey(
                name: "FK_Ships_Cooldown_CooldownId",
                table: "Ships");

            migrationBuilder.DropForeignKey(
                name: "FK_Ships_ShipCargo_CargoId",
                table: "Ships");

            migrationBuilder.DropForeignKey(
                name: "FK_Ships_ShipCrew_CrewId",
                table: "Ships");

            migrationBuilder.DropForeignKey(
                name: "FK_Ships_ShipEngine_EngineId",
                table: "Ships");

            migrationBuilder.DropForeignKey(
                name: "FK_Ships_ShipFrame_FrameId",
                table: "Ships");

            migrationBuilder.DropForeignKey(
                name: "FK_Ships_ShipFuel_FuelId",
                table: "Ships");

            migrationBuilder.DropForeignKey(
                name: "FK_Ships_ShipNav_NavId",
                table: "Ships");

            migrationBuilder.DropForeignKey(
                name: "FK_Ships_ShipReactor_ReactorId",
                table: "Ships");

            migrationBuilder.DropForeignKey(
                name: "FK_Ships_ShipRegistration_RegistrationId",
                table: "Ships");

            migrationBuilder.DropForeignKey(
                name: "FK_WaypointModifier_Waypoints_WaypointId",
                table: "WaypointModifier");

            migrationBuilder.DropForeignKey(
                name: "FK_WaypointOrbital_Waypoints_WaypointId",
                table: "WaypointOrbital");

            migrationBuilder.DropForeignKey(
                name: "FK_Waypoints_Chart_ChartId",
                table: "Waypoints");

            migrationBuilder.DropForeignKey(
                name: "FK_Waypoints_WaypointFaction_FactionId",
                table: "Waypoints");

            migrationBuilder.DropForeignKey(
                name: "FK_WaypointTrait_Waypoints_WaypointId",
                table: "WaypointTrait");

            migrationBuilder.DropIndex(
                name: "IX_Waypoints_ChartId",
                table: "Waypoints");

            migrationBuilder.DropIndex(
                name: "IX_Waypoints_FactionId",
                table: "Waypoints");

            migrationBuilder.DropIndex(
                name: "IX_Ships_CargoId",
                table: "Ships");

            migrationBuilder.DropIndex(
                name: "IX_Ships_CooldownId",
                table: "Ships");

            migrationBuilder.DropIndex(
                name: "IX_Ships_CrewId",
                table: "Ships");

            migrationBuilder.DropIndex(
                name: "IX_Ships_EngineId",
                table: "Ships");

            migrationBuilder.DropIndex(
                name: "IX_Ships_FrameId",
                table: "Ships");

            migrationBuilder.DropIndex(
                name: "IX_Ships_FuelId",
                table: "Ships");

            migrationBuilder.DropIndex(
                name: "IX_Ships_NavId",
                table: "Ships");

            migrationBuilder.DropIndex(
                name: "IX_Ships_ReactorId",
                table: "Ships");

            migrationBuilder.DropIndex(
                name: "IX_Ships_RegistrationId",
                table: "Ships");

            migrationBuilder.DropIndex(
                name: "IX_ShipNav_RouteId",
                table: "ShipNav");

            migrationBuilder.DropIndex(
                name: "IX_ShipFuel_ConsumedId",
                table: "ShipFuel");

            migrationBuilder.DropColumn(
                name: "ChartId",
                table: "Waypoints");

            migrationBuilder.DropColumn(
                name: "FactionId",
                table: "Waypoints");

            migrationBuilder.DropColumn(
                name: "CargoId",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "CooldownId",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "CrewId",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "EngineId",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "FrameId",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "FuelId",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "NavId",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "ReactorId",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "RegistrationId",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "ConsumedId",
                table: "ShipFuel");

            migrationBuilder.RenameColumn(
                name: "RouteId",
                table: "ShipNav",
                newName: "ShipId");

            migrationBuilder.AlterColumn<int>(
                name: "WaypointId",
                table: "WaypointTrait",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WaypointId",
                table: "WaypointOrbital",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WaypointId",
                table: "WaypointModifier",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WaypointId",
                table: "WaypointFaction",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShipId",
                table: "ShipRegistration",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShipId",
                table: "ShipReactor",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShipNavId",
                table: "ShipNavRoute",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ShipId",
                table: "ShipMount",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShipId",
                table: "ShipModule",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipFuelId",
                table: "ShipFuelConsumed",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShipId",
                table: "ShipFuel",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShipId",
                table: "ShipFrame",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShipId",
                table: "ShipEngine",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShipId",
                table: "ShipCrew",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ShipCargoId",
                table: "ShipCargoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipId",
                table: "ShipCargo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "MarketId",
                table: "MarketTransaction",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MarketId",
                table: "MarketTradeGood",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipId",
                table: "Cooldown",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WaypointId",
                table: "Chart",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WaypointFaction_WaypointId",
                table: "WaypointFaction",
                column: "WaypointId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipRegistration_ShipId",
                table: "ShipRegistration",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipReactor_ShipId",
                table: "ShipReactor",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipNavRoute_ShipNavId",
                table: "ShipNavRoute",
                column: "ShipNavId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipNav_ShipId",
                table: "ShipNav",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipFuelConsumed_ShipFuelId",
                table: "ShipFuelConsumed",
                column: "ShipFuelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipFuel_ShipId",
                table: "ShipFuel",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipFrame_ShipId",
                table: "ShipFrame",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipEngine_ShipId",
                table: "ShipEngine",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipCrew_ShipId",
                table: "ShipCrew",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipCargo_ShipId",
                table: "ShipCargo",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cooldown_ShipId",
                table: "Cooldown",
                column: "ShipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chart_WaypointId",
                table: "Chart",
                column: "WaypointId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chart_Waypoints_WaypointId",
                table: "Chart",
                column: "WaypointId",
                principalTable: "Waypoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cooldown_Ships_ShipId",
                table: "Cooldown",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketTradeGood_Markets_MarketId",
                table: "MarketTradeGood",
                column: "MarketId",
                principalTable: "Markets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketTransaction_Markets_MarketId",
                table: "MarketTransaction",
                column: "MarketId",
                principalTable: "Markets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipCargo_Ships_ShipId",
                table: "ShipCargo",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipCargoItem_ShipCargo_ShipCargoId",
                table: "ShipCargoItem",
                column: "ShipCargoId",
                principalTable: "ShipCargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipCrew_Ships_ShipId",
                table: "ShipCrew",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipEngine_Ships_ShipId",
                table: "ShipEngine",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipFrame_Ships_ShipId",
                table: "ShipFrame",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipFuel_Ships_ShipId",
                table: "ShipFuel",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipFuelConsumed_ShipFuel_ShipFuelId",
                table: "ShipFuelConsumed",
                column: "ShipFuelId",
                principalTable: "ShipFuel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipModule_Ships_ShipId",
                table: "ShipModule",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipMount_Ships_ShipId",
                table: "ShipMount",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipNav_Ships_ShipId",
                table: "ShipNav",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipNavRoute_ShipNav_ShipNavId",
                table: "ShipNavRoute",
                column: "ShipNavId",
                principalTable: "ShipNav",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipReactor_Ships_ShipId",
                table: "ShipReactor",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipRegistration_Ships_ShipId",
                table: "ShipRegistration",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WaypointFaction_Waypoints_WaypointId",
                table: "WaypointFaction",
                column: "WaypointId",
                principalTable: "Waypoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WaypointModifier_Waypoints_WaypointId",
                table: "WaypointModifier",
                column: "WaypointId",
                principalTable: "Waypoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WaypointOrbital_Waypoints_WaypointId",
                table: "WaypointOrbital",
                column: "WaypointId",
                principalTable: "Waypoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WaypointTrait_Waypoints_WaypointId",
                table: "WaypointTrait",
                column: "WaypointId",
                principalTable: "Waypoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chart_Waypoints_WaypointId",
                table: "Chart");

            migrationBuilder.DropForeignKey(
                name: "FK_Cooldown_Ships_ShipId",
                table: "Cooldown");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketTradeGood_Markets_MarketId",
                table: "MarketTradeGood");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketTransaction_Markets_MarketId",
                table: "MarketTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipCargo_Ships_ShipId",
                table: "ShipCargo");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipCargoItem_ShipCargo_ShipCargoId",
                table: "ShipCargoItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipCrew_Ships_ShipId",
                table: "ShipCrew");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipEngine_Ships_ShipId",
                table: "ShipEngine");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipFrame_Ships_ShipId",
                table: "ShipFrame");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipFuel_Ships_ShipId",
                table: "ShipFuel");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipFuelConsumed_ShipFuel_ShipFuelId",
                table: "ShipFuelConsumed");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipModule_Ships_ShipId",
                table: "ShipModule");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipMount_Ships_ShipId",
                table: "ShipMount");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipNav_Ships_ShipId",
                table: "ShipNav");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipNavRoute_ShipNav_ShipNavId",
                table: "ShipNavRoute");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipReactor_Ships_ShipId",
                table: "ShipReactor");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipRegistration_Ships_ShipId",
                table: "ShipRegistration");

            migrationBuilder.DropForeignKey(
                name: "FK_WaypointFaction_Waypoints_WaypointId",
                table: "WaypointFaction");

            migrationBuilder.DropForeignKey(
                name: "FK_WaypointModifier_Waypoints_WaypointId",
                table: "WaypointModifier");

            migrationBuilder.DropForeignKey(
                name: "FK_WaypointOrbital_Waypoints_WaypointId",
                table: "WaypointOrbital");

            migrationBuilder.DropForeignKey(
                name: "FK_WaypointTrait_Waypoints_WaypointId",
                table: "WaypointTrait");

            migrationBuilder.DropIndex(
                name: "IX_WaypointFaction_WaypointId",
                table: "WaypointFaction");

            migrationBuilder.DropIndex(
                name: "IX_ShipRegistration_ShipId",
                table: "ShipRegistration");

            migrationBuilder.DropIndex(
                name: "IX_ShipReactor_ShipId",
                table: "ShipReactor");

            migrationBuilder.DropIndex(
                name: "IX_ShipNavRoute_ShipNavId",
                table: "ShipNavRoute");

            migrationBuilder.DropIndex(
                name: "IX_ShipNav_ShipId",
                table: "ShipNav");

            migrationBuilder.DropIndex(
                name: "IX_ShipFuelConsumed_ShipFuelId",
                table: "ShipFuelConsumed");

            migrationBuilder.DropIndex(
                name: "IX_ShipFuel_ShipId",
                table: "ShipFuel");

            migrationBuilder.DropIndex(
                name: "IX_ShipFrame_ShipId",
                table: "ShipFrame");

            migrationBuilder.DropIndex(
                name: "IX_ShipEngine_ShipId",
                table: "ShipEngine");

            migrationBuilder.DropIndex(
                name: "IX_ShipCrew_ShipId",
                table: "ShipCrew");

            migrationBuilder.DropIndex(
                name: "IX_ShipCargo_ShipId",
                table: "ShipCargo");

            migrationBuilder.DropIndex(
                name: "IX_Cooldown_ShipId",
                table: "Cooldown");

            migrationBuilder.DropIndex(
                name: "IX_Chart_WaypointId",
                table: "Chart");

            migrationBuilder.DropColumn(
                name: "WaypointId",
                table: "WaypointFaction");

            migrationBuilder.DropColumn(
                name: "ShipId",
                table: "ShipRegistration");

            migrationBuilder.DropColumn(
                name: "ShipId",
                table: "ShipReactor");

            migrationBuilder.DropColumn(
                name: "ShipNavId",
                table: "ShipNavRoute");

            migrationBuilder.DropColumn(
                name: "ShipFuelId",
                table: "ShipFuelConsumed");

            migrationBuilder.DropColumn(
                name: "ShipId",
                table: "ShipFuel");

            migrationBuilder.DropColumn(
                name: "ShipId",
                table: "ShipFrame");

            migrationBuilder.DropColumn(
                name: "ShipId",
                table: "ShipEngine");

            migrationBuilder.DropColumn(
                name: "ShipId",
                table: "ShipCrew");

            migrationBuilder.DropColumn(
                name: "ShipId",
                table: "ShipCargo");

            migrationBuilder.DropColumn(
                name: "ShipId",
                table: "Cooldown");

            migrationBuilder.DropColumn(
                name: "WaypointId",
                table: "Chart");

            migrationBuilder.RenameColumn(
                name: "ShipId",
                table: "ShipNav",
                newName: "RouteId");

            migrationBuilder.AlterColumn<int>(
                name: "WaypointId",
                table: "WaypointTrait",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "ChartId",
                table: "Waypoints",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FactionId",
                table: "Waypoints",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WaypointId",
                table: "WaypointOrbital",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "WaypointId",
                table: "WaypointModifier",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "CargoId",
                table: "Ships",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CooldownId",
                table: "Ships",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CrewId",
                table: "Ships",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EngineId",
                table: "Ships",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FrameId",
                table: "Ships",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FuelId",
                table: "Ships",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NavId",
                table: "Ships",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReactorId",
                table: "Ships",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RegistrationId",
                table: "Ships",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ShipId",
                table: "ShipMount",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "ShipId",
                table: "ShipModule",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "ConsumedId",
                table: "ShipFuel",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShipCargoId",
                table: "ShipCargoItem",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "MarketId",
                table: "MarketTransaction",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "MarketId",
                table: "MarketTradeGood",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Waypoints_ChartId",
                table: "Waypoints",
                column: "ChartId");

            migrationBuilder.CreateIndex(
                name: "IX_Waypoints_FactionId",
                table: "Waypoints",
                column: "FactionId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ShipNav_RouteId",
                table: "ShipNav",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipFuel_ConsumedId",
                table: "ShipFuel",
                column: "ConsumedId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketTradeGood_Markets_MarketId",
                table: "MarketTradeGood",
                column: "MarketId",
                principalTable: "Markets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketTransaction_Markets_MarketId",
                table: "MarketTransaction",
                column: "MarketId",
                principalTable: "Markets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipCargoItem_ShipCargo_ShipCargoId",
                table: "ShipCargoItem",
                column: "ShipCargoId",
                principalTable: "ShipCargo",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipFuel_ShipFuelConsumed_ConsumedId",
                table: "ShipFuel",
                column: "ConsumedId",
                principalTable: "ShipFuelConsumed",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipModule_Ships_ShipId",
                table: "ShipModule",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipMount_Ships_ShipId",
                table: "ShipMount",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipNav_ShipNavRoute_RouteId",
                table: "ShipNav",
                column: "RouteId",
                principalTable: "ShipNavRoute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ships_Cooldown_CooldownId",
                table: "Ships",
                column: "CooldownId",
                principalTable: "Cooldown",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ships_ShipCargo_CargoId",
                table: "Ships",
                column: "CargoId",
                principalTable: "ShipCargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ships_ShipCrew_CrewId",
                table: "Ships",
                column: "CrewId",
                principalTable: "ShipCrew",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ships_ShipEngine_EngineId",
                table: "Ships",
                column: "EngineId",
                principalTable: "ShipEngine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ships_ShipFrame_FrameId",
                table: "Ships",
                column: "FrameId",
                principalTable: "ShipFrame",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ships_ShipFuel_FuelId",
                table: "Ships",
                column: "FuelId",
                principalTable: "ShipFuel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ships_ShipNav_NavId",
                table: "Ships",
                column: "NavId",
                principalTable: "ShipNav",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ships_ShipReactor_ReactorId",
                table: "Ships",
                column: "ReactorId",
                principalTable: "ShipReactor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ships_ShipRegistration_RegistrationId",
                table: "Ships",
                column: "RegistrationId",
                principalTable: "ShipRegistration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WaypointModifier_Waypoints_WaypointId",
                table: "WaypointModifier",
                column: "WaypointId",
                principalTable: "Waypoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WaypointOrbital_Waypoints_WaypointId",
                table: "WaypointOrbital",
                column: "WaypointId",
                principalTable: "Waypoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Waypoints_Chart_ChartId",
                table: "Waypoints",
                column: "ChartId",
                principalTable: "Chart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Waypoints_WaypointFaction_FactionId",
                table: "Waypoints",
                column: "FactionId",
                principalTable: "WaypointFaction",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WaypointTrait_Waypoints_WaypointId",
                table: "WaypointTrait",
                column: "WaypointId",
                principalTable: "Waypoints",
                principalColumn: "Id");
        }
    }
}
