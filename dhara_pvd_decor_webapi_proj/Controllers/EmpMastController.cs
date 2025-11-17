using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpMastController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmpMastController(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        [HttpPost("insert_employee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddEmployee([FromBody] Add_Employee_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_employee_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@employee_id", request.Employee_id);
                        command.Parameters.AddWithValue("@desg_id", request.Desg_id);
                        command.Parameters.AddWithValue("@first_name", request.First_name);
                        command.Parameters.AddWithValue("@last_name", request.Last_name);
                        command.Parameters.AddWithValue("@gender", request.Gender);
                        command.Parameters.AddWithValue("@dob", request.Dob);
                        command.Parameters.AddWithValue("@phone_number", request.Phone_number);
                        command.Parameters.AddWithValue("@emailid", request.Emailid);
                        command.Parameters.AddWithValue("@address", request.Address);
                        command.Parameters.AddWithValue("@city_id", request.City_id);
                        command.Parameters.AddWithValue("@aadhaar_number", request.Aadhaar_number);
                        command.Parameters.AddWithValue("@pan_number", request.Pan_number);
                        command.Parameters.AddWithValue("@bankaccount_no", request.Bankaccount_no);
                        command.Parameters.AddWithValue("@ifsc_code", request.Ifsc_code);
                        command.Parameters.AddWithValue("@joining_date", request.Joining_date);
                        command.Parameters.AddWithValue("@relieving_date", request.Relieving_date);
                        command.Parameters.AddWithValue("@education", request.Education);
                        command.Parameters.AddWithValue("@exp_year", request.Exp_year);
                        command.Parameters.AddWithValue("@annual_salary", request.Annual_salary);
                        command.Parameters.AddWithValue("@active_status", request.Active_status);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Employee added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add employee." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "Duplicate record exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteEmployee/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmployee(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_employee_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@employee_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Employee deleted successfully." });
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




        [HttpPost("update_employee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEmployee([FromBody] Update_Employee_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_employee_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@employee_id", request.Employee_id);
                    parameters.Add("@desg_id", request.Desg_id);
                    parameters.Add("@first_name", request.First_name);
                    parameters.Add("@last_name", request.Last_name);
                    parameters.Add("@gender", request.Gender);
                    parameters.Add("@dob", request.Dob);
                    parameters.Add("@phone_number", request.Phone_number);
                    parameters.Add("@emailid", request.Emailid);
                    parameters.Add("@address", request.Address);
                    parameters.Add("@city_id", request.City_id);
                    parameters.Add("@aadhaar_number", request.Aadhaar_number);
                    parameters.Add("@pan_number", request.Pan_number);
                    parameters.Add("@bankaccount_no", request.Bankaccount_no);
                    parameters.Add("@ifsc_code", request.Ifsc_code);
                    parameters.Add("@joining_date", request.Joining_date);
                    parameters.Add("@relieving_date", request.Relieving_date);
                    parameters.Add("@education", request.Education);
                    parameters.Add("@exp_year", request.Exp_year);
                    parameters.Add("@annual_salary", request.Annual_salary);
                    parameters.Add("@active_status", request.Active_status);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@comp_id", request.Comp_id);
                    parameters.Add("@user_id", request.User_id);

                    rows_affected = await connection.ExecuteAsync(spname, parameters, commandType: CommandType.StoredProcedure);
                }

                if (rows_affected == 0)
                    return NotFound($"Employee with ID {request.Employee_id} not found");
                else
                    return Ok(new { message = "Employee updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("employee_list")]
        public async Task<ActionResult<IEnumerable<Employee_List>>> GetEmployeeList()
        {
            var employee_list = new List<Employee_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_employee_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var emp = new Employee_List
                                {
                                    Employee_id = reader.GetInt64(0),
                                    Desg_name = reader.GetString(1),
                                    First_name = reader.GetString(2),
                                    Last_name = reader.GetString(3),
                                    Gender = reader.GetString(4),
                                    Dob = reader.IsDBNull(5) ? "" : reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    Phone_number = reader.GetString(6),
                                    Emailid = reader.GetString(7),
                                    Address = reader.GetString(8),
                                    City_name = reader.GetString(9),
                                    Aadhaar_number = reader.GetString(10),
                                    Pan_number = reader.GetString(11),
                                    Bankaccount_no = reader.GetString(12),
                                    Ifsc_code = reader.GetString(13),
                                    Joining_date = reader.IsDBNull(14) ? "" : reader.GetDateTime(14).ToString("yyyy-MM-dd"),
                                    Relieving_date = reader.IsDBNull(15) ? "" : reader.GetDateTime(15).ToString("yyyy-MM-dd"),
                                    Education = reader.GetString(16),
                                    Exp_year = reader.GetDecimal(17),
                                    Annual_salary = reader.GetDecimal(18),
                                    Active_status = reader.GetBoolean(19),
                                    Created_date = reader.IsDBNull(20) ? "" : reader.GetDateTime(20).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(21) ? "" : reader.GetDateTime(21).ToString("yyyy-MM-dd"),
                                    Fin_year_name = reader.GetString(22),
                                    Comp_name = reader.GetString(23),
                                    User_name = reader.IsDBNull(24) ? "" : reader.GetString(24),
                                };

                                employee_list.Add(emp);
                            }
                        }
                    }
                }

                return Ok(employee_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("employee/{id}")]
        public async Task<ActionResult<Single_Employee_>> GetEmployeeById(long id)
        {
            Single_Employee_? emp = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_employee_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@employee_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                emp = new Single_Employee_
                                {
                                    Employee_id = reader.GetInt64(0),
                                    Desg_id = reader.GetInt64(1),
                                    First_name = reader.GetString(2),
                                    Last_name = reader.GetString(3),
                                    Gender = reader.GetString(4),
                                    Dob = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                                    Phone_number = reader.GetString(6),
                                    Emailid = reader.GetString(7),
                                    Address = reader.GetString(8),
                                    City_id = reader.GetInt64(9),
                                    Aadhaar_number = reader.GetString(10),
                                    Pan_number = reader.GetString(11),
                                    Bankaccount_no = reader.GetString(12),
                                    Ifsc_code = reader.GetString(13),
                                    Joining_date = reader.IsDBNull(14) ? null : reader.GetDateTime(14),
                                    Relieving_date = reader.IsDBNull(15) ? null : reader.GetDateTime(15),
                                    Education = reader.GetString(16),
                                    Exp_year = reader.GetDecimal(17),
                                    Annual_salary = reader.GetDecimal(18),
                                    Active_status = reader.GetBoolean(19),
                                    Created_date = reader.IsDBNull(20) ? null : reader.GetDateTime(20),
                                    Updated_date = reader.IsDBNull(21) ? null : reader.GetDateTime(21),
                                    Fin_year_id = reader.GetInt64(22),
                                    Comp_id = reader.GetInt64(23),
                                    User_id = reader.GetInt64(24),
                                };
                            }
                        }
                    }
                }

                if (emp == null)
                    return NotFound($"Employee with ID {id} not found");

                return Ok(emp);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("dropdown_employee_list")]
        public async Task<ActionResult<IEnumerable<Drop_Employee_>>> GetDropEmployeeList()
        {
            var emp_list = new List<Drop_Employee_>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_employee_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "employee_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var emp = new Drop_Employee_
                                {
                                    Employee_id = reader.GetInt64(0),
                                    First_name = reader.GetString(1)
                                };

                                emp_list.Add(emp);
                            }
                        }
                    }
                }

                return Ok(emp_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        public class Add_Employee_Request
        {
            public long Employee_id { get; set; } = 0;
            public long Desg_id { get; set; } = 0;
            public string First_name { get; set; } = "";
            public string Last_name { get; set; } = "";
            public string Gender { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Phone_number { get; set; } = "";
            public string Emailid { get; set; } = "";
            public string Address { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Aadhaar_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Bankaccount_no { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public DateTime? Joining_date { get; set; }
            public DateTime? Relieving_date { get; set; }
            public string Education { get; set; } = "";
            public decimal Exp_year { get; set; } = 0;
            public decimal Annual_salary { get; set; } = 0;
            public bool Active_status { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }


        public class Update_Employee_Request
        {
            public long Employee_id { get; set; } = 0;
            public long Desg_id { get; set; } = 0;
            public string First_name { get; set; } = "";
            public string Last_name { get; set; } = "";
            public string Gender { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Phone_number { get; set; } = "";
            public string Emailid { get; set; } = "";
            public string Address { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Aadhaar_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Bankaccount_no { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public DateTime? Joining_date { get; set; }
            public DateTime? Relieving_date { get; set; }
            public string Education { get; set; } = "";
            public decimal Exp_year { get; set; } = 0;
            public decimal Annual_salary { get; set; } = 0;
            public bool Active_status { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }




        public class Employee_List
        {
            public long Employee_id { get; set; } = 0;
            public string Desg_name { get; set; } = "";
            public string First_name { get; set; } = "";
            public string Last_name { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Dob { get; set; } = "";
            public string Phone_number { get; set; } = "";
            public string Emailid { get; set; } = "";
            public string Address { get; set; } = "";
            public string City_name { get; set; } = "";
            public string Aadhaar_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Bankaccount_no { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public string Joining_date { get; set; } = "";
            public string Relieving_date { get; set; } = "";
            public string Education { get; set; } = "";
            public decimal Exp_year { get; set; } = 0;
            public decimal Annual_salary { get; set; } = 0;
            public bool Active_status { get; set; } = false;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string Fin_year_name { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string User_name { get; set; } = "";
        }




        public class Single_Employee_
        {
            public long Employee_id { get; set; } = 0;
            public long Desg_id { get; set; } = 0;
            public string First_name { get; set; } = "";
            public string Last_name { get; set; } = "";
            public string Gender { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Phone_number { get; set; } = "";
            public string Emailid { get; set; } = "";
            public string Address { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Aadhaar_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Bankaccount_no { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public DateTime? Joining_date { get; set; }
            public DateTime? Relieving_date { get; set; }
            public string Education { get; set; } = "";
            public decimal Exp_year { get; set; } = 0;
            public decimal Annual_salary { get; set; } = 0;
            public bool Active_status { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }



        public class Drop_Employee_
        {
            public long Employee_id { get; set; } = 0;
            public string First_name { get; set; } = "";
        }


    }
}
