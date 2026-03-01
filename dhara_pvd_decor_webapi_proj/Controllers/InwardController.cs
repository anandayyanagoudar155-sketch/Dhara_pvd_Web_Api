using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using dhara_pvd_decor_webapi_proj.Services;
using Microsoft.Extensions.Configuration;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InwardController : Controller
    {
        private readonly IInwardService _service;
        private readonly IDistributedCache _cache;

        public InwardController(IInwardService service, IDistributedCache cache)
        {
            _service = service;
            _cache = cache;
        }


        [HttpPost("insert_inward")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddInward([FromBody] AddInwardRequest request)
        {
            try
            {
                int rowsAffected = await _service.AddInward(request);

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "inward Added successfully." });
                }
                else
                {
                    return StatusCode(500, new { errorMessage = "Failed to inward State." });
                }
                  
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "inward name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("delete_inward/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteInward(long id)
        {
            try
            {
                int rowsAffected = await _service.DeleteInward(id);

                if (rowsAffected > 0)
                    return Ok(new { message = "inward deleted successfully." });
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



        [HttpPost("update_inward")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> Updateinward([FromBody] UpdateInwardRequest request)
        {

            try
            {
                int rows_affected = await _service.Updateinward(request);

                if (rows_affected == 0)
                    return NotFound(new { errorMessage = $"Inward with ID {request.Inward_Id} not found" });
                else
                    return Ok(new { message = "Inward updated successfully" });
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }


        [HttpGet("get_inward_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Inward_List>>> GetInwardList()
        {
            try
            {
                return Ok(await _service.GetInwardList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("inward/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SingleInwardList>> GetInwardById(long id)
        {
            try
            {
                var data = await _service.GetInwardById(id);

                if (data == null)
                    return NotFound($"inward with ID {id} not found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("dropdown_inward_list")]
        public async Task<ActionResult<IEnumerable<Drop_Inward_List>>> Get_drop_inwardlist()
        {
            try
            {
                return Ok(await _service.Get_drop_inwardlist());
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("insert_inward_details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddInwardDetails([FromBody] AddInwardDetailsRequest request)
        {
            try
            {
                int rowsAffected = await _service.AddInwardDetails(request);

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Inward Details Added Successfully." });
                }
                else
                {
                    return StatusCode(500, new { errorMessage = "Failed to Insert Inward Details." });
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Duplicate inward detail entry already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_inward_details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteInwardDetails(long id)
        {

            try
            {
                int rowsAffected = await _service.DeleteInwardDetails(id);

                if (rowsAffected > 0)
                    return Ok(new { message = "Inward Details deleted successfully." });
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


        [HttpPost("update_inward_details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateInwardDetails([FromBody] UpdateInwardDetailsRequest request)
        {

            try
            {
                int rows_affected = await _service.UpdateInwardDetails(request);

                if (rows_affected == 0)
                    return NotFound(new { message = $"Inward Details with ID {request.Inward_Details_Id} not found" });
                else
                    return Ok(new { message = "Inward updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("get_inward_details_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Inward_Details_List>>> GetInwardDetailsList()
        {
            try
            {
                return Ok(await _service.GetInwardDetailsList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("inward_details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SingleInwardDetailsList>> GetInwardDetailsByInwardId(long id)
        {
            try
            {
                var data = await _service.GetInwardDetailsByInwardId(id);

                if (data == null)
                    return NotFound($"Inward Details with Inward ID {id} not found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("inward_quantity_summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InwardQuantitySummary>> GetInwardQuantitySummary(long inwardId, long productId)
        {
            try
            {
                if (inwardId <= 0 || productId <= 0)
                    return BadRequest("Invalid inwardId or productId.");

                var data = await _service.GetInwardQuantitySummary(inwardId, productId);

                if (data == null)
                    return NotFound($"No quantity summary found for Inward ID {inwardId} and Product ID {productId}");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public class AddInwardRequest
        {
            public long Inward_Id { get; set; } = 0;
            public string Inward_name { get; set; } = "";
            public long Customer_Id { get; set; } = 0;
            public DateTime Inward_Date { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }

        public class UpdateInwardRequest
        {
            public long Inward_Id { get; set; } = 0;
            public string Inward_name { get; set; } = "";
            public long Customer_Id { get; set; } = 0;
            public DateTime Inward_Date { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }

        public class Inward_List
        {
            public long Inward_Id { get; set; } = 0;
            public string Inward_name { get; set; } = "";
            public long Customer_Id { get; set; } = 0;
            public string Customer_Name { get; set; } = "";
            public string Inward_Date { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";
        }

        public class SingleInwardList
        {
            public long Inward_Id { get; set; } = 0;
            public string Inward_name { get; set; } = "";
            public long Customer_Id { get; set; } = 0;
            public DateTime? Inward_Date { get; set; }
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
        }

        public class Drop_Inward_List
        {
            public long Inward_Id { get; set; } = 0;
            public string Inward_name { get; set; } = "";
        }

        public class AddInwardDetailsRequest
        {
            public long Inward_Details_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0; 
            public long Product_Id { get; set; } = 0;
            public decimal TotalQuantity { get; set; } = 0;
            public decimal Balance_Quantity { get; set; } = 0;
            public bool Inward_Status { get; set; } = true;
            public string Remarks { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long Created_By { get; set; } = 0;
            public long Modified_By { get; set; } = 0;
        }

        public class UpdateInwardDetailsRequest
        {
            public long Inward_Details_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public decimal TotalQuantity { get; set; } = 0;
            public decimal Balance_Quantity { get; set; } = 0;
            public bool Inward_Status { get; set; } = true;
            public string Remarks { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long Created_By { get; set; } = 0;
            public long Modified_By { get; set; } = 0;
        }


        public class Inward_Details_List
        {
            public long Inward_Details_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0;
            public string Inward_name { get; set; } = "";
            public long Product_Id { get; set; } = 0;
            public string Product_Name { get; set; } = "";
            public decimal TotalQuantity { get; set; } = 0;
            public decimal Balance_Quantity { get; set; } = 0;
            public bool Inward_Status { get; set; } = true;
            public string Remarks { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public string Fin_Name { get; set; } = "";
            public long Comp_Id { get; set; } = 0;
            public string Comp_Name { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public long Created_By { get; set; } = 0;
            public long Modified_By { get; set; } = 0;
            public string Created_By_Name { get; set; } = "";
            public string Modified_By_Name { get; set; } = "";
        }


        public class SingleInwardDetailsList
        {
            public long Inward_Details_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public string Product_Name { get; set; } = "";
            public decimal TotalQuantity { get; set; } = 0;
            public decimal Balance_Quantity { get; set; } = 0;
            public bool Inward_Status { get; set; } = true;
            public string Remarks { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public string Fin_Name { get; set; } = "";
            public long Comp_Id { get; set; } = 0;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long Created_By { get; set; } = 0;
            public long Modified_By { get; set; } = 0;
        }

        public class InwardQuantitySummary
        {
            public long Inward_Id { get; set; }
            public long Product_Id { get; set; }
            public decimal Total_Sold_Quantity { get; set; }
            public decimal Total_Return_Quantity { get; set; }
        }

    }
}
