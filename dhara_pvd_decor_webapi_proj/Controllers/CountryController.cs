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
    public class CountryController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        private readonly ICountryService _countryService;

        public CountryController(
            IConfiguration configuration,
            IDistributedCache cache,
            ICountryService countryService)
        {
            _configuration = configuration;
            _cache = cache;
            _countryService = countryService;
        }


        [HttpPost("Addcountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Addcountry([FromBody] AddCountryRequest request)
        {
            try
            {
                var result = await _countryService.AddCountry(request);

                if (result)
                    return Ok(new { message = "Country Added successfully." });
                else
                    return StatusCode(500, new { errorMessage = "Failed to add Country." });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") ||
                    ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Country name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteCountry/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCountry(long id)
        {
            try
            {
                var result = await _countryService.DeleteCountry(id);

                if (result)
                    return Ok(new { message = "Country deleted successfully." });
                else
                    return StatusCode(500, new { errorMessage = "No record deleted." });
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



        [HttpPost("UpdateCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCountry([FromBody] UpdateCountryRequest request)
        {
            try
            {
                var result = await _countryService.UpdateCountry(request);

                if (!result)
                    return NotFound($"Country with ID {request.Country_id} not found");

                return Ok(new { message = "Country updated successfully." });
            }

            catch (SqlException ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }


        [HttpGet("country_list")]
        public async Task<IActionResult> Get_country_list()
        {
            try
            {
                var data = await _countryService.GetCountryList();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        [HttpGet("country/{id}")]
        public async Task<IActionResult> Get_country_by_id(long id)
        {
            try
            {
                var data = await _countryService.GetCountryById(id);

                if (data == null)
                    return NotFound($"Country with ID {id} not found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }



        [HttpGet("dropdown_country_list")]
        public async Task<IActionResult> Get_drop_countrylist()
        {
            try
            {
                var data = await _countryService.GetDropCountryList();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        public class AddCountryRequest
        {

            public long Country_id { get; set; } = 0;
            public string Country_name { get; set; } = "";
            public DateTime Created_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;

        }

        public class UpdateCountryRequest
        {
            public long Country_id { get; set; } = 0;
            public string Country_name { get; set; } = "";
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;

        }

        public class country_list
        {
            public long Country_id { get; set; } = 0;
            public string Country_name { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string? Updated_date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public long? Modified_by { get; set; } = 0;
            public string? Modified_by_name { get; set; } = "";

        }


        public class Single_country_list
        {
            public long Country_id { get; set; } = 0;
            public string Country_name { get; set; } = "";
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;

        }



        public class drop_country_list
        {
            public long Country_id { get; set; } = 0;
            public string Country_name { get; set; } = "";

        }

    }
}
