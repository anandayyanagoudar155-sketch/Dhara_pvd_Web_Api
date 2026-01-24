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
    public class CustomerController : Controller
    {
        private readonly ICustomerService _service;
        private readonly IDistributedCache _cache;

        public CustomerController(ICustomerService service, IDistributedCache cache)
        {
            _service = service;
            _cache = cache;
        }


        [HttpPost("insert_customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCustomer([FromBody] AddCustomerRequest request)
        {
 
            try
            {
                int rows = await _service.AddCustomer(request);

                if (rows > 0)
                    return Ok(new { message = "Customer added successfully." });
                else
                    return StatusCode(500, new { errorMessage = "Failed to add customer." });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "Customer with same details already exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteCustomer/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCustomer(long id)
        {
            try
            {
                int rows = await _service.DeleteCustomer(id);

                if (rows > 0)
                    return Ok(new { message = "Customer deleted successfully." });
                else
                    return NotFound(new { errorMessage = $"Customer Id {id} not found." });

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




        [HttpPost("UpdatecCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerRequest request)
        {
            try
            {
                int rows = await _service.UpdateCustomer(request);

                if (rows == 0)
                    return NotFound(new { errorMessage = $"Customer Id {request.Customer_Id} not found." });
                else
                    return Ok(new { message = "Customer updated successfully." });

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


        [HttpGet("customer_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Customer_List>>> Get_Customer_List()
        {
        
            try
            {
                return Ok(await _service.Get_Customer_List());

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("customer/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Single_Customer_List>> GetCustomerById(long id)
        {
            try
            {

                var data = await _service.GetCustomerById(id);

                if (data == null)
                    return NotFound($"Customer with Id {id} not found.");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("dropdown_customer_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<Drop_Customer_List>>> Get_drop_customerlist()
        {
     
            try
            {
                return Ok(await _service.Get_drop_customerlist());

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpPost("insert_custdetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCustDetail([FromBody] Add_CustDetail_Request request)
        {
  
            try
            {
                int rows = await _service.Add_CustDetail_Request(request);

                if (rows > 0)
                    return Ok(new { message = "Customer Detail added successfully." });

                return StatusCode(500, new { errorMessage = "Failed to add Customer Detail." });

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "Duplicate entry exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }




        [HttpDelete("delete_custdetail/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCustDetail(long id)
        {
            try
            {
                int rows = await _service.DeleteCustDetail(id);


                if (rows > 0)
                        return Ok(new { message = "Customer Detail deleted successfully." });

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



        [HttpPost("update_custdetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCustDetail([FromBody] Update_CustDetail_Request request)
        {
    
            try
            {
                int rows = await _service.UpdateCustDetail(request);

                if (rows == 0)
                    return NotFound($"Customer Detail with ID {request.Cust_detail_id} not found");

                return Ok(new { message = "Customer Detail updated successfully." });
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



        [HttpGet("custdetail_list")]
        public async Task<ActionResult<IEnumerable<CustDetail_List>>> Get_CustDetail_list()
        {
  
            try
            {
                return Ok(await _service.Get_CustDetail_list());

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("custdetail/{id}")]
        public async Task<ActionResult<List<Single_CustDetail>>> Get_CustDetail_by_id(long id)
        {
    
            try
            {
                var data = await _service.Get_CustDetail_by_id(id);

                if (data.Count == 0)
                    return NotFound($"No customer details found for customer_id {id}");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("dropdown_custdetail_list")]
        public async Task<ActionResult<IEnumerable<Drop_CustDetail>>> Get_drop_custdetail_list()
        {
            try
            {
                return Ok(await _service.Get_drop_custdetail_list());

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        public class AddCustomerRequest
        {
            public string Customer_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public long City_Id { get; set; } = 0;
            public string Cust_Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; } = true;
            public string Customer_Notes { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }


        public class UpdateCustomerRequest
        {
            public long Customer_Id { get; set; } = 0;
            public string Customer_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public long City_Id { get; set; } = 0;
            public string Cust_Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; } = true;
            public string Customer_Notes { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }


        public class Customer_List
        {
            public long Customer_Id { get; set; } = 0;
            public string Customer_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public long City_Id { get; set; } = 0;
            public string City_Name { get; set; } = "";
            public string Cust_Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public string Dob { get; set; } = "";
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; }
            public string Customer_Notes { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";
        }


        public class Single_Customer_List
        {
            public long Customer_Id { get; set; } = 0;
            public string Customer_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public long City_Id { get; set; } = 0;
            public string Cust_Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; }
            public string Customer_Notes { get; set; } = "";
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
        }


        public class Drop_Customer_List
        {
            public long Customer_Id { get; set; } = 0;
            public string Customer_Name { get; set; } = "";
        }


        public class Add_CustDetail_Request
        {
            public long Cust_detail_id { get; set; } = 0;
            public long Customer_id { get; set; } = 0;
            public decimal Opening_balance { get; set; } = 0;
            public decimal Invoice_balance { get; set; } = 0;
            public decimal Outstanding_balance { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }

        public class Update_CustDetail_Request
        {
            public long Cust_detail_id { get; set; } = 0;
            public long Customer_id { get; set; } = 0;
            public decimal Opening_balance { get; set; } = 0;
            public decimal Invoice_balance { get; set; } = 0;
            public decimal Outstanding_balance { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }


        public class CustDetail_List
        {
            public long Cust_detail_id { get; set; } = 0;
            public long Customer_name { get; set; } = 0;
            public decimal Opening_balance { get; set; } = 0;
            public decimal Invoice_balance { get; set; } = 0;
            public decimal Outstanding_balance { get; set; } = 0;
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public long Fin_year_id { get; set; } = 0;
            public string Fin_year_name { get; set; } = "";
            public long Comp_id { get; set; } = 0;
            public string Comp_name { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";
        }



        public class Single_CustDetail
        {
            public long Cust_detail_id { get; set; } = 0;
            public long Customer_id { get; set; } = 0;
            public decimal Opening_balance { get; set; } = 0;
            public decimal Invoice_balance { get; set; } = 0;
            public decimal Outstanding_balance { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public string Fin_year_name { get; set; } = "";
            public long Comp_id { get; set; } = 0;
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }



        public class Drop_CustDetail
        {
            public long Cust_detail_id { get; set; } = 0;
        }

    }
}
