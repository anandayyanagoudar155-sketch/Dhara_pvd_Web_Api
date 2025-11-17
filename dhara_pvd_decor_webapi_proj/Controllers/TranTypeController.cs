using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranTypeController : Controller
    {
        private readonly IConfiguration _configuration;

        public TranTypeController(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        [HttpPost("insert_trans_type")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddTransType([FromBody] AddTrans_typeRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_trans_type_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@trans_id", 0);
                        command.Parameters.AddWithValue("@transtype_name", request.Transtype_name);
                        command.Parameters.AddWithValue("@transtype_desc", request.Transtype_desc);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Transaction Type added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add Transaction Type." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Transaction Type already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteTransType/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTransType(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_trans_type_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@trans_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Transaction Type deleted successfully." });
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





        [HttpPost("UpdateTransType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateTransType([FromBody] UpdateTrans_typeRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_trans_type_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@trans_id", request.Trans_id);
                    parameters.Add("@transtype_name", request.Transtype_name);
                    parameters.Add("@transtype_desc", request.Transtype_desc);
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
                    return NotFound($"Transaction Type with ID {request.Trans_id} not found");
                else
                    return Ok(new { message = "Transaction Type updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("trans_type_list")]
        public async Task<ActionResult<IEnumerable<trans_type_List>>> Get_trans_type_list()
        {
            var transTypes = new List<trans_type_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_trans_type_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new trans_type_List
                                {
                                    Trans_id = reader.GetInt64(0),
                                    Transtype_name = reader.GetString(1),
                                    Transtype_desc = reader.GetString(2),
                                    Created_Date = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(4) ? "" : reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                };

                                transTypes.Add(item);
                            }
                        }
                    }
                }

                return Ok(transTypes);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }






        [HttpGet("trans_type/{id}")]
        public async Task<ActionResult<Singletrans_type>> Get_trans_type_by_id(long id)
        {
            Singletrans_type? item = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_trans_type_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@trans_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                item = new Singletrans_type
                                {
                                    Trans_id = reader.GetInt64(0),
                                    Transtype_name = reader.GetString(1),
                                    Transtype_desc = reader.GetString(2),
                                    Created_Date = reader.GetDateTime(3),
                                    Updated_Date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                    User_Id = reader.IsDBNull(5) ? 0 : reader.GetInt64(5),
                                };
                            }
                        }
                    }
                }

                if (item == null)
                    return NotFound($"Transaction Type with ID {id} not found");

                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("dropdown_trans_type_list")]
        public async Task<ActionResult<IEnumerable<Drop_trans_type_List>>> Get_drop_trans_type_list()
        {
            var list = new List<Drop_trans_type_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_trans_type_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "trans_type_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new Drop_trans_type_List
                                {
                                    Trans_id = reader.GetInt64(0),
                                    Transtype_name = reader.GetString(1)
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



        public class AddTrans_typeRequest
        {
            public long Trans_id { get; set; } = 0;
            public string Transtype_name { get; set; } = "";
            public string Transtype_desc { get; set; } = "";
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class UpdateTrans_typeRequest
        {
            public long Trans_id { get; set; } = 0;
            public string Transtype_name { get; set; } = "";
            public string Transtype_desc { get; set; } = "";
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class trans_type_List
        {
            public long Trans_id { get; set; } = 0;
            public string Transtype_name { get; set; } = "";
            public string Transtype_desc { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }

        public class Singletrans_type
        {
            public long Trans_id { get; set; } = 0;
            public string Transtype_name { get; set; } = "";
            public string Transtype_desc { get; set; } = "";
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_trans_type_List
        {
            public long Trans_id { get; set; } = 0;
            public string Transtype_name { get; set; } = "";
        }



    }
}
