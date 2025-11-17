using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DailyConsumptionController : Controller
    {
        private readonly IConfiguration _configuration;

        public DailyConsumptionController(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        [HttpPost("insert_dailyconsumption")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddDailyConsumption([FromBody] AddDailyConsumptionRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_dailyconsumption_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@dailycons_id", request.Dailycons_id);
                        command.Parameters.AddWithValue("@dailycons_date", request.Dailycons_date);
                        command.Parameters.AddWithValue("@product_id", request.product_id);
                        command.Parameters.AddWithValue("@unit_id", request.unit_id);
                        command.Parameters.AddWithValue("@quantityconsumed", request.quantityconsumed);
                        command.Parameters.AddWithValue("@purpose", request.purpose);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_Year_Id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_Id);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Daily Consumption Added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add Daily Consumption." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_dailyconsumption/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDailyConsumption(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_dailyconsumption_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@Dailycons_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Daily Consumption deleted successfully." });
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


        [HttpPost("update_dailyconsumption")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateDailyConsumption([FromBody] UpdateDailyConsumptionRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_dailyconsumption_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@dailycons_id", request.Dailycons_id);
                    parameters.Add("@dailycons_date", request.Dailycons_date);
                    parameters.Add("@product_id", request.product_id);
                    parameters.Add("@unit_id", request.unit_id);
                    parameters.Add("@quantityconsumed", request.quantityconsumed);
                    parameters.Add("@purpose", request.purpose);
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
                    return NotFound($"Daily Consumption with ID {request.Dailycons_id} not found");
                else
                    return Ok(new { message = "Daily Consumption Updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }



        [HttpGet("dailyconsumption_list")]
        public async Task<ActionResult<IEnumerable<DailyConsumption_List>>> GetDailyConsumptionList()
        {
            var list = new List<DailyConsumption_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_dailyconsumption_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new DailyConsumption_List
                                {
                                    Dailycons_id = reader.GetInt64(0),
                                    Dailycons_date = reader.GetDateTime(1),
                                    product_name = reader.GetString(2),
                                    unit_name = reader.GetString(3),
                                    quantityconsumed = reader.GetDecimal(4),
                                    purpose = reader.GetString(5),
                                    Fin_Year_Name = reader.GetString(6),
                                    Comp_Name = reader.GetString(7),
                                    Created_Date = reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(9) ? "" : reader.GetDateTime(9).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(10) ? "" : reader.GetString(10),
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




        [HttpGet("dailyconsumption/{id}")]
        public async Task<ActionResult<SingleDailyConsumption>> GetDailyConsumptionById(long id)
        {
            SingleDailyConsumption? daily = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_dailyconsumption_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@Dailycons_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                daily = new SingleDailyConsumption
                                {
                                    Dailycons_id = reader.GetInt64(0),
                                    Dailycons_date = reader.GetDateTime(1),
                                    product_id = reader.GetInt64(2),
                                    unit_id = reader.GetInt64(3),
                                    quantityconsumed = reader.GetDecimal(4),
                                    purpose = reader.GetString(5),
                                    Fin_Year_Id = reader.GetInt64(6),
                                    Comp_Id = reader.GetInt64(7),
                                    Created_Date = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                                    Updated_Date = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                                    User_Id = reader.IsDBNull(10) ? 0 : reader.GetInt64(10),
                                };
                            }
                        }
                    }
                }

                if (daily == null)
                    return NotFound($"Daily Consumption with ID {id} not found");

                return Ok(daily);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }









        public class AddDailyConsumptionRequest
        {
            public long Dailycons_id { get; set; } = 0;
            public DateTime Dailycons_date { get; set; }
            public long product_id { get; set; } = 0;
            public long unit_id { get; set; } = 0;
            public decimal quantityconsumed { get; set; } = 0;
            public string purpose { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class UpdateDailyConsumptionRequest
        {
            public long Dailycons_id { get; set; } = 0;
            public DateTime Dailycons_date { get; set; }
            public long product_id { get; set; } = 0;
            public long unit_id { get; set; } = 0;
            public decimal quantityconsumed { get; set; } = 0;
            public string purpose { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class DailyConsumption_List
        {
            public long Dailycons_id { get; set; } = 0;
            public DateTime Dailycons_date { get; set; }
            public string product_name { get; set; } = "";
            public string unit_name { get; set; } = "";
            public decimal quantityconsumed { get; set; } = 0;
            public string purpose { get; set; } = "";
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }

        public class SingleDailyConsumption
        {
            public long Dailycons_id { get; set; } = 0;
            public DateTime Dailycons_date { get; set; }
            public long product_id { get; set; } = 0;
            public long unit_id { get; set; } = 0;
            public decimal quantityconsumed { get; set; } = 0;
            public string purpose { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

    }
}
