using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayTypeController : Controller
    {
        private readonly IConfiguration _configuration;

        public PayTypeController(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        [HttpPost("insert_paytype")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPaytype([FromBody] AddPaytypeRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_paytype_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@paytype_id", request.Paytype_Id);
                        command.Parameters.AddWithValue("@paytype_name", request.Paytype_Name);
                        command.Parameters.AddWithValue("@paytype_desc", request.Paytype_Desc);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Paytype added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Paytype." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Paytype name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpPost("update_paytype")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePaytype([FromBody] UpdatePaytypeRequest request)
        {
            int rowsAffected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_paytype_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@paytype_id", request.Paytype_Id);
                    parameters.Add("@paytype_name", request.Paytype_Name);
                    parameters.Add("@paytype_desc", request.Paytype_Desc);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_id);

                    rowsAffected = await connection.ExecuteAsync(spname, parameters, commandType: CommandType.StoredProcedure);
                }

                if (rowsAffected == 0)
                    return NotFound(new { message = $"Paytype with ID {request.Paytype_Id} not found." });
                else
                    return Ok(new { message = "Paytype updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("delete_paytype/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePaytype(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_paytype_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@paytype_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Paytype deleted successfully." });
                        else
                            return NotFound(new { errorMessage = "No record deleted" });
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


        [HttpGet("paytype_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Paytype_list>>> GetPaytypeList()
        {
            var paytypeList = new List<Paytype_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_paytype_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var paytype = new Paytype_list
                                {
                                    Paytype_Id = reader.GetInt64(0),
                                    Paytype_Name = reader.GetString(1),
                                    Paytype_Desc = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Created_date = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(4) ? "" : reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                };
                                paytypeList.Add(paytype);
                            }
                        }
                    }
                }

                return Ok(paytypeList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }



        [HttpGet("paytype/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Single_Paytype_list>> GetPaytypeById(long id)
        {
            Single_Paytype_list? paytype = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_paytype_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@paytype_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                paytype = new Single_Paytype_list
                                {
                                    Paytype_Id = reader.GetInt64(0),
                                    Paytype_Name = reader.GetString(1),
                                    Paytype_Desc = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Created_date = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                                    Updated_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                    User_id = reader.IsDBNull(5) ? 0 : reader.GetInt64(5)
                                };
                            }
                        }
                    }
                }

                if (paytype == null)
                    return NotFound(new { message = $"Paytype with Id {id} not found." });

                return Ok(paytype);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        [HttpGet("dropdown_paytype_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<drop_Paytype_list>>> GetDropdownPaytypeList()
        {
            var paytypeList = new List<drop_Paytype_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_paytype_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "paytypelist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var paytype = new drop_Paytype_list
                                {
                                    Paytype_Id = reader.GetInt64(0),
                                    Paytype_Name = reader.GetString(1)
                                };
                                paytypeList.Add(paytype);
                            }
                        }
                    }
                }

                return Ok(paytypeList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }




        public class AddPaytypeRequest
        {
            public long Paytype_Id { get; set; } = 0;
            public string Paytype_Name { get; set; } = "";
            public string Paytype_Desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long? User_id { get; set; } = 0;
        }


        public class UpdatePaytypeRequest
        {
            public long Paytype_Id { get; set; } = 0;
            public string Paytype_Name { get; set; } = "";
            public string Paytype_Desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }

        public class Paytype_list
        {
            public long Paytype_Id { get; set; } = 0;
            public string Paytype_Name { get; set; } = "";
            public string Paytype_Desc { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";

        }


        public class Single_Paytype_list
        {
            public long Paytype_Id { get; set; } = 0;
            public string Paytype_Name { get; set; } = "";
            public string Paytype_Desc { get; set; } = "";
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class drop_Paytype_list
        {
            public long Paytype_Id { get; set; } = 0;
            public string Paytype_Name { get; set; } = "";


        }
    }
}
