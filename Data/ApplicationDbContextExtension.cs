using System.Text.RegularExpressions;
using CentaureaTest.Models;
using Microsoft.EntityFrameworkCore;

namespace CentaureaTest.Data
{

    public static class ApplicationDbContextExtension
    {
        public static DataGridSignature? GetDataGridSignature(this ApplicationDbContext dbContext, int gridId)
        {
            return dbContext.Fields
                .Where(field => field.GridId == gridId)
                .OrderBy(field => field.Id)
                .ToDataGridSignature();
        }

        public static IEnumerable<FieldsTable> GetGridFields(this ApplicationDbContext dbContext, int gridId)
        {
            return dbContext.Fields
                .Where(field => field.GridId == gridId)
                .OrderBy(field => field.Id);
        }

        public static IEnumerable<DataGridRow> GetGridRows(this ApplicationDbContext dbContext, int gridId)
        {
            var fieldIds = dbContext.Fields
                .Where(field => field.GridId == gridId)
                .Select(field => field.Id)
                .ToList();

            var values = dbContext.Values
                .Where(value => fieldIds.Contains(value.FieldId))
                .OrderBy(value => value.RowIndex);

            var valueGroups = values
                .OrderBy(value => value.RowIndex)
                .GroupBy(value => value.RowIndex)
                .ToList();

            return valueGroups.Select(group => new DataGridRow(group));
        }

        public static async Task<DataGrid?> GetDataGridAsync(this ApplicationDbContext dbContext, int gridId)
        {
            var grid = await dbContext.Grids.FindAsync(gridId);
            if (grid is null)
            {
                return null;
            }

            var signature = dbContext.GetDataGridSignature(gridId);
            if (signature is null)
            {
                return null;
            }

            var rows = dbContext.GetGridRows(gridId);

            return new DataGrid(grid.Id, grid.Name, signature, rows);
        }
    }

}