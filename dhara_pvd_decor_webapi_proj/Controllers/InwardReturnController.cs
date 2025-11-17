using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InwardReturnController : Controller
    {
        private readonly IConfiguration _configuration;

        public InwardReturnController(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        [HttpPost("insert_returninward")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Add_inwardreturn([FromBody] AddInwardreturnRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_inward_return_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@inwardreturn_id", 0);
                        command.Parameters.AddWithValue("@inward_id", request.Inward_Id);
                        command.Parameters.AddWithValue("@customer_id", request.Customer_Id);
                        command.Parameters.AddWithValue("@product_id", request.Product_Id);
                        command.Parameters.AddWithValue("@returnquantity", request.ReturnQuantity);
                        command.Parameters.AddWithValue("@remarks", request.Remarks);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_Year_Id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_Id);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "inwardreturn Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add inwardreturn." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "same inwardreturn  already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("DeleteInwardReturn/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteInwardReturn(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_inward_return_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@inwardreturn_id", id);

                        int rowAffected = await command.ExecuteNonQueryAsync();

                        if (rowAffected > 0)
                            return Ok(new { message = "inwardreturn deleted successfully." });
                        else
                            return StatusCode(500, new { message = "No Record Deleted" });

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



        [HttpPost("UpdateInwardReturn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> UpdateInwardReturn([FromBody] UpdateInwardreturnRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {

                    string spname = "sp_inward_return_ins_upd_del";

                    await connection.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@inwardreturn_id", request.Inwardreturn_Id);
                    parameters.Add("@inward_id", request.Inward_Id);
                    parameters.Add("@customer_id", request.Customer_Id);
                    parameters.Add("@product_id", request.Product_Id);
                    parameters.Add("@returnquantity", request.ReturnQuantity);
                    parameters.Add("@remarks", request.Remarks);
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
                    return NotFound($"inwardreturn with ID {request.Inwardreturn_Id} not found");
                else
                    return Ok(new { message = "inwardreturn updated successfully" });
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }


        [HttpGet("inwardreturn_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<Inwardreturn_List>>> Get_inwardreturn_list()
        {
            var Inwardreturn_List = new List<Inwardreturn_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_inward_return_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var InwardReturn = new Inwardreturn_List
                                {
                                    Inwardreturn_Id = reader.GetInt64(0),
                                    Inward_Id = reader.GetInt64(1),
                                    Customer_Name = reader.GetString(2),
                                    Product_Name = reader.GetString(3),
                                    ReturnQuantity = reader.GetDecimal(4),
                                    Remarks = reader.GetString(5),
                                    Fin_Year_Name = reader.GetString(6),
                                    Comp_Name = reader.GetString(7),
                                    Created_Date = reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(9) ? "" : reader.GetDateTime(9).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                };

                                Inwardreturn_List.Add(InwardReturn);
                            }
                        }
                    }
                }

                return Ok(Inwardreturn_List);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("InwardReturn/{id}")]
        public async Task<ActionResult<SingleInwardreturn>> Get_InwardReturn_by_id(long id)
        {
            SingleInwardreturn? InwardReturn = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_inward_return_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@inwardreturn_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                InwardReturn = new SingleInwardreturn
                                {
                                    Inwardreturn_Id = reader.GetInt64(0),
                                    Inward_Id = reader.GetInt64(1),
                                    Customer_Id = reader.GetInt64(2),
                                    Product_Id = reader.GetInt64(3),
                                    ReturnQuantity = reader.GetDecimal(4),
                                    Remarks = reader.GetString(5),
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

                if (InwardReturn == null)
                    return NotFound($"inwardreturn with ID {id} not found");

                return Ok(InwardReturn);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        public class AddInwardreturnRequest
        {
            public long Inwardreturn_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0;
            public long Customer_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public decimal ReturnQuantity { get; set; } = 0;
            public string Remarks { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class UpdateInwardreturnRequest
        {
            public long Inwardreturn_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0;
            public long Customer_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public decimal ReturnQuantity { get; set; } = 0;
            public string Remarks { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class Inwardreturn_List
        {
            public long Inwardreturn_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0;
            public string Customer_Name { get; set; } = "";
            public string Product_Name { get; set; } = "";
            public decimal ReturnQuantity { get; set; } = 0;
            public string Remarks { get; set; } = "";
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }

        public class SingleInwardreturn
        {
            public long Inwardreturn_Id { get; set; } = 0;
            public long Inward_Id { get; set; } = 0;
            public long Customer_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public decimal ReturnQuantity { get; set; } = 0;
            public string Remarks { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }
    }
}
