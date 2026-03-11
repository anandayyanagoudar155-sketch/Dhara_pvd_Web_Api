using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authorization;
using dhara_pvd_decor_webapi_proj.Services;
using Org.BouncyCastle.Asn1.Ocsp;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InwardReturnController : Controller
    {
        private readonly IDistributedCache _cache;
        private readonly IInwardReturnService _service;

        public InwardReturnController( IDistributedCache cache, IInwardReturnService service)
        {
            _service = service;
            _cache = cache;

        }


        [HttpPost("insert_returninward")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Add_inwardreturn([FromBody] AddInwardreturnRequest request)
        {
            try
            {
                int rowsAffected = await _service.Add_inwardreturn(request);

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "inwardreturn Added successfully." });
                }
                else
                {
                    return StatusCode(500, new { errorMessage = "Failed to add inwardreturn." });
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "same inwardreturn  already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("DeleteInwardReturn/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteInwardReturn(long id)
        {
            try
            {
                int rowAffected = await _service.DeleteInwardReturn(id); ;

                if (rowAffected > 0)
                    return Ok(new { message = "inwardreturn deleted successfully." });
                else
                    return StatusCode(500, new { message = "No Record Deleted" });

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



        [HttpPost("UpdateInwardReturn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> UpdateInwardReturn([FromBody] UpdateInwardreturnRequest request)
        {
            int rows_affected;

            try
            {
                rows_affected = await _service.UpdateInwardReturn(request);

                if (rows_affected == 0)
                    return NotFound($"inwardreturn with ID {request.Inwardreturn_Id} not found");
                else
                    return Ok(new { message = "inwardreturn updated successfully" });
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }


        [HttpGet("inwardreturn_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<Inwardreturn_List>>> Get_inwardreturn_list()
        {
            try
            {
                return Ok(await _service.Get_inwardreturn_list());
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("InwardReturn/{id}")]
        public async Task<ActionResult<SingleInwardreturn>> Get_InwardReturn_by_id(long id)
        {
            try
            {
                var data = await _service.Get_InwardReturn_by_id(id);

                if (data == null)
                    return NotFound($"inward with ID {id} not found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("inward_for_return")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Drop_InwardDetail>>> GetInwardForReturn(long customerId, long compId, long finYearId)
        {
            try
            {
                if (customerId <= 0 || compId <= 0 || finYearId <= 0)
                    return BadRequest("Invalid customerId, compId, or finYearId.");

                var data = await _service.Get_inward_for_return(customerId, compId, finYearId);

                if (data == null || data.Count == 0)
                    return NotFound("No inward records found.");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("products_for_return")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Drop_ProductDetail>>> GetProductsForReturn(long inwardId)
        {
            try
            {
                if (inwardId <= 0)
                    return BadRequest("Invalid inwardId.");

                var data = await _service.Get_products_for_return(inwardId);

                if (data == null || data.Count == 0)
                    return NotFound($"No products found for Inward ID {inwardId}");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public class AddInwardreturnRequest
        {
            public long Inwardreturn_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0;
            public long Customer_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public decimal ReturnQuantity { get; set; } = 0;
            public DateTime Return_Date { get; set; }
            public string Remarks { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class UpdateInwardreturnRequest
        {
            public long Inwardreturn_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0;
            public long Customer_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public decimal ReturnQuantity { get; set; } = 0;
            public DateTime Return_Date { get; set; }
            public string Remarks { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class Inwardreturn_List
        {
            public long Inwardreturn_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0;
            public string Customer_Name { get; set; } = "";
            public string Product_Name { get; set; } = "";
            public decimal ReturnQuantity { get; set; } = 0;
            public string Return_Date { get; set; } = "";
            public string Remarks { get; set; } = "";
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }

        public class SingleInwardreturn
        {
            public long Inwardreturn_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0;
            public long Customer_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public decimal ReturnQuantity { get; set; } = 0;
            public DateTime? Return_Date { get; set; }
            public string Remarks { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class Drop_InwardDetail
        {
            public long Inward_Id { get; set; } = 0;
            public string Inward_Name { get; set; } = "";
        }

        public class Drop_ProductDetail
        {
            public long Inward_Details_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public string Product_Name { get; set; } = "";
        }
    }
}
