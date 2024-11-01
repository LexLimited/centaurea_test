using System.ComponentModel.DataAnnotations;
using CentaureaTest.Data;
using CentaureaTest.Models;
using CentaureaTest.Models.Auth;
using CentaureaTest.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CentaureaTest.Controllers
{

    [ApiController]
    [Route("api/datagrid")]
    [Authorize(Roles = "Admin, Superuser")]
    public sealed class DataGridController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DataGridController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public DataGridController(
            ApplicationDbContext dbContext,
            ILogger<DataGridController> logger,
            UserManager<ApplicationUser> userManager
        )
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>Returns a list of existing grid</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin") || User.IsInRole("Superuser"))
            {
                return Ok(_dbContext.Grids);
            }

            if (User.IsInRole("User"))
            {
                var username = User.Identity?.Name;
                if (username is null)
                {
                    return Problem("User exists, but the name is null");
                }

                var userGridIds = await _dbContext.GridPermssions
                    .Where(permission => permission.UserName == username)
                    .Select(permission => permission.GridId)
                    .ToListAsync();

                var grids = _dbContext.Grids.Where(grid => userGridIds.Contains(grid.Id));
                return Ok(grids);
            }

            return Redirect("/app/auth");
        }

        /// <summary>Returns an existing grid</summary>
        // [Authorize]
        [HttpGet("grid")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGridById([FromQuery] int gridId)
        {
            if (!IsUserAllowedGrid(gridId))
            {
                return Unauthorized($"You're not allowed the access to grid {gridId}");
            }

            var grid = await _dbContext.GetDataGridAsync(gridId);
            return grid is null ? new NotFoundResult() : Ok(grid);
        }
        
        /// <summary>Creates a new grid</summary>
        // [Authorize(Roles = "Admin, Superuser")]
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
        [AllowAnonymous]
        public async Task<IActionResult> AddField(int gridId, [FromBody] DataGridFieldSignatureDto fieldSignatureDto)
        {
            if (!IsUserAllowedGrid(gridId))
            {
                return Unauthorized($"You're not allowed the access to grid ${gridId}");
            }

            var fieldSignature = fieldSignatureDto.ToDataGridFieldSignature();
            if (fieldSignature is null)
            {
                return BadRequest("Invalid field signature");
            }

            try
            {
                await _dbContext.AddFieldToGridWithDependenciesAsync(gridId, fieldSignatureDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            // TODO! Move this logic elsewhere
            if (fieldSignatureDto.Type == DataGridValueType.SingleSelect)
            {
                var options = fieldSignatureDto.Options;
                if (options is null)
                {
                    return BadRequest("Trying to create single select field with null options");
                }

                await _dbContext.CreateSingleSelectOptionsAsync(gridId, options);
            }

            if (fieldSignatureDto.Type == DataGridValueType.MultiSelect)
            {
                var options = fieldSignatureDto.Options;
                if (options is null)
                {
                    return BadRequest("Trying to create multi select field with null options");
                }

                await _dbContext.CreateMultiSelectOptionsAsync(gridId, options);
            }

            return Ok(fieldSignature);
        }

        /// <summary>Sets a single value (cell) within the field</summary>
        [HttpPut("value")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateValue([FromQuery] int fieldId, [FromBody] DataGridValueDto valueDto)
        {
            // TODO! Check if the value belongs to an allowed grid

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
        [AllowAnonymous]
        public async Task<IActionResult> InsertValue([FromQuery] int fieldId, [FromBody] DataGridValueDto valueDto)
        {
            // TODO! Check if the value belongs to an allowed grid

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
        [AllowAnonymous]
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

        /// TODO! Unfinished, rewrite it
        /// <summary>Updates an existing field</summary>
        [HttpPut("grid/{gridId}/field")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateField(int gridId, [FromQuery, Required] int fieldId, [FromBody, Required] DataGridFieldSignatureDto fieldDto)
        {
            if (!IsUserAllowedGrid(gridId))
            {
                return Unauthorized($"You're not allowed the access to grid ${gridId}");
            }

            var field = await _dbContext.Fields.FindAsync(fieldId);
            if (field is null)
            {
                return BadRequest($"Field {fieldId} does not exist");
            }

            _dbContext.Fields.Remove(field);
            if (await _dbContext.SaveChangesAsync() != 1)
            {
                return Problem("Failed to remove a field");   
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
        [AllowAnonymous]
        public async Task<IActionResult> RenameField(int fieldId, [FromQuery, Required] string newName)
        {
            // TODO! Check if the field belongs to an allowed grid

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
        [AllowAnonymous]
        public async Task<IActionResult> DeleteField(int fieldId)
        {
            // TODO! Check if the field belongs to an allowed grid

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
        [AllowAnonymous]
        public async Task<IActionResult> DeleteFields([FromBody] List<int> fieldIds)
        {
            // TODO! Check if the field belongs to an allowed grid

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
        [AllowAnonymous]
        public async Task<IActionResult> AddRow(int gridId, [FromBody] List<DataGridValueDto> valueDtos)
        {
            if (!IsUserAllowedGrid(gridId))
            {
                return Unauthorized($"You're not allowed the access to grid ${gridId}");
            }

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
        [AllowAnonymous]
        public async Task<IActionResult> DeleteRow(int gridId, [FromQuery, Required] int rowIndex)
        {
            if (!IsUserAllowedGrid(gridId))
            {
                return Unauthorized($"You're not allowed the access to grid ${gridId}");
            }

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

        /// <summary>Gets permissions for a grid</summary>
        [HttpGet("grid/{gridId}/permissions")]
        public IActionResult GetGridPermission(int gridId)
        {
            var permissions = _dbContext.GridPermssions
                .Where(permission => permission.GridId == gridId)
                .Select(permission => new GridPermission
                {
                    GridId = gridId,
                    UserName = permission.UserName,
                });

            return Ok(permissions);
        }

        [HttpGet("grid/{gridId}/rows")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGridRows(int gridId)
        {
            // TODO! Check that the grid is referenced by a field that belong
            // to a grid that the user is allowed to access

            if (await _dbContext.Grids.FindAsync(gridId) is null)
            {
                return BadRequest($"Grid {gridId} does not exist");
            }

            return Ok(await _dbContext.GetGridRowDict(gridId));
        }

        /// <summary>Ads permissions for a grid by username</summary>
        [HttpPut("grid/{gridId}/permissions")]
        public async Task<IActionResult> AddGridPermission(int gridId, [FromBody, Required] List<string> allowedUsers)
        {
            if (await _dbContext.Grids.FindAsync(gridId) is null)
            {
                return BadRequest($"Grid {gridId} does not exist");
            }

            // Filter already existing permissions
            var newAllowedUsers = allowedUsers
                .Where(
                    userName => !_dbContext.GridPermssions.Where(permission => permission.UserName == userName).Any()
                )
                .ToList();

            var permissions = newAllowedUsers.Select(username => new GridPermission
            {
                GridId = gridId,
                UserName = username,
            });

            foreach (var permission in permissions)
            {
                if (await _userManager.FindByNameAsync(permission.UserName) is null)
                {
                    return BadRequest($"User {permission.UserName} does not exist");
                }
            }

            await _dbContext.AddRangeAsync(permissions);

            return await _dbContext.SaveChangesAsync() == newAllowedUsers.Count
                ? Ok(newAllowedUsers) : Problem("Failed to set some users");
        }

        // TODO! Wrap this in a transaction
        /// <summary>Resets permissions for a grid by username</summary>
        [HttpPost("grid/{gridId}/permissions")]
        public async Task<IActionResult> SetGridPermission(int gridId, [FromBody, Required] List<string> allowedUsers)
        {
            if (await _dbContext.Grids.FindAsync(gridId) is null)
            {
                return BadRequest($"Grid {gridId} does not exist");
            }

            var allGridPermissions = await _dbContext.GridPermssions
                .Where(permission => permission.GridId == gridId)
                .ToListAsync();

            _dbContext.RemoveRange(allGridPermissions);
            if (await _dbContext.SaveChangesAsync() != allGridPermissions.Count)
            {
                return Problem("Failed to remove some grid permissions");
            }

            var permissions = allowedUsers.Select(username => new GridPermission
            {
                GridId = gridId,
                UserName = username,
            });

            foreach (var permission in permissions)
            {
                if (await _userManager.FindByNameAsync(permission.UserName) is null)
                {
                    return BadRequest($"User {permission.UserName} does not exist");
                }
            }

            await _dbContext.AddRangeAsync(permissions);

            return await _dbContext.SaveChangesAsync() == allowedUsers.Count
                ? Ok(allowedUsers) : Problem("Failed to set some users");
        }

        [HttpGet("field/{fieldId}/options/single_select")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFieldSingleSelectOptions([FromRoute, Required] int fieldId)
        {
            // TODO! Check that the field belongs to a allowed table

            var field = await _dbContext.Fields.FindAsync(fieldId);
            if (field is null)
            {
                return BadRequest($"Field {fieldId} does not exist");
            }

            if (field.Type != Models.DataGridValueType.SingleSelect)
            {
                return BadRequest($"Single select options queried for field of type {field.Type}");
            }

            return Ok(_dbContext.SingleSelectOptions.Where(option => option.FieldId == fieldId));
        }

        [HttpGet("field/{fieldId}/options/multi_select")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFieldMultiSelectOptions([FromRoute, Required] int fieldId)
        {
            // TODO! Check that the field belongs to an allowed table

            var field = await _dbContext.Fields.FindAsync(fieldId);
            if (field is null)
            {
                return BadRequest($"Field {fieldId} does not exist");
            }

            if (field.Type != Models.DataGridValueType.MultiSelect)
            {
                return BadRequest($"Multi select options queried for field of type {field.Type}");
            }
            return Ok(_dbContext.MultiSelectOptions.Where(option => option.FieldId == fieldId));
        }

        /// <summary>Gets the field ids for a grid</summary>
        [HttpGet("grid/{gridId}/field_signatures")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGridFieldSignatures([FromRoute, Required] int gridId)
        {
            // TODO! Check that the grid is referenced by a field that belong
            // to a grid that the user is allowed to access

            if (await _dbContext.Grids.FindAsync(gridId) is null)
            {
                return BadRequest($"Grid {gridId} does not exist");
            }

            var signatures = _dbContext.Fields
                .Where(field => field.GridId == gridId)
                .Select(field => field.ToDataGridFieldSignature());

            return Ok(signatures);
        }

        private bool IsUserAllowedGrid(int gridId)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Superuser"))
            {
                return true;
            }

            if (User.IsInRole("User"))
            {
                var username = User.Identity?.Name;
                if (username is null)
                {
                    return false;
                }

                return _dbContext.GridPermssions
                    .Where(permission => permission.GridId == gridId && permission.UserName == username)
                    .Any();
            }

            return false;
        }
    }

}