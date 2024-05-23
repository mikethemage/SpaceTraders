using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceTraders.Migrations
{
    /// <inheritdoc />
    public partial class Factionfixup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FactionTrait_Factions_FactionId",
                table: "FactionTrait");

            migrationBuilder.AlterColumn<int>(
                name: "FactionId",
                table: "FactionTrait",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FactionTrait_Factions_FactionId",
                table: "FactionTrait",
                column: "FactionId",
                principalTable: "Factions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FactionTrait_Factions_FactionId",
                table: "FactionTrait");

            migrationBuilder.AlterColumn<int>(
                name: "FactionId",
                table: "FactionTrait",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_FactionTrait_Factions_FactionId",
                table: "FactionTrait",
                column: "FactionId",
                principalTable: "Factions",
                principalColumn: "Id");
        }
    }
}
