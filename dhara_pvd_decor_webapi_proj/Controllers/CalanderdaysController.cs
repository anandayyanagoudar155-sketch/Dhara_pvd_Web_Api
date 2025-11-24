using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalanderdaysController : Controller
    {
        private readonly IConfiguration _configuration;

        public CalanderdaysController(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        [HttpPost("insert_emp_calenderdays")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add_emp_calenderdays([FromBody] AddEmpCalanderRequest request)
        {

            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {

                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_emp_calenderdays_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@emp_calender_id", 0);
                        command.Parameters.AddWithValue("@emp_calender_code", request.Emp_calander_code);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                        command.Parameters.AddWithValue("@month_id", request.Month_id);
                        command.Parameters.AddWithValue("@month_days", request.Month_days);
                        command.Parameters.AddWithValue("@emp_holidays", request.Emp_holidays);
                        command.Parameters.AddWithValue("@emp_weekend", request.Emp_Weekends);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "emp_calenderdays Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add emp_calenderdays." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "emp_calenderdays name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("Delete_emp_calenderdays/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deleteemp_calenderdays(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_emp_calenderdays_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@emp_calender_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "emp calenderday deleted successfully." });
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




        [HttpPost("Update_emp_calenderday")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> Update_Emp_Calander_days([FromBody] UpdateEmpCalanderRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_emp_calenderdays_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@emp_calender_id", request.Emp_calander_id);
                    parameters.Add("@emp_calender_code", request.Emp_calander_code);
                    parameters.Add("@comp_id", request.Comp_id);
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@month_id", request.Month_id);
                    parameters.Add("@month_days", request.Month_days);
                    parameters.Add("@emp_holidays", request.Emp_holidays);
                    parameters.Add("@emp_weekend", request.Emp_Weekends);
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
                    return NotFound($"emp calenderday with ID {request.Emp_calander_id} not found");
                else
                    return Ok(new { message = "emp calenderday updated successfully." });
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }




        [HttpGet("emp_calenderday_list")]
        public async Task<ActionResult<IEnumerable<EmpCalander_list>>> Get_emp_calenderday_list()
        {
            var emp_calenderday_list = new List<EmpCalander_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_emp_calenderdays_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var emp_calenderday = new EmpCalander_list
                                {
                                    Emp_calander_id = reader.GetInt64(0),
                                    Emp_calander_code = reader.GetString(1),
                                    Comp_name = reader.GetString(2),
                                    Fin_name = reader.GetString(3),
                                    Month_name = reader.GetString(4),
                                    Month_days = reader.GetDecimal(5),
                                    Emp_holidays = reader.GetDecimal(6),
                                    Emp_Weekends = reader.GetDecimal(7),
                                    Created_date = reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(9) ? "" : reader.GetDateTime(9).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(10) ? "" : reader.GetString(10)
                                };

                                emp_calenderday_list.Add(emp_calenderday);
                            }
                        }
                    }
                }

                return Ok(emp_calenderday_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("emp_calenderday/{id}")]
        public async Task<ActionResult<Single_EmpCalander_list>> Get_emp_calenderday_by_id(long id)
        {
            Single_EmpCalander_list? emp_calenderday = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_emp_calenderdays_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@emp_calender_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                emp_calenderday = new Single_EmpCalander_list
                                {
                                    Emp_calander_id = reader.GetInt64(0),
                                    Emp_calander_code = reader.GetString(1),
                                    Comp_id = reader.GetInt64(2),
                                    Fin_year_id = reader.GetInt64(3),
                                    Month_id = reader.GetInt64(4),
                                    Month_days = reader.GetDecimal(5),
                                    Emp_holidays = reader.GetDecimal(6),
                                    Emp_Weekends = reader.GetDecimal(7),
                                    Created_date = reader.GetDateTime(8),
                                    Updated_date = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                                    User_id = reader.IsDBNull(10) ? 0 : reader.GetInt64(10)
                                };
                            }
                        }
                    }
                }

                if (emp_calenderday == null)
                    return NotFound($"emp_calenderday with ID {id} not found");

                return Ok(emp_calenderday);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("dropdown_emp_calenderday_list")]
        public async Task<ActionResult<IEnumerable<drop_EmpCalander_list>>> Get_drop_emp_calenderdaylist()
        {
            var emp_calenderday_list = new List<drop_EmpCalander_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_emp_calenderdays_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "emp_calander_list");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var emp_calenderday = new drop_EmpCalander_list
                                {
                                    Emp_calander_id = reader.GetInt64(0),
                                    Emp_calander_code = reader.GetString(1)
                                };

                                emp_calenderday_list.Add(emp_calenderday);
                            }
                        }
                    }
                }

                return Ok(emp_calenderday_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        public class AddEmpCalanderRequest
        {

            public long Emp_calander_id { get; set; } = 0;
            public string Emp_calander_code { get; set; } = "";
            public long Comp_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public decimal Month_days { get; set; } = 0;
            public decimal Emp_holidays { get; set; } = 0;
            public decimal Emp_Weekends { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long? User_id { get; set; } = 0;

        }


        public class UpdateEmpCalanderRequest
        {
            public long Emp_calander_id { get; set; } = 0;
            public string Emp_calander_code { get; set; } = "";
            public long Comp_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public decimal Month_days { get; set; } = 0;
            public decimal Emp_holidays { get; set; } = 0;
            public decimal Emp_Weekends { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class EmpCalander_list
        {
            public long Emp_calander_id { get; set; } = 0;
            public string Emp_calander_code { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Fin_name { get; set; } = "";
            public string Month_name { get; set; } = "";
            public decimal Month_days { get; set; } = 0;
            public decimal Emp_holidays { get; set; } = 0;
            public decimal Emp_Weekends { get; set; } = 0;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";

        }


        public class Single_EmpCalander_list
        {
            public long Emp_calander_id { get; set; } = 0;
            public string Emp_calander_code { get; set; } = "";
            public long Comp_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public decimal Month_days { get; set; } = 0;
            public decimal Emp_holidays { get; set; } = 0;
            public decimal Emp_Weekends { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class drop_EmpCalander_list
        {
            public long Emp_calander_id { get; set; } = 0;
            public string Emp_calander_code { get; set; } = "";

        }

    }
}
