using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : Controller
    {
        private readonly IConfiguration _configuration;

        public CompanyController(IConfiguration configuration)
        {

            _configuration = configuration;

        }

        [HttpPost("InsertCompany")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AddCompany([FromBody] AddCompanyRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_company_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                        command.Parameters.AddWithValue("@comp_code", request.Comp_code);
                        command.Parameters.AddWithValue("@comp_name", request.Comp_name);
                        command.Parameters.AddWithValue("@comp_short_name", request.Comp_short_name);
                        command.Parameters.AddWithValue("@comp_type", request.Comp_type);
                        command.Parameters.AddWithValue("@comp_desc", request.Comp_desc);
                        command.Parameters.AddWithValue("@cin_number", request.Cin_number);
                        command.Parameters.AddWithValue("@gst_number", request.Gst_number);
                        command.Parameters.AddWithValue("@pan_number", request.Pan_number);
                        command.Parameters.AddWithValue("@contperson_name", request.Contperson_name);
                        command.Parameters.AddWithValue("@contact_email", request.Contact_email);
                        command.Parameters.AddWithValue("@contact_phone", request.Contact_phone);
                        command.Parameters.AddWithValue("@address_line1", request.Address_line1);
                        command.Parameters.AddWithValue("@address_line2", request.Address_line2);
                        command.Parameters.AddWithValue("@city_id", request.City_id);
                        command.Parameters.AddWithValue("@pincode", request.Pincode);
                        command.Parameters.AddWithValue("@is_active", request.Is_active);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@updated_date", request.Updated_date);
                        command.Parameters.AddWithValue("@logo_path", request.Logo_path);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Company Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Company." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Company name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("Deletecompany/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deletecompany(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_company_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@comp_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "company deleted successfully." });
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



        [HttpPost("Updatecompany")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> Updatecompany([FromBody] UpdateCompanyRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_company_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@comp_id", request.Comp_id);
                    parameters.Add("@comp_code", request.Comp_code);
                    parameters.Add("@comp_name", request.Comp_name);
                    parameters.Add("@comp_short_name", request.Comp_short_name);
                    parameters.Add("@comp_type", request.Comp_type);
                    parameters.Add("@comp_desc", request.Comp_desc);
                    parameters.Add("@cin_number", request.Cin_number);
                    parameters.Add("@gst_number", request.Gst_number);
                    parameters.Add("@pan_number", request.Pan_number);
                    parameters.Add("@contperson_name", request.Contperson_name);
                    parameters.Add("@contact_email", request.Contact_email);
                    parameters.Add("@contact_phone", request.Contact_phone);
                    parameters.Add("@address_line1", request.Address_line1);
                    parameters.Add("@address_line2", request.Address_line2);
                    parameters.Add("@city_id", request.City_id);
                    parameters.Add("@pincode", request.Pincode);
                    parameters.Add("@is_active", request.Is_active);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@logo_path", request.Logo_path);
                    parameters.Add("@user_id", request.User_id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"company with ID {request.Comp_id} not found");
                else
                    return Ok(new { message = "company updated successfully." });
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }


        [HttpGet("company_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<company_list>>> Get_CompanyList()
        {
            var company_list = new List<company_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_company_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var company = new company_list
                                {
                                    Comp_id = reader.GetInt64(0),
                                    Comp_code = reader.GetString(1),
                                    Comp_name = reader.GetString(2),
                                    Comp_short_name = reader.GetString(3),
                                    Comp_type = reader.GetString(4),
                                    Comp_desc = reader.GetString(5),
                                    Cin_number = reader.GetString(6),
                                    Gst_number = reader.GetString(7),
                                    Pan_number = reader.GetString(8),
                                    Contperson_name = reader.GetString(9),
                                    Contact_email = reader.GetString(10),
                                    Contact_phone = reader.GetString(11),
                                    Address_line1 = reader.GetString(12),
                                    Address_line2 = reader.GetString(13),
                                    City_name = reader.GetString(14),
                                    Pincode = reader.GetString(15),
                                    Is_active = reader.GetBoolean(16),
                                    Created_date = reader.GetDateTime(17).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(18) ? "" : reader.GetDateTime(18).ToString("yyyy-MM-dd"),
                                    Logo_path = reader.IsDBNull(19) ? "" : reader.GetString(19),
                                    User_name = reader.IsDBNull(20) ? "" : reader.GetString(20)
                                };

                                company_list.Add(company);
                            }
                        }

                    }
                }

                return Ok(company_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("company/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<single_company_list>> Gete_Company_by_id(long id)
        {
            single_company_list? company = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_company_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@comp_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                company = new single_company_list
                                {
                                    Comp_id = reader.GetInt64(0),
                                    Comp_code = reader.GetString(1),
                                    Comp_name = reader.GetString(2),
                                    Comp_short_name = reader.GetString(3),
                                    Comp_type = reader.GetString(4),
                                    Comp_desc = reader.GetString(5),
                                    Cin_number = reader.GetString(6),
                                    Gst_number = reader.GetString(7),
                                    Pan_number = reader.GetString(8),
                                    Contperson_name = reader.GetString(9),
                                    Contact_email = reader.GetString(10),
                                    Contact_phone = reader.GetString(11),
                                    Address_line1 = reader.GetString(12),
                                    Address_line2 = reader.GetString(13),
                                    City_id = reader.GetInt64(14),
                                    Pincode = reader.GetString(15),
                                    Is_active = reader.GetBoolean(16),
                                    Created_date = reader.GetDateTime(17),
                                    Updated_date = reader.IsDBNull(18) ? null : reader.GetDateTime(18),
                                    Logo_path = reader.IsDBNull(19) ? "" : reader.GetString(19),
                                    User_id = reader.IsDBNull(20) ? 0 : reader.GetInt64(20)
                                };
                            }
                        }
                    }
                }

                if (company == null)
                    return NotFound($"company with ID {id} not found");

                return Ok(company);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("dropdown_company_list")]
        public async Task<ActionResult<IEnumerable<drop_company_list>>> Get_drop_companylist()
        {
            var company_list = new List<drop_company_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_company_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "companylist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var company = new drop_company_list
                                {
                                    Comp_id = reader.GetInt64(0),
                                    Comp_name = reader.GetString(1)
                                };

                                company_list.Add(company);
                            }
                        }
                    }
                }

                return Ok(company_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        public class AddCompanyRequest
        {

            public long Comp_id { get; set; } = 0;
            public string Comp_code { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Comp_short_name { get; set; } = "";
            public string Comp_type { get; set; } = "";
            public string Comp_desc { get; set; } = "";
            public string Cin_number { get; set; } = "";
            public string Gst_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Contperson_name { get; set; } = "";
            public string Contact_email { get; set; } = "";
            public string Contact_phone { get; set; } = "";
            public string Address_line1 { get; set; } = "";
            public string Address_line2 { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Pincode { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public string Logo_path { get; set; } = "";
            public long User_id { get; set; } = 0;

        }


        public class UpdateCompanyRequest
        {

            public long Comp_id { get; set; } = 0;
            public string Comp_code { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Comp_short_name { get; set; } = "";
            public string Comp_type { get; set; } = "";
            public string Comp_desc { get; set; } = "";
            public string Cin_number { get; set; } = "";
            public string Gst_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Contperson_name { get; set; } = "";
            public string Contact_email { get; set; } = "";
            public string Contact_phone { get; set; } = "";
            public string Address_line1 { get; set; } = "";
            public string Address_line2 { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Pincode { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public string Logo_path { get; set; } = "";
            public long User_id { get; set; } = 0;

        }


        public class company_list
        {
            public long Comp_id { get; set; } = 0;
            public string Comp_code { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Comp_short_name { get; set; } = "";
            public string Comp_type { get; set; } = "";
            public string Comp_desc { get; set; } = "";
            public string Cin_number { get; set; } = "";
            public string Gst_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Contperson_name { get; set; } = "";
            public string Contact_email { get; set; } = "";
            public string Contact_phone { get; set; } = "";
            public string Address_line1 { get; set; } = "";
            public string Address_line2 { get; set; } = "";
            public string City_name { get; set; } = "";
            public string Pincode { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string Logo_path { get; set; } = "";
            public string User_name { get; set; } = "";

        }


        public class single_company_list
        {

            public long Comp_id { get; set; } = 0;
            public string Comp_code { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Comp_short_name { get; set; } = "";
            public string Comp_type { get; set; } = "";
            public string Comp_desc { get; set; } = "";
            public string Cin_number { get; set; } = "";
            public string Gst_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Contperson_name { get; set; } = "";
            public string Contact_email { get; set; } = "";
            public string Contact_phone { get; set; } = "";
            public string Address_line1 { get; set; } = "";
            public string Address_line2 { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Pincode { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public string Logo_path { get; set; } = "";
            public long User_id { get; set; } = 0;

        }


        public class drop_company_list
        {
            public long Comp_id { get; set; } = 0;
            public string Comp_name { get; set; } = "";

        }


    }
}
