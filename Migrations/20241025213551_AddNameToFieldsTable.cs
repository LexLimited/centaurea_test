using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace centaureatest.Migrations
{
    /// <inheritdoc />
    public partial class AddNameToFieldsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "fields",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "fields");
        }
    }
}
