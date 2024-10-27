using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace centaureatest.Migrations
{
    /// <inheritdoc />
    public partial class SetupTPHInheritancee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Discriminator",
                table: "values",
                newName: "ValueType");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "values",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AddColumn<string>(
                name: "FieldType",
                table: "fields",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OptionSetId",
                table: "fields",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReferencedGridId",
                table: "fields",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Regex",
                table: "fields",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "fields");

            migrationBuilder.DropColumn(
                name: "OptionSetId",
                table: "fields");

            migrationBuilder.DropColumn(
                name: "ReferencedGridId",
                table: "fields");

            migrationBuilder.DropColumn(
                name: "Regex",
                table: "fields");

            migrationBuilder.RenameColumn(
                name: "ValueType",
                table: "values",
                newName: "Discriminator");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "values",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
