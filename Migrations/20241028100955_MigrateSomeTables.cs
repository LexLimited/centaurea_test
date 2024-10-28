using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace centaureatest.Migrations
{
    /// <inheritdoc />
    public partial class MigrateSomeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TableId",
                table: "single_select");

            migrationBuilder.DropColumn(
                name: "OptionTableId",
                table: "fields");

            migrationBuilder.DropColumn(
                name: "SingleSelectFieldsTable_OptionTableId",
                table: "fields");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TableId",
                table: "single_select",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OptionTableId",
                table: "fields",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SingleSelectFieldsTable_OptionTableId",
                table: "fields",
                type: "integer",
                nullable: true);
        }
    }
}
