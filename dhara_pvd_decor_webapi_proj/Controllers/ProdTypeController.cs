using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdTypeController : Controller
    {
        private readonly IConfiguration _configuration;

        public ProdTypeController(IConfiguration configuration)
        {

            _configuration = configuration;

        }

        [HttpPost("insert_prodtype")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddProdtype([FromBody] AddProdtypeRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_prodtype_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@prodtype_id", request.Prodtype_Id);
                        command.Parameters.AddWithValue("@prodtype_name", request.Prodtype_Name);
                        command.Parameters.AddWithValue("@prodtype_desc", request.Prodtype_Desc);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Product type added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add product type." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "Product type name already exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_prodtype/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProdtype(long id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_prodtype_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@prodtype_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Product type deleted successfully." });
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


        [HttpPost("update_prodtype")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateProdtype([FromBody] UpdateProdtypeRequest request)
        {
            int rowsAffected;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_prodtype_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@prodtype_id", request.Prodtype_Id);
                    parameters.Add("@prodtype_name", request.Prodtype_Name);
                    parameters.Add("@prodtype_desc", request.Prodtype_Desc);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_id);

                    rowsAffected = await connection.ExecuteAsync(
                        spName,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }

                if (rowsAffected == 0)
                    return NotFound($"Product type with Id {request.Prodtype_Id} not found");
                else
                    return Ok(new { message = "Product type updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        [HttpGet("prodtype_list")]
        public async Task<ActionResult<IEnumerable<Prodtype_list>>> GetProdtypeList()
        {
            var prodtypeList = new List<Prodtype_list>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_prodtype_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var prodtype = new Prodtype_list
                                {
                                    Prodtype_Id = reader.GetInt64(0),
                                    Prodtype_Name = reader.GetString(1),
                                    Prodtype_Desc = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Created_date = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(4) ? "" : reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(5) ? "" : reader.GetString(5)
                                };

                                prodtypeList.Add(prodtype);
                            }
                        }
                    }
                }

                return Ok(prodtypeList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        [HttpGet("prodtype/{id}")]
        public async Task<ActionResult<Single_Prodtype_list>> GetProdtypeById(long id)
        {
            Single_Prodtype_list? prodtype = null;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_prodtype_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@prodtype_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                prodtype = new Single_Prodtype_list
                                {
                                    Prodtype_Id = reader.GetInt64(0),
                                    Prodtype_Name = reader.GetString(1),
                                    Prodtype_Desc = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Created_date = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                                    Updated_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                    User_id = reader.IsDBNull(5) ? 0 : reader.GetInt64(5)
                                };
                            }
                        }
                    }
                }

                if (prodtype == null)
                    return NotFound($"Product type with Id {id} not found");

                return Ok(prodtype);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        [HttpGet("dropdown_prodtype_list")]
        public async Task<ActionResult<IEnumerable<drop_Prodtype_list>>> GetDropProdtypeList()
        {
            var prodtypeList = new List<drop_Prodtype_list>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_prodtype_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "prodtypelist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var prodtype = new drop_Prodtype_list
                                {
                                    Prodtype_Id = reader.GetInt64(0),
                                    Prodtype_Name = reader.GetString(1)
                                };

                                prodtypeList.Add(prodtype);
                            }
                        }
                    }
                }

                return Ok(prodtypeList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }



        public class AddProdtypeRequest
        {
            public long Prodtype_Id { get; set; } = 0;
            public string Prodtype_Name { get; set; } = "";
            public string Prodtype_Desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long? User_id { get; set; } = 0;
        }


        public class UpdateProdtypeRequest
        {
            public long Prodtype_Id { get; set; } = 0;
            public string Prodtype_Name { get; set; } = "";
            public string Prodtype_Desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }

        public class Prodtype_list
        {
            public long Prodtype_Id { get; set; } = 0;
            public string Prodtype_Name { get; set; } = "";
            public string Prodtype_Desc { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";

        }


        public class Single_Prodtype_list
        {
            public long Prodtype_Id { get; set; } = 0;
            public string Prodtype_Name { get; set; } = "";
            public string Prodtype_Desc { get; set; } = "";
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class drop_Prodtype_list
        {
            public long Prodtype_Id { get; set; } = 0;
            public string Prodtype_Name { get; set; } = "";

        }
    }
}
