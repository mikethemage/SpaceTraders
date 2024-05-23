using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceTraders.Migrations
{
    /// <inheritdoc />
    public partial class Contractfixuppt3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractDeliverGood_ContractTerms_ContractTermsId",
                table: "ContractDeliverGood");

            migrationBuilder.AlterColumn<int>(
                name: "ContractTermsId",
                table: "ContractDeliverGood",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractDeliverGood_ContractTerms_ContractTermsId",
                table: "ContractDeliverGood",
                column: "ContractTermsId",
                principalTable: "ContractTerms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractDeliverGood_ContractTerms_ContractTermsId",
                table: "ContractDeliverGood");

            migrationBuilder.AlterColumn<int>(
                name: "ContractTermsId",
                table: "ContractDeliverGood",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractDeliverGood_ContractTerms_ContractTermsId",
                table: "ContractDeliverGood",
                column: "ContractTermsId",
                principalTable: "ContractTerms",
                principalColumn: "Id");
        }
    }
}
