using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealingInWriting.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    RegistrationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuestName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    GuestEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    GuestPhone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IsAdminOverride = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.RegistrationId);
                    table.ForeignKey(
                        name: "FK_Registrations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registrations_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_EventId_GuestEmail",
                table: "Registrations",
                columns: new[] { "EventId", "GuestEmail" });

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_EventId_UserId",
                table: "Registrations",
                columns: new[] { "EventId", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_UserId",
                table: "Registrations",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Registrations");
        }
    }
}
