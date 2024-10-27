using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace centaureatest.Migrations
{
    /// <inheritdoc />
    public partial class ValueStuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldId",
                table: "values");

            migrationBuilder.RenameColumn(
                name: "StringValue",
                table: "values",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "RowId",
                table: "values",
                newName: "RowIndex");

            migrationBuilder.RenameColumn(
                name: "RegexValue",
                table: "values",
                newName: "DataGridRegexValue_Value");

            migrationBuilder.RenameColumn(
                name: "NumericValue",
                table: "values",
                newName: "DataGridNumericValue_Value");

            migrationBuilder.RenameColumn(
                name: "GridId",
                table: "values",
                newName: "ColumnId");

            migrationBuilder.AddColumn<int>(
                name: "OptionId",
                table: "values",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<List<int>>(
                name: "OptionIds",
                table: "values",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReferencedFieldId",
                table: "values",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptionId",
                table: "values");

            migrationBuilder.DropColumn(
                name: "OptionIds",
                table: "values");

            migrationBuilder.DropColumn(
                name: "ReferencedFieldId",
                table: "values");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "values",
                newName: "StringValue");

            migrationBuilder.RenameColumn(
                name: "RowIndex",
                table: "values",
                newName: "RowId");

            migrationBuilder.RenameColumn(
                name: "DataGridRegexValue_Value",
                table: "values",
                newName: "RegexValue");

            migrationBuilder.RenameColumn(
                name: "DataGridNumericValue_Value",
                table: "values",
                newName: "NumericValue");

            migrationBuilder.RenameColumn(
                name: "ColumnId",
                table: "values",
                newName: "GridId");

            migrationBuilder.AddColumn<int>(
                name: "FieldId",
                table: "values",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
