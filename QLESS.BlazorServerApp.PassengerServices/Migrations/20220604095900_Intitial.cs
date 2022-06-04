using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QLESS.BlazorServerApp.PassengerServices.Migrations
{
    public partial class Intitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FareStrategyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountStrategyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    InitialBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumReloadAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumReloadAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BaseFare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Validity = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Privilege",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IdentificationNumberPattern = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Privilege", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardTypePrivilege",
                columns: table => new
                {
                    CardTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrivilegeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTypePrivilege", x => new { x.CardTypeId, x.PrivilegeId });
                    table.ForeignKey(
                        name: "FK_CardTypePrivilege_CardType_CardTypeId",
                        column: x => x.CardTypeId,
                        principalTable: "CardType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardTypePrivilege_Privilege_PrivilegeId",
                        column: x => x.PrivilegeId,
                        principalTable: "Privilege",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrivilegeCard",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentificationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivilegeCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivilegeCard_Privilege_TypeId",
                        column: x => x.TypeId,
                        principalTable: "Privilege",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Card",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Expiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrivilegeCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Card", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Card_CardType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "CardType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Card_PrivilegeCard_PrivilegeCardId",
                        column: x => x.PrivilegeCardId,
                        principalTable: "PrivilegeCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Trip",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Entry = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Exit = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EntryStationNumber = table.Column<int>(type: "int", nullable: false),
                    ExitStationNumber = table.Column<int>(type: "int", nullable: false),
                    CardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trip", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trip_Card_CardId",
                        column: x => x.CardId,
                        principalTable: "Card",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Card_PrivilegeCardId",
                table: "Card",
                column: "PrivilegeCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Card_TypeId",
                table: "Card",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTypePrivilege_PrivilegeId",
                table: "CardTypePrivilege",
                column: "PrivilegeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivilegeCard_TypeId",
                table: "PrivilegeCard",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_CardId",
                table: "Trip",
                column: "CardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardTypePrivilege");

            migrationBuilder.DropTable(
                name: "Trip");

            migrationBuilder.DropTable(
                name: "Card");

            migrationBuilder.DropTable(
                name: "CardType");

            migrationBuilder.DropTable(
                name: "PrivilegeCard");

            migrationBuilder.DropTable(
                name: "Privilege");
        }
    }
}
