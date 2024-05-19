using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceTraders.Migrations
{
    /// <inheritdoc />
    public partial class addnavigationpropertiestoshipetc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationId",
                table: "ShipNavRouteWaypoint");

            migrationBuilder.DropColumn(
                name: "OriginId",
                table: "ShipNavRouteWaypoint");
        }
    }
}
