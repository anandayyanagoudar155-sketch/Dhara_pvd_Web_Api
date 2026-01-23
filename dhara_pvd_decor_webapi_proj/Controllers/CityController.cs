using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using dhara_pvd_decor_webapi_proj.Services.Interfaces;
using dhara_pvd_decor_webapi_proj.Services.Implementations;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        private readonly ICityService _cityService;

        public CityController(IConfiguration configuration, 
            IDistributedCache cache ,
            ICityService cityService)
        {

            _configuration = configuration;
            _cache = cache;
            _cityService = cityService;

        }

        [HttpPost("insert_city")]
        public async Task<IActionResult> AddCity([FromBody] AddCityRequest request)
        {
            try
            {
                var result = await _cityService.AddCity(request);

                return result
                    ? Ok(new { message = "City Added successfully." })
                    : StatusCode(500, new { errorMessage = "Failed to add City." });
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "City name already exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        [HttpDelete("DeleteCity/{id}")]
        public async Task<IActionResult> DeleteCity(long id)
        {
            try
            {
                var result = await _cityService.DeleteCity(id);

                return result
                    ? Ok(new { message = "City deleted successfully." })
                    : NotFound(new { errorMessage = "City not found." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpPost("UpdateCity")]
        public async Task<IActionResult> UpdateCity([FromBody] UpdatecityRequest request)
        {
            try
            {
                var result = await _cityService.UpdateCity(request);

                return result
                    ? Ok(new { message = "City updated successfully." })
                    : NotFound(new { errorMessage = $"City with ID {request.City_id} not found." });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        [HttpGet("city_list")]
        public async Task<IActionResult> GetCityList()
        {
            try
            {
                return Ok(await _cityService.GetCityList());
            }
            catch (SqlException ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        [HttpGet("city/{id}")]
        public async Task<IActionResult> GetCityById(long id)
        {
            try
            {
                var city = await _cityService.GetCityById(id);

                return city == null
                    ? NotFound(new { errorMessage = $"City with ID {id} not found." })
                    : Ok(city);
            }
            catch (SqlException ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        [HttpGet("dropdown_city_list")]
        public async Task<IActionResult> GetDropdownCityList(long id = 0)
        {
            try
            {
                return Ok(await _cityService.GetDropdownCityList(id));
            }
            catch (SqlException ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        public class AddCityRequest
        {

            public long City_id { get; set; } = 0;
            public string City_name { get; set; } = "";
            public long State_id { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;

        }


        public class UpdatecityRequest
        {
            public long City_id { get; set; } = 0;
            public string City_name { get; set; } = "";
            public long State_id { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }


        public class city_list
        {
            public long City_id { get; set; } = 0;
            public string City_name { get; set; } = "";
            public string State_name { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public long? Modified_by { get; set; } = 0;
            public string? Modified_by_name { get; set; } = "";

        }

        public class Single_city_list
        {
            public long City_id { get; set; } = 0;
            public string City_name { get; set; } = "";
            public long State_id { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;

        }

        public class drop_city_list
        {
            public long City_id { get; set; } = 0;
            public string City_name { get; set; } = "";

        }

    }
}
