using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesInvoiceController : Controller
    {
        private readonly IConfiguration _configuration;

        public SalesInvoiceController(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        [HttpPost("insert_sales_invoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AddSalesInvoice([FromBody] AddsalesinvoiceRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_salesinvoice_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@sales_id", request.Sales_id);
                        command.Parameters.AddWithValue("@prefix", request.Prefix);
                        command.Parameters.AddWithValue("@suffix", request.Suffix);
                        command.Parameters.AddWithValue("@customer_id", request.Customer_id);
                        command.Parameters.AddWithValue("@sales_date", request.Sales_date);
                        command.Parameters.AddWithValue("@gross_total", request.Gross_total);
                        command.Parameters.AddWithValue("@sgst_total", request.Sgst_total);
                        command.Parameters.AddWithValue("@cgst_total", request.Cgst_total);
                        command.Parameters.AddWithValue("@igst_total", request.Igst_total);
                        command.Parameters.AddWithValue("@discount_total", request.Discount_total);
                        command.Parameters.AddWithValue("@roundoff_total", request.Roundoff_total);
                        command.Parameters.AddWithValue("@net_total", request.Net_total);
                        command.Parameters.AddWithValue("@balance_total", request.Balance_total);
                        command.Parameters.AddWithValue("@payment_status", request.Payment_status);
                        command.Parameters.AddWithValue("@isactive", request.Isactive);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_Year_Id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_Id);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Sales Invoice Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Sales Invoice." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Sales Invoice already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteSalesInvoice/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSalesInvoice(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_salesinvoice_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@sales_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Sales Invoice deleted successfully." });
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




        [HttpPost("UpdateSalesInvoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateSalesInvoice([FromBody] UpdatesalesinvoiceRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_salesinvoice_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@sales_id", request.Sales_id);
                    parameters.Add("@prefix", request.Prefix);
                    parameters.Add("@suffix", request.Suffix);
                    parameters.Add("@customer_id", request.Customer_id);
                    parameters.Add("@sales_date", request.Sales_date);
                    parameters.Add("@gross_total", request.Gross_total);
                    parameters.Add("@sgst_total", request.Sgst_total);
                    parameters.Add("@cgst_total", request.Cgst_total);
                    parameters.Add("@igst_total", request.Igst_total);
                    parameters.Add("@discount_total", request.Discount_total);
                    parameters.Add("@roundoff_total", request.Roundoff_total);
                    parameters.Add("@net_total", request.Net_total);
                    parameters.Add("@balance_total", request.Balance_total);
                    parameters.Add("@payment_status", request.Payment_status);
                    parameters.Add("@isactive", request.Isactive);
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
                    return NotFound($"Sales Invoice with ID {request.Sales_id} not found");
                else
                    return Ok(new { message = "Sales Invoice updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("salesinvoice_list")]
        public async Task<ActionResult<IEnumerable<salesinvoice_List>>> Get_salesinvoice_list()
        {
            var sales_list = new List<salesinvoice_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_salesinvoice_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var sales = new salesinvoice_List
                                {
                                    Sales_id = reader.GetInt64(0),
                                    Prefix = reader.GetString(1),
                                    Suffix = reader.GetString(2),
                                    Customer_name = reader.GetString(3),
                                    Sales_date = reader.GetDateTime(4),
                                    Gross_total = reader.GetDecimal(5),
                                    Sgst_total = reader.GetDecimal(6),
                                    Cgst_total = reader.GetDecimal(7),
                                    Igst_total = reader.GetDecimal(8),
                                    Discount_total = reader.GetDecimal(9),
                                    Roundoff_total = reader.GetDecimal(10),
                                    Net_total = reader.GetDecimal(11),
                                    Balance_total = reader.GetDecimal(12),
                                    Payment_status = reader.GetBoolean(13),
                                    Isactive = reader.GetBoolean(14),
                                    Fin_Year_Name = reader.IsDBNull(15) ? "" : reader.GetString(15),
                                    Comp_Name = reader.IsDBNull(16) ? "" : reader.GetString(16),
                                    Created_Date = reader.IsDBNull(17) ? "" : reader.GetDateTime(17).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(18) ? "" : reader.GetDateTime(18).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(19) ? "" : reader.GetString(19)
                                };

                                sales_list.Add(sales);
                            }
                        }
                    }
                }

                return Ok(sales_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("salesinvoice/{id}")]
        public async Task<ActionResult<Singlesalesinvoice>> Get_salesinvoice_by_id(long id)
        {
            Singlesalesinvoice? sales = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_salesinvoice_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@sales_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                sales = new Singlesalesinvoice
                                {
                                    Sales_id = reader.GetInt64(0),
                                    Prefix = reader.GetString(1),
                                    Suffix = reader.GetString(2),
                                    Customer_id = reader.GetInt64(3),
                                    Sales_date = reader.GetDateTime(4),
                                    Gross_total = reader.GetDecimal(5),
                                    Sgst_total = reader.GetDecimal(6),
                                    Cgst_total = reader.GetDecimal(7),
                                    Igst_total = reader.GetDecimal(8),
                                    Discount_total = reader.GetDecimal(9),
                                    Roundoff_total = reader.GetDecimal(10),
                                    Net_total = reader.GetDecimal(11),
                                    Balance_total = reader.GetDecimal(12),
                                    Payment_status = reader.GetBoolean(13),
                                    Isactive = reader.GetBoolean(14),
                                    Fin_Year_Id = reader.GetInt64(15),
                                    Comp_Id = reader.GetInt64(16),
                                    Created_Date = reader.IsDBNull(17) ? (DateTime?)null : reader.GetDateTime(17),
                                    Updated_Date = reader.IsDBNull(18) ? (DateTime?)null : reader.GetDateTime(18),
                                    User_Id = reader.IsDBNull(19) ? 0 : reader.GetInt64(19)
                                };
                            }
                        }
                    }
                }

                if (sales == null)
                    return NotFound($"Sales Invoice with ID {id} not found");

                return Ok(sales);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        //[HttpGet("dropdown_salesinvoice_list")]
        //public async Task<ActionResult<IEnumerable<Drop_salesinvoice_List>>> Get_drop_salesinvoice_list()
        //{
        //    var sales_list = new List<Drop_salesinvoice_List>();
        //    var connectionstring = _configuration.GetConnectionString("DefaultConnection");

        //    try
        //    {
        //        using (var connection = new SqlConnection(connectionstring))
        //        {
        //            string spName = "sp_salesinvoice_mast_ins_upd_del";

        //            await connection.OpenAsync();

        //            using (var command = new SqlCommand(spName, connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddWithValue("@action", "salesinvoice_mastlist");

        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var sales = new Drop_salesinvoice_List
        //                        {
        //                            Sales_id = reader.GetInt64(0),
        //                        };

        //                        sales_list.Add(sales);
        //                    }
        //                }
        //            }
        //        }

        //        return Ok(sales_list);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error: {ex.Message}");
        //    }
        //}


        [HttpPost("insert_salesdetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> Add_Sales_Detail([FromBody] Add_salesinvoice_dtl_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try 
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_salesinvoicedetails_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@sales_detail_id", 0);
                        command.Parameters.AddWithValue("@sales_id", request.Sales_Id);
                        command.Parameters.AddWithValue("@inward_id", request.Inward_Id);
                        command.Parameters.AddWithValue("@product_id", request.Product_Id);
                        command.Parameters.AddWithValue("@colour_id", request.Colour_Id);
                        command.Parameters.AddWithValue("@unit_id", request.Unit_Id);
                        command.Parameters.AddWithValue("@length", request.Length);
                        command.Parameters.AddWithValue("@width", request.Width);
                        command.Parameters.AddWithValue("@height", request.Height);
                        command.Parameters.AddWithValue("@kg", request.Kg);
                        command.Parameters.AddWithValue("@liters", request.Liters);
                        command.Parameters.AddWithValue("@totalsqf_runningfeet", request.Totalsqf_runningfeet);
                        command.Parameters.AddWithValue("@rate", request.Rate);
                        command.Parameters.AddWithValue("@totalquantity", request.Totalsqf_runningfeet);
                        command.Parameters.AddWithValue("@gross_amt", request.Gross_amt);
                        command.Parameters.AddWithValue("@sgst_perc", request.Sgst_perc);
                        command.Parameters.AddWithValue("@sgst_amt", request.Sgst_amt);
                        command.Parameters.AddWithValue("@cgst_perc", request.Cgst_perc);
                        command.Parameters.AddWithValue("@cgst_amt", request.Cgst_amt);
                        command.Parameters.AddWithValue("@igst_perc", request.Igst_perc);
                        command.Parameters.AddWithValue("@igst_amt", request.Igst_amt);
                        command.Parameters.AddWithValue("@discount_perc", request.Discount_perc);
                        command.Parameters.AddWithValue("@discount_amt", request.Discount_amt);
                        command.Parameters.AddWithValue("@total_amt", request.Total_amt);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_Id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_Id);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowAffected = await command.ExecuteNonQueryAsync();

                        if (rowAffected > 0) 
                        {
                            return Ok(new { message = "SalesInvoice Detail Added sucessfully" });
                        }
                        else
                        {
                            return BadRequest(new { message = "Filed to add SalesInvoice Detail " });

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "SalesInvoice Detail already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("DeleteSalesInvoiceDtl/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSalesInvoiceDtl(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_salesinvoicedetails_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@sales_detail_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Sales invoice detail deleted successfully." });
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


        [HttpPost("UpdateSalesInvoiceDtl")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateSalesInvoiceDtl([FromBody] Update_salesinvoice_dtl_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_salesinvoicedetails_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@sales_detail_id", request.Sales_detail_Id);
                    parameters.Add("@sales_id", request.Sales_Id);
                    parameters.Add("@inward_id", request.Inward_Id);
                    parameters.Add("@product_id", request.Product_Id);
                    parameters.Add("@colour_id", request.Colour_Id);
                    parameters.Add("@unit_id", request.Unit_Id);
                    parameters.Add("@length", request.Length);
                    parameters.Add("@width", request.Width);
                    parameters.Add("@height", request.Height);
                    parameters.Add("@kg", request.Kg);
                    parameters.Add("@liters", request.Liters);
                    parameters.Add("@totalsqf_runningfeet", request.Totalsqf_runningfeet);
                    parameters.Add("@rate", request.Rate);
                    parameters.Add("@totalquantity", request.Totalquantity);
                    parameters.Add("@gross_amt", request.Gross_amt);
                    parameters.Add("@sgst_perc", request.Sgst_perc);
                    parameters.Add("@sgst_amt", request.Sgst_amt);
                    parameters.Add("@cgst_perc", request.Cgst_perc);
                    parameters.Add("@cgst_amt", request.Cgst_amt);
                    parameters.Add("@igst_perc", request.Igst_perc);
                    parameters.Add("@igst_amt", request.Igst_amt);
                    parameters.Add("@discount_perc", request.Discount_perc);
                    parameters.Add("@discount_amt", request.Discount_amt);
                    parameters.Add("@total_amt", request.Total_amt);
                    parameters.Add("@fin_year_id", request.Fin_year_Id);
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
                    return NotFound($"Sales invoice detail with ID {request.Sales_detail_Id} not found");
                else
                    return Ok(new { message = "Sales invoice detail updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("salesinvoice_dtl_list")]
        public async Task<ActionResult<IEnumerable<sales_salesinvoice_dtl_List>>> Get_salesinvoice_dtl_list()
        {
            var list = new List<sales_salesinvoice_dtl_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_salesinvoicedetails_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new sales_salesinvoice_dtl_List
                                {
                                    Sales_detail_Id = reader.GetInt64(0),
                                    Sales_Id = reader.GetInt64(1),
                                    Inward_Id = reader.GetInt64(2),
                                    Product_Name = reader.GetString(3),
                                    Colour_Name = reader.GetString(4),
                                    Unit_Name = reader.GetString(5),
                                    Length = reader.GetDecimal(6),
                                    Width = reader.GetDecimal(7),
                                    Height = reader.GetDecimal(8),
                                    Kg = reader.GetDecimal(9),
                                    Liters = reader.GetDecimal(10),
                                    Totalsqf_runningfeet = reader.GetDecimal(11),
                                    Rate = reader.GetDecimal(12),
                                    Totalquantity = reader.GetDecimal(13),
                                    Gross_amt = reader.GetDecimal(14),
                                    Sgst_perc = reader.GetDecimal(15),
                                    Sgst_amt = reader.GetDecimal(16),
                                    Cgst_perc = reader.GetDecimal(17),
                                    Cgst_amt = reader.GetDecimal(18),
                                    Igst_perc = reader.GetDecimal(19),
                                    Igst_amt = reader.GetDecimal(20),
                                    Discount_perc = reader.GetDecimal(21),
                                    Discount_amt = reader.GetDecimal(22),
                                    Total_amt = reader.GetDecimal(23),
                                    Fin_Year_Name = reader.IsDBNull(24) ? "" : reader.GetString(24),
                                    Comp_Name = reader.IsDBNull(25) ? "" : reader.GetString(25),
                                    Created_Date = reader.IsDBNull(26) ? "" : reader.GetDateTime(26).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(27) ? "" : reader.GetDateTime(27).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(28) ? "" : reader.GetString(28),
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




        [HttpGet("salesinvoice_dtl/{id}")]
        public async Task<ActionResult<Single_salesinvoice_dtl>> Get_salesinvoice_dtl_by_id(long id)
        {
            Single_salesinvoice_dtl? item = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_salesinvoicedetails_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@sales_detail_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                item = new Single_salesinvoice_dtl
                                {
                                    Sales_detail_Id = reader.GetInt64(0),
                                    Sales_Id = reader.GetInt64(1),
                                    Inward_Id = reader.GetInt64(2),
                                    Product_Id = reader.GetInt64(3),
                                    Colour_Id = reader.GetInt64(4),
                                    Unit_Id = reader.GetInt64(5),
                                    Length = reader.GetDecimal(6),
                                    Width = reader.GetDecimal(7),
                                    Height = reader.GetDecimal(8),
                                    Kg = reader.GetDecimal(9),
                                    Liters = reader.GetDecimal(10),
                                    Totalsqf_runningfeet = reader.GetDecimal(11),
                                    Rate = reader.GetDecimal(12),
                                    Totalquantity = reader.GetDecimal(13),
                                    Gross_amt = reader.GetDecimal(14),
                                    Sgst_perc = reader.GetDecimal(15),
                                    Sgst_amt = reader.GetDecimal(16),
                                    Cgst_perc = reader.GetDecimal(17),
                                    Cgst_amt = reader.GetDecimal(18),
                                    Igst_perc = reader.GetDecimal(19),
                                    Igst_amt = reader.GetDecimal(20),
                                    Discount_perc = reader.GetDecimal(21),
                                    Discount_amt = reader.GetDecimal(22),
                                    Total_amt = reader.GetDecimal(23),
                                    Fin_year_Id = reader.GetInt64(24),
                                    Comp_Id = reader.GetInt64(25),
                                    Created_Date = reader.GetDateTime(26),
                                    Updated_Date = reader.IsDBNull(27) ? default : reader.GetDateTime(27),
                                    User_Id = reader.IsDBNull(28) ? 0 : reader.GetInt64(28),
                                };
                            }
                        }
                    }
                }

                if (item == null)
                    return NotFound($"Sales invoice detail with ID {id} not found");

                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        //[HttpGet("dropdown_salesinvoice_dtl_list")]
        //public async Task<ActionResult<IEnumerable<Drop_salesinvoice_dtl_List>>> Get_dropdown_salesinvoice_dtl_list()
        //{
        //    var list = new List<Drop_salesinvoice_dtl_List>();
        //    var connectionstring = _configuration.GetConnectionString("DefaultConnection");

        //    try
        //    {
        //        using (var connection = new SqlConnection(connectionstring))
        //        {
        //            string spName = "sp_salesinvoicedetails_ins_upd_del";

        //            await connection.OpenAsync();

        //            using (var command = new SqlCommand(spName, connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddWithValue("@action", "salesinvoice_dtl_mastlist");

        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var item = new Drop_salesinvoice_dtl_List
        //                        {
        //                            Sales_detail_Id = reader.GetInt64(0),
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





        public class AddsalesinvoiceRequest
        {
            public long Sales_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public long Customer_id { get; set; } = 0;
            public DateTime Sales_date { get; set; }
            public decimal Gross_total { get; set; } = 0;
            public decimal Sgst_total { get; set; } = 0;
            public decimal Cgst_total { get; set; } = 0;
            public decimal Igst_total { get; set; } = 0;
            public decimal Discount_total { get; set; } = 0;
            public decimal Roundoff_total { get; set; } = 0;
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public bool Isactive { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class UpdatesalesinvoiceRequest
        {
            public long Sales_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public long Customer_id { get; set; } = 0;
            public DateTime Sales_date { get; set; }
            public decimal Gross_total { get; set; } = 0;
            public decimal Sgst_total { get; set; } = 0;
            public decimal Cgst_total { get; set; } = 0;
            public decimal Igst_total { get; set; } = 0;
            public decimal Discount_total { get; set; } = 0;
            public decimal Roundoff_total { get; set; } = 0;
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public bool Isactive { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class salesinvoice_List
        {
            public long Sales_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public string Customer_name { get; set; } = "";
            public DateTime Sales_date { get; set; }
            public decimal Gross_total { get; set; } = 0;
            public decimal Sgst_total { get; set; } = 0;
            public decimal Cgst_total { get; set; } = 0;
            public decimal Igst_total { get; set; } = 0;
            public decimal Discount_total { get; set; } = 0;
            public decimal Roundoff_total { get; set; } = 0;
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public bool Isactive { get; set; } = false;
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }

        public class Singlesalesinvoice
        {
            public long Sales_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public long Customer_id { get; set; } = 0;
            public DateTime Sales_date { get; set; }
            public decimal Gross_total { get; set; } = 0;
            public decimal Sgst_total { get; set; } = 0;
            public decimal Cgst_total { get; set; } = 0;
            public decimal Igst_total { get; set; } = 0;
            public decimal Discount_total { get; set; } = 0;
            public decimal Roundoff_total { get; set; } = 0;
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public bool Isactive { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_salesinvoice_List
        {
            public long Sales_id { get; set; } = 0;
        }


        public class Add_salesinvoice_dtl_Request 
        {

             public long Sales_detail_Id { get; set; } = 0;
             public long Sales_Id { get; set; }=0;
             public long Inward_Id { get; set; } = 0;
             public long Product_Id { get; set; }=0;
             public long Colour_Id { get; set; } = 0;
             public long Unit_Id { get; set; }=0;
             public decimal Length  { get; set; } =0;
             public decimal Width  { get; set; } =0;
             public decimal Height  { get; set; } =0;
             public decimal Kg  { get; set; } =0;
             public decimal Liters  { get; set; } =0;
             public decimal Totalsqf_runningfeet  { get; set; } =0;
             public decimal Rate  { get; set; }=0;
             public decimal Totalquantity  { get; set; }=0;
             public decimal Gross_amt  { get; set; }=0;
             public decimal Sgst_perc { get; set; } = 0;
             public decimal Sgst_amt  { get; set; }=0;
             public decimal Cgst_perc { get; set; } = 0;
             public decimal Cgst_amt  { get; set; }=0;
             public decimal Igst_perc { get; set; } = 0;
             public decimal Igst_amt  { get; set; }=0;
             public decimal Discount_perc { get; set; } = 0;
             public decimal Discount_amt  { get; set; }=0;
             public decimal Total_amt  { get; set; }=0;
             public long Fin_year_Id { get; set; } = 0;
             public long Comp_Id { get; set; }=0;
             public DateTime Created_Date { get; set; }
             public DateTime Updated_Date { get; set; }
             public long User_Id { get; set; } = 0;
        }

        public class Update_salesinvoice_dtl_Request
        {
            public long Sales_detail_Id { get; set; } = 0;
            public long Sales_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public long Colour_Id { get; set; } = 0;
            public long Unit_Id { get; set; } = 0;
            public decimal Length { get; set; } = 0;
            public decimal Width { get; set; } = 0;
            public decimal Height { get; set; } = 0;
            public decimal Kg { get; set; } = 0;
            public decimal Liters { get; set; } = 0;
            public decimal Totalsqf_runningfeet { get; set; } = 0;
            public decimal Rate { get; set; } = 0;
            public decimal Totalquantity { get; set; } = 0;
            public decimal Gross_amt { get; set; } = 0;
            public decimal Sgst_perc { get; set; } = 0;
            public decimal Sgst_amt { get; set; } = 0;
            public decimal Cgst_perc { get; set; } = 0;
            public decimal Cgst_amt { get; set; } = 0;
            public decimal Igst_perc { get; set; } = 0;
            public decimal Igst_amt { get; set; } = 0;
            public decimal Discount_perc { get; set; } = 0;
            public decimal Discount_amt { get; set; } = 0;
            public decimal Total_amt { get; set; } = 0;
            public long Fin_year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class sales_salesinvoice_dtl_List
        {
            public long Sales_detail_Id { get; set; } = 0;
            public long Sales_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0;
            public string Product_Name { get; set; } = "";
            public string Colour_Name { get; set; } = "";
            public string Unit_Name { get; set; } = "";
            public decimal Length { get; set; } = 0;
            public decimal Width { get; set; } = 0;
            public decimal Height { get; set; } = 0;
            public decimal Kg { get; set; } = 0;
            public decimal Liters { get; set; } = 0;
            public decimal Totalsqf_runningfeet { get; set; } = 0;
            public decimal Rate { get; set; } = 0;
            public decimal Totalquantity { get; set; } = 0;
            public decimal Gross_amt { get; set; } = 0;
            public decimal Sgst_perc { get; set; } = 0;
            public decimal Sgst_amt { get; set; } = 0;
            public decimal Cgst_perc { get; set; } = 0;
            public decimal Cgst_amt { get; set; } = 0;
            public decimal Igst_perc { get; set; } = 0;
            public decimal Igst_amt { get; set; } = 0;
            public decimal Discount_perc { get; set; } = 0;
            public decimal Discount_amt { get; set; } = 0;
            public decimal Total_amt { get; set; } = 0;
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }

        public class Single_salesinvoice_dtl
        {
            public long Sales_detail_Id { get; set; } = 0;
            public long Sales_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public long Colour_Id { get; set; } = 0;
            public long Unit_Id { get; set; } = 0;
            public decimal Length { get; set; } = 0;
            public decimal Width { get; set; } = 0;
            public decimal Height { get; set; } = 0;
            public decimal Kg { get; set; } = 0;
            public decimal Liters { get; set; } = 0;
            public decimal Totalsqf_runningfeet { get; set; } = 0;
            public decimal Rate { get; set; } = 0;
            public decimal Totalquantity { get; set; } = 0;
            public decimal Gross_amt { get; set; } = 0;
            public decimal Sgst_perc { get; set; } = 0;
            public decimal Sgst_amt { get; set; } = 0;
            public decimal Cgst_perc { get; set; } = 0;
            public decimal Cgst_amt { get; set; } = 0;
            public decimal Igst_perc { get; set; } = 0;
            public decimal Igst_amt { get; set; } = 0;
            public decimal Discount_perc { get; set; } = 0;
            public decimal Discount_amt { get; set; } = 0;
            public decimal Total_amt { get; set; } = 0;
            public long Fin_year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_salesinvoice_dtl_List
        {
            public long Sales_detail_Id { get; set; } = 0;
        }

    }
}
