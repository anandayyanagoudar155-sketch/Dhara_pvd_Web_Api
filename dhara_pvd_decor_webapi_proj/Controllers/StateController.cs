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
    public class StateController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        private readonly IStateService _stateService;

        public StateController(
            IConfiguration configuration,
            IDistributedCache cache,
            IStateService stateService)
        {
            _configuration = configuration;
            _cache = cache;
            _stateService = stateService;
        }

        [HttpPost("insert_state")]
        public async Task<IActionResult> AddState([FromBody] AddStateRequest request)
        {
            try
            {
                var result = await _stateService.AddState(request);

                return result
                    ? Ok(new { message = "State Added successfully." })
                    : StatusCode(500, new { errorMessage = "Failed to add State." });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") ||
                    ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "State name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        [HttpDelete("DeleteState/{id}")]
        public async Task<IActionResult> DeleteState(long id)
        {
            try
            {
                var result = await _stateService.DeleteState(id);

                return result
                    ? Ok(new { message = "State deleted successfully." })
                    : StatusCode(500, new { errorMessage = "No record deleted." });
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


        [HttpPost("UpdateState")]
        public async Task<IActionResult> UpdateState([FromBody] UpdateStateRequest request)
        {
            try
            {
                var result = await _stateService.UpdateState(request);

                if (!result)
                    return NotFound($"State with ID {request.State_id} not found");

                return Ok(new { message = "State updated successfully." });
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

 
        [HttpGet("state_list")]
        public async Task<IActionResult> Get_state_list()
        {
            var data = await _stateService.GetStateList();
            return Ok(data);
        }


        [HttpGet("state/{id}")]
        public async Task<IActionResult> Get_state_by_id(long id)
        {
            var data = await _stateService.GetStateById(id);

            if (data == null)
                return NotFound($"state with ID {id} not found");

            return Ok(data);
        }


        [HttpGet("dropdown_state_list")]
        public async Task<IActionResult> Get_drop_statelist(long country_id = 0)
        {
            var data = await _stateService.GetDropStateList(country_id);
            return Ok(data);
        }



        public class AddStateRequest
        {
            public long State_id { get; set; } = 0;
            public string State_name { get; set; } = "";
            public long Country_id { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;

        }


        public class UpdateStateRequest
        {
            public long State_id { get; set; } = 0;
            public string State_name { get; set; } = "";
            public long Country_id { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;

        }


        public class state_list
        {
            public long State_id { get; set; } = 0;
            public string State_name { get; set; } = "";
            public string Country_name { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public long? Modified_by { get; set; } = 0;
            public string? Modified_by_name { get; set; } = "";

        }


        public class Single_state_list
        {
            public long State_id { get; set; } = 0;
            public string State_name { get; set; } = "";
            public long Country_id { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;

        }


        public class drop_state_list
        {
            public long State_id { get; set; } = 0;
            public string State_name { get; set; } = "";

        }
    }
}
