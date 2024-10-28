using CentaureaTest.Auth;
using CentaureaTest.Data;
using CentaureaTest.Models;
using CentaureaTest.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentaureaTest.Controllers
{

    [ApiController]
    [Route("api/datagrid")]
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
        [Authorize(Roles = "Admin, SuperUser")]
        [HttpPost("grid")]
        public async Task<IActionResult> CreateGrid([FromBody] CreateDataGridDto gridDto)
        {
            try
            {
                await _dbContext.CreateTablesFromDtoTransactionAsync(gridDto);
                return Ok();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
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
                await _dbContext.InsertRowAsync(gridId, valueDtos.Select(valueDto => valueDto.ToDataGridValue()));
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }

            return Ok();
        }
    }

}