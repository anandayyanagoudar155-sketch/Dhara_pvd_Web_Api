using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InwardController : Controller
    {
        private readonly IConfiguration _configuration;

        public InwardController(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        [HttpPost("insert_inward")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddInward([FromBody] AddInwardRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {

                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_inward_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@inward_id", 0);
                        command.Parameters.AddWithValue("@customer_id", request.Customer_Id);
                        command.Parameters.AddWithValue("@product_id", request.Product_Id);
                        command.Parameters.AddWithValue("@totalquantity", request.TotalQuantity);
                        command.Parameters.AddWithValue("@balance_quantity", request.BalanceQuantity);
                        command.Parameters.AddWithValue("@inward_status", request.Inward_Status);
                        command.Parameters.AddWithValue("@remarks", request.Remarks);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_Year_Id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_Id);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "inward Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to inward State." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "inward name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("delete_inward/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteInward(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_inward_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@inward_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "inward deleted successfully." });
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



        [HttpPost("update_inward")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> Updateinward([FromBody] UpdateInwardRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_inward_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@inward_id", request.Inward_Id);
                    parameters.Add("@customer_id", request.Customer_Id);
                    parameters.Add("@product_id", request.Product_Id);
                    parameters.Add("@totalquantity", request.TotalQuantity);
                    parameters.Add("@balance_quantity", request.BalanceQuantity);
                    parameters.Add("@inward_status", request.Inward_Status);
                    parameters.Add("@remarks", request.Remarks);
                    parameters.Add("@fin_year_id", request.Fin_Year_Id);
                    parameters.Add("@comp_id", request.Comp_Id);
                    parameters.Add("@updated_date", request.Updated_Date);
                    parameters.Add("@user_id", request.User_Id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"Inward with ID {request.Inward_Id} not found");
                else
                    return Ok("Inward updated successfully");
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }


        [HttpGet("get_inward_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Inward_List>>> GetInwardList()
        {
            var inwardList = new List<Inward_List>();
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_inward_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var inward = new Inward_List
                                {
                                    Inward_Id = reader.GetInt64(0),
                                    Customer_Name = reader.GetString(1),
                                    Product_Name = reader.GetString(2),
                                    TotalQuantity = reader.GetDecimal(3),
                                    BalanceQuantity = reader.GetDecimal(4),
                                    Inward_Status = reader.GetBoolean(5),
                                    Remarks = reader.GetString(6),
                                    Fin_Year_Name = reader.GetString(7),
                                    Comp_Name = reader.GetString(8),
                                    Created_Date = reader.GetDateTime(9).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(10) ? "" : reader.GetDateTime(10).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(11) ? "" : reader.GetString(11)
                                };

                                inwardList.Add(inward);
                            }
                        }
                    }
                }
                return Ok(inwardList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("inward/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SingleInwardList>> GetInwardById(long id)
        {
            try
            {
                SingleInwardList? inward = null;
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_inward_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@inward_id", id);

                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                inward = new SingleInwardList
                                {
                                    Inward_Id = reader.GetInt64(0),
                                    Customer_Id = reader.GetInt64(1),
                                    Product_Id = reader.GetInt64(2),
                                    TotalQuantity = reader.GetDecimal(3),
                                    BalanceQuantity = reader.GetDecimal(4),
                                    Inward_Status = reader.GetBoolean(5),
                                    Remarks = reader.GetString(6),
                                    Fin_Year_Id = reader.GetInt64(7),
                                    Comp_Id = reader.GetInt64(8),
                                    Created_Date = reader.GetDateTime(9),
                                    Updated_Date = reader.IsDBNull(10) ? null : reader.GetDateTime(10),
                                    User_Id = reader.IsDBNull(11) ? 0 : reader.GetInt64(11),
                                };
                            }
                        }
                    }
                }
                if (inward == null)
                    return NotFound($"inward with ID {id} not found");

                return Ok(inward);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        public class AddInwardRequest
        {
            public long Inward_Id { get; set; } = 0;
            public long Customer_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public decimal TotalQuantity { get; set; } = 0;
            public decimal BalanceQuantity { get; set; } = 0;
            public bool Inward_Status { get; set; } = false;
            public string Remarks { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class UpdateInwardRequest
        {
            public long Inward_Id { get; set; } = 0;
            public long Customer_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public decimal TotalQuantity { get; set; } = 0;
            public decimal BalanceQuantity { get; set; } = 0;
            public bool Inward_Status { get; set; } = false;
            public string Remarks { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class Inward_List
        {
            public long Inward_Id { get; set; } = 0;
            public string Customer_Name { get; set; } = "";
            public string Product_Name { get; set; } = "";
            public decimal TotalQuantity { get; set; } = 0;
            public decimal BalanceQuantity { get; set; } = 0;
            public bool Inward_Status { get; set; } = false;
            public string Remarks { get; set; } = "";
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }

        public class SingleInwardList
        {
            public long Inward_Id { get; set; } = 0;
            public long Customer_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public decimal TotalQuantity { get; set; } = 0;
            public decimal BalanceQuantity { get; set; } = 0;
            public bool Inward_Status { get; set; } = false;
            public string Remarks { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


    
    }
}
