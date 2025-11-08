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
using Microsoft.AspNetCore.Connections;
using System.Reflection.PortableExecutable;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {

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
        public async Task<IActionResult> Addcountry([FromBody] AddCountryRequest request)
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
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {

                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_state_mast_ins_upd_del", connection))
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


        [HttpGet("state/{id}")]
        public async Task<ActionResult<Single_state_list>> Get_state_by_id(long id)
        {
            Single_state_list? state = null;
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
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@state_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                state = new Single_state_list
                                {
                                    State_id = reader.GetInt64(0),
                                    State_name = reader.GetString(1),
                                    Country_id = reader.GetInt64(2),
                                    Created_date = reader.GetDateTime(3),
                                    Updated_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                    User_id = reader.IsDBNull(5) ? 0 : reader.GetInt64(5),
                                };
                            }
                        }
                    }
                }

                if (state == null)
                    return NotFound($"state with ID {id} not found");

                return Ok(state);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("dropdown_state_list")]
        public async Task<ActionResult<IEnumerable<drop_state_list>>> Get_drop_statelist()
        {
            var state_list = new List<drop_state_list>();
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
                        command.Parameters.AddWithValue("@action", "state_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var state = new drop_state_list
                                {
                                    State_id = reader.GetInt64(0),
                                    State_name = reader.GetString(1)
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



        [HttpPost("insert_city")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AddCity([FromBody] AddCityRequest request)
        {

            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {

                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_city_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@city_id", 0);
                        command.Parameters.AddWithValue("@city_name", request.City_name);
                        command.Parameters.AddWithValue("@state_id", request.State_id);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@updated_date", request.Updated_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "City Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add City." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "city name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteCity/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteCity(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_city_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@city_id", id);

                        int rows_Affected = await command.ExecuteNonQueryAsync();

                        if (rows_Affected > 0)
                            return Ok(new { message = "city deleted successfully." });
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




        [HttpPost("UpdateCity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> Updatecity([FromBody] UpdatecityRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_city_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@city_id", request.City_id);
                    parameters.Add("@city_name", request.City_name);
                    parameters.Add("@state_id", request.State_id);
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
                    return NotFound($"city with ID {request.City_id} not found");
                else
                    return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }


        [HttpGet("city_list")]
        public async Task<ActionResult<IEnumerable<city_list>>> Get_city_list()
        {
            var city_list = new List<city_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_city_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var city = new city_list
                                {
                                    City_id = reader.GetInt64(0),
                                    City_name = reader.GetString(1),
                                    State_name = reader.GetString(2),
                                    Created_date = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(4) ? "" : reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                };

                                city_list.Add(city);
                            }
                        }
                    }
                }

                return Ok(city_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("city/{id}")]
        public async Task<ActionResult<Single_city_list>> Get_city_by_id(long id)
        {
            Single_city_list? city = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_city_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@city_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                city = new Single_city_list
                                {
                                    City_id = reader.GetInt64(0),
                                    City_name = reader.GetString(1),
                                    State_id = reader.GetInt64(2),
                                    Created_date = reader.GetDateTime(3),
                                    Updated_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                    User_id = reader.IsDBNull(5) ? 0 : reader.GetInt64(5),
                                };
                            }
                        }
                    }
                }

                if (city == null)
                    return NotFound($"city with ID {id} not found");

                return Ok(city);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("dropdown_city_list")]
        public async Task<ActionResult<IEnumerable<drop_city_list>>> Get_drop_citylist()
        {
            var city_list = new List<drop_city_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_city_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "citymastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var city = new drop_city_list
                                {
                                    City_id = reader.GetInt64(0),
                                    City_name = reader.GetString(1)
                                };

                                city_list.Add(city);
                            }
                        }
                    }
                }

                return Ok(city_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpPost("InsertCompany")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AddCompany([FromBody] AddCompanyRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_company_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                        command.Parameters.AddWithValue("@comp_code", request.Comp_code);
                        command.Parameters.AddWithValue("@comp_name", request.Comp_name);
                        command.Parameters.AddWithValue("@comp_short_name", request.Comp_short_name);
                        command.Parameters.AddWithValue("@comp_type", request.Comp_type);
                        command.Parameters.AddWithValue("@comp_desc", request.Comp_desc);
                        command.Parameters.AddWithValue("@cin_number", request.Cin_number);
                        command.Parameters.AddWithValue("@gst_number", request.Gst_number);
                        command.Parameters.AddWithValue("@pan_number", request.Pan_number);
                        command.Parameters.AddWithValue("@contperson_name", request.Contperson_name);
                        command.Parameters.AddWithValue("@contact_email", request.Contact_email);
                        command.Parameters.AddWithValue("@contact_phone", request.Contact_phone);
                        command.Parameters.AddWithValue("@address_line1", request.Address_line1);
                        command.Parameters.AddWithValue("@address_line2", request.Address_line2);
                        command.Parameters.AddWithValue("@city_id", request.City_id);
                        command.Parameters.AddWithValue("@pincode", request.Pincode);
                        command.Parameters.AddWithValue("@is_active", request.Is_active);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@updated_date", request.Updated_date);
                        command.Parameters.AddWithValue("@logo_path", request.Logo_path);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Company Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Company." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Company name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("Deletecompany/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deletecompany(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_company_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@comp_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "company deleted successfully." });
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



        [HttpPost("Updatecompany")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> Updatecompany([FromBody] UpdateCompanyRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_company_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@comp_id", request.Comp_id);
                    parameters.Add("@comp_code", request.Comp_code);
                    parameters.Add("@comp_name", request.Comp_name);
                    parameters.Add("@comp_short_name", request.Comp_short_name);
                    parameters.Add("@comp_type", request.Comp_type);
                    parameters.Add("@comp_desc", request.Comp_desc);
                    parameters.Add("@cin_number", request.Cin_number);
                    parameters.Add("@gst_number", request.Gst_number);
                    parameters.Add("@pan_number", request.Pan_number);
                    parameters.Add("@contperson_name", request.Contperson_name);
                    parameters.Add("@contact_email", request.Contact_email);
                    parameters.Add("@contact_phone", request.Contact_phone);
                    parameters.Add("@address_line1", request.Address_line1);
                    parameters.Add("@address_line2", request.Address_line2);
                    parameters.Add("@city_id", request.City_id);
                    parameters.Add("@pincode", request.Pincode);
                    parameters.Add("@is_active", request.Is_active);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@logo_path", request.Logo_path);
                    parameters.Add("@user_id", request.User_id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"company with ID {request.Comp_id} not found");
                else
                    return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }


        [HttpGet("company_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<company_list>>> Get_CompanyList()
        {
            var company_list = new List<company_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_company_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var company = new company_list
                                {
                                    Comp_id = reader.GetInt64(0),
                                    Comp_code = reader.GetString(1),
                                    Comp_name = reader.GetString(2),
                                    Comp_short_name = reader.GetString(3),
                                    Comp_type = reader.GetString(4),
                                    Comp_desc = reader.GetString(5),
                                    Cin_number = reader.GetString(6),
                                    Gst_number = reader.GetString(7),
                                    Pan_number = reader.GetString(8),
                                    Contperson_name = reader.GetString(9),
                                    Contact_email = reader.GetString(10),
                                    Contact_phone = reader.GetString(11),
                                    Address_line1 = reader.GetString(12),
                                    Address_line2 = reader.GetString(13),
                                    City_name = reader.GetString(14),
                                    Pincode = reader.GetString(15),
                                    Is_active = reader.GetBoolean(16),
                                    Created_date = reader.GetDateTime(17).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(18) ? "" : reader.GetDateTime(18).ToString("yyyy-MM-dd"),
                                    Logo_path = reader.IsDBNull(19) ? "" : reader.GetString(19),
                                    User_name = reader.IsDBNull(20) ? "" : reader.GetString(20)
                                };

                                company_list.Add(company);
                            }
                        }

                    }
                }

                return Ok(company_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("company/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<single_company_list>> Gete_Company_by_id(long id)
        {
            single_company_list? company = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_company_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@comp_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                company = new single_company_list
                                {
                                    Comp_id = reader.GetInt64(0),
                                    Comp_code = reader.GetString(1),
                                    Comp_name = reader.GetString(2),
                                    Comp_short_name = reader.GetString(3),
                                    Comp_type = reader.GetString(4),
                                    Comp_desc = reader.GetString(5),
                                    Cin_number = reader.GetString(6),
                                    Gst_number = reader.GetString(7),
                                    Pan_number = reader.GetString(8),
                                    Contperson_name = reader.GetString(9),
                                    Contact_email = reader.GetString(10),
                                    Contact_phone = reader.GetString(11),
                                    Address_line1 = reader.GetString(12),
                                    Address_line2 = reader.GetString(13),
                                    City_id = reader.GetInt64(14),
                                    Pincode = reader.GetString(15),
                                    Is_active = reader.GetBoolean(16),
                                    Created_date = reader.GetDateTime(17),
                                    Updated_date = reader.IsDBNull(18) ? null : reader.GetDateTime(18),
                                    Logo_path = reader.IsDBNull(19) ? "" : reader.GetString(19),
                                    User_id = reader.IsDBNull(20) ? 0 : reader.GetInt64(20)
                                };
                            }
                        }
                    }
                }

                if (company == null)
                    return NotFound($"company with ID {id} not found");

                return Ok(company);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("dropdown_company_list")]
        public async Task<ActionResult<IEnumerable<drop_state_list>>> Get_drop_companylist()
        {
            var company_list = new List<drop_company_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_company_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "companylist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var company = new drop_company_list
                                {
                                    Comp_id = reader.GetInt64(0),
                                    Comp_name = reader.GetString(1)
                                };

                                company_list.Add(company);
                            }
                        }
                    }
                }

                return Ok(company_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpPost("insert_month")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AddMonth([FromBody] AddMonthRequest request)
        {

            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {

                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_month_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@month_id", 0);
                        command.Parameters.AddWithValue("@month_name", request.Month_name);
                        command.Parameters.AddWithValue("@start_date", request.Start_date);
                        command.Parameters.AddWithValue("@end_date", request.End_date);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "month Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add month." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "month name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("Deletemonth/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deletemonth(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_month_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@month_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "month deleted successfully." });
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



        [HttpPost("Updatemonth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> Updatemonth([FromBody] UpdateMonthRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_month_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@month_id", request.Month_id);
                    parameters.Add("@month_name", request.Month_name);
                    parameters.Add("@start_date", request.Start_date);
                    parameters.Add("@end_date", request.End_date);
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
                    return NotFound($"month with ID {request.Month_id} not found");
                else
                    return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }


        [HttpGet("month_list")]
        public async Task<ActionResult<IEnumerable<month_list>>> Get_month_list()
        {
            var month_list = new List<month_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_month_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var month = new month_list
                                {
                                    Month_id = reader.GetInt64(0),
                                    Month_name = reader.GetString(1),
                                    Start_date = reader.GetDateTime(2).ToString("yyyy-MM-dd"),
                                    End_date = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    Created_date = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(5) ? "" : reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                };

                                month_list.Add(month);
                            }
                        }
                    }
                }

                return Ok(month_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("month/{id}")]
        public async Task<ActionResult<Single_month_list>> Get_month_by_id(long id)
        {
            Single_month_list? month = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_month_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@month_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                month = new Single_month_list
                                {
                                    Month_id = reader.GetInt64(0),
                                    Month_name = reader.GetString(1),
                                    Start_date = reader.GetDateTime(2),
                                    End_date = reader.GetDateTime(3),
                                    Created_date = reader.GetDateTime(4),
                                    Updated_date = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                                    User_id = reader.IsDBNull(6) ? 0 : reader.GetInt64(6),
                                };
                            }
                        }
                    }
                }

                if (month == null)
                    return NotFound($"month with ID {id} not found");

                return Ok(month);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("dropdown_month_list")]
        public async Task<ActionResult<IEnumerable<drop_month_list>>> Get_drop_monthlist()
        {
            var month_list = new List<drop_month_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_month_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "month_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var month = new drop_month_list
                                {
                                    Month_id = reader.GetInt64(0),
                                    Month_name = reader.GetString(1),
                                };

                                month_list.Add(month);
                            }
                        }
                    }
                }

                return Ok(month_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpPost("insert_fin_year")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Addfin_year([FromBody] AddFinYearRequest request)
        {

            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {

                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_fin_year_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "update");
                        command.Parameters.AddWithValue("@fin_year_id", 0);
                        command.Parameters.AddWithValue("@fin_name", request.Fin_name);
                        command.Parameters.AddWithValue("@short_fin_year", request.Short_fin_year);
                        command.Parameters.AddWithValue("@year_start", request.Year_start);
                        command.Parameters.AddWithValue("@year_end", request.Year_end);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "fin year Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add fin year." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "fin year name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }




        [HttpDelete("DeleteFinYear/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteFinYear(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_fin_year_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@fin_year_id", id);

                        int row_Affected = await command.ExecuteNonQueryAsync();

                        if (row_Affected > 0)
                            return Ok(new { message = "fin year deleted successfully." });
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



        [HttpPost("UpdateFinYear")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> UpdateFinYear([FromBody] UpdateFinYearRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_fin_year_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@fin_name", request.Fin_name);
                    parameters.Add("@short_fin_year", request.Short_fin_year);
                    parameters.Add("@year_start", request.Year_start);
                    parameters.Add("@year_end", request.Year_end);
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
                    return NotFound($"FinYaer with ID {request.Fin_year_id} not found");
                else
                    return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }



        [HttpGet("fin_year_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<FinYearlist>>> Get_FinYear_list()
        {
            var finyear_list = new List<FinYearlist>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_fin_year_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var finyear = new FinYearlist
                                {
                                    Fin_year_id = reader.GetInt64(0),
                                    Fin_name = reader.GetString(1),
                                    Short_fin_year = reader.GetString(2),
                                    Year_start = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    Year_end = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    Created_date = reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(6) ? "" : reader.GetDateTime(6).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(7) ? "" : reader.GetString(7)
                                };

                                finyear_list.Add(finyear);
                            }
                        }
                    }
                }

                return Ok(finyear_list);
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("fin_year/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<Single_FinYear_list>> Get_FinYear_By_id(long id) 
        {
            Single_FinYear_list? finyear = null;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_fin_year_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("fin_year_id",id);

                        using (var reader = await command.ExecuteReaderAsync()) 
                        {
                            while(await reader.ReadAsync())
                            {
                                finyear = new Single_FinYear_list
                                {
                                    Fin_year_id=reader.GetInt64(0),
                                    Fin_name=reader.GetString(1),
                                    Short_fin_year=reader.GetString(2),
                                    Year_start=reader.GetDateTime(3),
                                    Year_end=reader.GetDateTime(4),
                                    Created_date=reader.GetDateTime(5),
                                    Updated_date= reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                                    User_id= reader.IsDBNull(7) ? 0 : reader.GetInt64(7)
                                };                                

                            }
                        }
                    }
                }


                if (finyear == null)
                    return NotFound($"state with ID {id} not found");

                return Ok(finyear);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("dropdown_finyear_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<drop_FinYear_list>>> Get_finyear_list() 
        {

            var fin_year_list = new List<drop_FinYear_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_fin_year_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "fin_year_mastlist");

                        using (var reader = await command.ExecuteReaderAsync()) 
                        {
                            while (await reader.ReadAsync())
                            {
                                var fin_year = new drop_FinYear_list
                                {
                                    Fin_year_id = reader.GetInt64(0),
                                    Fin_name = reader.GetString(1)
                                };

                                fin_year_list.Add(fin_year);
                            }
                        }

                    }

                }
                    return Ok(fin_year_list);
                }

                catch (Exception ex)
                {
                    return BadRequest($"Error: {ex.Message}");
                }
        }




        [HttpPost("insert_emp_calenderdays")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Add_emp_calenderdays([FromBody] AddEmpCalanderRequest request)
        {

            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {

                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_emp_calenderdays_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@emp_calender_id", 0);
                        command.Parameters.AddWithValue("@emp_calender_code", request.Emp_calander_code);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                        command.Parameters.AddWithValue("@month_id", request.Month_id);
                        command.Parameters.AddWithValue("@month_days", request.Month_days);
                        command.Parameters.AddWithValue("@emp_holidays", request.Emp_holidays);
                        command.Parameters.AddWithValue("@emp_weekend", request.Emp_Weekends);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "emp_calenderdays Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add emp_calenderdays." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "emp_calenderdays name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("Delete_emp_calenderdays/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deleteemp_calenderdays(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_emp_calenderdays_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@emp_calender_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "emp calenderday deleted successfully." });
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




        [HttpPost("Update_emp_calenderday")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> Update_Emp_Calander_days([FromBody] UpdateEmpCalanderRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_emp_calenderdays_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@emp_calender_id", request.Emp_calander_id);
                    parameters.Add("@emp_calender_code", request.Emp_calander_code);
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@month_id", request.Month_id);
                    parameters.Add("@month_days", request.Month_days);
                    parameters.Add("@emp_holidays", request.Emp_holidays);
                    parameters.Add("@emp_weekend", request.Emp_Weekends);
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
                    return NotFound($"emp calenderday with ID {request.Emp_calander_id} not found");
                else
                    return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }




        [HttpGet("emp_calenderday_list")]
        public async Task<ActionResult<IEnumerable<EmpCalander_list>>> Get_emp_calenderday_list()
        {
            var emp_calenderday_list = new List<EmpCalander_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_emp_calenderdays_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var emp_calenderday = new EmpCalander_list
                                {
                                    Emp_calander_id = reader.GetInt64(0),
                                    Emp_calander_code = reader.GetString(1),
                                    Fin_name = reader.GetString(2),
                                    Month_name = reader.GetString(3),
                                    Month_days = reader.GetDecimal(4),
                                    Emp_holidays = reader.GetDecimal(5),
                                    Emp_Weekends = reader.GetDecimal(6),
                                    Created_date = reader.GetDateTime(7).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(8) ? "" : reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(9) ? "" : reader.GetString(9)
                                };

                                emp_calenderday_list.Add(emp_calenderday);
                            }
                        }
                    }
                }

                return Ok(emp_calenderday_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("emp_calenderday/{id}")]
        public async Task<ActionResult<Single_EmpCalander_list>> Get_emp_calenderday_by_id(long id)
        {
            Single_EmpCalander_list? emp_calenderday = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_emp_calenderdays_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@emp_calender_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                emp_calenderday = new Single_EmpCalander_list
                                {
                                    Emp_calander_id = reader.GetInt64(0),
                                    Emp_calander_code = reader.GetString(1),
                                    Fin_year_id = reader.GetInt64(2),
                                    Month_id = reader.GetInt64(3),
                                    Month_days = reader.GetDecimal(4),
                                    Emp_holidays = reader.GetDecimal(5),
                                    Emp_Weekends = reader.GetDecimal(6),
                                    Created_date = reader.GetDateTime(7),
                                    Updated_date = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                                    User_id = reader.IsDBNull(9) ? 0 : reader.GetInt64(9)
                                };
                            }
                        }
                    }
                }

                if (emp_calenderday == null)
                    return NotFound($"emp_calenderday with ID {id} not found");

                return Ok(emp_calenderday);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("dropdown_emp_calenderday_list")]
        public async Task<ActionResult<IEnumerable<drop_EmpCalander_list>>> Get_drop_emp_calenderdaylist()
        {
            var emp_calenderday_list = new List<drop_EmpCalander_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_emp_calenderdays_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "emp_calander_list");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var emp_calenderday = new drop_EmpCalander_list
                                {
                                    Emp_calander_id = reader.GetInt64(0),
                                    Emp_calander_code = reader.GetString(1)
                                };

                                emp_calenderday_list.Add(emp_calenderday);
                            }
                        }
                    }
                }

                return Ok(emp_calenderday_list);
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

        public class AddCountryRequest
        {

            public long Country_id { get; set; } = 0;
            public string Country_name { get; set; } = "";
            public DateTime Created_date { get; set; }
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

        public class country_list
        {
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



        public class drop_country_list
        {
            public long Country_id { get; set; } = 0;
            public string Country_name { get; set; } = "";

        }

        public class AddStateRequest
        {
            public long State_id { get; set; } = 0;
            public string State_name { get; set; } = "";
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


        public class Single_state_list
        {
            public long State_id { get; set; } = 0;
            public string State_name { get; set; } = "";
            public long Country_id { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class drop_state_list
        {
            public long State_id { get; set; } = 0;
            public string State_name { get; set; } = "";

        }


        public class AddCityRequest
        {

            public long City_id { get; set; } = 0;
            public string City_name { get; set; } = "";
            public long State_id { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class UpdatecityRequest
        {
            public long City_id { get; set; } = 0;
            public string City_name { get; set; } = "";
            public long State_id { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }


        public class city_list
        {
            public long City_id { get; set; } = 0;
            public string City_name { get; set; } = "";
            public string State_name { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";

        }

        public class Single_city_list
        {
            public long City_id { get; set; } = 0;
            public string City_name { get; set; } = "";
            public long State_id { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }

        public class drop_city_list
        {
            public long City_id { get; set; } = 0;
            public string City_name { get; set; } = "";

        }


        public class AddCompanyRequest
        {

            public long Comp_id { get; set; } = 0;
            public string Comp_code { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Comp_short_name { get; set; } = "";
            public string Comp_type { get; set; } = "";
            public string Comp_desc { get; set; } = "";
            public string Cin_number { get; set; } = "";
            public string Gst_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Contperson_name { get; set; } = "";
            public string Contact_email { get; set; } = "";
            public string Contact_phone { get; set; } = "";
            public string Address_line1 { get; set; } = "";
            public string Address_line2 { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Pincode { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public string Logo_path { get; set; } = "";
            public long User_id { get; set; } = 0;

        }


        public class UpdateCompanyRequest
        {

            public long Comp_id { get; set; } = 0;
            public string Comp_code { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Comp_short_name { get; set; } = "";
            public string Comp_type { get; set; } = "";
            public string Comp_desc { get; set; } = "";
            public string Cin_number { get; set; } = "";
            public string Gst_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Contperson_name { get; set; } = "";
            public string Contact_email { get; set; } = "";
            public string Contact_phone { get; set; } = "";
            public string Address_line1 { get; set; } = "";
            public string Address_line2 { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Pincode { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public string Logo_path { get; set; } = "";
            public long User_id { get; set; } = 0;

        }


        public class company_list
        {
            public long Comp_id { get; set; } = 0;
            public string Comp_code { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Comp_short_name { get; set; } = "";
            public string Comp_type { get; set; } = "";
            public string Comp_desc { get; set; } = "";
            public string Cin_number { get; set; } = "";
            public string Gst_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Contperson_name { get; set; } = "";
            public string Contact_email { get; set; } = "";
            public string Contact_phone { get; set; } = "";
            public string Address_line1 { get; set; } = "";
            public string Address_line2 { get; set; } = "";
            public string City_name { get; set; } = "";
            public string Pincode { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string Logo_path { get; set; } = "";
            public string User_name { get; set; } = "";

        }


        public class single_company_list
        {

            public long Comp_id { get; set; } = 0;
            public string Comp_code { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Comp_short_name { get; set; } = "";
            public string Comp_type { get; set; } = "";
            public string Comp_desc { get; set; } = "";
            public string Cin_number { get; set; } = "";
            public string Gst_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Contperson_name { get; set; } = "";
            public string Contact_email { get; set; } = "";
            public string Contact_phone { get; set; } = "";
            public string Address_line1 { get; set; } = "";
            public string Address_line2 { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Pincode { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public string Logo_path { get; set; } = "";
            public long User_id { get; set; } = 0;

        }


        public class drop_company_list
        {
            public long Comp_id { get; set; } = 0;
            public string Comp_name { get; set; } = "";

        }

        public class AddMonthRequest
        {
            public long Month_id { get; set; } = 0;
            public string Month_name { get; set; } = "";
            public DateTime Start_date { get; set; }
            public DateTime End_date { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }


        public class UpdateMonthRequest
        {
            public long Month_id { get; set; } = 0;
            public string Month_name { get; set; } = "";
            public DateTime? Start_date { get; set; }
            public DateTime? End_date { get; set; }
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }

        public class month_list
        {
            public long Month_id { get; set; } = 0;
            public string Month_name { get; set; } = "";
            public string Start_date { get; set; } = "";
            public string End_date { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";

        }


        public class Single_month_list
        {
            public long Month_id { get; set; } = 0;
            public string Month_name { get; set; } = "";
            public DateTime? Start_date { get; set; }
            public DateTime? End_date { get; set; }
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }



        public class drop_month_list
        {
            public long Month_id { get; set; } = 0;
            public string Month_name { get; set; } = "";

        }

        public class AddFinYearRequest
        {
            public long Fin_year_id { get; set; } = 0;
            public string Fin_name { get; set; } = "";
            public string Short_fin_year { get; set; } = "";
            public DateTime Year_start { get; set; }
            public DateTime Year_end { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long? User_id { get; set; } = 0;
        }



        public class UpdateFinYearRequest
        {
            public long Fin_year_id { get; set; } = 0;
            public string Fin_name { get; set; } = "";
            public string Short_fin_year { get; set; } = "";
            public DateTime Year_start { get; set; }
            public DateTime Year_end { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long? User_id { get; set; } = 0;

        }


        public class FinYearlist
        {
            public long Fin_year_id { get; set; } = 0;
            public string Fin_name { get; set; } = "";
            public string Short_fin_year { get; set; } = "";
            public string Year_start { get; set; } = "";
            public string Year_end { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";

        }


        public class Single_FinYear_list
        {
            public long Fin_year_id { get; set; } = 0;
            public string Fin_name { get; set; } = "";
            public string Short_fin_year { get; set; } = "";
            public DateTime? Year_start { get; set; }
            public DateTime? Year_end { get; set; }
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class drop_FinYear_list
        {
            public long Fin_year_id { get; set; } = 0;
            public string Fin_name { get; set; } = "";

        }


        public class AddEmpCalanderRequest 
        {

            public long Emp_calander_id { get; set; } = 0;
            public string Emp_calander_code { get; set; } = "";
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public decimal Month_days { get; set; } = 0;
            public decimal Emp_holidays { get; set; } = 0;
            public decimal Emp_Weekends { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long? User_id { get; set; } = 0;

        }


        public class UpdateEmpCalanderRequest
        {
            public long Emp_calander_id { get; set; } = 0;
            public string Emp_calander_code { get; set; } = "";
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public decimal Month_days { get; set; } = 0;
            public decimal Emp_holidays { get; set; } = 0;
            public decimal Emp_Weekends { get; set; } = 0;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class EmpCalander_list
        {
            public long Emp_calander_id { get; set; } = 0;
            public string Emp_calander_code { get; set; } = "";
            public string Fin_name { get; set; } = "";
            public string Month_name { get; set; } = "";
            public decimal Month_days { get; set; } = 0;
            public decimal Emp_holidays { get; set; } = 0;
            public decimal Emp_Weekends { get; set; } = 0;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";

        }


        public class Single_EmpCalander_list
        {
            public long Emp_calander_id { get; set; } = 0;
            public string Emp_calander_code { get; set; } = "";
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public decimal Month_days { get; set; } = 0;
            public decimal Emp_holidays { get; set; } = 0;
            public decimal Emp_Weekends { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class drop_EmpCalander_list
        {
            public long Emp_calander_id { get; set; } = 0;
            public string Emp_calander_code { get; set; } = "";

        }


    }

}
