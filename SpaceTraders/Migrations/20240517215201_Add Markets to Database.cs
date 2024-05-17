using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceTraders.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketstoDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Markets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketTradeGood",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    TradeVolume = table.Column<int>(type: "INTEGER", nullable: false),
                    Supply = table.Column<string>(type: "TEXT", nullable: false),
                    Activity = table.Column<string>(type: "TEXT", nullable: true),
                    PurchasePrice = table.Column<int>(type: "INTEGER", nullable: false),
                    SellPrice = table.Column<int>(type: "INTEGER", nullable: false),
                    MarketId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketTradeGood", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketTradeGood_Markets_MarketId",
                        column: x => x.MarketId,
                        principalTable: "Markets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MarketTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WaypointSymbol = table.Column<string>(type: "TEXT", nullable: false),
                    ShipSymbol = table.Column<string>(type: "TEXT", nullable: false),
                    TradeSymbol = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Units = table.Column<int>(type: "INTEGER", nullable: false),
                    PricePerUnit = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPrice = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MarketId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketTransaction_Markets_MarketId",
                        column: x => x.MarketId,
                        principalTable: "Markets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TradeGood",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExportId = table.Column<int>(type: "INTEGER", nullable: true),
                    ImportId = table.Column<int>(type: "INTEGER", nullable: true),
                    ExchangeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeGood", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeGood_Markets_ExchangeId",
                        column: x => x.ExchangeId,
                        principalTable: "Markets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TradeGood_Markets_ExportId",
                        column: x => x.ExportId,
                        principalTable: "Markets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TradeGood_Markets_ImportId",
                        column: x => x.ImportId,
                        principalTable: "Markets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketTradeGood_MarketId",
                table: "MarketTradeGood",
                column: "MarketId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketTransaction_MarketId",
                table: "MarketTransaction",
                column: "MarketId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeGood_ExchangeId",
                table: "TradeGood",
                column: "ExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeGood_ExportId",
                table: "TradeGood",
                column: "ExportId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeGood_ImportId",
                table: "TradeGood",
                column: "ImportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketTradeGood");

            migrationBuilder.DropTable(
                name: "MarketTransaction");

            migrationBuilder.DropTable(
                name: "TradeGood");

            migrationBuilder.DropTable(
                name: "Markets");
        }
    }
}
