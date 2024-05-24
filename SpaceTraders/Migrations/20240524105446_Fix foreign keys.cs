using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceTraders.Migrations
{
    /// <inheritdoc />
    public partial class Fixforeignkeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipEngine_ShipRequirements_ShipEngineId",
                table: "ShipEngine");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipFrame_ShipRequirements_ShipFrameId",
                table: "ShipFrame");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipModule_ShipRequirements_ShipModuleId",
                table: "ShipModule");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipMount_ShipRequirements_ShipMountId",
                table: "ShipMount");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipReactor_ShipRequirements_ShipReactorId",
                table: "ShipReactor");

            migrationBuilder.DropIndex(
                name: "IX_ShipReactor_ShipReactorId",
                table: "ShipReactor");

            migrationBuilder.DropIndex(
                name: "IX_ShipMount_ShipMountId",
                table: "ShipMount");

            migrationBuilder.DropIndex(
                name: "IX_ShipModule_ShipModuleId",
                table: "ShipModule");

            migrationBuilder.DropIndex(
                name: "IX_ShipFrame_ShipFrameId",
                table: "ShipFrame");

            migrationBuilder.DropIndex(
                name: "IX_ShipEngine_ShipEngineId",
                table: "ShipEngine");

            migrationBuilder.DropColumn(
                name: "ShipEngineId",
                table: "ShipRequirements");

            migrationBuilder.DropColumn(
                name: "ShipFrameId",
                table: "ShipRequirements");

            migrationBuilder.DropColumn(
                name: "ShipModuleId",
                table: "ShipRequirements");

            migrationBuilder.DropColumn(
                name: "ShipMountId",
                table: "ShipRequirements");

            migrationBuilder.DropColumn(
                name: "ShipReactorId",
                table: "ShipRequirements");

            migrationBuilder.DropColumn(
                name: "DestinationId",
                table: "ShipNavRouteWaypoint");

            migrationBuilder.DropColumn(
                name: "OriginId",
                table: "ShipNavRouteWaypoint");

            migrationBuilder.AddColumn<int>(
                name: "RequirementsId",
                table: "ShipReactor",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequirementsId",
                table: "ShipMount",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequirementsId",
                table: "ShipModule",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequirementsId",
                table: "ShipFrame",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequirementsId",
                table: "ShipEngine",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShipReactor_RequirementsId",
                table: "ShipReactor",
                column: "RequirementsId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipMount_RequirementsId",
                table: "ShipMount",
                column: "RequirementsId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipModule_RequirementsId",
                table: "ShipModule",
                column: "RequirementsId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipFrame_RequirementsId",
                table: "ShipFrame",
                column: "RequirementsId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipEngine_RequirementsId",
                table: "ShipEngine",
                column: "RequirementsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipEngine_ShipRequirements_RequirementsId",
                table: "ShipEngine",
                column: "RequirementsId",
                principalTable: "ShipRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipFrame_ShipRequirements_RequirementsId",
                table: "ShipFrame",
                column: "RequirementsId",
                principalTable: "ShipRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipModule_ShipRequirements_RequirementsId",
                table: "ShipModule",
                column: "RequirementsId",
                principalTable: "ShipRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipMount_ShipRequirements_RequirementsId",
                table: "ShipMount",
                column: "RequirementsId",
                principalTable: "ShipRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipReactor_ShipRequirements_RequirementsId",
                table: "ShipReactor",
                column: "RequirementsId",
                principalTable: "ShipRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipEngine_ShipRequirements_RequirementsId",
                table: "ShipEngine");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipFrame_ShipRequirements_RequirementsId",
                table: "ShipFrame");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipModule_ShipRequirements_RequirementsId",
                table: "ShipModule");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipMount_ShipRequirements_RequirementsId",
                table: "ShipMount");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipReactor_ShipRequirements_RequirementsId",
                table: "ShipReactor");

            migrationBuilder.DropIndex(
                name: "IX_ShipReactor_RequirementsId",
                table: "ShipReactor");

            migrationBuilder.DropIndex(
                name: "IX_ShipMount_RequirementsId",
                table: "ShipMount");

            migrationBuilder.DropIndex(
                name: "IX_ShipModule_RequirementsId",
                table: "ShipModule");

            migrationBuilder.DropIndex(
                name: "IX_ShipFrame_RequirementsId",
                table: "ShipFrame");

            migrationBuilder.DropIndex(
                name: "IX_ShipEngine_RequirementsId",
                table: "ShipEngine");

            migrationBuilder.DropColumn(
                name: "RequirementsId",
                table: "ShipReactor");

            migrationBuilder.DropColumn(
                name: "RequirementsId",
                table: "ShipMount");

            migrationBuilder.DropColumn(
                name: "RequirementsId",
                table: "ShipModule");

            migrationBuilder.DropColumn(
                name: "RequirementsId",
                table: "ShipFrame");

            migrationBuilder.DropColumn(
                name: "RequirementsId",
                table: "ShipEngine");

            migrationBuilder.AddColumn<int>(
                name: "ShipEngineId",
                table: "ShipRequirements",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipFrameId",
                table: "ShipRequirements",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipModuleId",
                table: "ShipRequirements",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipMountId",
                table: "ShipRequirements",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShipReactorId",
                table: "ShipRequirements",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DestinationId",
                table: "ShipNavRouteWaypoint",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginId",
                table: "ShipNavRouteWaypoint",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipReactor_ShipReactorId",
                table: "ShipReactor",
                column: "ShipReactorId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipMount_ShipMountId",
                table: "ShipMount",
                column: "ShipMountId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipModule_ShipModuleId",
                table: "ShipModule",
                column: "ShipModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipFrame_ShipFrameId",
                table: "ShipFrame",
                column: "ShipFrameId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipEngine_ShipEngineId",
                table: "ShipEngine",
                column: "ShipEngineId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipEngine_ShipRequirements_ShipEngineId",
                table: "ShipEngine",
                column: "ShipEngineId",
                principalTable: "ShipRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipFrame_ShipRequirements_ShipFrameId",
                table: "ShipFrame",
                column: "ShipFrameId",
                principalTable: "ShipRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipModule_ShipRequirements_ShipModuleId",
                table: "ShipModule",
                column: "ShipModuleId",
                principalTable: "ShipRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipMount_ShipRequirements_ShipMountId",
                table: "ShipMount",
                column: "ShipMountId",
                principalTable: "ShipRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipReactor_ShipRequirements_ShipReactorId",
                table: "ShipReactor",
                column: "ShipReactorId",
                principalTable: "ShipRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
