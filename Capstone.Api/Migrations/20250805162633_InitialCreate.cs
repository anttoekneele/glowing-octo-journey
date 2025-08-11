using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Capstone.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommunicationTypes",
                columns: table => new
                {
                    TypeCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationTypes", x => x.TypeCode);
                });

            migrationBuilder.CreateTable(
                name: "Communications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Communications_CommunicationTypes_TypeCode",
                        column: x => x.TypeCode,
                        principalTable: "CommunicationTypes",
                        principalColumn: "TypeCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunicationTypeStatuses",
                columns: table => new
                {
                    TypeCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StatusCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationTypeStatuses", x => new { x.TypeCode, x.StatusCode });
                    table.ForeignKey(
                        name: "FK_CommunicationTypeStatuses_CommunicationTypes_TypeCode",
                        column: x => x.TypeCode,
                        principalTable: "CommunicationTypes",
                        principalColumn: "TypeCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunicationStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunicationStatusHistories_Communications_CommunicationId",
                        column: x => x.CommunicationId,
                        principalTable: "Communications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CommunicationTypes",
                columns: new[] { "TypeCode", "DisplayName" },
                values: new object[,]
                {
                    { "EMAIL", "Email" },
                    { "SMS", "SMS" }
                });

            migrationBuilder.InsertData(
                table: "CommunicationTypeStatuses",
                columns: new[] { "StatusCode", "TypeCode", "Description" },
                values: new object[,]
                {
                    { "DRAFT", "EMAIL", "Draft" },
                    { "SENT", "EMAIL", "Sent" },
                    { "DELIVERED", "SMS", "Delivered" },
                    { "PENDING", "SMS", "Pending" }
                });

            migrationBuilder.InsertData(
                table: "Communications",
                columns: new[] { "Id", "CurrentStatus", "LastUpdatedUtc", "Title", "TypeCode" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "DRAFT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Welcome Email", "EMAIL" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "PENDING", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Verification SMS", "SMS" }
                });

            migrationBuilder.InsertData(
                table: "CommunicationStatusHistories",
                columns: new[] { "Id", "CommunicationId", "OccurredUtc", "StatusCode" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "DRAFT" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PENDING" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Communications_TypeCode",
                table: "Communications",
                column: "TypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationStatusHistories_CommunicationId",
                table: "CommunicationStatusHistories",
                column: "CommunicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunicationStatusHistories");

            migrationBuilder.DropTable(
                name: "CommunicationTypeStatuses");

            migrationBuilder.DropTable(
                name: "Communications");

            migrationBuilder.DropTable(
                name: "CommunicationTypes");
        }
    }
}
