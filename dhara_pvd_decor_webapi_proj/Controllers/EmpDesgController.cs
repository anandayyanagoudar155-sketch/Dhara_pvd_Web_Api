using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpDesgController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmpDesgController(IConfiguration configuration)
        {

            _configuration = configuration;

        }



        [HttpPost("insert_emp_desg")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add_Emp_Desg([FromBody] Add_Emp_desg_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_employee_desg_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@desg_id", request.Desg_id);
                        command.Parameters.AddWithValue("@desg_name", request.Desg_name);
                        command.Parameters.AddWithValue("@desg_desc", request.Desg_desc);
                        command.Parameters.AddWithValue("@daily_wk_hr", request.Daily_wk_hr);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Designation Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Designation." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Designation name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteEmpDesg/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmpDesg(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_employee_desg_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@desg_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Designation deleted successfully." });
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




        [HttpPost("UpdateEmpDesg")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update_Emp_Desg([FromBody] Update_Emp_desg_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_employee_desg_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@desg_id", request.Desg_id);
                    parameters.Add("@desg_name", request.Desg_name);
                    parameters.Add("@desg_desc", request.Desg_desc);
                    parameters.Add("@daily_wk_hr", request.Daily_wk_hr);
                    parameters.Add("@created_date", request.Created_Date);
                    parameters.Add("@updated_date", request.Updated_Date);
                    parameters.Add("@user_id", request.User_Id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"Designation with ID {request.Desg_id} not found");
                else
                    return Ok(new { message = "Designation Updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("emp_desg_list")]
        public async Task<ActionResult<IEnumerable<Emp_desg_List>>> Get_emp_desg_list()
        {
            var desg_list = new List<Emp_desg_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_employee_desg_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var desg = new Emp_desg_List
                                {
                                    Desg_id = reader.GetInt64(0),
                                    Desg_name = reader.GetString(1),
                                    Desg_desc = reader.GetString(2),
                                    Daily_wk_hr = reader.GetDecimal(3),
                                    Created_Date = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(5) ? "" : reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                };

                                desg_list.Add(desg);
                            }
                        }
                    }
                }

                return Ok(desg_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("emp_desg/{id}")]
        public async Task<ActionResult<Single_Emp_desg_>> Get_emp_desg_by_id(long id)
        {
            Single_Emp_desg_? desg = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_employee_desg_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@desg_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                desg = new Single_Emp_desg_
                                {
                                    Desg_id = reader.GetInt64(0),
                                    Desg_name = reader.GetString(1),
                                    Desg_desc = reader.GetString(2),
                                    Daily_wk_hr = reader.GetDecimal(3),
                                    Created_Date = reader.GetDateTime(4),
                                    Updated_Date = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                                    User_Id = reader.IsDBNull(6) ? 0 : reader.GetInt64(6),
                                };
                            }
                        }
                    }
                }

                if (desg == null)
                    return NotFound($"Designation with ID {id} not found");

                return Ok(desg);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("dropdown_emp_desg_list")]
        public async Task<ActionResult<IEnumerable<Drop_Emp_desg_>>> Get_drop_emp_desg_list()
        {
            var desg_list = new List<Drop_Emp_desg_>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_employee_desg_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "employee_desg_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var desg = new Drop_Emp_desg_
                                {
                                    Desg_id = reader.GetInt64(0),
                                    Desg_name = reader.GetString(1),
                                };

                                desg_list.Add(desg);
                            }
                        }
                    }
                }

                return Ok(desg_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        public class Add_Emp_desg_Request
        {
            public long Desg_id { get; set; } = 0;
            public string Desg_name { get; set; } = "";
            public string Desg_desc { get; set; } = "";
            public decimal Daily_wk_hr { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;

        }

        public class Update_Emp_desg_Request
        {
            public long Desg_id { get; set; } = 0;
            public string Desg_name { get; set; } = "";
            public string Desg_desc { get; set; } = "";
            public decimal Daily_wk_hr { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class Emp_desg_List
        {
            public long Desg_id { get; set; } = 0;
            public string Desg_name { get; set; } = "";
            public string Desg_desc { get; set; } = "";
            public decimal Daily_wk_hr { get; set; } = 0;
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_name { get; set; } = "";
        }

        public class Single_Emp_desg_
        {
            public long Desg_id { get; set; } = 0;
            public string Desg_name { get; set; } = "";
            public string Desg_desc { get; set; } = "";
            public decimal Daily_wk_hr { get; set; } = 0;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_Emp_desg_
        {
            public long Desg_id { get; set; } = 0;
            public string Desg_name { get; set; } = "";

        }


    }
}
