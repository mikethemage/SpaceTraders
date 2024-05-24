using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceTraders.Migrations
{
    /// <inheritdoc />
    public partial class Newattemptatfixingforeignkeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShipReactorId",
                table: "ShipReactor");

            migrationBuilder.DropColumn(
                name: "ShipMountId",
                table: "ShipMount");

            migrationBuilder.DropColumn(
                name: "ShipModuleId",
                table: "ShipModule");

            migrationBuilder.DropColumn(
                name: "ShipFrameId",
                table: "ShipFrame");

            migrationBuilder.DropColumn(
                name: "ShipEngineId",
                table: "ShipEngine");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShipReactorId",
                table: "ShipReactor",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShipMountId",
                table: "ShipMount",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShipModuleId",
                table: "ShipModule",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShipFrameId",
                table: "ShipFrame",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShipEngineId",
                table: "ShipEngine",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
