using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceTraders.Migrations
{
    /// <inheritdoc />
    public partial class AddContracts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContractPayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OnAccepted = table.Column<int>(type: "INTEGER", nullable: false),
                    OnFulfilled = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractPayment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractTerms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Deadline = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaymentId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractTerms_ContractPayment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "ContractPayment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractDeliverGood",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TradeSymbol = table.Column<string>(type: "TEXT", nullable: false),
                    DestinationSymbol = table.Column<string>(type: "TEXT", nullable: false),
                    UnitsRequired = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitsFulfilled = table.Column<int>(type: "INTEGER", nullable: false),
                    ContractTermsId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractDeliverGood", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractDeliverGood_ContractTerms_ContractTermsId",
                        column: x => x.ContractTermsId,
                        principalTable: "ContractTerms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContractId = table.Column<string>(type: "TEXT", nullable: false),
                    FactionSymbol = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    TermsId = table.Column<int>(type: "INTEGER", nullable: false),
                    Accepted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Fulfilled = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeadlineToAccept = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_ContractTerms_TermsId",
                        column: x => x.TermsId,
                        principalTable: "ContractTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractDeliverGood_ContractTermsId",
                table: "ContractDeliverGood",
                column: "ContractTermsId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_TermsId",
                table: "Contracts",
                column: "TermsId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractTerms_PaymentId",
                table: "ContractTerms",
                column: "PaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractDeliverGood");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "ContractTerms");

            migrationBuilder.DropTable(
                name: "ContractPayment");
        }
    }
}
