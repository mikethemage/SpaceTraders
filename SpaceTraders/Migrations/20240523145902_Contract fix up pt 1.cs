using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceTraders.Migrations
{
    /// <inheritdoc />
    public partial class Contractfixuppt1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_ContractTerms_TermsId",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "TermsId",
                table: "Contracts",
                newName: "ContractTermsId");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_TermsId",
                table: "Contracts",
                newName: "IX_Contracts_ContractTermsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_ContractTerms_ContractTermsId",
                table: "Contracts",
                column: "ContractTermsId",
                principalTable: "ContractTerms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_ContractTerms_ContractTermsId",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "ContractTermsId",
                table: "Contracts",
                newName: "TermsId");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_ContractTermsId",
                table: "Contracts",
                newName: "IX_Contracts_TermsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_ContractTerms_TermsId",
                table: "Contracts",
                column: "TermsId",
                principalTable: "ContractTerms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
