using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptController : Controller
    {
        private readonly IConfiguration _configuration;

        public ReceiptController(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        [HttpPost("insert_receipt")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddReceipt([FromBody] AddReceiptRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_receipt_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@receipt_id", 0);
                        command.Parameters.AddWithValue("@sales_id", request.Sales_id);
                        command.Parameters.AddWithValue("@recepit_date", request.Recepit_date);
                        command.Parameters.AddWithValue("@net_total", request.Net_total);
                        command.Parameters.AddWithValue("@balance_amount", request.Balance_amount);
                        command.Parameters.AddWithValue("@receipt_status", request.Receipt_status);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_Year_Id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_Id);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Receipt added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Receipt." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Receipt already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_receipt/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteReceipt(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_receipt_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@receipt_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Receipt deleted successfully." });
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




        [HttpPost("update_receipt")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateReceipt([FromBody] UpdateReceiptRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_receipt_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@receipt_id", request.Receipt_id);
                    parameters.Add("@sales_id", request.Sales_id);
                    parameters.Add("@recepit_date", request.Recepit_date);
                    parameters.Add("@net_total", request.Net_total);
                    parameters.Add("@balance_amount", request.Balance_amount);
                    parameters.Add("@receipt_status", request.Receipt_status);
                    parameters.Add("@fin_year_id", request.Fin_Year_Id);
                    parameters.Add("@comp_id", request.Comp_Id);
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
                    return NotFound($"Receipt with ID {request.Receipt_id} not found");
                else
                    return Ok(new { message = "Receipt updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        [HttpGet("receipt_list")]
        public async Task<ActionResult<IEnumerable<Receipt_List>>> GetReceiptList()
        {
            var receipt_list = new List<Receipt_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_receipt_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var receipt = new Receipt_List
                                {
                                    Receipt_id = reader.GetInt64(0),
                                    Sales_id = reader.GetInt64(1),
                                    Recepit_date = reader.GetDateTime(2),
                                    Net_total = reader.GetDecimal(3),
                                    Balance_amount = reader.GetDecimal(4),
                                    Receipt_status = reader.GetBoolean(5),
                                    Fin_Year_Name = reader.GetString(6),
                                    Comp_Name = reader.GetString(7),
                                    Created_Date = reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(9) ? "" : reader.GetDateTime(9).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(10) ? "" : reader.GetString(10)
                                };

                                receipt_list.Add(receipt);
                            }
                        }
                    }
                }

                return Ok(receipt_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("receipt/{id}")]
        public async Task<ActionResult<SingleReceiptinvoice>> GetReceiptById(long id)
        {
            SingleReceiptinvoice? receipt = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_receipt_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@receipt_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                receipt = new SingleReceiptinvoice
                                {
                                    Receipt_id = reader.GetInt64(0),
                                    Sales_id = reader.GetInt64(1),
                                    Recepit_date = reader.GetDateTime(2),
                                    Net_total = reader.GetDecimal(3),
                                    Balance_amount = reader.GetDecimal(4),
                                    Receipt_status = reader.GetBoolean(5),
                                    Fin_Year_Id = reader.GetInt64(6),
                                    Comp_Id = reader.GetInt64(7),
                                    Created_Date = reader.GetDateTime(8),
                                    Updated_Date = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                                    User_Id = reader.IsDBNull(10) ? 0 : reader.GetInt64(10)
                                };
                            }
                        }
                    }
                }

                if (receipt == null)
                    return NotFound($"Receipt with ID {id} not found");

                return Ok(receipt);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("dropdown_receipt_list")]
        public async Task<ActionResult<IEnumerable<Drop_Receipt_List>>> GetDropReceiptList()
        {
            var receipt_list = new List<Drop_Receipt_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_receipt_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "receiptlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var receipt = new Drop_Receipt_List
                                {
                                    Receipt_id = reader.GetInt64(0),
                                    Sales_id = reader.GetInt64(1)
                                };

                                receipt_list.Add(receipt);
                            }
                        }
                    }
                }

                return Ok(receipt_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpPost("insert_receipt_dtl")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddReceiptDtl([FromBody] Add_receipt_dtl_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_receipt_details_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@receipt_dtl_id", 0);
                        command.Parameters.AddWithValue("@receipt_id", request.Receipt_id);
                        command.Parameters.AddWithValue("@paytype_id", request.Paytype_id);
                        command.Parameters.AddWithValue("@total_amt", request.Total_amt);
                        command.Parameters.AddWithValue("@cheque_number", request.Cheque_number);
                        command.Parameters.AddWithValue("@cheque_bankname", request.Cheque_bankname);
                        command.Parameters.AddWithValue("@ifsc_code", request.Ifsc_code);
                        command.Parameters.AddWithValue("@cheque_date", request.Cheque_date);
                        command.Parameters.AddWithValue("@account_number", request.Account_number);
                        command.Parameters.AddWithValue("@transaction_id", request.Transaction_id);
                        command.Parameters.AddWithValue("@card_number", request.Card_number);
                        command.Parameters.AddWithValue("@transaction_date", request.Transaction_date);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rows = await command.ExecuteNonQueryAsync();

                        if (rows > 0)
                            return Ok(new { message = "Receipt Detail added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add Receipt Detail." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("delete_receipt_dtl/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteReceiptDtl(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_receipt_details_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@receipt_dtl_id", id);

                        int rows = await command.ExecuteNonQueryAsync();

                        if (rows > 0)
                            return Ok(new { message = "Receipt Detail deleted successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "No record deleted." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpPost("update_receipt_dtl")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateReceiptDtl([FromBody] Update_receipt_dtl_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_receipt_details_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@receipt_dtl_id", request.Receipt_dtl_id);
                    parameters.Add("@receipt_id", request.Receipt_id);
                    parameters.Add("@paytype_id", request.Paytype_id);
                    parameters.Add("@total_amt", request.Total_amt);
                    parameters.Add("@cheque_number", request.Cheque_number);
                    parameters.Add("@cheque_bankname", request.Cheque_bankname);
                    parameters.Add("@ifsc_code", request.Ifsc_code);
                    parameters.Add("@cheque_date", request.Cheque_date);
                    parameters.Add("@account_number", request.Account_number);
                    parameters.Add("@transaction_id", request.Transaction_id);
                    parameters.Add("@card_number", request.Card_number);
                    parameters.Add("@transaction_date", request.Transaction_date);
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@comp_id", request.Comp_id);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_id);

                    rows_affected = await connection.ExecuteAsync(spname, parameters, commandType: CommandType.StoredProcedure);
                }

                if (rows_affected == 0)
                    return NotFound($"Receipt Detail with ID {request.Receipt_dtl_id} not found");
                else
                    return Ok(new { message = "Receipt Detail updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("receipt_dtl_list")]
        public async Task<ActionResult<IEnumerable<receipt_dtl_List>>> Get_receipt_dtl_list()
        {
            var list = new List<receipt_dtl_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_receipt_details_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new receipt_dtl_List
                                {
                                    Receipt_dtl_id = reader.GetInt64(0),
                                    Receipt_id = reader.GetInt64(1),
                                    Paytype_name = reader.GetString(2),
                                    Total_amt = reader.GetDecimal(3),
                                    Cheque_number = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Cheque_bankname = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    Ifsc_code = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                    Cheque_date = reader.IsDBNull(7) ? "" : reader.GetDateTime(7).ToString("yyyy-MM-dd"),
                                    Account_number = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                    Transaction_id = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                    Card_number = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                    Transaction_date = reader.IsDBNull(11) ? "" : reader.GetDateTime(11).ToString("yyyy-MM-dd"),
                                    Fin_Year_Name = reader.GetString(12),
                                    Comp_Name = reader.GetString(13),
                                    Created_Date = reader.GetDateTime(14).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(15) ? "" : reader.GetDateTime(15).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(16) ? "" : reader.GetString(16)
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





        [HttpGet("receipt_dtl/{id}")]
        public async Task<ActionResult<Single_receipt_dtl>> Get_receipt_dtl_by_id(long id)
        {
            Single_receipt_dtl? item = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_receipt_details_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@receipt_dtl_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                item = new Single_receipt_dtl
                                {
                                    Receipt_dtl_id = reader.GetInt64(0),
                                    Receipt_id = reader.GetInt64(1),
                                    Paytype_id = reader.GetInt64(2),
                                    Total_amt = reader.GetDecimal(3),
                                    Cheque_number = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Cheque_bankname = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    Ifsc_code = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                    Cheque_date = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                                    Account_number = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                    Transaction_id = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                    Card_number = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                    Transaction_date = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                                    Fin_year_id = reader.GetInt64(12),
                                    Comp_id = reader.GetInt64(13),
                                    Created_date = reader.GetDateTime(14),
                                    Updated_date = reader.IsDBNull(15) ? null : reader.GetDateTime(15),
                                    User_id = reader.GetInt64(16)
                                };
                            }
                        }
                    }
                }

                if (item == null)
                    return NotFound($"Receipt Detail with ID {id} not found");

                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        //[HttpGet("dropdown_receipt_dtl_list")]
        //public async Task<ActionResult<IEnumerable<Drop_receipt_dtl>>> Get_drop_receipt_dtl_list()
        //{
        //    var list = new List<Drop_receipt_dtl>();
        //    var connectionstring = _configuration.GetConnectionString("DefaultConnection");

        //    try
        //    {
        //        using (var connection = new SqlConnection(connectionstring))
        //        {
        //            await connection.OpenAsync();

        //            using (var command = new SqlCommand("sp_receipt_details_ins_upd_del", connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddWithValue("@action", "receipt_dtl_mastlist");

        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        list.Add(new Drop_receipt_dtl
        //                        {
        //                            Receipt_dtl_id = reader.GetInt64(0)
        //                        });
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


        public class AddReceiptRequest
        {

            public long Receipt_id { get; set; } = 0;
            public long Sales_id { get; set; } = 0;
            public DateTime Recepit_date { get; set; }
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_amount { get; set; } = 0;
            public bool Receipt_status { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class UpdateReceiptRequest
        {
            public long Receipt_id { get; set; } = 0;
            public long Sales_id { get; set; } = 0;
            public DateTime Recepit_date { get; set; }
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_amount { get; set; } = 0;
            public bool Receipt_status { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class Receipt_List
        {
            public long Receipt_id { get; set; } = 0;
            public long Sales_id { get; set; } = 0;
            public DateTime Recepit_date { get; set; }
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_amount { get; set; } = 0;
            public bool Receipt_status { get; set; } = false;
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }

        public class SingleReceiptinvoice
        {
            public long Receipt_id { get; set; } = 0;
            public long Sales_id { get; set; } = 0;
            public DateTime Recepit_date { get; set; }
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_amount { get; set; } = 0;
            public bool Receipt_status { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_Receipt_List
        {
            public long Receipt_id { get; set; } = 0;
            public long Sales_id { get; set; } = 0;
        }

        public class Add_receipt_dtl_Request
        {
            public long Receipt_dtl_id { get; set; } = 0;
            public long Receipt_id { get; set; } = 0;
            public long Paytype_id { get; set; } = 0;
            public decimal Total_amt { get; set; } = 0;
            public string Cheque_number { get; set; } = "";
            public string Cheque_bankname { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public DateTime? Cheque_date { get; set; }
            public string Account_number { get; set; } = "";
            public string Transaction_id { get; set; } = "";
            public string Card_number { get; set; } = "";
            public DateTime? Transaction_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }



        public class Update_receipt_dtl_Request
        {
            public long Receipt_dtl_id { get; set; } = 0;
            public long Receipt_id { get; set; } = 0;
            public long Paytype_id { get; set; } = 0;
            public decimal Total_amt { get; set; } = 0;
            public string Cheque_number { get; set; } = "";
            public string Cheque_bankname { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public DateTime? Cheque_date { get; set; }
            public string Account_number { get; set; } = "";
            public string Transaction_id { get; set; } = "";
            public string Card_number { get; set; } = "";
            public DateTime? Transaction_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }


        public class receipt_dtl_List
        {
            public long Receipt_dtl_id { get; set; } = 0;
            public long Receipt_id { get; set; } = 0;
            public string Paytype_name { get; set; } = "";
            public decimal Total_amt { get; set; } = 0;
            public string Cheque_number { get; set; } = "";
            public string Cheque_bankname { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public string Cheque_date { get; set; } = "";
            public string Account_number { get; set; } = "";
            public string Transaction_id { get; set; } = "";
            public string Card_number { get; set; } = "";
            public string Transaction_date { get; set; } = "";
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }



        public class Single_receipt_dtl
        {
            public long Receipt_dtl_id { get; set; } = 0;
            public long Receipt_id { get; set; } = 0;
            public long Paytype_id { get; set; } = 0;
            public decimal Total_amt { get; set; } = 0;
            public string Cheque_number { get; set; } = "";
            public string Cheque_bankname { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public DateTime? Cheque_date { get; set; }
            public string Account_number { get; set; } = "";
            public string Transaction_id { get; set; } = "";
            public string Card_number { get; set; } = "";
            public DateTime? Transaction_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }




        public class Drop_receipt_dtl
        {
            public long Receipt_dtl_id { get; set; } = 0;
        }


    }
}
