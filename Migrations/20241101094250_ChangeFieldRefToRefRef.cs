using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace centaureatest.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class ChangeFieldRefToRefRef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReferencedFieldId",
                table: "values",
                newName: "ReferencedRowIndex");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReferencedRowIndex",
                table: "values",
                newName: "ReferencedFieldId");
        }
    }
}
