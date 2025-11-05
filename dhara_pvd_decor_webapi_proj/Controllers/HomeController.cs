using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request) {

            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {

                using (var connection = new SqlConnection(connectionstring))
                {

                    string spName = "sp_user_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "login");
                    parameters.Add("@user_name", request.Email);
                    parameters.Add("@user_password", request.Password);

                    var user = await connection.QueryFirstOrDefaultAsync<User>(
                    spName,
                    parameters,
                    commandType: CommandType.StoredProcedure
                    );

                    if (user != null)
                    {
                        return Ok(new
                        {
                            user_id = user.user_id,
                            user_name = user.user_name,
                            user_role = user.user_role,
                            is_login = user.is_login
                        });
                    }
                    else
                    {
                        return BadRequest(new { message = "Invalid Email Or Password." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });

            }

        }



        [HttpPost("Addcountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Addbooking([FromBody] AddCountryRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_country_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@country_id", 0);
                        command.Parameters.AddWithValue("@country_name", request.Country_name);
                        command.Parameters.AddWithValue("@created_date", request.Created_date.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Country Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Country." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Country name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteCountry/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCountry(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_country_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@country_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Country deleted successfully." });
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



        [HttpPost("UpdateCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> UpdateCountry([FromBody] UpdateCountryRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_country_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@country_id", request.Country_id);
                    parameters.Add("@country_name", request.Country_name);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"Country with ID {request.Country_id} not found");
                else
                    return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }



        [HttpGet("country_list")]
        public async Task<ActionResult<IEnumerable<country_list>>> Get_country_list()
        {
            var country_list = new List<country_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_country_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var country = new country_list
                                {
                                    Country_id = reader.GetInt64(0),
                                    Country_name = reader.GetString(1),
                                    Created_date = reader.GetDateTime(2).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(3) ? "" : reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                };

                                country_list.Add(country);
                            }
                        }
                    }
                }

                return Ok(country_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("country/{id}")]
        public async Task<ActionResult<Single_country_list>> Get_country_by_id(long id)
        {
            Single_country_list? country = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_country_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@country_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                country = new Single_country_list
                                {
                                    Country_id = reader.GetInt64(0),
                                    Country_name = reader.GetString(1),
                                    Created_date = reader.GetDateTime(2),
                                    Updated_date = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                                    User_id = reader.IsDBNull(4) ? 0 : reader.GetInt64(4),
                                };
                            }
                        }
                    }
                }

                if (country == null)
                    return NotFound($"Country with ID {id} not found");

                return Ok(country);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("dropdown_country_list")]
        public async Task<ActionResult<IEnumerable<drop_country_list>>> Get_drop_countrylist()
        {
            var country_list = new List<drop_country_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_country_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "countrylist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var country = new drop_country_list
                                {
                                    Country_id = reader.GetInt64(0),
                                    Country_name = reader.GetString(1)
                                };

                                country_list.Add(country);  
                            }
                        }
                    }
                }

                return Ok(country_list);  
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpPost("insert_state")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AddState([FromBody] AddStateRequest request)
        {

            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

           try 
            {
                using (SqlConnection connection= new SqlConnection(connectionstring))
                {

                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand ("sp_state_mast_ins_upd_del", connection)) 
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@state_id", request.State_id);
                        command.Parameters.AddWithValue("@state_name", request.State_name);
                        command.Parameters.AddWithValue("@country_id", request.Country_id);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "State Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add State." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "State name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteState/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteState(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_state_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@state_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "State deleted successfully." });
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


        [HttpPost("UpdateState")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> UpdateState([FromBody] UpdateStateRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_state_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@state_id", request.State_id);
                    parameters.Add("@state_name", request.State_name);
                    parameters.Add("@country_id", request.Country_id);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"State with ID {request.State_id} not found");
                else
                    return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }



        [HttpGet("state_list")]
        public async Task<ActionResult<IEnumerable<state_list>>> Get_state_list()
        {
            var state_list = new List<state_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_state_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var state = new state_list
                                {
                                    State_id = reader.GetInt64(0),
                                    State_name = reader.GetString(1),
                                    Country_name = reader.GetString(2),
                                    Created_date = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(4) ? "" : reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                };

                                state_list.Add(state);
                            }
                        }
                    }
                }

                return Ok(state_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        public class UserLoginRequest
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";

        }

        public class User
        {
            public long user_id { get; set; } = 0;
            public string user_name { get; set; } = "";
            public string user_role { get; set; } = "";
            public bool is_login { get; set; } = false;
        }

        public class AddCountryRequest { 
        
            public long Country_id { get; set; }=0;
            public string Country_name { get; set; } = "";
            public DateTime Created_date { get; set; } 
            public long User_id { get; set; } = 0;
        
        }

        public class country_list {
            public long Country_id { get; set; } = 0;
            public string Country_name { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";

        }


        public class Single_country_list
        {
            public long Country_id { get; set; } = 0;
            public string Country_name { get; set; } = "";
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class UpdateCountryRequest
        {
            public long Country_id { get; set; } = 0;
            public string Country_name { get; set; } = "";
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; } 
            public long User_id { get; set; } = 0;

        }


        public class drop_country_list 
        {
            public long Country_id { get; set; } = 0;
            public string Country_name { get; set; } = "";

        }

        public class AddStateRequest
        {
            public long State_id { get; set; } = 0;
            public string State_name { get; set; } ="";
            public long Country_id { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class UpdateStateRequest
        {
            public long State_id { get; set; } = 0;
            public string State_name { get; set; } = "";
            public long Country_id { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class state_list
        {
            public long State_id { get; set; } = 0;
            public string State_name { get; set; } = "";
            public string Country_name { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";

        }
    }
}
