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

    }
}
