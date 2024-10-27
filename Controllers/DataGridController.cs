using CentaureaTest.Data;
using CentaureaTest.Models;
using CentaureaTest.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CentaureaTest.Controllers
{

    [ApiController]
    [Route("api/datagrid")]
    // [Authorize]
    public sealed class DataGridController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DataGridController> _logger;

        public DataGridController(ApplicationDbContext dbContext, ILogger<DataGridController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Dummy page for memes
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(_dbContext.Grids);
        }

        /// <summary>
        /// Returns an existing grid
        /// </summary>
        // [Authorize]
        [HttpGet("grid")]
        public async Task<IActionResult> GetGridById([FromQuery] int gridId)
        {
            var grid = await _dbContext.GetDataGridAsync(gridId);
            
            if (grid is null)
            {
                return new NotFoundResult();
            }

            _logger.LogInformation("Got the following fields:");
            foreach (var field in grid.Signature.Fields)
            {
                _logger.LogInformation("Field: {}", field);
            }

            return Ok(grid);
        }
        
        /// <summary>
        /// Creates a new grid
        /// </summary>
        // [Authorize]
        [HttpPost("grid")]
        public async Task<IActionResult> CreateDataGrid([FromBody] DataGridDto gridDto)
        {
            var grid = gridDto.ToDataGrid();
            Console.WriteLine($"Will insert the following grid: {grid}");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Insert into grids table
                var gridsTable = grid.ToGridsTable();
                await _dbContext.AddAsync(gridsTable);
                var nAdded = await _dbContext.SaveChangesAsync();

                if (nAdded == 0)
                {
                    throw new Exception("Failed to insert new grid");
                }

                if (grid.Signature.Fields.Any(field => field is null))
                {
                    throw new Exception("Grid signature contains a null field");
                }

                // Insert into fields table
                await _dbContext.Fields.AddRangeAsync(grid.Signature.Fields.ToFieldsTables(gridsTable.Id));
                nAdded = await _dbContext.SaveChangesAsync();

                if (nAdded != grid.Signature.Fields.Count)
                {
                    throw new Exception("Not all fields were written");
                }

                // Both grids and fields tables are correctly inserted
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError(e, "While adding a grid");

                return Problem("Failed to create a grid");
            }

            return Ok();
        }

        /// <summary>
        /// Deletes an existing grid
        /// </summary>
        [HttpDelete("grid/{gridId}")]
        public async Task<IActionResult> DeleteGrid(int gridId)
        {
            var grid = await _dbContext.Grids.FindAsync(gridId);
            if (grid is null)
            {
                return NotFound($"Grid {gridId} does not exist");
            }

            var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // TODO! Delete values first

                // Delete columns first

                var fields = _dbContext.Fields
                    .Where(field => field.GridId == gridId)
                    .ToList();

                _dbContext.Fields.RemoveRange(fields);
                var nDeleted = await _dbContext.SaveChangesAsync();

                if (nDeleted != fields.Count)
                {
                    throw new Exception("Failed to delete some fields");
                }

                // TODO! Delete dependent columns

                // Delete the grid after all delemndencies were deleted

                _dbContext.Grids.Remove(grid);
                nDeleted = await _dbContext.SaveChangesAsync();
            
                if (nDeleted != 1)
                {
                    throw new Exception("Failed to delete a grid");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "While deleting a grid");
                await transaction.RollbackAsync();

                return Problem();
            }

            await transaction.CommitAsync();

            return Ok(gridId);
        }

        /// <summary>
        /// Adds a new column to an existing table
        /// </summary>
        [HttpPost("{gridId}/field")]
        public async Task<IActionResult> AddField(int gridId, [FromBody] DataGridFieldSignatureDto fieldSignatureDto)
        {
            var fieldSignature = fieldSignatureDto.ToDataGridFieldSignature();

            if (fieldSignature is null)
            {
                return BadRequest("Invalid field signature");
            }

            var grid = await _dbContext.Grids.FindAsync(gridId);
            if (grid is null)
            {
                return NotFound($"Grid {gridId} does not exist");
            }

            var fieldsTable = fieldSignature.ToFieldsTable(gridId);
            await _dbContext.Fields.AddAsync(fieldsTable);
            
            var nAdded = await _dbContext.SaveChangesAsync();
            if (nAdded != 1)
            {
                return Problem("Failed to add a field");
            }

            return Ok(fieldsTable);
        }

        /// <summary>
        /// Deletes a field from a grid
        /// </summary>
        [HttpDelete("field/{fieldId}")]
        public async Task<IActionResult> DeleteField(int fieldId)
        {
            var field = await _dbContext.Fields.FindAsync(fieldId);
            if (field is null)
            {
                return NotFound($"Field {fieldId} does not exist");
            }

            _dbContext.Fields.Remove(field);
            await _dbContext.SaveChangesAsync();

            return Ok(fieldId);
        }

        /// <summary>
        /// Deletes a list of fields from a grid
        /// </summary>
        [HttpDelete("fields")]
        public async Task<IActionResult> DeleteFields([FromBody] List<int> fieldIds)
        {
            if (fieldIds is null)
            {
                return BadRequest("List if field ids is expected");
            }

            // Nothing to do
            if (fieldIds.Count == 0)
            {
                return Ok();
            }

            var fields = await _dbContext.Fields.Where(field => fieldIds.Contains(field.Id)).ToListAsync();

            if (fields.Count != fieldIds.Count)
            {
                return NotFound("Some fields weren't found");
            }

            _dbContext.Fields.RemoveRange(fields);
            await _dbContext.SaveChangesAsync();

            return Ok(fields.Select(field => field.Id));
        }

        /// <summary>
        /// Adds a new row to the grid
        /// </summary>
        [HttpPost("row/{gridId}")]
        public async Task<IActionResult> AddRow(int gridId, [FromBody] List<DataGridValueDto> valueDtos)
        {
            _logger.LogInformation("Received the following value DTOs:");
            foreach (var valueDto in valueDtos)
            {
                _logger.LogInformation("Value DTO: \n{}", valueDto);
            }

            var grid = await _dbContext.Grids.FindAsync(gridId);
            if (grid is null)
            {
                return NotFound($"Grid {gridId} does not exist");
            }

            var gridFields = _dbContext.GetGridFields(gridId).ToList();
            var signature = gridFields.ToDataGridSignature();

            if (!gridFields.Any())
            {
                return Problem($"Grid {gridId} doesn't have any fields");
            }

            if (signature.Fields.Count != valueDtos.Count)
            {
                return BadRequest("Number of values does not match the signature");
            }

            var values = valueDtos.ToDataGridValues().ToList();
            
            // Validate the values against the signature
            var (areValuesValid, message) = signature.ValidateValues(values);
            if (!areValuesValid)
            {
                return BadRequest(message);
            }

            if (values.Count == 0)
            {
                return Problem("Values are valid, but the number of values is 0");
            }

            // Assign field ids to the values
            for (int i = 0; i < values.Count; ++i)
            {
                values[i].FieldId = gridFields[i].Id;
            }

            // Calculate the index for the new row            
            // Find all the values corresponding to some column

            var sampleFieldId = values[0].FieldId;

            var fieldValues = _dbContext.Values
                .Where(value => value.FieldId == sampleFieldId)
                .Select(value => value.RowIndex);

            // If the column contains no values, default to 0, otherwise increment
            var newRowIndex = fieldValues.Any() ? fieldValues.Max() + 1 : 0;

            // Set the index and write the values
            foreach (var value in values)
            {
                value.RowIndex = newRowIndex;
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                _dbContext.AddRange(values);
                if (await _dbContext.SaveChangesAsync() != values.Count)
                {
                    throw new Exception("Not all values were inserted");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "While inserting a row");
                return Problem("Failed to insert a row");
            }

            return Ok(values);
        }
    }

}