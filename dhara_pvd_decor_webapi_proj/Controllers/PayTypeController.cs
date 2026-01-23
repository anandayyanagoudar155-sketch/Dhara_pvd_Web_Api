using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authorization;
using dhara_pvd_decor_webapi_proj.Services;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PayTypeController : Controller
    {
        private readonly IPayTypeService _service;
        private readonly IDistributedCache _cache;

        public PayTypeController(IPayTypeService service, IDistributedCache cache)
        {
            _service = service;
            _cache = cache;
        }


        [HttpPost("insert_paytype")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPaytype([FromBody] AddPaytypeRequest request)
        {
            try
            {
                int rows = await _service.AddPaytype(request);

                if (rows > 0)
                {
                    return Ok(new { message = "Paytype added successfully." });
                }
                else
                {
                    return StatusCode(500, new { errorMessage = "Failed to add Paytype." });
                }
                    
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Paytype name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_paytype/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePaytype(long id)
        {
            try
            {
                int rows = await _service.DeletePaytype(id);

                if (rows > 0)
                    return Ok(new { message = "Paytype deleted successfully." });
                else
                    return NotFound(new { errorMessage = "No record deleted" });
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


        [HttpPost("update_paytype")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePaytype([FromBody] UpdatePaytypeRequest request)
        {
        
            try
            {
                int rows = await _service.UpdatePaytype(request);

                if (rows == 0)
                    return NotFound(new { message = $"Paytype with ID {request.Paytype_Id} not found." });
                else
                    return Ok(new { message = "Paytype updated successfully." });
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


        [HttpGet("paytype_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Paytype_list>> GetPaytypeList()
        {
            try
            {
                return Ok(await _service.GetPaytypeList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }



        [HttpGet("paytype/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Single_Paytype_list>> GetPaytypeById(long id)
        {
            try
            {
                var data = await _service.GetPaytypeById(id);

                if (data == null)
                    return NotFound(new { message = $"Paytype with Id {id} not found." });

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        [HttpGet("dropdown_paytype_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<drop_Paytype_list>> GetDropdownPaytypeList()
        {
            try
            {
                return Ok(await _service.GetDropdownPaytypeList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }




        public class AddPaytypeRequest
        {
            public long Paytype_Id { get; set; } = 0;
            public string Paytype_Name { get; set; } = "";
            public string Paytype_Desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }


        public class UpdatePaytypeRequest
        {
            public long Paytype_Id { get; set; } = 0;
            public string Paytype_Name { get; set; } = "";
            public string Paytype_Desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }

        public class Paytype_list
        {
            public long Paytype_Id { get; set; } = 0;
            public string Paytype_Name { get; set; } = "";
            public string Paytype_Desc { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";

        }


        public class Single_Paytype_list
        {
            public long Paytype_Id { get; set; } = 0;
            public string Paytype_Name { get; set; } = "";
            public string Paytype_Desc { get; set; } = "";
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;

        }


        public class drop_Paytype_list
        {
            public long Paytype_Id { get; set; } = 0;
            public string Paytype_Name { get; set; } = "";


        }
    }
}
