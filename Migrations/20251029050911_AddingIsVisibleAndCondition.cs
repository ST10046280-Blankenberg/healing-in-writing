using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealingInWriting.Migrations
{
    /// <inheritdoc />
    public partial class AddingIsVisibleAndCondition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Condition",
                table: "Books",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Books",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Condition",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Books");
        }
    }
}
