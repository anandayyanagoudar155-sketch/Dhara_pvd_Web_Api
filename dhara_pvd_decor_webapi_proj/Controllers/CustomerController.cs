using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly IConfiguration _configuration;

        public CustomerController(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        [HttpPost("insert_customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCustomer([FromBody] AddCustomerRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_customer_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@customer_id", 0);
                        command.Parameters.AddWithValue("@customer_name", request.Customer_Name);
                        command.Parameters.AddWithValue("@prefix", request.Prefix);
                        command.Parameters.AddWithValue("@gender", request.Gender);
                        command.Parameters.AddWithValue("@phonenumber", request.Phonenumber);
                        command.Parameters.AddWithValue("@city_id", request.City_Id);
                        command.Parameters.AddWithValue("@cust_address", request.Cust_Address);
                        command.Parameters.AddWithValue("@email_id", request.Email_Id);
                        command.Parameters.AddWithValue("@dob", request.Dob);
                        command.Parameters.AddWithValue("@aadhaar_number", request.Aadhaar_Number);
                        command.Parameters.AddWithValue("@license_number", request.License_Number);
                        command.Parameters.AddWithValue("@pan_number", request.Pan_Number);
                        command.Parameters.AddWithValue("@gst_number", request.Gst_Number);
                        command.Parameters.AddWithValue("@is_active", request.Is_Active);
                        command.Parameters.AddWithValue("@customer_notes", request.Customer_Notes);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rows = await command.ExecuteNonQueryAsync();

                        if (rows > 0)
                            return Ok(new { message = "Customer added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add customer." });
                    }
                }
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
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_customer_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@customer_id", id);

                        int rows = await command.ExecuteNonQueryAsync();

                        if (rows > 0)
                            return Ok(new { message = "Customer deleted successfully." });
                        else
                            return NotFound(new { errorMessage = $"Customer Id {id} not found." });
                    }
                }
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
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@customer_id", request.Customer_Id);
                    parameters.Add("@customer_name", request.Customer_Name);
                    parameters.Add("@prefix", request.Prefix);
                    parameters.Add("@gender", request.Gender);
                    parameters.Add("@phonenumber", request.Phonenumber);
                    parameters.Add("@city_id", request.City_Id);
                    parameters.Add("@cust_address", request.Cust_Address);
                    parameters.Add("@email_id", request.Email_Id);
                    parameters.Add("@dob", request.Dob);
                    parameters.Add("@aadhaar_number", request.Aadhaar_Number);
                    parameters.Add("@license_number", request.License_Number);
                    parameters.Add("@pan_number", request.Pan_Number);
                    parameters.Add("@gst_number", request.Gst_Number);
                    parameters.Add("@is_active", request.Is_Active);
                    parameters.Add("@customer_notes", request.Customer_Notes);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_Id);

                    int rows = await connection.ExecuteAsync(
                        "sp_customer_mast_ins_upd_del",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    if (rows == 0)
                        return NotFound(new { errorMessage = $"Customer Id {request.Customer_Id} not found." });
                    else
                        return Ok(new { message = "Customer updated successfully." });
                }
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
            var customer_list = new List<Customer_List>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_customer_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var customer = new Customer_List
                                {
                                    Customer_Id = reader.GetInt64(0),
                                    Customer_Name = reader.GetString(1),
                                    Prefix = reader.GetString(2),
                                    Gender = reader.GetString(3),
                                    Phonenumber = reader.GetString(4),
                                    City_Name = reader.GetString(5),
                                    Cust_Address = reader.GetString(6),
                                    Email_Id = reader.GetString(7),
                                    Dob = reader.IsDBNull(8) ? "" : reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    Aadhaar_Number = reader.GetString(9),
                                    License_Number = reader.GetString(10),
                                    Pan_Number = reader.GetString(11),
                                    Gst_Number = reader.GetString(12),
                                    Is_Active = reader.GetBoolean(13),
                                    Customer_Notes = reader.GetString(14),
                                    Created_Date = reader.GetDateTime(15).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(16) ? "" : reader.GetDateTime(16).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(17) ? "" : reader.GetString(17)
                                };
                                customer_list.Add(customer);
                            }
                        }
                    }
                }
                return Ok(customer_list);
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
            Single_Customer_List? customer = null;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_customer_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@customer_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                customer = new Single_Customer_List
                                {
                                    Customer_Id = reader.GetInt64(0),
                                    Customer_Name = reader.GetString(1),
                                    Prefix = reader.GetString(2),
                                    Gender = reader.GetString(3),
                                    Phonenumber = reader.GetString(4),
                                    City_Id = reader.GetInt64(5),
                                    Cust_Address = reader.GetString(6),
                                    Email_Id = reader.GetString(7),
                                    Dob = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                                    Aadhaar_Number = reader.GetString(9),
                                    License_Number = reader.GetString(10),
                                    Pan_Number = reader.GetString(11),
                                    Gst_Number = reader.GetString(12),
                                    Is_Active = reader.GetBoolean(13),
                                    Customer_Notes = reader.GetString(14),
                                    Created_Date = reader.IsDBNull(15) ? null : reader.GetDateTime(15),
                                    Updated_Date = reader.IsDBNull(16) ? null : reader.GetDateTime(16),
                                    User_Id = reader.IsDBNull(17) ? 0 : reader.GetInt64(17)
                                };
                            }
                        }
                    }
                }

                if (customer == null)
                    return NotFound($"Customer with Id {id} not found.");

                return Ok(customer);
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
            var customer_list = new List<Drop_Customer_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_customer_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "customerlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var customer = new Drop_Customer_List
                                {
                                    Customer_Id = reader.GetInt64(0),
                                    Customer_Name = reader.GetString(1)
                                };

                                customer_list.Add(customer);
                            }
                        }
                    }
                }

                return Ok(customer_list);
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
            public long User_Id { get; set; } = 0;
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
            public long User_Id { get; set; } = 0;
        }


        public class Customer_List
        {
            public long Customer_Id { get; set; } = 0;
            public string Customer_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
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
            public string User_Name { get; set; } = "";
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
            public long User_Id { get; set; } = 0;
        }


        public class Drop_Customer_List
        {
            public long Customer_Id { get; set; } = 0;
            public string Customer_Name { get; set; } = "";
        }




    }
}
