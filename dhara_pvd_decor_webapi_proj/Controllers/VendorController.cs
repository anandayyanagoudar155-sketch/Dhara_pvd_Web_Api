using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : Controller
    {
        private readonly IConfiguration _configuration;

        public VendorController(IConfiguration configuration)
        {

            _configuration = configuration;

        }



        [HttpPost("insert_vendor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Addvendor([FromBody] AddVendorRequest request)
        {

            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {

                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_vendor_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@vendor_id", 0);
                        command.Parameters.AddWithValue("@vendor_name", request.Vendor_Name);
                        command.Parameters.AddWithValue("@prefix", request.Prefix);
                        command.Parameters.AddWithValue("@gender", request.Gender);
                        command.Parameters.AddWithValue("@phonenumber", request.Phonenumber);
                        command.Parameters.AddWithValue("@city_id", request.City_Id);
                        command.Parameters.AddWithValue("@address", request.Address);
                        command.Parameters.AddWithValue("@email_id", request.Email_Id);
                        command.Parameters.AddWithValue("@dob", request.Dob);
                        command.Parameters.AddWithValue("@aadhaar_number", request.Aadhaar_Number);
                        command.Parameters.AddWithValue("@license_number", request.License_Number);
                        command.Parameters.AddWithValue("@pan_number", request.Pan_Number);
                        command.Parameters.AddWithValue("@gst_number", request.Gst_Number);
                        command.Parameters.AddWithValue("@is_active", request.Is_Active);
                        command.Parameters.AddWithValue("@vendor_notes", request.Vendor_Notes);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);


                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "vendor Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add vendor." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "vendor name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteVendor/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteVendor(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_vendor_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@vendor_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Vendor deleted successfully." });
                        else
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


        [HttpPost("UpdateVendor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> Updatevendor([FromBody] UpdateVendorRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_vendor_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@vendor_id", request.Vendor_Id);
                    parameters.Add("@vendor_name", request.Vendor_Name);
                    parameters.Add("@prefix", request.Prefix);
                    parameters.Add("@gender", request.Gender);
                    parameters.Add("@phonenumber", request.Phonenumber);
                    parameters.Add("@city_id", request.City_Id);
                    parameters.Add("@address", request.Address);
                    parameters.Add("@email_id", request.Email_Id);
                    parameters.Add("@dob", request.Dob);
                    parameters.Add("@aadhaar_number", request.Aadhaar_Number);
                    parameters.Add("@license_number", request.License_Number);
                    parameters.Add("@pan_number", request.Pan_Number);
                    parameters.Add("@gst_number", request.Gst_Number);
                    parameters.Add("@is_active", request.Is_Active);
                    parameters.Add("@vendor_notes", request.Vendor_Notes);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_Id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"vendor with ID {request.Vendor_Id} not found");
                else
                    return Ok(new { message = "Vendor updated successfully." });
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }


        [HttpGet("vendor_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Vendor_List>>> Get_vendor_list()
        {
            var vendor_list = new List<Vendor_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_vendor_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var vendor = new Vendor_List
                                {
                                    Vendor_Id = reader.GetInt64(0),
                                    Vendor_Name = reader.GetString(1),
                                    Prefix = reader.GetString(2),
                                    Gender = reader.GetString(3),
                                    Phonenumber = reader.GetString(4),
                                    City_Name = reader.GetString(5),
                                    Address = reader.GetString(6),
                                    Email_Id = reader.GetString(7),
                                    Dob = reader.IsDBNull(8) ? "" : reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    Aadhaar_Number = reader.GetString(9),
                                    License_Number = reader.GetString(10),
                                    Pan_Number = reader.GetString(11),
                                    Gst_Number = reader.GetString(12),
                                    Is_Active = reader.GetBoolean(13),
                                    Vendor_Notes = reader.GetString(14),
                                    Created_Date = reader.GetDateTime(15).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(16) ? "" : reader.GetDateTime(16).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(17) ? "" : reader.GetString(17)
                                };

                                vendor_list.Add(vendor);
                            }
                        }
                    }
                }

                return Ok(vendor_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("vendor/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Single_Vendor_List>> Get_vendor_by_id(long id)
        {
            Single_Vendor_List? vendor = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_vendor_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@vendor_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                vendor = new Single_Vendor_List
                                {
                                    Vendor_Id = reader.GetInt64(0),
                                    Vendor_Name = reader.GetString(1),
                                    Prefix = reader.GetString(2),
                                    Gender = reader.GetString(3),
                                    Phonenumber = reader.GetString(4),
                                    City_Id = reader.GetInt64(5),
                                    Address = reader.GetString(6),
                                    Email_Id = reader.GetString(7),
                                    Dob = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                                    Aadhaar_Number = reader.GetString(9),
                                    License_Number = reader.GetString(10),
                                    Pan_Number = reader.GetString(11),
                                    Gst_Number = reader.GetString(12),
                                    Is_Active = reader.GetBoolean(13),
                                    Vendor_Notes = reader.GetString(14),
                                    Created_Date = reader.IsDBNull(15) ? null : reader.GetDateTime(15),
                                    Updated_Date = reader.IsDBNull(16) ? null : reader.GetDateTime(16),
                                    User_Id = reader.IsDBNull(17) ? 0 : reader.GetInt64(17)
                                };
                            }
                        }
                    }
                }

                if (vendor == null)
                    return NotFound($"vendor with Id {id} not found");

                return Ok(vendor);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("dropdownlist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Drop_Vendor_List>>> Get_drop_vendorlist()
        {
            var vendor_list = new List<Drop_Vendor_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_vendor_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "vendor_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var vendor = new Drop_Vendor_List
                                {
                                    Vendor_Id = reader.GetInt64(0),
                                    Vendor_Name = reader.GetString(1)
                                };

                                vendor_list.Add(vendor);
                            }
                        }
                    }
                }

                return Ok(vendor_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpPost("insert_vendor_detail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddVendorDetail([FromBody] Add_VendorDetail_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_vendor_detail_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@vendor_detail_id", request.Vendor_detail_id);
                        command.Parameters.AddWithValue("@vendor_id", request.Vendor_id);
                        command.Parameters.AddWithValue("@opening_balance", request.Opening_balance);
                        command.Parameters.AddWithValue("@invoice_balance", request.Invoice_balance);
                        command.Parameters.AddWithValue("@outstanding_balance", request.Outstanding_balance);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Vendor Detail added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Vendor Detail." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Vendor Detail already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }




        [HttpDelete("delete_vendor_detail/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteVendorDetail(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_vendor_detail_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@vendor_detail_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Vendor Detail deleted successfully." });
                        else
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




        [HttpPost("update_vendor_detail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateVendorDetail([FromBody] Update_VendorDetail_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_vendor_detail_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@vendor_detail_id", request.Vendor_detail_id);
                    parameters.Add("@vendor_id", request.Vendor_id);
                    parameters.Add("@opening_balance", request.Opening_balance);
                    parameters.Add("@invoice_balance", request.Invoice_balance);
                    parameters.Add("@outstanding_balance", request.Outstanding_balance);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@comp_id", request.Comp_id);
                    parameters.Add("@user_id", request.User_id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"Vendor Detail with ID {request.Vendor_detail_id} not found");
                else
                    return Ok(new { message = "Vendor Detail updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("vendor_detail_list")]
        public async Task<ActionResult<IEnumerable<VendorDetail_List>>> Get_VendorDetail_List()
        {
            var vendor_list = new List<VendorDetail_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_vendor_detail_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var vendor = new VendorDetail_List
                                {
                                    Vendor_detail_id = reader.GetInt64(0),
                                    Vendor_id = reader.GetInt64(1),
                                    Opening_balance = reader.GetDecimal(2),
                                    Invoice_balance = reader.GetDecimal(3),
                                    Outstanding_balance = reader.GetDecimal(4),
                                    Created_Date = reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(6) ? "" : reader.GetDateTime(6).ToString("yyyy-MM-dd"),
                                    Fin_Year_Name = reader.GetString(7),
                                    Comp_Name = reader.GetString(8),
                                    User_Name = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                };

                                vendor_list.Add(vendor);
                            }
                        }
                    }
                }

                return Ok(vendor_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("vendor_detail/{id}")]
        public async Task<ActionResult<Single_VendorDetail>> Get_vendor_detail_by_id(long id)
        {
            Single_VendorDetail? vendor = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_vendor_detail_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@vendor_detail_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                vendor = new Single_VendorDetail
                                {
                                    Vendor_detail_id = reader.GetInt64(0),
                                    Vendor_id = reader.GetInt64(1),
                                    Opening_balance = reader.GetDecimal(2),
                                    Invoice_balance = reader.GetDecimal(3),
                                    Outstanding_balance = reader.GetDecimal(4),
                                    Created_date = reader.GetDateTime(5),
                                    Updated_date = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                                    Fin_year_id = reader.GetInt64(7),
                                    Comp_id = reader.GetInt64(8),
                                    User_id = reader.IsDBNull(9) ? 0 : reader.GetInt64(9),
                                };
                            }
                        }
                    }
                }

                if (vendor == null)
                    return NotFound($"Vendor Detail with ID {id} not found");

                return Ok(vendor);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("dropdown_vendor_detail")]
        public async Task<ActionResult<IEnumerable<Drop_VendorDetail>>> Get_drop_vendor_detail()
        {
            var vendor_list = new List<Drop_VendorDetail>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_vendor_detail_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "vendor_detaillist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new Drop_VendorDetail
                                {
                                    Vendor_detail_id = reader.GetInt64(0)
                                };

                                vendor_list.Add(item);
                            }
                        }
                    }
                }

                return Ok(vendor_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        public class AddVendorRequest
        {
            public string Vendor_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public long City_Id { get; set; } = 0;
            public string Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; } = true;
            public string Vendor_Notes { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_Id { get; set; } = 0;
        }



        public class UpdateVendorRequest
        {
            public long Vendor_Id { get; set; } = 0;
            public string Vendor_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public long City_Id { get; set; } = 0;
            public string Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; } = true;
            public string Vendor_Notes { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_Id { get; set; } = 0;
        }




        public class Vendor_List
        {
            public long Vendor_Id { get; set; } = 0;
            public string Vendor_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public string City_Name { get; set; } = "";
            public string Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public string Dob { get; set; } = "";
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; }
            public string Vendor_Notes { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }


        public class Single_Vendor_List
        {
            public long Vendor_Id { get; set; } = 0;
            public string Vendor_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public long City_Id { get; set; } = 0;
            public string Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; }
            public string Vendor_Notes { get; set; } = "";
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_Vendor_List
        {
            public long Vendor_Id { get; set; } = 0;
            public string Vendor_Name { get; set; } = "";
        }


        public class Add_VendorDetail_Request
        {
            public long Vendor_detail_id { get; set; } = 0;
            public long Vendor_id { get; set; } = 0;
            public decimal Opening_balance { get; set; } = 0;
            public decimal Invoice_balance { get; set; } = 0;
            public decimal Outstanding_balance { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }


        public class Update_VendorDetail_Request
        {
            public long Vendor_detail_id { get; set; } = 0;
            public long Vendor_id { get; set; } = 0;
            public decimal Opening_balance { get; set; } = 0;
            public decimal Invoice_balance { get; set; } = 0;
            public decimal Outstanding_balance { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }


        public class VendorDetail_List
        {
            public long Vendor_detail_id { get; set; } = 0;
            public long Vendor_id { get; set; } = 0;
            public decimal Opening_balance { get; set; } = 0;
            public decimal Invoice_balance { get; set; } = 0;
            public decimal Outstanding_balance { get; set; } = 0;
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string User_Name { get; set; } = "";
        }


        public class Single_VendorDetail
        {
            public long Vendor_detail_id { get; set; } = 0;
            public long Vendor_id { get; set; } = 0;
            public decimal Opening_balance { get; set; } = 0;
            public decimal Invoice_balance { get; set; } = 0;
            public decimal Outstanding_balance { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }

        public class Drop_VendorDetail
        {
            public long Vendor_detail_id { get; set; } = 0;
        }

    }
}
