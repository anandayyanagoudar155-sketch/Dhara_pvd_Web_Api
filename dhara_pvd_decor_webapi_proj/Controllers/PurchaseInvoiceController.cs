using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseInvoiceController : Controller
    {
        private readonly IConfiguration _configuration;

        public PurchaseInvoiceController(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        [HttpPost("insert_purchaseinvoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPurchaseInvoice([FromBody] AddpurchaseinvoiceRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_purchaseinvoice_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@purchase_id", request.Purchase_id);
                        command.Parameters.AddWithValue("@prefix", request.Prefix);
                        command.Parameters.AddWithValue("@suffix", request.Suffix);
                        command.Parameters.AddWithValue("@invoice_no", request.Invoice_no);
                        command.Parameters.AddWithValue("@purchase_date", request.Purchase_date);
                        command.Parameters.AddWithValue("@vendor_id", request.Vendor_id);
                        command.Parameters.AddWithValue("@gross_total", request.Gross_total);
                        command.Parameters.AddWithValue("@sgst_total", request.Sgst_total);
                        command.Parameters.AddWithValue("@cgst_total", request.Cgst_total);
                        command.Parameters.AddWithValue("@igst_total", request.Igst_total);
                        command.Parameters.AddWithValue("@discount_total", request.Discount_total);
                        command.Parameters.AddWithValue("@roundoff_total", request.Roundoff_total);
                        command.Parameters.AddWithValue("@net_total", request.Net_total);
                        command.Parameters.AddWithValue("@balance_total", request.Balance_total);
                        command.Parameters.AddWithValue("@paymentstatus", request.Payment_status);
                        command.Parameters.AddWithValue("@is_active", request.Isactive);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_Year_Id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_Id);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Purchase Invoice Added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add Purchase Invoice." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Invoice number already exists." });
                }
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }




        [HttpDelete("Delete_Purchase_invoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Delete_Purchase_Ivoice(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_purchaseinvoice_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@purchase_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "PurchaseInvoice deleted successfully." });
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


        [HttpPost("UpdatePurchaseInvoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdatePurchaseInvoice([FromBody] UpdatepurchaseinvoiceRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_purchaseinvoice_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@purchase_id", request.Purchase_id);
                    parameters.Add("@prefix", request.Prefix);
                    parameters.Add("@suffix", request.Suffix);
                    parameters.Add("@invoice_no", request.Invoice_no);
                    parameters.Add("@purchase_date", request.Purchase_date);
                    parameters.Add("@vendor_id", request.Vendor_id);
                    parameters.Add("@gross_total", request.Gross_total);
                    parameters.Add("@sgst_total", request.Sgst_total);
                    parameters.Add("@cgst_total", request.Cgst_total);
                    parameters.Add("@igst_total", request.Igst_total);
                    parameters.Add("@discount_total", request.Discount_total);
                    parameters.Add("@roundoff_total", request.Roundoff_total);
                    parameters.Add("@net_total", request.Net_total);
                    parameters.Add("@balance_total", request.Balance_total);
                    parameters.Add("@paymentstatus", request.Payment_status);
                    parameters.Add("@is_active", request.Isactive);
                    parameters.Add("@fin_year_id", request.Fin_Year_Id);
                    parameters.Add("@comp_id", request.Comp_Id);
                    parameters.Add("@created_date", request.Created_Date);
                    parameters.Add("@updated_date", request.Updated_Date);
                    parameters.Add("@user_id", request.User_Id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"Purchase Invoice with ID {request.Purchase_id} not found");
                else
                    return Ok(new { message = "Purchase Invoice updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("purchaseinvoice_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<purchaseinvoice_List>>> Get_purchaseinvoice_list()
        {
            var list = new List<purchaseinvoice_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_purchaseinvoice_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new purchaseinvoice_List
                                {
                                    Purchase_id = reader.GetInt64(0),
                                    Prefix = reader.GetString(1),
                                    Suffix = reader.GetString(2),
                                    Invoice_no = reader.GetString(3),
                                    Purchase_date = reader.GetDateTime(4),
                                    Vendor_name = reader.GetString(5),
                                    Gross_total = reader.GetDecimal(6),
                                    Sgst_total = reader.GetDecimal(7),
                                    Cgst_total = reader.GetDecimal(8),
                                    Igst_total = reader.GetDecimal(9),
                                    Discount_total = reader.GetDecimal(10),
                                    Roundoff_total = reader.GetDecimal(11),
                                    Net_total = reader.GetDecimal(12),
                                    Balance_total = reader.GetDecimal(13),
                                    Payment_status = reader.GetBoolean(14),
                                    Isactive = reader.GetBoolean(15),
                                    Fin_Year_Name = reader.GetString(16),
                                    Comp_Name = reader.GetString(17),
                                    Created_Date = reader.GetDateTime(18).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(19) ? "" : reader.GetDateTime(19).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(20) ? "" : reader.GetString(20)
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



        [HttpGet("purchaseinvoice/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Singlepurchaseinvoice>> Get_purchaseinvoice_by_id(long id)
        {
            Singlepurchaseinvoice? invoice = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_purchaseinvoice_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@purchase_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                invoice = new Singlepurchaseinvoice
                                {
                                    Purchase_id = reader.GetInt64(0),
                                    Prefix = reader.GetString(1),
                                    Suffix = reader.GetString(2),
                                    Invoice_no = reader.GetString(3),
                                    Purchase_date = reader.GetDateTime(4),
                                    Vendor_id = reader.GetInt64(5),
                                    Gross_total = reader.GetDecimal(6),
                                    Sgst_total = reader.GetDecimal(7),
                                    Cgst_total = reader.GetDecimal(8),
                                    Igst_total = reader.GetDecimal(9),
                                    Discount_total = reader.GetDecimal(10),
                                    Roundoff_total = reader.GetDecimal(11),
                                    Net_total = reader.GetDecimal(12),
                                    Balance_total = reader.GetDecimal(13),
                                    Payment_status = reader.GetBoolean(14),
                                    Isactive = reader.GetBoolean(15),
                                    Fin_Year_Id = reader.GetInt64(16),
                                    Comp_Id = reader.GetInt64(17),
                                    Created_Date = reader.IsDBNull(18) ? null : reader.GetDateTime(18),
                                    Updated_Date = reader.IsDBNull(19) ? null : reader.GetDateTime(19),
                                    User_Id = reader.IsDBNull(20) ? 0 : reader.GetInt64(20)
                                };
                            }
                        }
                    }
                }

                if (invoice == null)
                    return NotFound($"Purchase Invoice with ID {id} not found");

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        //[HttpGet("dropdown_purchaseinvoice")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<IEnumerable<Drop_purchaseinvoice_List>>> Get_drop_purchaseinvoice_list()
        //{
        //    var list = new List<Drop_purchaseinvoice_List>();
        //    var connectionstring = _configuration.GetConnectionString("DefaultConnection");

        //    try
        //    {
        //        using (var connection = new SqlConnection(connectionstring))
        //        {
        //            string spName = "sp_purchaseinvoice_mast_ins_upd_del";

        //            await connection.OpenAsync();

        //            using (var command = new SqlCommand(spName, connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddWithValue("@action", "purchaseinvoice_mastlist");

        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        list.Add(new Drop_purchaseinvoice_List
        //                        {
        //                            Sales_id = reader.GetInt64(0)
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


        [HttpPost("insert_purchase_detail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPurchaseDetail([FromBody] Add_purchase_dtl_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_purchaseinvoice_details_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@purchase_detail_id", request.Purchase_Detail_Id);
                        command.Parameters.AddWithValue("@purchase_id", request.Purchase_Id);
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
                        command.Parameters.AddWithValue("@gross_amt", request.Gross_amt);
                        command.Parameters.AddWithValue("@totalquantity", request.Totalquantity);
                        command.Parameters.AddWithValue("@sgst_perc", request.Sgst_perc);
                        command.Parameters.AddWithValue("@sgst_amt", request.Sgst_amt);
                        command.Parameters.AddWithValue("@cgst_perc", request.Cgst_perc);
                        command.Parameters.AddWithValue("@cgst_amt", request.Cgst_amt);
                        command.Parameters.AddWithValue("@igst_perc", request.Igst_perc);
                        command.Parameters.AddWithValue("@igst_amt", request.Igst_amt);
                        command.Parameters.AddWithValue("@discount_perc", request.Discount_perc);
                        command.Parameters.AddWithValue("@discount_amt", request.Discount_amt);
                        command.Parameters.AddWithValue("@total_amt", request.Total_amt);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@updated_date", request.Updated_Date);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_Id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_Id);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Purchase Detail Added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add Purchase Detail." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_purchase_detail/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePurchaseDetail(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_purchaseinvoice_details_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@purchase_detail_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Purchase Detail deleted successfully." });
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



        [HttpPost("update_purchase_detail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdatePurchaseDetail([FromBody] Update_purchase_dtl_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_purchaseinvoice_details_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@purchase_detail_id", request.Purchase_Detail_Id);
                    parameters.Add("@purchase_id", request.Purchase_Id);
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
                    parameters.Add("@gross_amt", request.Gross_amt);
                    parameters.Add("@totalquantity", request.Totalquantity);
                    parameters.Add("@sgst_perc", request.Sgst_perc);
                    parameters.Add("@sgst_amt", request.Sgst_amt);
                    parameters.Add("@cgst_perc", request.Cgst_perc);
                    parameters.Add("@cgst_amt", request.Cgst_amt);
                    parameters.Add("@igst_perc", request.Igst_perc);
                    parameters.Add("@igst_amt", request.Igst_amt);
                    parameters.Add("@discount_perc", request.Discount_perc);
                    parameters.Add("@discount_amt", request.Discount_amt);
                    parameters.Add("@total_amt", request.Total_amt);
                    parameters.Add("@created_date", request.Created_Date);
                    parameters.Add("@updated_date", request.Updated_Date);
                    parameters.Add("@fin_year_id", request.Fin_year_Id);
                    parameters.Add("@comp_id", request.Comp_Id);
                    parameters.Add("@user_id", request.User_Id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"Purchase Detail with ID {request.Purchase_Detail_Id} not found");
                else
                    return Ok(new { message = "Purchase Detail updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("purchase_detail_list")]
        public async Task<ActionResult<IEnumerable<purchase_dtl_List>>> Get_purchase_detail_list()
        {
            var list = new List<purchase_dtl_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_purchaseinvoice_details_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var data = new purchase_dtl_List
                                {
                                    Purchase_Detail_Id = reader.GetInt64(0),
                                    Purchase_Id = reader.GetInt64(1),
                                    Product_Name = reader.GetString(2),
                                    Colour_Name = reader.GetString(3),
                                    Unit_Name = reader.GetString(4),
                                    Length = reader.GetDecimal(5),
                                    Width = reader.GetDecimal(6),
                                    Height = reader.GetDecimal(7),
                                    Kg = reader.GetDecimal(8),
                                    Liters = reader.GetDecimal(9),
                                    Totalsqf_runningfeet = reader.GetDecimal(10),
                                    Rate = reader.GetDecimal(11),
                                    Gross_amt = reader.GetDecimal(12),
                                    Totalquantity = reader.GetDecimal(13),
                                    Sgst_perc = reader.GetDecimal(14),
                                    Sgst_amt = reader.GetDecimal(15),
                                    Cgst_perc = reader.GetDecimal(16),
                                    Cgst_amt = reader.GetDecimal(17),
                                    Igst_perc = reader.GetDecimal(18),
                                    Igst_amt = reader.GetDecimal(19),
                                    Discount_perc = reader.GetDecimal(20),
                                    Discount_amt = reader.GetDecimal(21),
                                    Total_amt = reader.GetDecimal(22),
                                    Created_Date = reader.GetDateTime(23).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(24) ? "" : reader.GetDateTime(24).ToString("yyyy-MM-dd"),
                                    Fin_Year_Name = reader.GetString(25),
                                    Comp_Name = reader.GetString(26),
                                    User_Name = reader.GetString(27)
                                };

                                list.Add(data);
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



        [HttpGet("purchase_detail/{id}")]
        public async Task<ActionResult<Single_purchase_dtl>> Get_purchase_detail_by_id(long id)
        {
            Single_purchase_dtl? data = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_purchaseinvoice_details_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@purchase_detail_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                data = new Single_purchase_dtl
                                {
                                    Purchase_Detail_Id = reader.GetInt64(0),
                                    Purchase_Id = reader.GetInt64(1),
                                    Product_Id = reader.GetInt64(2),
                                    Colour_Id = reader.GetInt64(3),
                                    Unit_Id = reader.GetInt64(4),
                                    Length = reader.GetDecimal(5),
                                    Width = reader.GetDecimal(6),
                                    Height = reader.GetDecimal(7),
                                    Kg = reader.GetDecimal(8),
                                    Liters = reader.GetDecimal(9),
                                    Totalsqf_runningfeet = reader.GetDecimal(10),
                                    Rate = reader.GetDecimal(11),
                                    Gross_amt = reader.GetDecimal(12),
                                    Totalquantity = reader.GetDecimal(13),
                                    Sgst_perc = reader.GetDecimal(14),
                                    Sgst_amt = reader.GetDecimal(15),
                                    Cgst_perc = reader.GetDecimal(16),
                                    Cgst_amt = reader.GetDecimal(17),
                                    Igst_perc = reader.GetDecimal(18),
                                    Igst_amt = reader.GetDecimal(19),
                                    Discount_perc = reader.GetDecimal(20),
                                    Discount_amt = reader.GetDecimal(21),
                                    Total_amt = reader.GetDecimal(22),
                                    Created_Date = reader.GetDateTime(23),
                                    Updated_Date = reader.GetDateTime(24),
                                    Fin_year_Id = reader.GetInt64(25),
                                    Comp_Id = reader.GetInt64(26),
                                    User_Id = reader.GetInt64(27)
                                };
                            }
                        }
                    }
                }

                if (data == null)
                    return NotFound($"Purchase Detail with ID {id} not found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        //[HttpGet("dropdown_purchase_detail_list")]
        //public async Task<ActionResult<IEnumerable<Drop_purchase_dtl_Request>>> Get_drop_purchase_detail_list()
        //{
        //    var list = new List<Drop_purchase_dtl_Request>();
        //    var connectionstring = _configuration.GetConnectionString("DefaultConnection");

        //    try
        //    {
        //        using (var connection = new SqlConnection(connectionstring))
        //        {
        //            string spName = "sp_purchaseinvoice_details_ins_upd_del";

        //            await connection.OpenAsync();

        //            using (var command = new SqlCommand(spName, connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddWithValue("@action", "purchase_dtl_list");

        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var data = new Drop_purchase_dtl_Request
        //                        {
        //                            Purchase_Detail_Id = reader.GetInt64(0)
        //                        };

        //                        list.Add(data);
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







        public class AddpurchaseinvoiceRequest
        {
            public long Purchase_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public string Invoice_no { get; set; } = "";
            public DateTime Purchase_date { get; set; }
            public long Vendor_id { get; set; } = 0;
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

        public class UpdatepurchaseinvoiceRequest
        {
            public long Purchase_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public string Invoice_no { get; set; } = "";
            public DateTime Purchase_date { get; set; }
            public long Vendor_id { get; set; } = 0;
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

        public class purchaseinvoice_List
        {
            public long Purchase_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public string Invoice_no { get; set; } = "";
            public DateTime Purchase_date { get; set; }
            public string Vendor_name { get; set; } = "";
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

        public class Singlepurchaseinvoice
        {
            public long Purchase_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public string Invoice_no { get; set; } = "";
            public DateTime Purchase_date { get; set; }
            public long Vendor_id { get; set; } = 0;
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


        public class Drop_purchaseinvoice_List
        {
            public long Sales_id { get; set; } = 0;
        }


        public class Add_purchase_dtl_Request
        {
            public long Purchase_Detail_Id { get; set; } = 0;
            public long Purchase_Id { get; set; } = 0;
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
            public decimal Gross_amt { get; set; } = 0;
            public decimal Totalquantity { get; set; } = 0;
            public decimal Sgst_perc { get; set; } = 0;
            public decimal Sgst_amt { get; set; } = 0;
            public decimal Cgst_perc { get; set; } = 0;
            public decimal Cgst_amt { get; set; } = 0;
            public decimal Igst_perc { get; set; } = 0;
            public decimal Igst_amt { get; set; } = 0;
            public decimal Discount_perc { get; set; } = 0;
            public decimal Discount_amt { get; set; } = 0;
            public decimal Total_amt { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long Fin_year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public long User_Id { get; set; } = 0;
        }




        public class Update_purchase_dtl_Request
        {
            public long Purchase_Detail_Id { get; set; } = 0;
            public long Purchase_Id { get; set; } = 0;
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
            public decimal Gross_amt { get; set; } = 0;
            public decimal Totalquantity { get; set; } = 0;
            public decimal Sgst_perc { get; set; } = 0;
            public decimal Sgst_amt { get; set; } = 0;
            public decimal Cgst_perc { get; set; } = 0;
            public decimal Cgst_amt { get; set; } = 0;
            public decimal Igst_perc { get; set; } = 0;
            public decimal Igst_amt { get; set; } = 0;
            public decimal Discount_perc { get; set; } = 0;
            public decimal Discount_amt { get; set; } = 0;
            public decimal Total_amt { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long Fin_year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public long User_Id { get; set; } = 0;
        }



        public class purchase_dtl_List
        {
            public long Purchase_Detail_Id { get; set; } = 0;
            public long Purchase_Id { get; set; } = 0;
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
            public decimal Gross_amt { get; set; } = 0;
            public decimal Totalquantity { get; set; } = 0;
            public decimal Sgst_perc { get; set; } = 0;
            public decimal Sgst_amt { get; set; } = 0;
            public decimal Cgst_perc { get; set; } = 0;
            public decimal Cgst_amt { get; set; } = 0;
            public decimal Igst_perc { get; set; } = 0;
            public decimal Igst_amt { get; set; } = 0;
            public decimal Discount_perc { get; set; } = 0;
            public decimal Discount_amt { get; set; } = 0;
            public decimal Total_amt { get; set; } = 0;
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string User_Name { get; set; } = "";
        }



        public class Single_purchase_dtl
        {
            public long Purchase_Detail_Id { get; set; } = 0;
            public long Purchase_Id { get; set; } = 0;
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
            public decimal Gross_amt { get; set; } = 0;
            public decimal Totalquantity { get; set; } = 0;
            public decimal Sgst_perc { get; set; } = 0;
            public decimal Sgst_amt { get; set; } = 0;
            public decimal Cgst_perc { get; set; } = 0;
            public decimal Cgst_amt { get; set; } = 0;
            public decimal Igst_perc { get; set; } = 0;
            public decimal Igst_amt { get; set; } = 0;
            public decimal Discount_perc { get; set; } = 0;
            public decimal Discount_amt { get; set; } = 0;
            public decimal Total_amt { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long Fin_year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public long User_Id { get; set; } = 0;
        }



        public class Drop_purchase_dtl_Request
        {
            public long Purchase_Detail_Id { get; set; } = 0;
        }


    }
}
