using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text.RegularExpressions;
using CentaureaTest.Models;
using CentaureaTest.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Query.Internal;

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

        public static IEnumerable<DataGridValue> GetGridRow(this ApplicationDbContext dbContext, int gridId, int rowIndex)
        {
            var fields = dbContext.GetGridFields(gridId);
            return dbContext.Values
                .Where(value => fields.Any(field => field.Id == value.FieldId) && value.RowIndex == rowIndex);
        }

        public static IEnumerable<(int RowIndex, DataGridRow Row)> GetGridRows(this ApplicationDbContext dbContext, int gridId)
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

            return valueGroups.Select(group => (group.Key, new DataGridRow(group)));
        }

        public static IEnumerable<DataGridValue> GetGridRowValues(this ApplicationDbContext dbContext, int gridId, int rowIndex)
        {
            var fieldIds = dbContext.Fields
                .Where(field => field.GridId == gridId)
                .Select(field => field.Id)
                .ToList();

            return dbContext.Values
                .Where(value => fieldIds.Contains(value.FieldId) && value.RowIndex == rowIndex)
                .OrderBy(value => value.RowIndex);
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

            var rows = dbContext.GetGridRows(gridId).Select(row => row.Row);

            return new DataGrid(grid.Id, grid.Name, signature, rows);
        }

        public static async Task DeleteSingleSelectTableAsync(this ApplicationDbContext dbContext, int fieldId)
        {
            var options = dbContext.SingleSelectOptions
                .Where(option => option.FieldId == fieldId)
                .ToList();

            dbContext.RemoveRange(options);
            if (await dbContext.SaveChangesAsync() != options.Count)
            {
                throw new Exception($"Failed to delete some single select options for field {fieldId}");
            }
        }

        public static async Task DeleteMultiSelectTableAsync(this ApplicationDbContext dbContext, int fieldId)
        {
            var options = dbContext.MultiSelectOptions
                .Where(option => option.FieldId == fieldId)
                .ToList();

            dbContext.RemoveRange(options);
            if (await dbContext.SaveChangesAsync() != options.Count)
            {
                throw new Exception($"Failed to delete some multi select options for field {fieldId}");
            }
        }

        /// <summary>
        /// Deletes single/multi select options if the field has a corresponding type
        /// </summary>
        public static async Task TryDeleteSelectOptionsAsync(this ApplicationDbContext dbContext, FieldsTable field)
        {
            if (field.Type == DataGridValueType.SingleSelect)
            {
                await dbContext.DeleteSingleSelectTableAsync(field.Id);
            }
            else if (field.Type == DataGridValueType.MultiSelect)
            {
                await dbContext.DeleteMultiSelectTableAsync(field.Id);
            }
        }

        /// <summary>
        /// Iterates over field tables and deletes single/multi select options if the
        /// field has a corresponding type
        /// </summary>
        public static async Task TryDeleteSelectOptionsAsync(this ApplicationDbContext dbContext, IEnumerable<FieldsTable> fields)
        {
            foreach (var field in fields)
            {
                await dbContext.TryDeleteSelectOptionsAsync(field);
            }
        }

        // TODO! Consider if I need to retain both delete field and delete fields
        // or should the delete fields simply call delete field and if not,
        // could could I reduce the code duplication
        /// <summary>Deletes the field and dependent values</summary>
        /// <remarks>Not transactional</remarks>
        public static async Task DeleteFieldAsync(this ApplicationDbContext dbContext, int fieldId)
        {
            var field = dbContext.Fields.Find(fieldId)
                ?? throw new Exception($"Field {fieldId} does not exist");
        
            var values = dbContext.Values
                .Where(value => value.FieldId == fieldId)
                .ToList();

            // If single select or multi select, remove corresponding options
            // TODO! Do something about this code duplication
            await dbContext.TryDeleteSelectOptionsAsync(field);

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
            
            await dbContext.DeleteFieldsAsync(fields);
        }
        /// <summary>Deletes the fields and dependent values</summary>
        /// <remarks>Not transactional</remarks>
        public static async Task DeleteFieldsAsync(this ApplicationDbContext dbContext, IEnumerable<FieldsTable> fields)
        {
            // NB! Some redundancy here
            var fieldIds = fields.Select(field => field.Id).ToList();

            var values = dbContext.Values
                .Where(value => fieldIds.Contains(value.FieldId))
                .ToList();

            // If the field is single or multi select, delete the corresponding choice tables
            await dbContext.TryDeleteSelectOptionsAsync(fields);

            // Remove the dependent values
            dbContext.Values.RemoveRange(values);
            if (await dbContext.SaveChangesAsync() != values.Count)
            {
                throw new Exception("Failed to delete some values");
            }
            // Remove the fields
            dbContext.Fields.RemoveRange(fields);
            if (await dbContext.SaveChangesAsync() != fieldIds.Count)
            {
                throw new Exception("Failed to delete some fields");
            }
        }

        /// <summary>
        /// Deletes the grid, its fields and value and all fields that depend on the grid
        /// form other tables.
        /// A field depends on the grid with 'gridId' if it is a 'Ref' field with ReferencedGridId
        /// equal to 'gridId'
        ///</summary>
        public static async Task DeleteGridTransactionAsync(this ApplicationDbContext dbContext, int gridId, bool deleteDependentFields = true)
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

                await dbContext.DeleteFieldsAsync(fields);
                if (deleteDependentFields)
                {
                    await dbContext.DeleteDependentFieldsAsync(grid.Id);
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            await transaction.CommitAsync();
        }

        /// <summary>Deletes all fields dependent on gridId</summary>
        public static async Task DeleteDependentFieldsAsync(this ApplicationDbContext dbContext, int gridId)
        {
            var dependentFields = await dbContext.Fields.OfType<RefFieldsTable>()
                .Where(field => field.ReferencedGridId == gridId)
                .ToListAsync();

            if (dependentFields is null)
            {
                return;
            }

            await dbContext.DeleteFieldsAsync(dependentFields);
        }

        /// <summary>Validates all values including single and multi select</summary>
        /// <remarks>Throws if the value is invalid (see 'ValidateValue' of signature for more)</remarks>
        public static async void ValidateValue(this ApplicationDbContext dbContext, DataGridFieldSignature signature, DataGridValue value)
        {
            var (ok, message) = value.Validate(signature);
            if (!ok)
            {
                throw new Exception(message);
            }

            /// Validate ref, single and multi select
            if (value.Type == DataGridValueType.Ref)
            {
                var refValue = (DataGridRefValue)value ?? throw new Exception("Inconsistent object");
                if (await dbContext.Fields.FindAsync(refValue.ReferencedFieldId) is null)
                {
                    throw new Exception($"Field {refValue.ReferencedFieldId} does not exist");
                }
            }

            if (value.Type == DataGridValueType.SingleSelect)
            {
                var singleSelectValue = (DataGridSingleSelectValue)value ?? throw new Exception("Inconsistent object");
                if (await dbContext.Fields.FindAsync(singleSelectValue.OptionId) is null)
                {
                    throw new Exception($"Option {singleSelectValue.OptionId} does not exist");
                }
            }

            if (value.Type == DataGridValueType.MultiSelect)
            {
                var multiSelectValue = (DataGridMultiSelectValue)value ?? throw new Exception("Inconsistent object");
                var missingOptionIds = multiSelectValue.OptionIds
                    .Where(optionId => !dbContext.MultiSelectOptions
                    .Any(option => option.Id == optionId));

                if (missingOptionIds.Any())
                {
                    throw new Exception("Some multi select options do not exist");
                }
            }
        }

        /// <summary>Validates all values including single and multi select</summary>
        /// <remarks>Throws if values are invalid</remarks>
        public static async Task ValidateValues(this ApplicationDbContext dbContext, DataGridSignature signature, IEnumerable<DataGridValue> values)
        {
            /// Validate signature constraints
            var (ok, message) = signature.ValidateValues(values);
            if (!ok)
            {
                throw new Exception(message);
            }

            /// Validate ref, single and multi select
            var refValues = values.OfType<DataGridRefValue>();
            foreach (var value in refValues)
            {
                if (await dbContext.Fields.FindAsync(value.ReferencedFieldId) is null)
                {
                    throw new Exception($"Field {value.ReferencedFieldId} does not exist");
                }
            }

            var singleSelectValues = values.OfType<DataGridSingleSelectValue>();
            foreach (var value in singleSelectValues)
            {
                if (await dbContext.SingleSelectOptions.FindAsync(value.OptionId) is null)
                {
                    throw new Exception($"Single select option {value.OptionId} does not exist");
                }
            }

            var multiSelectValues = values.OfType<DataGridMultiSelectValue>();
            foreach (var value in multiSelectValues)
            {
                var missingOptionIds = value.OptionIds
                    .Where(optionId => !dbContext.MultiSelectOptions.Any(option => option.Id == optionId));

                if (missingOptionIds.Any())
                {
                    throw new Exception("Some multi select options do not exist");
                }
            }
        }

        /// <summary>Updates an existing value Throws on failure</summary>
        public static async Task UpdateValueAsync(this ApplicationDbContext dbContext, DataGridValue value)
        {
            var field = await dbContext.Fields.FindAsync(value.FieldId);

            if (field is null)
            {
                throw new Exception($"Field {value.FieldId} does not exist");
            }

            var signature = field.ToDataGridFieldSignature();
            dbContext.ValidateValue(signature, value);


            /// TODO! Lyuti costyl -- this must be rewritten
            DataGridValue existingValue = value.Type switch
            {
                DataGridValueType.String => await dbContext.Values.OfType<DataGridStringValue>()
                    .FirstOrDefaultAsync(v => v.FieldId == value.FieldId && v.RowIndex == value.RowIndex) as DataGridValue,

                DataGridValueType.Numeric => await dbContext.Values.OfType<DataGridNumericValue>()
                    .FirstOrDefaultAsync(v => v.FieldId == value.FieldId && v.RowIndex == value.RowIndex),

                DataGridValueType.Regex => await dbContext.Values.OfType<DataGridRegexValue>()
                    .FirstOrDefaultAsync(v => v.FieldId == value.FieldId && v.RowIndex == value.RowIndex),

                DataGridValueType.Ref => await dbContext.Values.OfType<DataGridRefValue>()
                    .FirstOrDefaultAsync(v => v.FieldId == value.FieldId && v.RowIndex == value.RowIndex),

                _ => throw new NotImplementedException($"Updating values of type {value.Type} is not implemented")
            } ?? throw new Exception($"Value at field {value.FieldId}, row {value.RowIndex} not found");

            // Update the existing value based on type-specific casting
            switch (value)
            {
                case DataGridStringValue stringValue:
                    ((DataGridStringValue)existingValue).Value = stringValue.Value;
                    break;

                case DataGridNumericValue numericValue:
                    ((DataGridNumericValue)existingValue).Value = numericValue.Value;
                    break;

                case DataGridRegexValue regexValue:
                    ((DataGridRegexValue)existingValue).Value = regexValue.Value;
                    break;

                case DataGridRefValue refValue:
                    ((DataGridRefValue)existingValue).ReferencedFieldId = refValue.ReferencedFieldId;
                    break;

                default:
                    throw new NotImplementedException($"Updating values of type {value.Type} is not implemented");
            }

            await dbContext.SaveChangesAsync();
        }

        /// <remarks>
        /// I think is safe to use as is and not wrap into a transaction?<br/>
        /// Throws on failure
        /// </remarks>
        public static async Task InsertRowAsync(this ApplicationDbContext dbContext, int gridId, IEnumerable<DataGridValue> valuesEnumerable)
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
            await dbContext.ValidateValues(signature, values);

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

        public static void DeleteRow(this ApplicationDbContext dbContext, int gridId, int rowIndex)
        {
            dbContext.Values.RemoveRange(dbContext.GetGridRowValues(gridId, rowIndex));
        }

        public static GridsTable? GetGridForFieldId(this ApplicationDbContext dbContext, int fieldId)
        {
            var field = dbContext.Fields.Find(fieldId);
            if (field is null)
            {
                return null;
            }
        
            return dbContext.Grids.FirstOrDefault(grid => grid.Id == field.GridId);
        
        }

        public static DataGridValue? GetValue(this ApplicationDbContext dbContext, int fieldId, int rowIndex)
        {
            var grid = dbContext.GetGridForFieldId(fieldId);
            if (grid is null)
            {
                return null;
            }

            return dbContext.GetGridRowValues(grid.Id, rowIndex)
                .FirstOrDefault(value => value.RowIndex == rowIndex);
        }

        /// <summary>Change an existing value</summary>
        public static void PutValue(this ApplicationDbContext dbContext, int fieldId, int rowIndex, DataGridValue newValue)
        {
            var value = dbContext.GetValue(fieldId, rowIndex)
                ?? throw new Exception($"Value in field {fieldId} in row {rowIndex} does not exist");
            
            if (value.Type != newValue.Type)
            {
                throw new Exception($"Invalid value type {newValue.Type}, expected: {value.Type}");
            }

            dbContext.Values.Add(newValue);
            dbContext.SaveChanges();

            throw new NotImplementedException("PutValue extension method on ApplicationDbContext is not implemented");
        }

        /// <summary>Inserts options into single select table</summary>
        /// <remarks>Throws on failure</remarks>
        /// <returns>Ids of inserted options</returns>
        public static async Task<IEnumerable<int>> CreateSingleSelectOptionsAsync(this ApplicationDbContext dbContext, int fieldId, IEnumerable<string> options)
        {
            var tables = options.Select(option => new SingleSelectTable(fieldId, option)).ToList();

            await dbContext.SingleSelectOptions.AddRangeAsync(tables);
            if (await dbContext.SaveChangesAsync() != tables.Count)
            {
                throw new Exception("Failed to insert some options into single select table");
            }

            return tables.Select(table => table.Id);
        }

        /// <summary>Inserts options into multi select table</summary>
        /// <remarks>Throws on failure</remarks>
        /// <returns>Ids of inserted options</returns>
        public static async Task<IEnumerable<int>> CreateMultiSelectOptionsAsync(this ApplicationDbContext dbContext, int fieldId, IEnumerable<string> options)
        {
            var tables = options.Select(option => new MultiSelectTable(fieldId, option)).ToList();

            await dbContext.MultiSelectOptions.AddRangeAsync(tables);
            if (await dbContext.SaveChangesAsync() != tables.Count)
            {
                throw new Exception("Failed to insert some option into multi select table");
            }

            return tables.Select(table => table.Id);
        }

        /// <summary>Creates a new grids table entry from grid dto</summary>
        /// <returns>Id of a newly created grid</returns>
        public static async Task<int> CreateGridsTableFromDto(this ApplicationDbContext dbContext, CreateDataGridDto gridDto)
        {
            // Insert into grids table
            var gridsTable = gridDto.ToGridsTable();
            dbContext.Grids.Add(gridsTable);
            await dbContext.SaveChangesAsync();
            return gridsTable.Id;
        }

        /// <summary>Create single and multiselect tables from dto</summary>
        /// <remarks>
        /// Not transactional<br/>
        /// Throws on error
        /// </remarks>
        public static async Task CreateSelectTablesFromDtoFields
        (
            this ApplicationDbContext dbContext,
            IEnumerable<(int FieldId, CreateDataGridFieldSignatureDto FieldSignatureDto)> fieldSignatureDtos
        )
        {
            var singleSelectFields = fieldSignatureDtos
                .Where(field => field.FieldSignatureDto.Type == DataGridValueType.SingleSelect);

            var multiSelectFields = fieldSignatureDtos
                .Where(field => field.FieldSignatureDto.Type == DataGridValueType.MultiSelect);

            foreach (var (fieldId, fieldSignatureDto) in singleSelectFields)
            {
                await dbContext.CreateSingleSelectOptionsAsync(
                    fieldId, fieldSignatureDto.Options ?? throw new Exception("'Options' field is required for 'SingleSelect' dto")
                );
            }

            foreach (var (fieldId, fieldSignatureDto) in multiSelectFields)
            {
                await dbContext.CreateMultiSelectOptionsAsync(
                    fieldId, fieldSignatureDto.Options ?? throw new Exception("'Options' field is required for 'MultiSelect' dto")
                );
            }
        }

        /// <summary>Inserts grids, fields, options and values base on the dto</summary>
        /// <remarks>Throws on failure</remarks>
        public static async Task CreateTablesFromDtoTransactionAsync(this ApplicationDbContext dbContext, CreateDataGridDto gridDto)
        {
            var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                // Create grids table
                var gridId = await dbContext.CreateGridsTableFromDto(gridDto);

                // Insert all field tables in one operation
                var fieldTables = gridDto.Signature.Fields.ToFieldsTables().ToList();
                foreach (var fieldTable in fieldTables)
                {
                    fieldTable.GridId = gridId;
                }

                dbContext.AddRange(fieldTables);

                // Save changes and get the generated field IDs
                if (await dbContext.SaveChangesAsync() != fieldTables.Count)
                {
                    throw new Exception("Some fields were not inserted");
                }

                // Map field IDs to their corresponding DTOs
                var fieldSignatureDtosWithIds = fieldTables
                    .Select((fieldTable, index) => (fieldTable.Id, gridDto.Signature.Fields[index]))
                    .ToList();

                // Insert select tables now that the field IDs are known
                await dbContext.CreateSelectTablesFromDtoFields(fieldSignatureDtosWithIds);

                // Insert into values table
                foreach (var row in gridDto.Rows)
                {
                    await dbContext.InsertRowAsync(gridId, row.ToDataGridValues());
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            await transaction.CommitAsync();
        }

    }

}