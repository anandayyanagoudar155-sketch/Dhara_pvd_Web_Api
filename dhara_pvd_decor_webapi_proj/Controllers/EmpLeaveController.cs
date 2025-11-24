using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpLeaveController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmpLeaveController(IConfiguration configuration)
        {

            _configuration = configuration;

        }

        [HttpPost("insert_emp_leave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddEmpLeave([FromBody] Add_Emp_Leave_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_emp_leave_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@emp_leave_id", request.Emp_leave_id);
                        command.Parameters.AddWithValue("@employee_id", request.Employee_id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                        command.Parameters.AddWithValue("@month_id", request.Month_id);
                        command.Parameters.AddWithValue("@emp_leave_date", request.Emp_leave_date);
                        command.Parameters.AddWithValue("@leavetype_id", request.Leavetype_id);
                        command.Parameters.AddWithValue("@total_allocated_leaves", request.Total_allocated_leaves);
                        command.Parameters.AddWithValue("@leaves_used", request.Leaves_used);
                        command.Parameters.AddWithValue("@leaves_balance", request.Leaves_balance);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Emp Leave added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add Emp Leave." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Duplicate Emp Leave entry already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }




        [HttpDelete("delete_emp_leave/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmpLeave(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_emp_leave_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@emp_leave_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Emp Leave deleted successfully." });
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



        [HttpPost("update_emp_leave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEmpLeave([FromBody] Update_Emp_Leave_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_emp_leave_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@emp_leave_id", request.Emp_leave_id);
                    parameters.Add("@employee_id", request.Employee_id);
                    parameters.Add("@comp_id", request.Comp_id);
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@month_id", request.Month_id);
                    parameters.Add("@emp_leave_date", request.Emp_leave_date);
                    parameters.Add("@leavetype_id", request.Leavetype_id);
                    parameters.Add("@total_allocated_leaves", request.Total_allocated_leaves);
                    parameters.Add("@leaves_used", request.Leaves_used);
                    parameters.Add("@leaves_balance", request.Leaves_balance);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"Emp Leave with ID {request.Emp_leave_id} not found");
                else
                    return Ok(new { message = "Emp Leave updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("emp_leave_list")]
        public async Task<ActionResult<IEnumerable<Emp_Leave_List>>> Get_Emp_Leave_List()
        {
            var list = new List<Emp_Leave_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_emp_leave_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new Emp_Leave_List
                                {
                                    Emp_leave_id = reader.GetInt64(0),
                                    First_name = reader.GetString(1),
                                    Last_name = reader.GetString(2),
                                    Comp_name = reader.GetString(3),
                                    Fin_name = reader.GetString(4),
                                    Month_name = reader.GetString(5),
                                    Emp_leave_date = reader.GetDateTime(6).ToString("yyyy-MM-dd"),
                                    Leave_name = reader.GetString(7),
                                    Total_allocated_leaves = reader.GetDecimal(8),
                                    Leaves_used = reader.GetDecimal(9),
                                    Leaves_balance = reader.GetDecimal(10),
                                    Created_date = reader.GetDateTime(11).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(12) ? "" : reader.GetDateTime(12).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(13) ? "" : reader.GetString(13)
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


        [HttpGet("emp_leave/{id}")]
        public async Task<ActionResult<Single_Emp_Leave>> Get_Emp_Leave_By_Id(long id)
        {
            Single_Emp_Leave? item = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_emp_leave_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@emp_leave_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                item = new Single_Emp_Leave
                                {
                                    Emp_leave_id = reader.GetInt64(0),
                                    Employee_id = reader.GetInt64(1),
                                    Comp_id = reader.GetInt64(2),
                                    Fin_year_id = reader.GetInt64(3),
                                    Month_id = reader.GetInt64(4),
                                    Emp_leave_date = reader.GetDateTime(5),
                                    Leavetype_id = reader.GetInt64(6),
                                    Total_allocated_leaves = reader.GetDecimal(7),
                                    Leaves_used = reader.GetDecimal(8),
                                    Leaves_balance = reader.GetDecimal(9),
                                    Created_date = reader.GetDateTime(10),
                                    Updated_date = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                                    User_id = reader.IsDBNull(12) ? 0 : reader.GetInt64(12)
                                };
                            }
                        }
                    }
                }

                if (item == null)
                    return NotFound($"Emp Leave with ID {id} not found");

                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("dropdown_emp_leave_list")]
        public async Task<ActionResult<IEnumerable<Drop_Emp_Leave>>> Get_Drop_Emp_Leave_List()
        {
            var list = new List<Drop_Emp_Leave>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_emp_leave_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "emp_leavelist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new Drop_Emp_Leave
                                {
                                    Emp_leave_id = reader.GetInt64(0),
                                    Employee_id = reader.GetInt64(1)
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





        public class Add_Emp_Leave_Request
        {
            public long Emp_leave_id { get; set; } = 0;
            public long Employee_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public DateTime? Emp_leave_date { get; set; }
            public long Leavetype_id { get; set; } = 0;
            public decimal Total_allocated_leaves { get; set; } = 0;
            public decimal Leaves_used { get; set; } = 0;
            public decimal Leaves_balance { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }


        public class Update_Emp_Leave_Request
        {
            public long Emp_leave_id { get; set; } = 0;
            public long Employee_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public DateTime? Emp_leave_date { get; set; }
            public long Leavetype_id { get; set; } = 0;
            public decimal Total_allocated_leaves { get; set; } = 0;
            public decimal Leaves_used { get; set; } = 0;
            public decimal Leaves_balance { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }


        public class Emp_Leave_List
        {
            public long Emp_leave_id { get; set; } = 0;
            public string First_name { get; set; } = "";
            public string Last_name { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Fin_name { get; set; } = "";
            public string Month_name { get; set; } = "";
            public string Emp_leave_date { get; set; } = "";
            public string Leave_name { get; set; } = "";
            public decimal Total_allocated_leaves { get; set; } = 0;
            public decimal Leaves_used { get; set; } = 0;
            public decimal Leaves_balance { get; set; } = 0;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";
        }


        public class Single_Emp_Leave
        {
            public long Emp_leave_id { get; set; } = 0;
            public long Employee_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public DateTime? Emp_leave_date { get; set; }
            public long Leavetype_id { get; set; } = 0;
            public decimal Total_allocated_leaves { get; set; } = 0;
            public decimal Leaves_used { get; set; } = 0;
            public decimal Leaves_balance { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }


        public class Drop_Emp_Leave
        {
            public long Emp_leave_id { get; set; } = 0;
            public long Employee_id { get; set; } = 0;
        }

    }
}
