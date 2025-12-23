using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HsnController : Controller
    {
        private readonly IConfiguration _configuration;

        public HsnController(IConfiguration configuration)
        {

            _configuration = configuration;

        }

        [HttpPost("insert_hsn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Add_hsn([FromBody] AddHsnRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection")
;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_hsn_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@hsn_id", 0);
                        command.Parameters.AddWithValue("@hsn_code", request.HsnCode);
                        command.Parameters.AddWithValue("@cgst_perc", request.Cgst_perc);
                        command.Parameters.AddWithValue("@sgst_perc", request.Sgst_perc);
                        command.Parameters.AddWithValue("@igst_perc", request.Igst_perc);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@updated_date", request.Updated_date);
                        command.Parameters.AddWithValue("@created_by", request.Created_by);
                        command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "hsn code Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add hsn code." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "hsn code already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("delete_hsn/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHsn(long id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_hsn_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@hsn_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "HSN deleted successfully." });
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


        [HttpPost("update_hsn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHsn([FromBody] UpdateHsnRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_hsn_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@hsn_id", request.HsnId);
                    parameters.Add("@hsn_code", request.HsnCode);
                    parameters.Add("@cgst_perc", request.Cgst_perc);
                    parameters.Add("@sgst_perc", request.Sgst_perc);
                    parameters.Add("@igst_perc", request.Igst_perc);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@created_by", request.Created_by);
                    parameters.Add("@modified_by", request.Modified_by);

                    int rowsAffected = await connection.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);

                    if (rowsAffected == 0)
                        return NotFound(new { message = $"HSN ID {request.HsnId} not found." });


                    return Ok(new { message = "HSN updated successfully." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }




        [HttpGet("hsn_list")]
        public async Task<ActionResult<IEnumerable<Hsn_list>>> GetHsnList()
        {
            var hsnList = new List<Hsn_list>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_hsn_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var hsn = new Hsn_list
                                {
                                    HsnId = reader.GetInt64(0),
                                    HsnCode = reader.GetString(1),
                                    Cgst_perc = reader.GetDecimal(2),
                                    Sgst_perc = reader.GetDecimal(3),
                                    Igst_perc = reader.GetDecimal(4),
                                    Created_date = reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(6) ? "" : reader.GetDateTime(6).ToString("yyyy-MM-dd"),
                                    Created_by = reader.GetInt64(7),
                                    Modified_by = reader.IsDBNull(8) ? 0 : reader.GetInt64(8),
                                    Created_by_name = reader.GetString(9),
                                    Modified_by_name = reader.IsDBNull(10) ? "" : reader.GetString(10)
                                };

                                hsnList.Add(hsn);
                            }
                        }
                    }
                }

                return Ok(hsnList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }



        [HttpGet("hsn/{id}")]
        public async Task<ActionResult<Single_Hsn_list>> GetHsnById(long id)
        {
            Single_Hsn_list? hsn = null;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_hsn_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@hsn_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                hsn = new Single_Hsn_list
                                {
                                    HsnId = reader.GetInt64(0),
                                    HsnCode = reader.GetString(1),
                                    Cgst_perc = reader.GetDecimal(2),
                                    Sgst_perc = reader.GetDecimal(3),
                                    Igst_perc = reader.GetDecimal(4),
                                    Created_date = reader.GetDateTime(5),
                                    Updated_date = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                                    Created_by = reader.IsDBNull(7) ? 0 : reader.GetInt64(7),
                                    Modified_by = reader.IsDBNull(8) ? 0 : reader.GetInt64(8)
                                };
                            }
                        }
                    }
                }

                if (hsn == null)
                    return NotFound(new { message = $"HSN ID {id} not found" });

                return Ok(hsn);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }





        [HttpGet("dropdown_hsn_list")]
        public async Task<ActionResult<IEnumerable<drop_Hsn_list>>> GetDropHsnList()
        {
            var hsnList = new List<drop_Hsn_list>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_hsn_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "hsnlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var hsn = new drop_Hsn_list
                                {
                                    HsnId = reader.GetInt64(0),
                                    HsnCode = reader.GetString(1)
                                };

                                hsnList.Add(hsn);
                            }
                        }
                    }
                }

                return Ok(hsnList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }




        public class AddHsnRequest
        {
            public long HsnId { get; set; } = 0;
            public string HsnCode { get; set; } = "";
            public decimal Cgst_perc { get; set; } = 0;
            public decimal Sgst_perc { get; set; } = 0;
            public decimal Igst_perc { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }


        public class UpdateHsnRequest
        {
            public long HsnId { get; set; } = 0;
            public string HsnCode { get; set; } = "";
            public decimal Cgst_perc { get; set; } = 0;
            public decimal Sgst_perc { get; set; } = 0;
            public decimal Igst_perc { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }

        public class Hsn_list
        {
            public long HsnId { get; set; } = 0;
            public string HsnCode { get; set; } = "";
            public decimal Cgst_perc { get; set; } = 0;
            public decimal Sgst_perc { get; set; } = 0;
            public decimal Igst_perc { get; set; } = 0;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";

        }


        public class Single_Hsn_list
        {
            public long HsnId { get; set; } = 0;
            public string HsnCode { get; set; } = "";
            public decimal Cgst_perc { get; set; } = 0;
            public decimal Sgst_perc { get; set; } = 0;
            public decimal Igst_perc { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;

        }


        public class drop_Hsn_list
        {
            public long HsnId { get; set; } = 0;
            public string HsnCode { get; set; } = "";

        }


    }
}
