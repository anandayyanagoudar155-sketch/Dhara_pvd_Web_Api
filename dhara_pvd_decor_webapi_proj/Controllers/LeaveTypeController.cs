using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveTypeController : Controller
    {
        private readonly IConfiguration _configuration;

        public LeaveTypeController(IConfiguration configuration)
        {

            _configuration = configuration;

        }

        [HttpPost("insert_leavetype")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddLeaveType([FromBody] Add_LeaveType_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_leavetype_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@leavetype_id", request.Leavetype_id);
                        command.Parameters.AddWithValue("@yearsincompany", request.Yearsincompany);
                        command.Parameters.AddWithValue("@allocated_leaves", request.Allocated_leaves);
                        command.Parameters.AddWithValue("@leave_name", request.Leave_name);
                        command.Parameters.AddWithValue("@leave_desc", request.Leave_desc);
                        command.Parameters.AddWithValue("@casual_leaves", request.Casual_leaves);
                        command.Parameters.AddWithValue("@sick_leaves", request.Sick_leaves);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Leave Type added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add Leave Type." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Leave Type name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }




        [HttpDelete("DeleteLeaveType/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteLeaveType(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_leavetype_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@leavetype_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Leave Type deleted successfully." });
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




        [HttpPost("UpdateLeaveType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateLeaveType([FromBody] Update_LeaveType_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_leavetype_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@leavetype_id", request.Leavetype_id);
                    parameters.Add("@yearsincompany", request.Yearsincompany);
                    parameters.Add("@allocated_leaves", request.Allocated_leaves);
                    parameters.Add("@leave_name", request.Leave_name);
                    parameters.Add("@leave_desc", request.Leave_desc);
                    parameters.Add("@casual_leaves", request.Casual_leaves);
                    parameters.Add("@sick_leaves", request.Sick_leaves);
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
                    return NotFound($"Leave Type with ID {request.Leavetype_id} not found");
                else
                    return Ok(new { message = "Leave Type updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("leavetype_list")]
        public async Task<ActionResult<IEnumerable<LeaveType_List>>> Get_LeaveType_List()
        {
            var leavelist = new List<LeaveType_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_leavetype_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var leavetype = new LeaveType_List
                                {
                                    Leavetype_id = reader.GetInt64(0),
                                    Yearsincompany = reader.GetDecimal(1),
                                    Allocated_leaves = reader.GetDecimal(2),
                                    Leave_name = reader.GetString(3),
                                    Leave_desc = reader.GetString(4),
                                    Casual_leaves = reader.GetDecimal(5),
                                    Sick_leaves = reader.GetDecimal(6),
                                    Created_date = reader.GetDateTime(7).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(8) ? "" : reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(9) ? "" : reader.GetString(9)
                                };

                                leavelist.Add(leavetype);
                            }
                        }
                    }
                }

                return Ok(leavelist);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("leavetype/{id}")]
        public async Task<ActionResult<Single_LeaveType_>> Get_LeaveType_By_Id(long id)
        {
            Single_LeaveType_? leavetype = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_leavetype_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@leavetype_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                leavetype = new Single_LeaveType_
                                {
                                    Leavetype_id = reader.GetInt64(0),
                                    Yearsincompany = reader.GetDecimal(1),
                                    Allocated_leaves = reader.GetDecimal(2),
                                    Leave_name = reader.GetString(3),
                                    Leave_desc = reader.GetString(4),
                                    Casual_leaves = reader.GetDecimal(5),
                                    Sick_leaves = reader.GetDecimal(6),
                                    Created_date = reader.GetDateTime(7),
                                    Updated_date = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                                    User_id = reader.IsDBNull(9) ? 0 : reader.GetInt64(9)
                                };
                            }
                        }
                    }
                }

                if (leavetype == null)
                    return NotFound($"Leave Type with ID {id} not found");

                return Ok(leavetype);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("dropdown_leavetype_list")]
        public async Task<ActionResult<IEnumerable<Drop_LeaveType_>>> Get_Drop_LeaveType_List()
        {
            var leavelist = new List<Drop_LeaveType_>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_leavetype_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "leavetype_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var leave = new Drop_LeaveType_
                                {
                                    Leavetype_id = reader.GetInt64(0),
                                    Leave_name = reader.GetString(1)
                                };

                                leavelist.Add(leave);
                            }
                        }
                    }
                }

                return Ok(leavelist);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }







        public class Add_LeaveType_Request
        {
            public long Leavetype_id { get; set; } = 0;
            public decimal Yearsincompany { get; set; } = 0;
            public decimal Allocated_leaves { get; set; } = 0;
            public string Leave_name { get; set; } = "";
            public string Leave_desc { get; set; } = "";
            public decimal Casual_leaves { get; set; } = 0;
            public decimal Sick_leaves { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }




        public class Update_LeaveType_Request
        {
            public long Leavetype_id { get; set; } = 0;
            public decimal Yearsincompany { get; set; } = 0;
            public decimal Allocated_leaves { get; set; } = 0;
            public string Leave_name { get; set; } = "";
            public string Leave_desc { get; set; } = "";
            public decimal Casual_leaves { get; set; } = 0;
            public decimal Sick_leaves { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }



        public class LeaveType_List
        {
            public long Leavetype_id { get; set; } = 0;
            public decimal Yearsincompany { get; set; } = 0;
            public decimal Allocated_leaves { get; set; } = 0;
            public string Leave_name { get; set; } = "";
            public string Leave_desc { get; set; } = "";
            public decimal Casual_leaves { get; set; } = 0;
            public decimal Sick_leaves { get; set; } = 0;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";
        }



        public class Single_LeaveType_
        {
            public long Leavetype_id { get; set; } = 0;
            public decimal Yearsincompany { get; set; } = 0;
            public decimal Allocated_leaves { get; set; } = 0;
            public string Leave_name { get; set; } = "";
            public string Leave_desc { get; set; } = "";
            public decimal Casual_leaves { get; set; } = 0;
            public decimal Sick_leaves { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }



        public class Drop_LeaveType_
        {
            public long Leavetype_id { get; set; } = 0;
            public string Leave_name { get; set; } = "";
        }

    }
}
