using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceTraders.Migrations
{
    /// <inheritdoc />
    public partial class Contractfixuppt2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_ContractTerms_ContractTermsId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractTerms_ContractPayment_PaymentId",
                table: "ContractTerms");

            migrationBuilder.DropIndex(
                name: "IX_ContractTerms_PaymentId",
                table: "ContractTerms");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ContractTermsId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ContractTermsId",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "ContractTerms",
                newName: "ContractId");

            migrationBuilder.AddColumn<int>(
                name: "ContractTermsId",
                table: "ContractPayment",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ContractTerms_ContractId",
                table: "ContractTerms",
                column: "ContractId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractPayment_ContractTermsId",
                table: "ContractPayment",
                column: "ContractTermsId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractPayment_ContractTerms_ContractTermsId",
                table: "ContractPayment",
                column: "ContractTermsId",
                principalTable: "ContractTerms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractTerms_Contracts_ContractId",
                table: "ContractTerms",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractPayment_ContractTerms_ContractTermsId",
                table: "ContractPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractTerms_Contracts_ContractId",
                table: "ContractTerms");

            migrationBuilder.DropIndex(
                name: "IX_ContractTerms_ContractId",
                table: "ContractTerms");

            migrationBuilder.DropIndex(
                name: "IX_ContractPayment_ContractTermsId",
                table: "ContractPayment");

            migrationBuilder.DropColumn(
                name: "ContractTermsId",
                table: "ContractPayment");

            migrationBuilder.RenameColumn(
                name: "ContractId",
                table: "ContractTerms",
                newName: "PaymentId");

            migrationBuilder.AddColumn<int>(
                name: "ContractTermsId",
                table: "Contracts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ContractTerms_PaymentId",
                table: "ContractTerms",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractTermsId",
                table: "Contracts",
                column: "ContractTermsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_ContractTerms_ContractTermsId",
                table: "Contracts",
                column: "ContractTermsId",
                principalTable: "ContractTerms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractTerms_ContractPayment_PaymentId",
                table: "ContractTerms",
                column: "PaymentId",
                principalTable: "ContractPayment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
