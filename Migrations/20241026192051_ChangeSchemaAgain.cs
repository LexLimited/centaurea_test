using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace centaureatest.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSchemaAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "values");

            migrationBuilder.RenameColumn(
                name: "Regex",
                table: "fields",
                newName: "RegexPattern");

            migrationBuilder.RenameColumn(
                name: "OptionSetId",
                table: "fields",
                newName: "SingleSelectFieldsTable_OptionTableId");

            migrationBuilder.AddColumn<int>(
                name: "OptionTableId",
                table: "fields",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptionTableId",
                table: "fields");

            migrationBuilder.RenameColumn(
                name: "SingleSelectFieldsTable_OptionTableId",
                table: "fields",
                newName: "OptionSetId");

            migrationBuilder.RenameColumn(
                name: "RegexPattern",
                table: "fields",
                newName: "Regex");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "values",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
