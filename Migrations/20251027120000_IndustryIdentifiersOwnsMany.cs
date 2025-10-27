using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealingInWriting.Migrations
{
    /// <inheritdoc />
    public partial class IndustryIdentifiersOwnsMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndustryIdentifiers_Capacity",
                table: "Books");

            // Normalise ISBNs into their own table so we retain each value with a foreign key to Books.
            migrationBuilder.CreateTable(
                name: "BookIndustryIdentifiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Identifier = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookIndustryIdentifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookIndustryIdentifiers_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookIndustryIdentifiers_BookId",
                table: "BookIndustryIdentifiers",
                column: "BookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookIndustryIdentifiers");

            // Roll back to the old (broken) capacity column if the migration is reverted.
            migrationBuilder.AddColumn<int>(
                name: "IndustryIdentifiers_Capacity",
                table: "Books",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
