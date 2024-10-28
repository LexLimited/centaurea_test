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
                .OrderBy(field => field.Order)
                .ToDataGridSignature();
        }

        /// <summary>Get all fields corresponding to 'gridId' order by 'Order'</summary>
        public static IEnumerable<FieldsTable> GetGridFields(this ApplicationDbContext dbContext, int gridId)
        {
            return dbContext.Fields
                .Where(field => field.GridId == gridId)
                .OrderBy(field => field.Order);
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

        /// <summary>Deletes the field and dependent values</summary>
        /// <remarks>Not transactional</remarks>
        public static async Task DeleteFieldAsync(this ApplicationDbContext dbContext, int fieldId)
        {
            var field = dbContext.Fields.Find(fieldId)
                ?? throw new Exception($"Field {fieldId} does not exist");
        
            var values = dbContext.Values
                .Where(value => value.FieldId == fieldId)
                .ToList();

            // Remove the dependent values
            dbContext.Values.RemoveRange(values);
            if (await dbContext.SaveChangesAsync() != values.Count)
            {
                throw new Exception("Failed to delete some values");
            }
            // Remove the fields
            dbContext.Fields.Remove(field);
            if (await dbContext.SaveChangesAsync() != 1)
            {
                throw new Exception("Failed to delete the field");
            }
        }

        /// <summary>Deletes the fields and dependent values</summary>
        /// <remarks>Not transactional</remarks>
        public static async Task DeleteFieldsAsync(this ApplicationDbContext dbContext, IEnumerable<int> fieldIds)
        {
            var fields = dbContext.Fields
                .Where(field => fieldIds.Contains(field.Id))
                .ToList();
            
            var values = dbContext.Values
                .Where(value => fieldIds.Contains(value.FieldId))
                .ToList();

            // Remove the dependent values
            dbContext.Values.RemoveRange(values);
            if (await dbContext.SaveChangesAsync() != values.Count)
            {
                throw new Exception("Failed to delete some values");
            }
            // Remove the fields
            dbContext.Fields.RemoveRange(fields);
            if (await dbContext.SaveChangesAsync() != fields.Count)
            {
                throw new Exception("Failed to delete some fields");
            }
        }
        /// <summary>Deletes the fields and dependent values</summary>
        /// <remarks>Not transactional</remarks>
        // public static async Task DeleteFieldsAsync(this ApplicationDbContext dbContext, IEnumerable<FieldsTable> fields)
        // {
        //     var fieldIds = fields.Select(field => field.Id);

        //     var values = dbContext.Values
        //         .Where(value => fieldIds.Contains(value.FieldId))
        //         .ToList();

        //     // Remove the dependent values
        //     dbContext.Values.RemoveRange(values);
        //     if (await dbContext.SaveChangesAsync() != values.Count)
        //     {
        //         throw new Exception("Failed to delete some values");
        //     }
        //     // Remove the fields
        //     dbContext.Fields.RemoveRange(fields);
        //     if (await dbContext.SaveChangesAsync() != fields.Count)
        //     {
        //         throw new Exception("Failed to delete some fields");
        //     }
        // }

        public static async Task DeleteGridTransactionAsync(this ApplicationDbContext dbContext, int gridId)
        {
            var grid = dbContext.Grids
                .Where(grid => grid.Id == gridId)
                .FirstOrDefault() ?? throw new Exception($"Grid {gridId} does not exist");

            var fields = dbContext.Fields
                .Where(field => field.GridId == gridId)
                .ToList();

            var fieldIds = fields.Select(field => field.Id);
            var values = dbContext.Values
                .Where(value => fieldIds.Contains(value.FieldId))
                .ToList();

            var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                dbContext.Grids.Remove(grid);
                if (await dbContext.SaveChangesAsync() != 1)
                {
                    throw new Exception("Failed to delete a grid");
                }

                dbContext.Fields.RemoveRange(fields);
                if (await dbContext.SaveChangesAsync() != fields.Count)
                {
                    throw new Exception("Not all fields were deleted");
                }

                // dbContext.DeleteFieldsAsync(fields);

                dbContext.Values.RemoveRange(values);
                if (await dbContext.SaveChangesAsync() != values.Count)
                {
                    throw new Exception("Not all values were deleted");
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            await transaction.CommitAsync();
        }

        /// <remarks>I think is safe to use as is and not wrap into a transaction?</remarks>
        public static async Task InsertRowIntoGridAsync(this ApplicationDbContext dbContext, int gridId, IEnumerable<DataGridValue> valuesEnumerable)
        {
            // TODO! Consideration: maybe don't do this
            var values = valuesEnumerable.ToList();

            var grid = await dbContext.Grids.FindAsync(gridId)
                ?? throw new Exception($"Grid {gridId} does not exist");
            
            var gridFields = dbContext.GetGridFields(gridId).ToList();
            var signature = gridFields.ToDataGridSignature();

            if (!gridFields.Any())
            {
                throw new Exception($"Grid {gridId} doesn't have any fields");
            }

            if (signature.Fields.Count != values.Count)
            {
                throw new Exception("Number of values does not match the signature");
            }

            // Validate the values against the signature
            var (areValuesValid, message) = signature.ValidateValues(values);
            if (!areValuesValid)
            {
                throw new Exception(message);
            }

            if (values.Count == 0)
            {
                throw new Exception("Values are valid, but the number of values is 0");
            }

            // Assign field ids to the values
            for (int i = 0; i < values.Count; ++i)
            {
                values[i].FieldId = gridFields[i].Id;
            }

            // Calculate the index for the new row            
            // Find all the values corresponding to some column

            var sampleFieldId = values[0].FieldId;

            var fieldValues = dbContext.Values
                .Where(value => value.FieldId == sampleFieldId)
                .Select(value => value.RowIndex);

            // If the column contains no values, default to 0, otherwise increment
            var newRowIndex = fieldValues.Any() ? fieldValues.Max() + 1 : 0;

            // Set the index and write the values
            foreach (var value in values)
            {
                value.RowIndex = newRowIndex;
            }

            dbContext.AddRange(values);
            if (await dbContext.SaveChangesAsync() != values.Count)
            {
                throw new Exception("Not all values were inserted");
            }
        }
    }

}