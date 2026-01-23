using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using dhara_pvd_decor_webapi_proj.Services;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FinYearController : Controller
    {
        private readonly IFinYearService _service;
        private readonly IDistributedCache _cache;

        public FinYearController(IFinYearService service, IDistributedCache cache)
        {
            _service = service;
            _cache = cache;
        }

        [HttpPost("insert_fin_year")]
        public async Task<IActionResult> Addfin_year([FromBody] AddFinYearRequest request)
        {
            try
            {
                int rows = await _service.AddFinYear(request);

                if (rows > 0)
                    return Ok(new { message = "fin year Added successfully." });

                return StatusCode(500, new { errorMessage = "Failed to add fin year." });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "fin year name already exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        [HttpDelete("DeleteFinYear/{id}")]
        public async Task<IActionResult> DeleteFinYear(long id)
        {
            try
            {
                int rows = await _service.DeleteFinYear(id);

                if (rows > 0)
                    return Ok(new { message = "fin year deleted successfully." });

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

        [HttpPost("UpdateFinYear")]
        public async Task<IActionResult> UpdateFinYear([FromBody] UpdateFinYearRequest request)
        {
            try
            {
                int rows = await _service.UpdateFinYear(request);

                if (rows == 0)
                    return NotFound($"FinYear with ID {request.Fin_year_id} not found");

                return Ok(new { message = "FinYear updated successfully." });
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

        [HttpGet("fin_year_list")]
        public async Task<IActionResult> Get_FinYear_list()
        {
            try
            {
                return Ok(await _service.GetFinYearList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpGet("fin_year/{id}")]
        public async Task<IActionResult> Get_FinYear_By_id(long id)
        {
            try
            {
                var data = await _service.GetFinYearById(id);

                if (data == null)
                    return NotFound($"Finyear with ID {id} not found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpGet("dropdown_finyear_list")]
        public async Task<IActionResult> Get_finyear_list(long userId = 0)
        {
            try
            {
                return Ok(await _service.GetDropFinYearList(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }
   



       public class AddFinYearRequest
        {
            public long Fin_year_id { get; set; } = 0;
            public string Fin_name { get; set; } = "";
            public string Short_fin_year { get; set; } = "";
            public DateTime Year_start { get; set; }
            public DateTime Year_end { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }



        public class UpdateFinYearRequest
        {
            public long Fin_year_id { get; set; } = 0;
            public string Fin_name { get; set; } = "";
            public string Short_fin_year { get; set; } = "";
            public DateTime Year_start { get; set; }
            public DateTime Year_end { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;

        }


        public class FinYearlist
        {
            public long Fin_year_id { get; set; } = 0;
            public string Fin_name { get; set; } = "";
            public string Short_fin_year { get; set; } = "";
            public string Year_start { get; set; } = "";
            public string Year_end { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";

        }


        public class Single_FinYear_list
        {
            public long Fin_year_id { get; set; } = 0;
            public string Fin_name { get; set; } = "";
            public string Short_fin_year { get; set; } = "";
            public DateTime? Year_start { get; set; }
            public DateTime? Year_end { get; set; }
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;

        }


        public class drop_FinYear_list
        {
            public long Fin_year_id { get; set; } = 0;
            public string Fin_name { get; set; } = "";

        }

    }
}
