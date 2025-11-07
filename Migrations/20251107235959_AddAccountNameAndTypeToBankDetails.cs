using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealingInWriting.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountNameAndTypeToBankDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "BankDetails",
                type: "TEXT",
                maxLength: 150,
                nullable: false,
                defaultValue: "Not Set");

            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "BankDetails",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "Not Set");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "BankDetails");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "BankDetails");
        }
    }
}
