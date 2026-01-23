using Dapper;
using Microsoft.AspNetCore.Mvc;
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
    public class MonthController : Controller
    {
        private readonly IMonthService _service;
        private readonly IDistributedCache _cache;

        public MonthController(IMonthService service, IDistributedCache cache)
        {
            _service = service;
            _cache = cache;
        }

        [HttpPost("insert_month")]
        public async Task<IActionResult> AddMonth([FromBody] AddMonthRequest request)
        {
            try
            {
                int rows = await _service.AddMonth(request);

                if (rows > 0)
                    return Ok(new { message = "Month added successfully." });

                return StatusCode(500, new { errorMessage = "Failed to add month." });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "Month name already exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        [HttpDelete("Deletemonth/{id}")]
        public async Task<IActionResult> DeleteMonth(long id)
        {
            try
            {
                int rows = await _service.DeleteMonth(id);

                if (rows > 0)
                    return Ok(new { message = "Month deleted successfully." });

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

        [HttpPost("Updatemonth")]
        public async Task<IActionResult> UpdateMonth([FromBody] UpdateMonthRequest request)
        {
            try
            {
                int rows = await _service.UpdateMonth(request);

                if (rows == 0)
                    return NotFound($"Month with ID {request.Month_id} not found");

                return Ok(new { message = "Month updated successfully." });
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

        [HttpGet("month_list")]
        public async Task<IActionResult> GetMonthList()
        {
            try
            {
                return Ok(await _service.GetMonthList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpGet("month/{id}")]
        public async Task<IActionResult> GetMonthById(long id)
        {
            try
            {
                var month = await _service.GetMonthById(id);

                if (month == null)
                    return NotFound($"Month with ID {id} not found");

                return Ok(month);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpGet("dropdown_month_list")]
        public async Task<IActionResult> GetDropMonthList()
        {
            try
            {
                return Ok(await _service.GetDropMonthList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        public class AddMonthRequest
        {
            public long Month_id { get; set; } = 0;
            public string Month_name { get; set; } = "";
            public DateTime Start_date { get; set; }
            public DateTime End_date { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }


        public class UpdateMonthRequest
        {
            public long Month_id { get; set; } = 0;
            public string Month_name { get; set; } = "";
            public DateTime? Start_date { get; set; }
            public DateTime? End_date { get; set; }
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;

        }

        public class month_list
        {
            public long Month_id { get; set; } = 0;
            public string Month_name { get; set; } = "";
            public string Start_date { get; set; } = "";
            public string End_date { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";

        }


        public class Single_month_list
        {
            public long Month_id { get; set; } = 0;
            public string Month_name { get; set; } = "";
            public DateTime? Start_date { get; set; }
            public DateTime? End_date { get; set; }
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;

        }



        public class drop_month_list
        {
            public long Month_id { get; set; } = 0;
            public string Month_name { get; set; } = "";

        }

    }
}
