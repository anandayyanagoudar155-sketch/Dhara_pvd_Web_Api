using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using dhara_pvd_decor_webapi_proj.Services;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UnitController : Controller
    {
        private readonly IUnitService _service;
        private readonly IDistributedCache _cache;

        public UnitController(IUnitService service, IDistributedCache cache)
        {
            _service = service;
            _cache = cache;
        }

        [HttpPost("insert_unit")]
        public async Task<IActionResult> AddUnit([FromBody] AddUnitRequest request)
        {
            try
            {
                var rows = await _service.AddUnit(request);

                if (rows > 0)
                    return Ok(new { message = "Unit added successfully." });

                return StatusCode(500, new { errorMessage = "Failed to add unit." });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "Unit name already exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        [HttpDelete("Deleteunit/{id}")]
        public async Task<IActionResult> DeleteUnit(long id)
        {
            try
            {
                var rows = await _service.DeleteUnit(id);

                if (rows > 0)
                    return Ok(new { message = "Unit deleted successfully." });

                return StatusCode(500, new { errorMessage = "No record deleted." });
            }
            catch (SqlException ex)
            {

                return BadRequest(new { errorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpPost("Updateunit")]
        public async Task<IActionResult> UpdateUnit([FromBody] UpdateUnitRequest request)
        {
            try
            {
                var rows = await _service.UpdateUnit(request);

                if (rows == 0)
                    return NotFound($"Unit with ID {request.UnitId} not found");

                return Ok(new { message = "Unit updated successfully." });
            }
            catch (SqlException ex)
            {

                return BadRequest(new { errorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpGet("unit_list")]
        public async Task<IActionResult> GetUnitList()
        {
            try
            {
                return Ok(await _service.GetUnitList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpGet("unit/{id}")]
        public async Task<IActionResult> GetUnitById(long id)
        {
            try
            {
                var unit = await _service.GetUnitById(id);

                if (unit == null)
                    return NotFound($"Unit with ID {id} not found");

                return Ok(unit);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpGet("dropdown_unit_list")]
        public async Task<IActionResult> GetDropUnitList()
        {
            try
            {
                return Ok(await _service.GetDropUnitList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        public class AddUnitRequest
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";
            public string UnitDesc { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }


        public class UpdateUnitRequest
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";
            public string UnitDesc { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }

        public class Unit_list
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";
            public string UnitDesc { get; set; } = "";
            public bool? IsActive { get; set; }
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";

        }


        public class Single_Unit_list
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";
            public string UnitDesc { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;

        }


        public class drop_Unit_list
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";

        }

    }
}
