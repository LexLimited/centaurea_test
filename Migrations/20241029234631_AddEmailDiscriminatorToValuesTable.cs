﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace centaureatest.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddEmailDiscriminatorToValuesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DataGridEmailValue_Value",
                table: "values",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataGridEmailValue_Value",
                table: "values");
        }
    }
}
