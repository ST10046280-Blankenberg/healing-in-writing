using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealingInWriting.Migrations
{
    /// <inheritdoc />
    public partial class AddIpAddressToRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "Registrations",
                type: "TEXT",
                maxLength: 45,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "Registrations");
        }
    }
}
