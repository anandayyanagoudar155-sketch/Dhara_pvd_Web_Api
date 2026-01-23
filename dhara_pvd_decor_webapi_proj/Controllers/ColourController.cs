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
    public class ColourController : Controller
    {
        private readonly IColourService _service;
        private readonly IDistributedCache _cache;

        public ColourController(IColourService service, IDistributedCache cache)
        {
            _service = service;
            _cache = cache;
        }

        [HttpPost("insert_colour")]
        public async Task<IActionResult> AddColour([FromBody] AddColourRequest request)
        {
            try
            {
                int rows = await _service.AddColour(request);

                if (rows > 0)
                    return Ok(new { message = "Colour added successfully." });

                return StatusCode(500, new { errorMessage = "Failed to add colour." });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "Colour name already exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        [HttpDelete("Deletecolour/{id}")]
        public async Task<IActionResult> DeleteColour(long id)
        {
            try
            {
                int rows = await _service.DeleteColour(id);

                if (rows > 0)
                    return Ok(new { message = "Colour deleted successfully." });

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

        [HttpPost("Updatecolour")]
        public async Task<IActionResult> UpdateColour([FromBody] UpdateColourRequest request)
        {
            try
            {
                int rows = await _service.UpdateColour(request);

                if (rows == 0)
                    return NotFound($"Colour with ID {request.ColourId} not found");

                return Ok(new { message = "Colour updated successfully." });
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

        [HttpGet("colour_list")]
        public async Task<IActionResult> GetColourList()
        {
            try
            {
                return Ok(await _service.GetColourList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpGet("colour/{id}")]
        public async Task<IActionResult> GetColourById(long id)
        {
            try
            {
                var data = await _service.GetColourById(id);

                if (data == null)
                    return NotFound($"Colour with ID {id} not found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpGet("dropdown_colour_list")]
        public async Task<IActionResult> GetDropColourList()
        {
            try
            {
                return Ok(await _service.GetDropColourList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        public class AddColourRequest
        {
            public long ColourId { get; set; } = 0;
            public string ColourName { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }


        public class UpdateColourRequest
        {
            public long ColourId { get; set; } = 0;
            public string ColourName { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }

        public class Colour_list
        {
            public long ColourId { get; set; } = 0;
            public string ColourName { get; set; } = "";
            public bool? IsActive { get; set; }
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";

        }


        public class Single_Colour_list
        {
            public long ColourId { get; set; } = 0;
            public string ColourName { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;

        }


        public class drop_Colour_list
        {
            public long ColourId { get; set; }
            public string ColourName { get; set; } = "";

        }


    }
}
