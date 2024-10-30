using centaureatest.Migrations.AuthDb;
using CentaureaTest.Models;
using CentaureaTest.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public static IEnumerable<DataGridValue> GetGridRow(this ApplicationDbContext dbContext, int gridId, int rowIndex)
        {
            var fields = dbContext.GetGridFields(gridId);
            return dbContext.Values.Where(value => value.RowIndex == rowIndex && fields.Any(field => field.Id == value.FieldId));
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

            var rows = dbContext
                .GetGridRows(gridId)
                .Select(row => row.Row)
                .OrderBy(row => row.Items.FirstOrDefault()?.RowIndex);

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

        /// <remarks>See this method overloaded for FieldsTable</remarks>
        public static async Task TryDeleteSelectOptionsAsync(this ApplicationDbContext dbContext, int fieldId)
        {
            var field = await dbContext.Fields.FindAsync(fieldId)
                ?? throw new Exception($"Field {fieldId} does not exist");

            await dbContext.TryDeleteSelectOptionsAsync(field);
        }

        /// <summary>
        /// Iterates over field tables and deletes single/multi select options accordingly
        /// </summary>
        public static async Task TryDeleteSelectOptionsAsync(this ApplicationDbContext dbContext, IEnumerable<FieldsTable> fields)
        {
            foreach (var field in fields)
            {
                await dbContext.TryDeleteSelectOptionsAsync(field);
            }
        }

        public static async Task DeleteFieldDependentValuesAsync(this ApplicationDbContext dbContext, int fieldId)
        {
            var dependentValues = await dbContext.Values
                .Where(value => value.FieldId == fieldId)
                .ToListAsync();
            
            dbContext.Values.RemoveRange(dependentValues);
            if (await dbContext.SaveChangesAsync() != dependentValues.Count)
            {
                throw new Exception("Some dependent fields were not deleted");
            }
        }

        public static async Task DeleteFieldDependentValuesAsync(this ApplicationDbContext dbContext, IEnumerable<int> fieldIds)
        {
            foreach (var fieldId in fieldIds)
            {
                await dbContext.DeleteFieldDependentValuesAsync(fieldId);
            }
        }

        public static async Task DeleteFieldDependentValuesAsync(this ApplicationDbContext dbContext, IEnumerable<FieldsTable> fields)
        {
            await dbContext.DeleteFieldDependentValuesAsync(fields.Select(field => field.Id));
        }

        // Deletes ref values referencing the field
        public static async Task DeleteFieldRefsAsync(this ApplicationDbContext dbContext, int fieldId)
        {
            var refs = await dbContext.Values
                .OfType<DataGridRefValue>()
                .Where(value => value.ReferencedFieldId == fieldId)
                .ToListAsync();
            
            dbContext.Values.RemoveRange(refs);

            if (await dbContext.SaveChangesAsync() != refs.Count)
            {
                throw new Exception("Failed to remove some reference values");
            }
        }

        /// <summary>Deletes dependent values, refs and single / multi select options dependent on 'fieldId'</summary>
        public static async Task DeleteFieldDependenciesAsync(this ApplicationDbContext dbContext, int fieldId)
        {
            await dbContext.DeleteFieldDependentValuesAsync(fieldId);
            await dbContext.DeleteFieldRefsAsync(fieldId);
            await dbContext.TryDeleteSelectOptionsAsync(fieldId);
        }

        /// <summary>Deletes the field and its dependencies</summary>
        /// <remarks>Not transactional</remarks>
        public static async Task DeleteFieldAsync(this ApplicationDbContext dbContext, int fieldId)
        {
            var field = dbContext.Fields.Find(fieldId)
                ?? throw new Exception($"Field {fieldId} does not exist");
        
            await dbContext.DeleteFieldDependenciesAsync(fieldId);

            // Remove the fields
            dbContext.Fields.Remove(field);
            if (await dbContext.SaveChangesAsync() != 1)
            {
                throw new Exception("Failed to delete the field");
            }
        }

        /// <summary>Deletes the fields with given ids and dependent values</summary>
        /// <remarks>Not transactional</remarks>
        public static async Task DeleteFieldsAsync(this ApplicationDbContext dbContext, IEnumerable<int> fieldIds)
        {
            foreach (var fieldId in fieldIds)
            {
                await dbContext.DeleteFieldAsync(fieldId);
            }
        }
        /// <summary>Deletes the fields and dependent values</summary>
        /// <remarks>Not transactional</remarks>
        public static async Task DeleteFieldsAsync(this ApplicationDbContext dbContext, IEnumerable<FieldsTable> fields)
        {
            await dbContext.DeleteFieldsAsync(fields.Select(field => field.Id));
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

        public static async Task ValidateValuesAsync(this ApplicationDbContext dbContext, IEnumerable<DataGridValue> values)
        {
            foreach (var value in values)
            {
                await dbContext.ValidateValueAsync(value);
            }
        }

        /// <summary>Validates the field signature and special values (complete validation)</summary>
        /// <remarks>Throws on validation failure</remarks>
        public static async Task ValidateValueAsync(this ApplicationDbContext dbContext, DataGridValue value)
        {
            var field = await dbContext.Fields.FindAsync(value.FieldId);
            if (field is null)
            {
                throw new Exception($"Field {value.FieldId} does not exist");
            }

            var signature = field.ToDataGridFieldSignature();
            var (ok, message) = value.Validate(signature);
            if (!ok)
            {
                throw new Exception(message);
            }

            await dbContext.ValidateSpecialValueAsync(value);
        }

        /// <summary>Validates that the reference of a ref value</summary>
        /// <remarks>Throws on validation failure</remarks>
        public static async Task ValidateValueAsync(this ApplicationDbContext dbContext, DataGridRefValue value)
        {
            if (await dbContext.Fields.FindAsync(value.ReferencedFieldId) is null)
            {
                throw new Exception($"Field {value.ReferencedFieldId} does not exist");
            }
        }

        /// <summary>Validate the options of a single select value</summary>
        /// <remarks>Throws on validation failure</remarks>
        public static async Task ValidateValueAsync(this ApplicationDbContext dbContext, DataGridSingleSelectValue value)
        {
            if (await dbContext.SingleSelectOptions.FindAsync(value.OptionId) is null)
            {
                throw new Exception($"Single select option {value.OptionId} does not exist");
            }
        }

        /// <summary>Validate the options of a multi select value</summary>
        /// <remarks>Throws on validation failure</remarks>
        public static async Task ValidateValueAsync(this ApplicationDbContext dbContext, DataGridMultiSelectValue value)
        {
            foreach (var optionId in value.OptionIds)
            {
                if (await dbContext.MultiSelectOptions.FindAsync(optionId) is null)
                {
                    throw new Exception($"Multi select option {optionId} does not exist");
                }
            }
        }

        /// <summary>Check if the value if special (ref, select) and if so, validates it</summary>
        public static async Task ValidateSpecialValueAsync(this ApplicationDbContext dbContext, DataGridValue value)
        {
            switch (value)
            {
                case DataGridRefValue refValue:
                    await dbContext.ValidateValueAsync(refValue);
                    break;
                
                case DataGridSingleSelectValue singleSelectValue:
                    await dbContext.ValidateValueAsync(singleSelectValue);
                    break;

                case DataGridMultiSelectValue multiSelectValue:
                    await dbContext.ValidateValueAsync(multiSelectValue);
                    break;
            }
        }

        /// <summary>Updates an existing value Throws on failure</summary>
        public static async Task UpdateValueAsync(this ApplicationDbContext dbContext, DataGridValue value)
        {
            await dbContext.ValidateValueAsync(value);

            var existingValue = await dbContext.Values
                .Where(existingValue => existingValue.FieldId == value.FieldId && existingValue.RowIndex == value.RowIndex)
                .FirstOrDefaultAsync()
                ?? throw new Exception($"Value at field {value.FieldId}, row {value.RowIndex} does not exist");

            // Kostyl
            // TODO! Fix the kostyl
            switch (value)
            {
                case DataGridStringValue castValue:
                    ((DataGridStringValue)existingValue).Value = castValue.Value;
                    break;

                case DataGridNumericValue castValue:
                    ((DataGridNumericValue)existingValue).Value = castValue.Value;
                    break;

                case DataGridRegexValue castValue:
                    ((DataGridRegexValue)existingValue).Value = castValue.Value;
                    break;

                case DataGridEmailValue castValue:
                    ((DataGridEmailValue)existingValue).Value = castValue.Value;
                    break;

                case DataGridRefValue castValue:
                    ((DataGridRefValue)existingValue).ReferencedFieldId = castValue.ReferencedFieldId;
                    break;

                case DataGridSingleSelectValue castValue:
                    ((DataGridSingleSelectValue)existingValue).OptionId = castValue.OptionId;
                    break;

                case DataGridMultiSelectValue castValue:
                    ((DataGridMultiSelectValue)existingValue).OptionIds = castValue.OptionIds;
                    break;

                default:
                    throw new NotImplementedException($"Updating values of type {value.Type} is not implemented");
            }

            // NB: doesn't check that the update was succesfull
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// If the value exist, updates it<br/>
        /// Otherwise, creates a new value with an incremeneted rowIndex
        /// </summary>
        /// <returns>Id of updated/insert value</returns>
        public static async Task<int> InsertValueAsync(this ApplicationDbContext dbContext, DataGridValue value)
        {
            var existingValue = await dbContext.Values.FindAsync(value.Id);

            if (existingValue is null)
            {
                await dbContext.ValidateValueAsync(value);

                await dbContext.Values.AddAsync(value);
                if (await dbContext.SaveChangesAsync() != 1)
                {
                    throw new Exception("Failed to add a new value");
                }

                return value.Id;   
            }
            else
            {
                try
                {
                    await dbContext.UpdateValueAsync(value);
                }
                catch (Exception e)
                {
                    throw new Exception("InsertValue tried to update an existing value and failed", e);
                }

                return existingValue.Id;
            }

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
            await dbContext.ValidateValuesAsync(values);

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

        public static async Task AddFieldToGridAsync(this ApplicationDbContext dbContext, int gridId, DataGridFieldSignature fieldSignature)
        {
            var grid = await dbContext.Grids.FindAsync(gridId);
            if (grid is null)
            {
                throw new Exception($"Grid {gridId} does not exist");
            }

            var fieldsTable = fieldSignature.ToFieldsTable(gridId);
            await dbContext.Fields.AddAsync(fieldsTable);
            
            var nAdded = await dbContext.SaveChangesAsync();
            if (nAdded != 1)
            {
                throw new Exception("Failed to add a field");
            }
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