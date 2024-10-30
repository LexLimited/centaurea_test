using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
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
            return grid is null ? new NotFoundResult() : Ok(grid);
        }
        
        /// <summary>Creates a new grid</summary>
        // [Authorize(Roles = "Admin, SuperUser")]
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

        /// <summary>Renames an existing grid</summary>
        [HttpPut("grid/{gridId}/rename")]
        public async Task<IActionResult> RenameGrid(int gridId, [FromQuery, Required] string newName)
        {
            var grid = await _dbContext.Grids.FindAsync(gridId);
            if (grid is null)
            {
                return BadRequest($"Grid {gridId} does not exist");
            }

            grid.Name = newName;
            return await _dbContext.SaveChangesAsync() == 1 ? Ok(newName) : Problem("Failed to update the grid's name");
        }

        /// <summary>Adds a new column to an existing table</summary>
        [HttpPost("grid/{gridId}/field")]
        public async Task<IActionResult> AddField(int gridId, [FromBody] DataGridFieldSignatureDto fieldSignatureDto)
        {
            var fieldSignature = fieldSignatureDto.ToDataGridFieldSignature();
            if (fieldSignature is null)
            {
                return BadRequest("Invalid field signature");
            }

            try
            {
                await _dbContext.AddFieldToGridAsync(gridId, fieldSignature);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(fieldSignature);
        }

        /// <summary>Sets a single value (cell) within the field</summary>
        [HttpPut("value")]
        public async Task<IActionResult> UpdateValue([FromQuery] int fieldId, [FromBody] DataGridValueDto valueDto)
        {
            try
            {
                var value = valueDto.ToDataGridValue();
                value.FieldId = fieldId;

                await _dbContext.UpdateValueAsync(value);
                return Ok(valueDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>Inserts a new value</summary>
        [HttpPost("value")]
        public async Task<IActionResult> InsertValue([FromQuery] int fieldId, [FromBody] DataGridValueDto valueDto)
        {
            try
            {
                var value = valueDto.ToDataGridValue();
                value.FieldId = fieldId;

                await _dbContext.InsertValueAsync(value);
                return Ok(valueDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("value")]
        public async Task<IActionResult> DeleteValue([FromQuery] int valueId)
        {
            var value = await _dbContext.Values.FindAsync(valueId);
            if (value is null)
            {
                return BadRequest($"Value {valueId} does not exist");
            }

            _dbContext.Remove(value);
            return await _dbContext.SaveChangesAsync() == 1
                ? Ok(valueId) : Problem("Failed to delete a value");
        }

        /// TODO! Unfinished and incorrect, rewrite it
        /// <summary>Updates an existing field</summary>
        [HttpPut("grid/{gridId}/field")]
        public async Task<IActionResult> UpdateField(int gridId, [FromQuery, Required] int fieldId, [FromBody, Required] DataGridFieldSignatureDto fieldDto)
        {
            var field = await _dbContext.Fields.FindAsync(fieldId);
            if (field is null)
            {
                return BadRequest($"Field {fieldId} does not exist");
            }

            var order = field.Order;
            _dbContext.Fields.Remove(field);
            if (await _dbContext.SaveChangesAsync() != 1)
            {
                return Problem("Failed to remove the field");   
            }

            var fieldSignature = fieldDto.ToDataGridFieldSignature();
            
            try
            {
                await _dbContext.AddFieldToGridAsync(gridId, fieldSignature);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(fieldDto);
        }

        /// <summary>Renames an existing field</summary>
        [HttpPut("field/{fieldId}/rename")]
        public async Task<IActionResult> RenameField(int fieldId, [FromQuery, Required] string newName)
        {
            var field = await _dbContext.Fields.FindAsync(fieldId);
            if (field is null)
            {
                return BadRequest($"Field {fieldId} does not exist");
            }

            field.Name = newName;
            return await _dbContext.SaveChangesAsync() == 1
                ? Ok(newName) : Problem("Failed to change the name of the field");
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

        /// <summary>Deletes a row by gridId and rowIndex</summary>
        [HttpDelete("row/{gridId}")]
        public async Task<IActionResult> DeleteRow(int gridId, [FromQuery, Required] int rowIndex)
        {
            var grid = await _dbContext.Grids.FindAsync(gridId);
            if (grid is null)
            {
                return BadRequest($"Grid {gridId} does not exist");
            }

            var values = _dbContext.GetGridRow(gridId, rowIndex).ToList();
            _dbContext.RemoveRange(values);

            return await _dbContext.SaveChangesAsync() == values.Count
                ? Ok(values.Select(value => value.Id)) : Problem("Failed to remove some values");
        }
    }

}