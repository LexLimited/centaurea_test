using CentaureaTest.Data;
using CentaureaTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentaureaTest.Controllers
{

    [ApiController]
    [Route("api/datagrid")]
    // [Authorize]
    public sealed class DataGridController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public DataGridController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public string Index()
        {
            return "index";
        }

        // [Authorize]
        [HttpGet("view")]
        public IActionResult GetGridById([FromQuery] int gridId)
        {
            return Ok(_dbContext.GetDataGrid(gridId));
        }
        
        // [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateDataGrid([FromBody] DataGrid grid)
        {
            await _dbContext.AddAsync(grid.ToGridsTable());
            var nWritten = await _dbContext.SaveChangesAsync();

            if (nWritten == 0)
            {
                return Problem("Failed to insert new grid");
            }

            return Ok();
        }
    }

}