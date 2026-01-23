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
    public class TranTypeController : Controller
    {
        private readonly ITranTypeService _service;
        private readonly IDistributedCache _cache;

        public TranTypeController(ITranTypeService service, IDistributedCache cache)
        {
            _service = service;
            _cache = cache;
        }


        [HttpPost("insert_trans_type")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddTransType([FromBody] AddTrans_typeRequest request)
        {
            try
            {
                int rows = await _service.AddTransType(request);

                if (rows > 0)
                            return Ok(new { message = "Transaction Type added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add Transaction Type." });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Transaction Type already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteTransType/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTransType(long id)
        {
            try
            {
                int rows = await _service.DeleteTransType(id);

                        if (rows > 0)
                            return Ok(new { message = "Transaction Type deleted successfully." });
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





        [HttpPost("UpdateTransType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateTransType([FromBody] UpdateTrans_typeRequest request)
        {
            try
            {
                int rows = await _service.UpdateTransType(request);

                if (rows == 0)
                    return NotFound($"Transaction Type with ID {request.Trans_id} not found");
                else
                    return Ok(new { message = "Transaction Type updated successfully." });
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



        [HttpGet("trans_type_list")]
        public async Task<ActionResult<IEnumerable<trans_type_List>>> Get_trans_type_list()
        {
            try
            {
                return Ok(await _service.Get_trans_type_list());
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("trans_type/{id}")]
        public async Task<ActionResult<Singletrans_type>> Get_trans_type_by_id(long id)
        {
            try
            {
                var data = await _service.Get_trans_type_by_id(id);

                if (data == null)
                    return NotFound($"Transaction Type with ID {id} not found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("dropdown_trans_type_list")]
        public async Task<ActionResult<IEnumerable<Drop_trans_type_List>>> Get_drop_trans_type_list()
        {
            try
            {
                return Ok(await _service.Get_drop_trans_type_list());
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        public class AddTrans_typeRequest
        {
            public long Trans_id { get; set; } = 0;
            public string Transtype_name { get; set; } = "";
            public string Transtype_desc { get; set; } = "";
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }

        public class UpdateTrans_typeRequest
        {
            public long Trans_id { get; set; } = 0;
            public string Transtype_name { get; set; } = "";
            public string Transtype_desc { get; set; } = "";
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }

        public class trans_type_List
        {
            public long Trans_id { get; set; } = 0;
            public string Transtype_name { get; set; } = "";
            public string Transtype_desc { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";
        }

        public class Singletrans_type
        {
            public long Trans_id { get; set; } = 0;
            public string Transtype_name { get; set; } = "";
            public string Transtype_desc { get; set; } = "";
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
        }


        public class Drop_trans_type_List
        {
            public long Trans_id { get; set; } = 0;
            public string Transtype_name { get; set; } = "";
        }



    }
}
