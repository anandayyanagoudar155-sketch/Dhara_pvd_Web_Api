using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        private readonly IConfiguration _configuration;

        public PaymentController(IConfiguration configuration)
        {

            _configuration = configuration;

        }

        [HttpPost("insert_payment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPayment([FromBody] Add_Payment_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_payment_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@payment_id", request.Payment_id);
                        command.Parameters.AddWithValue("@trans_id", request.Trans_id);
                        command.Parameters.AddWithValue("@purchase_id", request.Purchase_id);
                        command.Parameters.AddWithValue("@emp_payslip_id", request.Emp_payslip_id);
                        command.Parameters.AddWithValue("@payment_date", request.Payment_date);
                        command.Parameters.AddWithValue("@net_total", request.Net_total);
                        command.Parameters.AddWithValue("@balance_total", request.Balance_total);
                        command.Parameters.AddWithValue("@payment_status", request.Payment_status);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Payment added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Payment." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_payment/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePayment(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_payment_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@payment_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Payment deleted successfully." });
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


        [HttpPost("update_payment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdatePayment([FromBody] Update_Payment_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_payment_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@payment_id", request.Payment_id);
                    parameters.Add("@trans_id", request.Trans_id);
                    parameters.Add("@purchase_id", request.Purchase_id);
                    parameters.Add("@emp_payslip_id", request.Emp_payslip_id);
                    parameters.Add("@payment_date", request.Payment_date);
                    parameters.Add("@net_total", request.Net_total);
                    parameters.Add("@balance_total", request.Balance_total);
                    parameters.Add("@payment_status", request.Payment_status);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@comp_id", request.Comp_id);
                    parameters.Add("@user_id", request.User_id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"Payment with ID {request.Payment_id} not found");
                else
                    return Ok(new { message = "Payment updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("payment_list")]
        public async Task<ActionResult<IEnumerable<Payment_List>>> Get_payment_list()
        {
            var payment_list = new List<Payment_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_payment_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var payment = new Payment_List
                                {
                                    Payment_id = reader.GetInt64(0),
                                    Trans_id = reader.GetInt64(1),
                                    Purchase_id = reader.GetInt64(2),
                                    Emp_payslip_id = reader.GetInt64(3),
                                    Payment_date = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    Net_total = reader.GetDecimal(5),
                                    Balance_total = reader.GetDecimal(6),
                                    Payment_status = reader.GetBoolean(7),
                                    Created_date = reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(9) ? "" : reader.GetDateTime(9).ToString("yyyy-MM-dd"),
                                    Fin_year_name = reader.GetString(10),
                                    Comp_name = reader.GetString(11),
                                    User_name = reader.IsDBNull(12) ? "" : reader.GetString(12)
                                };

                                payment_list.Add(payment);
                            }
                        }
                    }
                }

                return Ok(payment_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("payment/{id}")]
        public async Task<ActionResult<Single_Payment>> Get_payment_by_id(long id)
        {
            Single_Payment? payment = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_payment_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@payment_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                payment = new Single_Payment
                                {
                                    Payment_id = reader.GetInt64(0),
                                    Trans_id = reader.GetInt64(1),
                                    Purchase_id = reader.GetInt64(2),
                                    Emp_payslip_id = reader.GetInt64(3),
                                    Payment_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                    Net_total = reader.GetDecimal(5),
                                    Balance_total = reader.GetDecimal(6),
                                    Payment_status = reader.GetBoolean(7),
                                    Created_date = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                                    Updated_date = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                                    Fin_year_id = reader.GetInt64(10),
                                    Comp_id = reader.GetInt64(11),
                                    User_id = reader.IsDBNull(12) ? 0 : reader.GetInt64(12)
                                };
                            }
                        }
                    }
                }

                if (payment == null)
                    return NotFound($"Payment with ID {id} not found");

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("dropdown_payment_list")]
        public async Task<ActionResult<IEnumerable<Drop_Payment>>> Get_drop_payment_list()
        {
            var payment_list = new List<Drop_Payment>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_payment_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "payment_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var payment = new Drop_Payment
                                {
                                    Payment_id = reader.GetInt64(0),
                                    User_id = reader.GetInt64(1)
                                };

                                payment_list.Add(payment);
                            }
                        }
                    }
                }

                return Ok(payment_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpPost("insert_paymentdetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPaymentDetail([FromBody] Add_PaymentDetail_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_payment_details_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@payment_detail_id", request.Payment_detail_id);
                        command.Parameters.AddWithValue("@payment_id", request.Payment_id);
                        command.Parameters.AddWithValue("@pay_type_id", request.Pay_type_id);
                        command.Parameters.AddWithValue("@total_amt", request.Total_amt);
                        command.Parameters.AddWithValue("@cheque_number", request.Cheque_number);
                        command.Parameters.AddWithValue("@cheque_bankname", request.Cheque_bankname);
                        command.Parameters.AddWithValue("@ifsc_code", request.Ifsc_code);
                        command.Parameters.AddWithValue("@cheque_date", request.Cheque_date);
                        command.Parameters.AddWithValue("@account_number", request.Account_number);
                        command.Parameters.AddWithValue("@transaction_id", request.Transaction_id);
                        command.Parameters.AddWithValue("@card_number", request.Card_number);
                        command.Parameters.AddWithValue("@transaction_date", request.Transaction_date);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Payment Detail Added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add Payment Detail." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Duplicate payment detail exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }




        [HttpDelete("delete_paymentdetail/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePaymentDetail(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_payment_details_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@payment_detail_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Payment Detail deleted successfully." });
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




        [HttpPost("update_paymentdetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdatePaymentDetail([FromBody] Update_PaymentDetail_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_payment_details_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@payment_detail_id", request.Payment_detail_id);
                    parameters.Add("@payment_id", request.Payment_id);
                    parameters.Add("@pay_type_id", request.Pay_type_id);
                    parameters.Add("@total_amt", request.Total_amt);
                    parameters.Add("@cheque_number", request.Cheque_number);
                    parameters.Add("@cheque_bankname", request.Cheque_bankname);
                    parameters.Add("@ifsc_code", request.Ifsc_code);
                    parameters.Add("@cheque_date", request.Cheque_date);
                    parameters.Add("@account_number", request.Account_number);
                    parameters.Add("@transaction_id", request.Transaction_id);
                    parameters.Add("@card_number", request.Card_number);
                    parameters.Add("@transaction_date", request.Transaction_date);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@comp_id", request.Comp_id);
                    parameters.Add("@user_id", request.User_id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"Payment Detail with ID {request.Payment_detail_id} not found");
                else
                    return Ok(new { message = "Payment Detail updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("paymentdetail_list")]
        public async Task<ActionResult<IEnumerable<PaymentDetail_List>>> Get_PaymentDetail_List()
        {
            var list = new List<PaymentDetail_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_payment_details_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new PaymentDetail_List
                                {
                                    Payment_detail_id = reader.GetInt64(0),
                                    Payment_id = reader.GetInt64(1),
                                    Pay_type_name = reader.GetString(2),
                                    Total_amt = reader.GetDecimal(3),
                                    Cheque_number = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Cheque_bankname = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    Ifsc_code = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                    Cheque_date = reader.IsDBNull(7) ? "" : reader.GetDateTime(7).ToString("yyyy-MM-dd"),
                                    Account_number = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                    Transaction_id = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                    Card_number = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                    Transaction_date = reader.IsDBNull(11) ? "" : reader.GetDateTime(11).ToString("yyyy-MM-dd"),
                                    Created_date = reader.IsDBNull(12) ? "" : reader.GetDateTime(12).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(13) ? "" : reader.GetDateTime(13).ToString("yyyy-MM-dd"),
                                    Fin_year_name = reader.GetString(14),
                                    Comp_name = reader.GetString(15),
                                    User_name = reader.GetString(16)
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



        [HttpGet("paymentdetail/{id}")]
        public async Task<ActionResult<Single_PaymentDetail>> Get_PaymentDetail_By_Id(long id)
        {
            Single_PaymentDetail? detail = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_payment_details_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@payment_detail_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                detail = new Single_PaymentDetail
                                {
                                    Payment_detail_id = reader.GetInt64(0),
                                    Payment_id = reader.GetInt64(1),
                                    Pay_type_id = reader.GetInt64(2),
                                    Total_amt = reader.GetDecimal(3),
                                    Cheque_number = reader.GetString(4),
                                    Cheque_bankname = reader.GetString(5),
                                    Ifsc_code = reader.GetString(6),
                                    Cheque_date = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                                    Account_number = reader.GetString(8),
                                    Transaction_id = reader.GetString(9),
                                    Card_number = reader.GetString(10),
                                    Transaction_date = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                                    Created_date = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                                    Updated_date = reader.IsDBNull(13) ? null : reader.GetDateTime(13),
                                    Fin_year_id = reader.GetInt64(14),
                                    Comp_id = reader.GetInt64(15),
                                    User_id = reader.GetInt64(16),
                                };
                            }
                        }
                    }
                }

                if (detail == null)
                    return NotFound($"Payment Detail with ID {id} not found");

                return Ok(detail);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        //[HttpGet("dropdown_paymentdetail_list")]
        //public async Task<ActionResult<IEnumerable<Drop_PaymentDetail>>> Get_Dropdown_PaymentDetail_List()
        //{
        //    var list = new List<Drop_PaymentDetail>();
        //    var connectionstring = _configuration.GetConnectionString("DefaultConnection");

        //    try
        //    {
        //        using (var connection = new SqlConnection(connectionstring))
        //        {
        //            string spName = "sp_payment_details_ins_upd_del";

        //            await connection.OpenAsync();

        //            using (var command = new SqlCommand(spName, connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddWithValue("@action", "paymentdetail_mastlist");

        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var item = new Drop_PaymentDetail
        //                        {
        //                            Payment_detail_id = reader.GetInt64(0),
        //                            User_id = reader.GetInt64(1)
        //                        };

        //                        list.Add(item);
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

        public class Add_Payment_Request
        {
            public long Payment_id { get; set; } = 0;
            public long Trans_id { get; set; } = 0;
            public long Purchase_id { get; set; } = 0;
            public long Emp_payslip_id { get; set; } = 0;
            public DateTime? Payment_date { get; set; }
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }

        public class Update_Payment_Request
        {
            public long Payment_id { get; set; } = 0;
            public long Trans_id { get; set; } = 0;
            public long Purchase_id { get; set; } = 0;
            public long Emp_payslip_id { get; set; } = 0;
            public DateTime? Payment_date { get; set; }
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }

        public class Payment_List
        {
            public long Payment_id { get; set; } = 0;
            public long Trans_id { get; set; } = 0;
            public long Purchase_id { get; set; } = 0;
            public long Emp_payslip_id { get; set; } = 0;
            public string Payment_date { get; set; } = "";
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string Fin_year_name { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string User_name { get; set; } = "";
        }

        public class Single_Payment
        {
            public long Payment_id { get; set; } = 0;
            public long Trans_id { get; set; } = 0;
            public long Purchase_id { get; set; } = 0;
            public long Emp_payslip_id { get; set; } = 0;
            public DateTime? Payment_date { get; set; }
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }

        public class Drop_Payment
        {
            public long Payment_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }


        public class Add_PaymentDetail_Request
        {
            public long Payment_detail_id { get; set; } = 0;
            public long Payment_id { get; set; } = 0;
            public long Pay_type_id { get; set; } = 0;
            public decimal Total_amt { get; set; } = 0;
            public string Cheque_number { get; set; } = "";
            public string Cheque_bankname { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public DateTime? Cheque_date { get; set; } = null;
            public string Account_number { get; set; } = "";
            public string Transaction_id { get; set; } = "";
            public string Card_number { get; set; } = "";
            public DateTime? Transaction_date { get; set; } = null;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }



        public class Update_PaymentDetail_Request
        {
            public long Payment_detail_id { get; set; } = 0;
            public long Payment_id { get; set; } = 0;
            public long Pay_type_id { get; set; } = 0;
            public decimal Total_amt { get; set; } = 0;
            public string Cheque_number { get; set; } = "";
            public string Cheque_bankname { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public DateTime? Cheque_date { get; set; } = null;
            public string Account_number { get; set; } = "";
            public string Transaction_id { get; set; } = "";
            public string Card_number { get; set; } = "";
            public DateTime? Transaction_date { get; set; } = null;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }



        public class PaymentDetail_List
        {
            public long Payment_detail_id { get; set; } = 0;
            public long Payment_id { get; set; } = 0;
            public string Pay_type_name { get; set; } = "";
            public decimal Total_amt { get; set; } = 0;
            public string Cheque_number { get; set; } = "";
            public string Cheque_bankname { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public string Cheque_date { get; set; } = "";
            public string Account_number { get; set; } = "";
            public string Transaction_id { get; set; } = "";
            public string Card_number { get; set; } = "";
            public string Transaction_date { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string Fin_year_name { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string User_name { get; set; } = "";
        }



        public class Single_PaymentDetail
        {
            public long Payment_detail_id { get; set; } = 0;
            public long Payment_id { get; set; } = 0;
            public long Pay_type_id { get; set; } = 0;
            public decimal Total_amt { get; set; } = 0;
            public string Cheque_number { get; set; } = "";
            public string Cheque_bankname { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public DateTime? Cheque_date { get; set; } = null;
            public string Account_number { get; set; } = "";
            public string Transaction_id { get; set; } = "";
            public string Card_number { get; set; } = "";
            public DateTime? Transaction_date { get; set; } = null;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }



        public class Drop_PaymentDetail
        {
            public long Payment_detail_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }


    }
}
