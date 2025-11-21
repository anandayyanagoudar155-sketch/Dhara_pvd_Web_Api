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


        [HttpPost("insert_custdetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCustDetail([FromBody] Add_CustDetail_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_cust_detail_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@cust_detail_id", request.Cust_detail_id);
                        command.Parameters.AddWithValue("@customer_id", request.Customer_id);
                        command.Parameters.AddWithValue("@opening_balance", request.Opening_balance);
                        command.Parameters.AddWithValue("@invoice_balance", request.Invoice_balance);
                        command.Parameters.AddWithValue("@outstanding_balance", request.Outstanding_balance);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Customer Detail added successfully." });

                        return StatusCode(500, new { errorMessage = "Failed to add Customer Detail." });
                    }
                }
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
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_cust_detail_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@cust_detail_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Customer Detail deleted successfully." });

                        return StatusCode(500, new { errorMessage = "No record deleted." });
                    }
                }
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
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_cust_detail_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@cust_detail_id", request.Cust_detail_id);
                    parameters.Add("@customer_id", request.Customer_id);
                    parameters.Add("@opening_balance", request.Opening_balance);
                    parameters.Add("@invoice_balance", request.Invoice_balance);
                    parameters.Add("@outstanding_balance", request.Outstanding_balance);
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@comp_id", request.Comp_id);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"Customer Detail with ID {request.Cust_detail_id} not found");

                return Ok(new { message = "Customer Detail updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("custdetail_list")]
        public async Task<ActionResult<IEnumerable<CustDetail_List>>> Get_CustDetail_list()
        {
            var list = new List<CustDetail_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_cust_detail_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new CustDetail_List
                                {
                                    Cust_detail_id = reader.GetInt64(0),
                                    Customer_name = reader.GetInt64(1),
                                    Opening_balance = reader.GetDecimal(2),
                                    Invoice_balance = reader.GetDecimal(3),
                                    Outstanding_balance = reader.GetDecimal(4),
                                    Fin_Year_Name = reader.GetString(5),
                                    Comp_Name = reader.GetString(6),
                                    Created_Date = reader.GetDateTime(7).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(8) ? "" : reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(9) ? "" : reader.GetString(9)
                                };

                                list.Add(item);
                            }
                        }
                    }
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("custdetail/{id}")]
        public async Task<ActionResult<Single_CustDetail>> Get_CustDetail_by_id(long id)
        {
            Single_CustDetail? data = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_cust_detail_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@cust_detail_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                data = new Single_CustDetail
                                {
                                    Cust_detail_id = reader.GetInt64(0),
                                    Customer_id = reader.GetInt64(1),
                                    Opening_balance = reader.GetDecimal(2),
                                    Invoice_balance = reader.GetDecimal(3),
                                    Outstanding_balance = reader.GetDecimal(4),
                                    Fin_year_id = reader.GetInt64(5),
                                    Comp_id = reader.GetInt64(6),
                                    Created_date = reader.GetDateTime(7),
                                    Updated_date = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                                    User_id = reader.IsDBNull(9) ? 0 : reader.GetInt64(9)
                                };
                            }
                        }
                    }
                }

                if (data == null)
                    return NotFound($"Customer Detail with ID {id} not found");

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
            var list = new List<Drop_CustDetail>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_cust_detail_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "cust_detail_list");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add(new Drop_CustDetail
                                {
                                    Cust_detail_id = reader.GetInt64(0)
                                });
                            }
                        }
                    }
                }

                return Ok(list);
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


        public class Add_CustDetail_Request
        {
            public long Cust_detail_id { get; set; } = 0;
            public long Customer_id { get; set; } = 0;
            public decimal Opening_balance { get; set; } = 0;
            public decimal Invoice_balance { get; set; } = 0;
            public decimal Outstanding_balance { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }

        public class Update_CustDetail_Request
        {
            public long Cust_detail_id { get; set; } = 0;
            public long Customer_id { get; set; } = 0;
            public decimal Opening_balance { get; set; } = 0;
            public decimal Invoice_balance { get; set; } = 0;
            public decimal Outstanding_balance { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }


        public class CustDetail_List
        {
            public long Cust_detail_id { get; set; } = 0;
            public long Customer_name { get; set; } = 0;
            public decimal Opening_balance { get; set; } = 0;
            public decimal Invoice_balance { get; set; } = 0;
            public decimal Outstanding_balance { get; set; } = 0;
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }



        public class Single_CustDetail
        {
            public long Cust_detail_id { get; set; } = 0;
            public long Customer_id { get; set; } = 0;
            public decimal Opening_balance { get; set; } = 0;
            public decimal Invoice_balance { get; set; } = 0;
            public decimal Outstanding_balance { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }



        public class Drop_CustDetail
        {
            public long Cust_detail_id { get; set; } = 0;
        }

    }
}
