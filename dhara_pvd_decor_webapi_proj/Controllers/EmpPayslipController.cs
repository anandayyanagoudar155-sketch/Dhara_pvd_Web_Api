using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpPayslipController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmpPayslipController(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        [HttpPost("insert_emp_payslip")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddEmpPayslip([FromBody] AddEmpPayslipRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_employee_payslip_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@emp_payslip_id", request.Emp_payslip_id);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                        command.Parameters.AddWithValue("@month_id", request.Month_id);
                        command.Parameters.AddWithValue("@company_id", request.Company_id);
                        command.Parameters.AddWithValue("@employee_id", request.Employee_id);
                        command.Parameters.AddWithValue("@emp_calender_id", request.Emp_calender_id);
                        command.Parameters.AddWithValue("@employee_working_days", request.Employee_working_days);
                        command.Parameters.AddWithValue("@actual_working_days", request.Actual_working_days);
                        command.Parameters.AddWithValue("@actual_working_hours", request.Actual_working_hours);
                        command.Parameters.AddWithValue("@monthly_salary", request.Monthly_salary);
                        command.Parameters.AddWithValue("@hourly_salary", request.Hourly_salary);
                        command.Parameters.AddWithValue("@over_time_hours", request.Over_time_hours);
                        command.Parameters.AddWithValue("@over_time_amount", request.Over_time_amount);
                        command.Parameters.AddWithValue("@gross_amount", request.Gross_amount);
                        command.Parameters.AddWithValue("@pf_amount", request.Pf_amount);
                        command.Parameters.AddWithValue("@tds_amount", request.Tds_amount);
                        command.Parameters.AddWithValue("@advance_amount", request.Advance_amount);
                        command.Parameters.AddWithValue("@netamount", request.Netamount);
                        command.Parameters.AddWithValue("@remarks", request.Remarks);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@updated_date", request.Updated_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Emp Payslip added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add Emp Payslip." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }





        [HttpDelete("delete_emp_payslip/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmpPayslip(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_employee_payslip_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@emp_payslip_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Emp Payslip deleted successfully." });
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





        [HttpPost("update_emp_payslip")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEmpPayslip([FromBody] UpdateEmpPayslipRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_employee_payslip_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@emp_payslip_id", request.Emp_payslip_id);
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@month_id", request.Month_id);
                    parameters.Add("@company_id", request.Company_id);
                    parameters.Add("@employee_id", request.Employee_id);
                    parameters.Add("@emp_calender_id", request.Emp_calender_id);
                    parameters.Add("@employee_working_days", request.Employee_working_days);
                    parameters.Add("@actual_working_days", request.Actual_working_days);
                    parameters.Add("@actual_working_hours", request.Actual_working_hours);
                    parameters.Add("@monthly_salary", request.Monthly_salary);
                    parameters.Add("@hourly_salary", request.Hourly_salary);
                    parameters.Add("@over_time_hours", request.Over_time_hours);
                    parameters.Add("@over_time_amount", request.Over_time_amount);
                    parameters.Add("@gross_amount", request.Gross_amount);
                    parameters.Add("@pf_amount", request.Pf_amount);
                    parameters.Add("@tds_amount", request.Tds_amount);
                    parameters.Add("@advance_amount", request.Advance_amount);
                    parameters.Add("@netamount", request.Netamount);
                    parameters.Add("@remarks", request.Remarks);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_id);

                    rows_affected = await connection.ExecuteAsync(spname, parameters,
                        commandType: CommandType.StoredProcedure);
                }

                if (rows_affected == 0)
                    return NotFound($"Emp Payslip with ID {request.Emp_payslip_id} not found");
                else
                    return Ok(new { message = "Emp Payslip updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("emp_payslip_list")]
        public async Task<ActionResult<IEnumerable<emp_payslip_list>>> Get_emp_payslip_list()
        {
            var list = new List<emp_payslip_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_employee_payslip_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new emp_payslip_list
                                {
                                    Emp_payslip_id = reader.GetInt64(0),
                                    Fin_name = reader.GetString(1),
                                    Month_name = reader.GetString(2),
                                    Company_name = reader.GetString(3),
                                    First_name = reader.GetString(4),
                                    Last_name = reader.GetString(5),
                                    Emp_calender_code = reader.GetString(6),
                                    Employee_working_days = reader.GetDecimal(7),
                                    Actual_working_days = reader.GetDecimal(8),
                                    Actual_working_hours = reader.GetDecimal(9),
                                    Monthly_salary = reader.GetDecimal(10),
                                    Hourly_salary = reader.GetDecimal(11),
                                    Over_time_hours = reader.GetDecimal(12),
                                    Over_time_amount = reader.GetDecimal(13),
                                    Gross_amount = reader.GetDecimal(14),
                                    Pf_amount = reader.GetDecimal(15),
                                    Tds_amount = reader.GetDecimal(16),
                                    Advance_amount = reader.GetDecimal(17),
                                    Netamount = reader.GetDecimal(18),
                                    Remarks = reader.GetString(19),
                                    Created_date = reader.GetDateTime(20).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(21) ? "" : reader.GetDateTime(21).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(22) ? "" : reader.GetString(22)
                                };

                                list.Add(obj);
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




        [HttpGet("emp_payslip/{id}")]
        public async Task<ActionResult<single_emp_payslip_list>> Get_emp_payslip_by_id(long id)
        {
            single_emp_payslip_list? obj = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_employee_payslip_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@emp_payslip_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                obj = new single_emp_payslip_list
                                {
                                    Emp_payslip_id = reader.GetInt64(0),
                                    Fin_year_id = reader.GetInt64(1),
                                    Month_id = reader.GetInt64(2),
                                    Company_id = reader.GetInt64(3),
                                    Employee_id = reader.GetInt64(4),
                                    Emp_calender_id = reader.GetInt64(5),
                                    Employee_working_days = reader.GetDecimal(6),
                                    Actual_working_days = reader.GetDecimal(7),
                                    Actual_working_hours = reader.GetDecimal(8),
                                    Monthly_salary = reader.GetDecimal(9),
                                    Hourly_salary = reader.GetDecimal(10),
                                    Over_time_hours = reader.GetDecimal(11),
                                    Over_time_amount = reader.GetDecimal(12),
                                    Gross_amount = reader.GetDecimal(13),
                                    Pf_amount = reader.GetDecimal(14),
                                    Tds_amount = reader.GetDecimal(15),
                                    Advance_amount = reader.GetDecimal(16),
                                    Netamount = reader.GetDecimal(17),
                                    Remarks = reader.GetString(18),
                                    Created_date = reader.IsDBNull(19) ? null : reader.GetDateTime(19),
                                    Updated_date = reader.IsDBNull(20) ? null : reader.GetDateTime(20),
                                    User_id = reader.GetInt64(21)
                                };
                            }
                        }
                    }
                }

                if (obj == null)
                    return NotFound($"Emp Payslip with ID {id} not found");

                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        //[HttpGet("dropdown_emp_payslip_list")]
        //public async Task<ActionResult<IEnumerable<drop_emp_payslip_list>>> Get_drop_emp_payslip_list()
        //{
        //    var list = new List<drop_emp_payslip_list>();
        //    var connectionstring = _configuration.GetConnectionString("DefaultConnection");

        //    try
        //    {
        //        using (var connection = new SqlConnection(connectionstring))
        //        {
        //            string spName = "sp_employee_payslip_ins_upd_del";

        //            await connection.OpenAsync();

        //            using (var command = new SqlCommand(spName, connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddWithValue("@action", "emp_payslip_mastlist");

        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var obj = new drop_emp_payslip_list
        //                        {
        //                            Emp_payslip_id = reader.GetInt64(0),
        //                            Employee_name = reader.GetString(1)
        //                        };

        //                        list.Add(obj);
        //                    }
        //                }
        //            }
        //        }

        //        return Ok(list);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error: {ex.Message}");
        //    }
        //}





        public class AddEmpPayslipRequest
        {
            public long Emp_payslip_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public long Company_id { get; set; } = 0;
            public long Employee_id { get; set; } = 0;
            public long Emp_calender_id { get; set; } = 0;
            public decimal Employee_working_days { get; set; } = 0;
            public decimal Actual_working_days { get; set; } = 0;
            public decimal Actual_working_hours { get; set; } = 0;
            public decimal Monthly_salary { get; set; } = 0;
            public decimal Hourly_salary { get; set; } = 0;
            public decimal Over_time_hours { get; set; } = 0;
            public decimal Over_time_amount { get; set; } = 0;
            public decimal Gross_amount { get; set; } = 0;
            public decimal Pf_amount { get; set; } = 0;
            public decimal Tds_amount { get; set; } = 0;
            public decimal Advance_amount { get; set; } = 0;
            public decimal Netamount { get; set; } = 0;
            public string Remarks { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }



        public class UpdateEmpPayslipRequest
        {
            public long Emp_payslip_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public long Company_id { get; set; } = 0;
            public long Employee_id { get; set; } = 0;
            public long Emp_calender_id { get; set; } = 0;
            public decimal Employee_working_days { get; set; } = 0;
            public decimal Actual_working_days { get; set; } = 0;
            public decimal Actual_working_hours { get; set; } = 0;
            public decimal Monthly_salary { get; set; } = 0;
            public decimal Hourly_salary { get; set; } = 0;
            public decimal Over_time_hours { get; set; } = 0;
            public decimal Over_time_amount { get; set; } = 0;
            public decimal Gross_amount { get; set; } = 0;
            public decimal Pf_amount { get; set; } = 0;
            public decimal Tds_amount { get; set; } = 0;
            public decimal Advance_amount { get; set; } = 0;
            public decimal Netamount { get; set; } = 0;
            public string Remarks { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }



        public class emp_payslip_list
        {
            public long Emp_payslip_id { get; set; } = 0;
            public string Fin_name { get; set; } = "";
            public string Month_name { get; set; } = "";
            public string Company_name { get; set; } = "";
            public string First_name { get; set; } = "";
            public string Last_name { get; set; } = "";
            public string Emp_calender_code { get; set; } = "";
            public decimal Employee_working_days { get; set; } = 0;
            public decimal Actual_working_days { get; set; } = 0;
            public decimal Actual_working_hours { get; set; } = 0;
            public decimal Monthly_salary { get; set; } = 0;
            public decimal Hourly_salary { get; set; } = 0;
            public decimal Over_time_hours { get; set; } = 0;
            public decimal Over_time_amount { get; set; } = 0;
            public decimal Gross_amount { get; set; } = 0;
            public decimal Pf_amount { get; set; } = 0;
            public decimal Tds_amount { get; set; } = 0;
            public decimal Advance_amount { get; set; } = 0;
            public decimal Netamount { get; set; } = 0;
            public string Remarks { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";
        }


        public class single_emp_payslip_list
        {
            public long Emp_payslip_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public long Company_id { get; set; } = 0;
            public long Employee_id { get; set; } = 0;
            public long Emp_calender_id { get; set; } = 0;
            public decimal Employee_working_days { get; set; } = 0;
            public decimal Actual_working_days { get; set; } = 0;
            public decimal Actual_working_hours { get; set; } = 0;
            public decimal Monthly_salary { get; set; } = 0;
            public decimal Hourly_salary { get; set; } = 0;
            public decimal Over_time_hours { get; set; } = 0;
            public decimal Over_time_amount { get; set; } = 0;
            public decimal Gross_amount { get; set; } = 0;
            public decimal Pf_amount { get; set; } = 0;
            public decimal Tds_amount { get; set; } = 0;
            public decimal Advance_amount { get; set; } = 0;
            public decimal Netamount { get; set; } = 0;
            public string Remarks { get; set; } = "";
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }



        public class drop_emp_payslip_list
        {
            public long Emp_payslip_id { get; set; } = 0;
            public string Employee_name { get; set; } = "";
        }


    }
}
