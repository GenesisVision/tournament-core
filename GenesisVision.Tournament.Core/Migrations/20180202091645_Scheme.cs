using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Tournament.Core.Migrations
{
    public partial class Scheme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    RegDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    RegisterDateFrom = table.Column<DateTime>(nullable: true),
                    RegisterDateTo = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradeServers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Host = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeServers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradeAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IpfsHash = table.Column<string>(nullable: true),
                    Login = table.Column<long>(nullable: false),
                    OrdersCount = table.Column<int>(nullable: false),
                    ParticipantId = table.Column<Guid>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    StartBalance = table.Column<decimal>(nullable: false),
                    TotalProfit = table.Column<decimal>(nullable: false),
                    TotalProfitInPercent = table.Column<decimal>(nullable: false),
                    TournamentId = table.Column<Guid>(nullable: false),
                    TradeServerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeAccounts_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeAccounts_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradeAccounts_TradeServers_TradeServerId",
                        column: x => x.TradeServerId,
                        principalTable: "TradeServers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Charts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    TradeAccountId = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Charts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Charts_TradeAccounts_TradeAccountId",
                        column: x => x.TradeAccountId,
                        principalTable: "TradeAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Direction = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Profit = table.Column<decimal>(nullable: false),
                    Symbol = table.Column<string>(nullable: true),
                    Ticket = table.Column<long>(nullable: false),
                    TradeAccountId = table.Column<Guid>(nullable: false),
                    Volume = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trades_TradeAccounts_TradeAccountId",
                        column: x => x.TradeAccountId,
                        principalTable: "TradeAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Charts_TradeAccountId",
                table: "Charts",
                column: "TradeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_Email",
                table: "Participants",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradeAccounts_ParticipantId",
                table: "TradeAccounts",
                column: "ParticipantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradeAccounts_TournamentId",
                table: "TradeAccounts",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeAccounts_TradeServerId",
                table: "TradeAccounts",
                column: "TradeServerId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_TradeAccountId",
                table: "Trades",
                column: "TradeAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Charts");

            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropTable(
                name: "TradeAccounts");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Tournaments");

            migrationBuilder.DropTable(
                name: "TradeServers");
        }
    }
}
