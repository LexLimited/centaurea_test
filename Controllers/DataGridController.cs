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

        /// <summary>Dummy page for memes</summary>
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(_dbContext.Grids);
        }

        /// <summary>Returns an existing grid</summary>
        // [Authorize]
        [HttpGet("grid")]
        public async Task<IActionResult> GetGridById([FromQuery] int gridId)
        {
            var grid = await _dbContext.GetDataGridAsync(gridId);
            
            return grid is null
                ? new NotFoundResult() : Ok(grid);
        }
        
        /// <summary>Creates a new grid</summary>
        // [Authorize]
        [HttpPost("grid")]
        public async Task<IActionResult> CreateDataGrid([FromBody] DataGridDto gridDto)
        {
            var grid = gridDto.ToDataGrid();
            Console.WriteLine($"Will insert the following grid: {grid}");

            if (!grid.Signature.Fields.GroupBy(field => field.Order).All(group => group.Count() == 1))
            {
                return BadRequest("Order must not repeat");
            }

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

                var createdGridId = gridsTable.Id;

                // Insert into fields table
                await _dbContext.Fields.AddRangeAsync(grid.Signature.Fields.ToFieldsTables(createdGridId));
                nAdded = await _dbContext.SaveChangesAsync();

                if (nAdded != grid.Signature.Fields.Count)
                {
                    throw new Exception("Not all fields were written");
                }

                // Insert into values table
                foreach (var row in grid.Rows)
                {
                    try
                    {
                        await _dbContext.InsertRowIntoGridAsync(createdGridId, row.Items);
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        return Problem(e.Message);
                    }
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

        /// <summary>Deletes an existing grid</summary>
        [HttpDelete("grid/{gridId}")]
        public async Task<IActionResult> DeleteGrid(int gridId)
        {
            try
            {
                await _dbContext.DeleteGridTransactionAsync(gridId);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
            
            return Ok(gridId);
        }

        /// <summary>Adds a new column to an existing table</summary>
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

        /// <summary>Deletes a field from a grid</summary>
        [HttpDelete("field/{fieldId}")]
        public async Task<IActionResult> DeleteField(int fieldId)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await _dbContext.DeleteFieldAsync(fieldId);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return Problem(e.Message);
            }
            await transaction.CommitAsync();

            return Ok(fieldId);
        }

        /// <summary>Deletes a list of fields from a grid</summary>
        [HttpDelete("fields")]
        public async Task<IActionResult> DeleteFields([FromBody] List<int> fieldIds)
        {
            if (fieldIds is null)
            {
                return BadRequest("Expected property 'fieldIds'");
            }

            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await _dbContext.DeleteFieldsAsync(fieldIds);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return Problem(e.Message);
            }
            await transaction.CommitAsync();

            return Ok(fieldIds);
        }

        /// <summary>Adds a new row to the grid</summary>
        [HttpPost("row/{gridId}")]
        public async Task<IActionResult> AddRow(int gridId, [FromBody] List<DataGridValueDto> valueDtos)
        {
            try
            {
                await _dbContext.InsertRowIntoGridAsync(gridId, valueDtos.Select(valueDto => valueDto.ToDataGridValue()));
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }

            return Ok();
        }
    }

}