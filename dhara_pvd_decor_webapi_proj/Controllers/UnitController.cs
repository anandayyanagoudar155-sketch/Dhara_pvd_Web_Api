using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitController : Controller
    {
        private readonly IConfiguration _configuration;
        public UnitController(IConfiguration configuration)
        {

            _configuration = configuration;

        }

        [HttpPost("insert_unit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Addunit([FromBody] AddUnitRequest request)
        {

            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {

                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_unit_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@unit_id", 0);
                        command.Parameters.AddWithValue("@unit_name", request.UnitName);
                        command.Parameters.AddWithValue("@unit_desc", request.UnitDesc);
                        command.Parameters.AddWithValue("@is_active", request.IsActive);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@created_by", request.Created_by);
                        command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "unit Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add unit." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "unit name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }





        [HttpDelete("Deleteunit/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deleteunit(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_unit_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@unit_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "unit deleted successfully." });
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



        [HttpPost("Updateunit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> Updateunit([FromBody] UpdateUnitRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_unit_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@unit_id", request.UnitId);
                    parameters.Add("@unit_name", request.UnitName);
                    parameters.Add("@unit_desc", request.UnitDesc);
                    parameters.Add("@is_active", request.IsActive);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@created_by", request.Created_by);
                    parameters.Add("@modified_by", request.Modified_by);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"unit with ID {request.UnitId} not found");
                else
                    return Ok(new { message = "unit updated successfully." });
            }


            catch (SqlException ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }



        [HttpGet("unit_list")]
        public async Task<ActionResult<IEnumerable<Unit_list>>> Get_unit_list()
        {
            var unit_list = new List<Unit_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_unit_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var unit = new Unit_list
                                {
                                    UnitId = reader.GetInt64(0),
                                    UnitName = reader.GetString(1),
                                    UnitDesc = reader.GetString(2),
                                    IsActive = reader.GetBoolean(3),
                                    Created_date = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(5) ? "" : reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    Created_by = reader.GetInt64(6),
                                    Modified_by = reader.IsDBNull(7) ? 0 : reader.GetInt64(7),
                                    Created_by_name = reader.GetString(8),
                                    Modified_by_name = reader.IsDBNull(9) ? "" : reader.GetString(9)
                                };

                                unit_list.Add(unit);
                            }
                        }
                    }
                }

                return Ok(unit_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("unit/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<Single_Unit_list>> Get_unit_by_id(long id)
        {
            try
            {
                Single_Unit_list? unit = null;
                var connectionstring = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_unit_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@unit_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                unit = new Single_Unit_list
                                {
                                    UnitId = reader.GetInt64(0),
                                    UnitName = reader.GetString(1),
                                    UnitDesc = reader.GetString(2),
                                    IsActive = reader.GetBoolean(3),
                                    Created_date = reader.GetDateTime(4),
                                    Updated_date = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                                    Created_by = reader.IsDBNull(6) ? 0 : reader.GetInt64(6),
                                    Modified_by = reader.IsDBNull(7) ? 0 : reader.GetInt64(7)

                                };
                            }
                        }
                    }
                }

                if (unit == null)
                    return NotFound($"unit with ID {id} not found");

                return Ok(unit);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("dropdown_unit_list")]
        public async Task<ActionResult<IEnumerable<drop_Unit_list>>> Get_drop_unitlist()
        {
            var unit_list = new List<drop_Unit_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_unit_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "unitlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var unit = new drop_Unit_list
                                {
                                    UnitId = reader.GetInt64(0),
                                    UnitName = reader.GetString(1)
                                };

                                unit_list.Add(unit);
                            }
                        }
                    }
                }

                return Ok(unit_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        public class AddUnitRequest
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";
            public string UnitDesc { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }


        public class UpdateUnitRequest
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";
            public string UnitDesc { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }

        public class Unit_list
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";
            public string UnitDesc { get; set; } = "";
            public bool? IsActive { get; set; }
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";

        }


        public class Single_Unit_list
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";
            public string UnitDesc { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;

        }


        public class drop_Unit_list
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";

        }

    }
}
