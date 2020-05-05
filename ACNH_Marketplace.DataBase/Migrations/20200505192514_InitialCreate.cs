using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace ACNH_Marketplace.DataBase.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    InGameName = table.Column<string>(nullable: true),
                    IslandName = table.Column<string>(nullable: true),
                    LastActiveDate = table.Column<DateTime>(nullable: false),
                    HosterRating = table.Column<float>(nullable: false),
                    VisitorRating = table.Column<float>(nullable: false),
                    ExchangeRating = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TurnipMarketHosters",
                columns: table => new
                {
                    Id = table.Column<byte[]>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    TurnipCost = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnipMarketHosters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurnipMarketHosters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TurnipMarketVisitors",
                columns: table => new
                {
                    Id = table.Column<byte[]>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnipMarketVisitors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurnipMarketVisitors_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRevies",
                columns: table => new
                {
                    Id = table.Column<byte[]>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    ReviewerId = table.Column<int>(nullable: false),
                    ReviewedId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRevies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRevies_Users_ReviewedId",
                        column: x => x.ReviewedId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRevies_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TurnipEntryFees",
                columns: table => new
                {
                    Id = table.Column<byte[]>(nullable: false),
                    TurnipMarketHosterId = table.Column<byte[]>(nullable: true),
                    TurnipMarketVisitorId = table.Column<byte[]>(nullable: true),
                    FeeType = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnipEntryFees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurnipEntryFees_TurnipMarketHosters_TurnipMarketHosterId",
                        column: x => x.TurnipMarketHosterId,
                        principalTable: "TurnipMarketHosters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TurnipEntryFees_TurnipMarketVisitors_TurnipMarketVisitorId",
                        column: x => x.TurnipMarketVisitorId,
                        principalTable: "TurnipMarketVisitors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TurnipEntryFees_TurnipMarketHosterId",
                table: "TurnipEntryFees",
                column: "TurnipMarketHosterId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnipEntryFees_TurnipMarketVisitorId",
                table: "TurnipEntryFees",
                column: "TurnipMarketVisitorId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnipMarketHosters_UserId",
                table: "TurnipMarketHosters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnipMarketVisitors_UserId",
                table: "TurnipMarketVisitors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRevies_ReviewedId",
                table: "UserRevies",
                column: "ReviewedId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRevies_ReviewerId",
                table: "UserRevies",
                column: "ReviewerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TurnipEntryFees");

            migrationBuilder.DropTable(
                name: "UserRevies");

            migrationBuilder.DropTable(
                name: "TurnipMarketHosters");

            migrationBuilder.DropTable(
                name: "TurnipMarketVisitors");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
