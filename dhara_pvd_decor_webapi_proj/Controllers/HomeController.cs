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
using System.ComponentModel;
using System.Reflection.Metadata;

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
                    return Ok(new { message = "Country updated successfully." });
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
                    return Ok(new { message = "State updated successfully." });
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
                    return Ok(new { message = "city updated successfully." });
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
                    return Ok(new { message = "company updated successfully." });
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
                    return Ok(new { message = "month updated successfully." });
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
                    return NotFound($"FinYear with ID {request.Fin_year_id} not found");
                else
                    return Ok(new { message = "FinYear updated successfully." });
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
                        command.Parameters.AddWithValue("fin_year_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                finyear = new Single_FinYear_list
                                {
                                    Fin_year_id = reader.GetInt64(0),
                                    Fin_name = reader.GetString(1),
                                    Short_fin_year = reader.GetString(2),
                                    Year_start = reader.GetDateTime(3),
                                    Year_end = reader.GetDateTime(4),
                                    Created_date = reader.GetDateTime(5),
                                    Updated_date = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                                    User_id = reader.IsDBNull(7) ? 0 : reader.GetInt64(7)
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
                    return Ok(new { message = "emp calenderday updated successfully." });
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





        [HttpPost("insert_colour")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Addcolour([FromBody] AddColourRequest request)
        {

            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {

                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_colour_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@colour_id", 0);
                        command.Parameters.AddWithValue("@colour_name", request.ColourName);
                        command.Parameters.AddWithValue("@is_active", request.IsActive);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "colour Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add colour." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "colour name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("Deletecolour/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deletecolour(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_colour_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@colour_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "colour deleted successfully." });
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





        [HttpPost("Updatecolour")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> Updatecolour([FromBody] UpdateColourRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_colour_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@colour_id", request.ColourId);
                    parameters.Add("@colour_name", request.ColourName);
                    parameters.Add("@is_active", request.IsActive);
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
                    return NotFound($"colour with ID {request.ColourId} not found");
                else
                    return Ok(new { message = "colour updated successfully." });
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }



        [HttpGet("colour_list")]
        public async Task<ActionResult<IEnumerable<Colour_list>>> Get_colour_list()
        {
            var colour_list = new List<Colour_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_colour_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var colour = new Colour_list
                                {
                                    ColourId = reader.GetInt64(0),
                                    ColourName = reader.GetString(1),
                                    IsActive = reader.GetBoolean(2),
                                    Created_date = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(4) ? "" : reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(5) ? "" : reader.GetString(5)
                                };

                                colour_list.Add(colour);
                            }
                        }
                    }
                }

                return Ok(colour_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("colour/{id}")]
        public async Task<ActionResult<Single_Colour_list>> Get_colour_by_id(long id)
        {
            Single_Colour_list? colour = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_colour_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@colour_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                colour = new Single_Colour_list
                                {
                                    ColourId = reader.GetInt64(0),
                                    ColourName = reader.GetString(1),
                                    IsActive = reader.GetBoolean(2),
                                    Created_date = reader.GetDateTime(3),
                                    Updated_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                    User_id = reader.IsDBNull(5) ? 0 : reader.GetInt64(5),
                                };
                            }
                        }
                    }
                }

                if (colour == null)
                    return NotFound($"colour with ID {id} not found");

                return Ok(colour);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("dropdown_colour_list")]
        public async Task<ActionResult<IEnumerable<drop_Colour_list>>> Get_drop_colourlist()
        {
            var colour_list = new List<drop_Colour_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_colour_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "colourlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var colour = new drop_Colour_list
                                {
                                    ColourId = reader.GetInt64(0),
                                    ColourName = reader.GetString(1)
                                };

                                colour_list.Add(colour);
                            }
                        }
                    }
                }

                return Ok(colour_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
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
                        command.Parameters.AddWithValue("@user_id", request.User_id);

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
                    parameters.Add("@user_id", request.User_id);

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
                                    User_name = reader.IsDBNull(6) ? "" : reader.GetString(6)
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
                                    User_id = reader.IsDBNull(6) ? 0 : reader.GetInt64(6)

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
                        command.Parameters.AddWithValue("@user_id", request.User_id);

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
                    parameters.Add("@user_id", request.User_id);

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
                                    User_name = reader.IsDBNull(7) ? "" : reader.GetString(7)
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
                                    User_id = reader.IsDBNull(7) ? 0 : reader.GetInt64(7)
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



        [HttpPost("insert_brand")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddBrand([FromBody] AddBrandRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_brand_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@brand_id", request.Brand_Id);
                        command.Parameters.AddWithValue("@brand_name", request.Brand_Name);
                        command.Parameters.AddWithValue("@brand_desc", request.Brand_Desc);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Brand added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add brand." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "Brand name already exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_brand/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBrand(long id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_brand_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@brand_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Brand deleted successfully." });
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


        [HttpPost("update_brand")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateBrand([FromBody] UpdateBrandRequest request)
        {
            int rowsAffected;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_brand_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@brand_id", request.Brand_Id);
                    parameters.Add("@brand_name", request.Brand_Name);
                    parameters.Add("@brand_desc", request.Brand_Desc);
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
                    return NotFound($"Brand with ID {request.Brand_Id} not found");
                else
                    return Ok(new { message = "Brand updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }



        [HttpGet("brand_list")]
        public async Task<ActionResult<IEnumerable<Brand_list>>> GetBrandList()
        {
            var brandList = new List<Brand_list>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_brand_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var brand = new Brand_list
                                {
                                    Brand_Id = reader.GetInt64(0),
                                    Brand_Name = reader.GetString(1),
                                    Brand_Desc = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Created_date = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(4) ? "" : reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(5) ? "" : reader.GetString(5)
                                };

                                brandList.Add(brand);
                            }
                        }
                    }
                }

                return Ok(brandList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }




        [HttpGet("brand/{id}")]
        public async Task<ActionResult<Single_Brand_list>> GetBrandById(long id)
        {
            Single_Brand_list? brand = null;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_brand_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@brand_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                brand = new Single_Brand_list
                                {
                                    Brand_Id = reader.GetInt64(0),
                                    Brand_Name = reader.GetString(1),
                                    Brand_Desc = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Created_date = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                                    Updated_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                    User_id = reader.IsDBNull(5) ? 0 : reader.GetInt64(5)
                                };
                            }
                        }
                    }
                }

                if (brand == null)
                    return NotFound($"Brand with Id {id} not found");

                return Ok(brand);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }




        [HttpGet("dropdown_brand_list")]
        public async Task<ActionResult<IEnumerable<drop_Brand_list>>> GetDropBrandList()
        {
            var brandList = new List<drop_Brand_list>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_brand_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "brandlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var brand = new drop_Brand_list
                                {
                                    Brand_Id = reader.GetInt64(0),
                                    Brand_Name = reader.GetString(1)
                                };

                                brandList.Add(brand);
                            }
                        }
                    }
                }

                return Ok(brandList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
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



        [HttpPost("insert_customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCustomer([FromBody] AddCustomerRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_customer_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@customer_id", 0);
                        command.Parameters.AddWithValue("@customer_name", request.Customer_Name);
                        command.Parameters.AddWithValue("@prefix", request.Prefix);
                        command.Parameters.AddWithValue("@gender", request.Gender);
                        command.Parameters.AddWithValue("@phonenumber", request.Phonenumber);
                        command.Parameters.AddWithValue("@city_id", request.City_Id);
                        command.Parameters.AddWithValue("@cust_address", request.Cust_Address);
                        command.Parameters.AddWithValue("@email_id", request.Email_Id);
                        command.Parameters.AddWithValue("@dob", request.Dob);
                        command.Parameters.AddWithValue("@aadhaar_number", request.Aadhaar_Number);
                        command.Parameters.AddWithValue("@license_number", request.License_Number);
                        command.Parameters.AddWithValue("@pan_number", request.Pan_Number);
                        command.Parameters.AddWithValue("@gst_number", request.Gst_Number);
                        command.Parameters.AddWithValue("@is_active", request.Is_Active);
                        command.Parameters.AddWithValue("@customer_notes", request.Customer_Notes);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rows = await command.ExecuteNonQueryAsync();

                        if (rows > 0)
                            return Ok(new { message = "Customer added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add customer." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "Customer with same details already exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteCustomer/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCustomer(long id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_customer_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@customer_id", id);

                        int rows = await command.ExecuteNonQueryAsync();

                        if (rows > 0)
                            return Ok(new { message = "Customer deleted successfully." });
                        else
                            return NotFound(new { errorMessage = $"Customer Id {id} not found." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }




        [HttpPost("UpdatecCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@customer_id", request.Customer_Id);
                    parameters.Add("@customer_name", request.Customer_Name);
                    parameters.Add("@prefix", request.Prefix);
                    parameters.Add("@gender", request.Gender);
                    parameters.Add("@phonenumber", request.Phonenumber);
                    parameters.Add("@city_id", request.City_Id);
                    parameters.Add("@cust_address", request.Cust_Address);
                    parameters.Add("@email_id", request.Email_Id);
                    parameters.Add("@dob", request.Dob);
                    parameters.Add("@aadhaar_number", request.Aadhaar_Number);
                    parameters.Add("@license_number", request.License_Number);
                    parameters.Add("@pan_number", request.Pan_Number);
                    parameters.Add("@gst_number", request.Gst_Number);
                    parameters.Add("@is_active", request.Is_Active);
                    parameters.Add("@customer_notes", request.Customer_Notes);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_Id);

                    int rows = await connection.ExecuteAsync(
                        "sp_customer_mast_ins_upd_del",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    if (rows == 0)
                        return NotFound(new { errorMessage = $"Customer Id {request.Customer_Id} not found." });
                    else
                        return Ok(new { message = "Customer updated successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpGet("customer_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Customer_List>>> Get_Customer_List()
        {
            var customer_list = new List<Customer_List>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_customer_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var customer = new Customer_List
                                {
                                    Customer_Id = reader.GetInt64(0),
                                    Customer_Name = reader.GetString(1),
                                    Prefix = reader.GetString(2),
                                    Gender = reader.GetString(3),
                                    Phonenumber = reader.GetString(4),
                                    City_Name = reader.GetString(5),
                                    Cust_Address = reader.GetString(6),
                                    Email_Id = reader.GetString(7),
                                    Dob = reader.IsDBNull(8) ? "" : reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    Aadhaar_Number = reader.GetString(9),
                                    License_Number = reader.GetString(10),
                                    Pan_Number = reader.GetString(11),
                                    Gst_Number = reader.GetString(12),
                                    Is_Active = reader.GetBoolean(13),
                                    Customer_Notes = reader.GetString(14),
                                    Created_Date = reader.GetDateTime(15).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(16) ? "" : reader.GetDateTime(16).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(17) ? "" : reader.GetString(17)
                                };
                                customer_list.Add(customer);
                            }
                        }
                    }
                }
                return Ok(customer_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("customer/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Single_Customer_List>> GetCustomerById(long id)
        {
            Single_Customer_List? customer = null;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_customer_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@customer_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                customer = new Single_Customer_List
                                {
                                    Customer_Id = reader.GetInt64(0),
                                    Customer_Name = reader.GetString(1),
                                    Prefix = reader.GetString(2),
                                    Gender = reader.GetString(3),
                                    Phonenumber = reader.GetString(4),
                                    City_Id = reader.GetInt64(5),
                                    Cust_Address = reader.GetString(6),
                                    Email_Id = reader.GetString(7),
                                    Dob = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                                    Aadhaar_Number = reader.GetString(9),
                                    License_Number = reader.GetString(10),
                                    Pan_Number = reader.GetString(11),
                                    Gst_Number = reader.GetString(12),
                                    Is_Active = reader.GetBoolean(13),
                                    Customer_Notes = reader.GetString(14),
                                    Created_Date = reader.IsDBNull(15) ? null : reader.GetDateTime(15),
                                    Updated_Date = reader.IsDBNull(16) ? null : reader.GetDateTime(16),
                                    User_Id = reader.IsDBNull(17) ? 0 : reader.GetInt64(17)
                                };
                            }
                        }
                    }
                }

                if (customer == null)
                    return NotFound($"Customer with Id {id} not found.");

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("dropdown_customer_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<Drop_Customer_List>>> Get_drop_customerlist()
        {
            var customer_list = new List<Drop_Customer_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_customer_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "customerlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var customer = new Drop_Customer_List
                                {
                                    Customer_Id = reader.GetInt64(0),
                                    Customer_Name = reader.GetString(1)
                                };

                                customer_list.Add(customer);
                            }
                        }
                    }
                }

                return Ok(customer_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpPost("insert_vendor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Addvendor([FromBody] AddVendorRequest request)
        {

            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {

                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_vendor_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@vendor_id", 0);
                        command.Parameters.AddWithValue("@vendor_name", request.Vendor_Name);
                        command.Parameters.AddWithValue("@prefix", request.Prefix);
                        command.Parameters.AddWithValue("@gender", request.Gender);
                        command.Parameters.AddWithValue("@phonenumber", request.Phonenumber);
                        command.Parameters.AddWithValue("@city_id", request.City_Id);
                        command.Parameters.AddWithValue("@address", request.Address);
                        command.Parameters.AddWithValue("@email_id", request.Email_Id);
                        command.Parameters.AddWithValue("@dob", request.Dob);
                        command.Parameters.AddWithValue("@aadhaar_number", request.Aadhaar_Number);
                        command.Parameters.AddWithValue("@license_number", request.License_Number);
                        command.Parameters.AddWithValue("@pan_number", request.Pan_Number);
                        command.Parameters.AddWithValue("@gst_number", request.Gst_Number);
                        command.Parameters.AddWithValue("@is_active", request.Is_Active);
                        command.Parameters.AddWithValue("@vendor_notes", request.Vendor_Notes);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);


                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "vendor Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add vendor." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "vendor name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteVendor/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteVendor(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_vendor_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@vendor_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Vendor deleted successfully." });
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


        [HttpPost("UpdateVendor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> Updatevendor([FromBody] UpdateVendorRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_vendor_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@vendor_id", request.Vendor_Id);
                    parameters.Add("@vendor_name", request.Vendor_Name);
                    parameters.Add("@prefix", request.Prefix);
                    parameters.Add("@gender", request.Gender);
                    parameters.Add("@phonenumber", request.Phonenumber);
                    parameters.Add("@city_id", request.City_Id);
                    parameters.Add("@address", request.Address);
                    parameters.Add("@email_id", request.Email_Id);
                    parameters.Add("@dob", request.Dob);
                    parameters.Add("@aadhaar_number", request.Aadhaar_Number);
                    parameters.Add("@license_number", request.License_Number);
                    parameters.Add("@pan_number", request.Pan_Number);
                    parameters.Add("@gst_number", request.Gst_Number);
                    parameters.Add("@is_active", request.Is_Active);
                    parameters.Add("@vendor_notes", request.Vendor_Notes);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_Id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"vendor with ID {request.Vendor_Id} not found");
                else
                    return Ok(new { message = "Vendor updated successfully." });
            }

            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }


        [HttpGet("vendor_list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Vendor_List>>> Get_vendor_list()
        {
            var vendor_list = new List<Vendor_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_vendor_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var vendor = new Vendor_List
                                {
                                    Vendor_Id = reader.GetInt64(0),
                                    Vendor_Name = reader.GetString(1),
                                    Prefix = reader.GetString(2),
                                    Gender = reader.GetString(3),
                                    Phonenumber = reader.GetString(4),
                                    City_Name = reader.GetString(5),
                                    Address = reader.GetString(6),
                                    Email_Id = reader.GetString(7),
                                    Dob = reader.IsDBNull(8) ? "" : reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    Aadhaar_Number = reader.GetString(9),
                                    License_Number = reader.GetString(10),
                                    Pan_Number = reader.GetString(11),
                                    Gst_Number = reader.GetString(12),
                                    Is_Active = reader.GetBoolean(13),
                                    Vendor_Notes = reader.GetString(14),
                                    Created_Date = reader.GetDateTime(15).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(16) ? "" : reader.GetDateTime(16).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(17) ? "" : reader.GetString(17)
                                };

                                vendor_list.Add(vendor);
                            }
                        }
                    }
                }

                return Ok(vendor_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("vendor/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Single_Vendor_List>> Get_vendor_by_id(long id)
        {
            Single_Vendor_List? vendor = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_vendor_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@vendor_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                vendor = new Single_Vendor_List
                                {
                                    Vendor_Id = reader.GetInt64(0),
                                    Vendor_Name = reader.GetString(1),
                                    Prefix = reader.GetString(2),
                                    Gender = reader.GetString(3),
                                    Phonenumber = reader.GetString(4),
                                    City_Id = reader.GetInt64(5),
                                    Address = reader.GetString(6),
                                    Email_Id = reader.GetString(7),
                                    Dob = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                                    Aadhaar_Number = reader.GetString(9),
                                    License_Number = reader.GetString(10),
                                    Pan_Number = reader.GetString(11),
                                    Gst_Number = reader.GetString(12),
                                    Is_Active = reader.GetBoolean(13),
                                    Vendor_Notes = reader.GetString(14),
                                    Created_Date = reader.IsDBNull(15) ? null : reader.GetDateTime(15),
                                    Updated_Date = reader.IsDBNull(16) ? null : reader.GetDateTime(16),
                                    User_Id = reader.IsDBNull(17) ? 0 : reader.GetInt64(17)
                                };
                            }
                        }
                    }
                }

                if (vendor == null)
                    return NotFound($"vendor with Id {id} not found");

                return Ok(vendor);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("dropdownlist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Drop_Vendor_List>>> Get_drop_vendorlist()
        {
            var vendor_list = new List<Drop_Vendor_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_vendor_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "vendor_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var vendor = new Drop_Vendor_List
                                {
                                    Vendor_Id = reader.GetInt64(0),
                                    Vendor_Name = reader.GetString(1)
                                };

                                vendor_list.Add(vendor);
                            }
                        }
                    }
                }

                return Ok(vendor_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpPost("insert_product")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Add_Product([FromBody] AddProductRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_product_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@product_id", 0);
                        command.Parameters.AddWithValue("@prodtype_id", request.Prodtype_id);
                        command.Parameters.AddWithValue("@brand_id", request.Brand_id);
                        command.Parameters.AddWithValue("@hsn_id", request.Hsn_id);
                        command.Parameters.AddWithValue("@unit_id", request.Unit_id);
                        command.Parameters.AddWithValue("@product_name", request.Product_name);
                        command.Parameters.AddWithValue("@product_desc", request.Product_desc);
                        command.Parameters.AddWithValue("@rate", request.Rate);
                        command.Parameters.AddWithValue("@opening_stock", request.Opening_stock);
                        command.Parameters.AddWithValue("@purchase", request.Purchase);
                        command.Parameters.AddWithValue("@sales", request.Sales);
                        command.Parameters.AddWithValue("@return", request.Return);
                        command.Parameters.AddWithValue("@current_stock", request.Current_stock);
                        command.Parameters.AddWithValue("@reorder_threshold", request.Reorder_threshold);
                        command.Parameters.AddWithValue("@reorder_desc", request.Reorder_desc);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Product Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Product." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Product name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteProduct/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_product_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@product_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Product deleted successfully." });
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



        [HttpPost("UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateProduct([FromBody] UpdateProductRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_product_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@product_id", request.Product_Id);
                    parameters.Add("@prodtype_id", request.Prodtype_id);
                    parameters.Add("@brand_id", request.Brand_id);
                    parameters.Add("@hsn_id", request.Hsn_id);
                    parameters.Add("@unit_id", request.Unit_id);
                    parameters.Add("@product_name", request.Product_name);
                    parameters.Add("@product_desc", request.Product_desc);
                    parameters.Add("@rate", request.Rate);
                    parameters.Add("@opening_stock", request.Opening_stock);
                    parameters.Add("@purchase", request.Purchase);
                    parameters.Add("@sales", request.Sales);
                    parameters.Add("@return", request.Return);
                    parameters.Add("@current_stock", request.Current_stock);
                    parameters.Add("@reorder_threshold", request.Reorder_threshold);
                    parameters.Add("@reorder_desc", request.Reorder_desc);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_Id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"Product with ID {request.Product_Id} not found");
                else
                    return Ok(new { message = "Product updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("product_list")]
        public async Task<ActionResult<IEnumerable<Product_list>>> Get_product_list()
        {
            var product_list = new List<Product_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_product_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var product = new Product_list
                                {
                                    Product_Id = reader.GetInt64(0),
                                    Prodtype_name = reader.GetString(1),
                                    Brand_name = reader.GetString(2),
                                    Hsn_name = reader.GetString(3),
                                    Unit_name = reader.GetString(4),
                                    Product_name = reader.GetString(5),
                                    Product_desc = reader.GetString(6),
                                    Rate = reader.GetDecimal(7),
                                    Opening_stock = reader.GetDecimal(8),
                                    Purchase = reader.GetDecimal(9),
                                    Sales = reader.GetDecimal(10),
                                    Return = reader.GetDecimal(11),
                                    Current_stock = reader.GetDecimal(12),
                                    Reorder_threshold = reader.GetDecimal(13),
                                    Reorder_desc = reader.GetString(14),
                                    Created_Date = reader.GetDateTime(15).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(16) ? "" : reader.GetDateTime(16).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(17) ? "" : reader.GetString(17),
                                };

                                product_list.Add(product);
                            }
                        }
                    }
                }

                return Ok(product_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("product/{id}")]
        public async Task<ActionResult<SingleProductList>> Get_product_by_id(long id)
        {
            SingleProductList? product = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_product_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@product_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                product = new SingleProductList
                                {
                                    Product_Id = reader.GetInt64(0),
                                    Prodtype_id = reader.GetInt64(1),
                                    Brand_id = reader.GetInt64(2),
                                    Hsn_id = reader.GetInt64(3),
                                    Unit_id = reader.GetInt64(4),
                                    Product_name = reader.GetString(5),
                                    Product_desc = reader.GetString(6),
                                    Rate = reader.GetDecimal(7),
                                    Opening_stock = reader.GetDecimal(8),
                                    Purchase = reader.GetDecimal(9),
                                    Sales = reader.GetDecimal(10),
                                    Return = reader.GetDecimal(11),
                                    Current_stock = reader.GetDecimal(12),
                                    Reorder_threshold = reader.GetDecimal(13),
                                    Reorder_desc = reader.GetString(14),
                                    Created_Date = reader.IsDBNull(15) ? (DateTime?)null : reader.GetDateTime(15),
                                    Updated_Date = reader.IsDBNull(16) ? (DateTime?)null : reader.GetDateTime(16),
                                    User_Id = reader.IsDBNull(17) ? 0 : reader.GetInt64(17),
                                };
                            }
                        }
                    }
                }

                if (product == null)
                    return NotFound($"Product with ID {id} not found");

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("dropdown_product_list")]
        public async Task<ActionResult<IEnumerable<Drop_Product_List>>> Get_drop_productlist()
        {
            var product_list = new List<Drop_Product_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_product_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "productlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var product = new Drop_Product_List
                                {
                                    Product_Id = reader.GetInt64(0),
                                    Product_name = reader.GetString(1)
                                };

                                product_list.Add(product);
                            }
                        }
                    }
                }

                return Ok(product_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
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
                        command.Parameters.AddWithValue("@balance", request.Balance);
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
                    parameters.Add("@balance", request.Balance);
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
                                    Balance = reader.GetDecimal(4),
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
                                    Balance = reader.GetDecimal(4),
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


        [HttpPost("insert_sales_invoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AddSalesInvoice([FromBody] AddsalesinvoiceRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_salesinvoice_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@sales_id", request.Sales_id);
                        command.Parameters.AddWithValue("@prefix", request.Prefix);
                        command.Parameters.AddWithValue("@suffix", request.Suffix);
                        command.Parameters.AddWithValue("@customer_id", request.Customer_id);
                        command.Parameters.AddWithValue("@sales_date", request.Sales_date);
                        command.Parameters.AddWithValue("@gross_total", request.Gross_total);
                        command.Parameters.AddWithValue("@sgst_total", request.Sgst_total);
                        command.Parameters.AddWithValue("@cgst_total", request.Cgst_total);
                        command.Parameters.AddWithValue("@igst_total", request.Igst_total);
                        command.Parameters.AddWithValue("@discount_total", request.Discount_total);
                        command.Parameters.AddWithValue("@roundoff_total", request.Roundoff_total);
                        command.Parameters.AddWithValue("@net_total", request.Net_total);
                        command.Parameters.AddWithValue("@balance_total", request.Balance_total);
                        command.Parameters.AddWithValue("@payment_status", request.Payment_status);
                        command.Parameters.AddWithValue("@isactive", request.Isactive);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_Year_Id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_Id);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Sales Invoice Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Sales Invoice." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Sales Invoice already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteSalesInvoice/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSalesInvoice(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_salesinvoice_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@sales_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Sales Invoice deleted successfully." });
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




        [HttpPost("UpdateSalesInvoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateSalesInvoice([FromBody] UpdatesalesinvoiceRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_salesinvoice_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@sales_id", request.Sales_id);
                    parameters.Add("@prefix", request.Prefix);
                    parameters.Add("@suffix", request.Suffix);
                    parameters.Add("@customer_id", request.Customer_id);
                    parameters.Add("@sales_date", request.Sales_date);
                    parameters.Add("@gross_total", request.Gross_total);
                    parameters.Add("@sgst_total", request.Sgst_total);
                    parameters.Add("@cgst_total", request.Cgst_total);
                    parameters.Add("@igst_total", request.Igst_total);
                    parameters.Add("@discount_total", request.Discount_total);
                    parameters.Add("@roundoff_total", request.Roundoff_total);
                    parameters.Add("@net_total", request.Net_total);
                    parameters.Add("@balance_total", request.Balance_total);
                    parameters.Add("@payment_status", request.Payment_status);
                    parameters.Add("@isactive", request.Isactive);
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
                    return NotFound($"Sales Invoice with ID {request.Sales_id} not found");
                else
                    return Ok(new { message = "Sales Invoice updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("salesinvoice_list")]
        public async Task<ActionResult<IEnumerable<salesinvoice_List>>> Get_salesinvoice_list()
        {
            var sales_list = new List<salesinvoice_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_salesinvoice_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var sales = new salesinvoice_List
                                {
                                    Sales_id = reader.GetInt64(0),
                                    Prefix = reader.GetString(1),
                                    Suffix = reader.GetString(2),
                                    Customer_name = reader.GetString(3),
                                    Sales_date = reader.GetDateTime(4),
                                    Gross_total = reader.GetDecimal(5),
                                    Sgst_total = reader.GetDecimal(6),
                                    Cgst_total = reader.GetDecimal(7),
                                    Igst_total = reader.GetDecimal(8),
                                    Discount_total = reader.GetDecimal(9),
                                    Roundoff_total = reader.GetDecimal(10),
                                    Net_total = reader.GetDecimal(11),
                                    Balance_total = reader.GetDecimal(12),
                                    Payment_status = reader.GetBoolean(13),
                                    Isactive = reader.GetBoolean(14),
                                    Fin_Year_Name = reader.IsDBNull(15) ? "" : reader.GetString(15),
                                    Comp_Name = reader.IsDBNull(16) ? "" : reader.GetString(16),
                                    Created_Date = reader.IsDBNull(17) ? "" : reader.GetDateTime(17).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(18) ? "" : reader.GetDateTime(18).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(19) ? "" : reader.GetString(19)
                                };

                                sales_list.Add(sales);
                            }
                        }
                    }
                }

                return Ok(sales_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("salesinvoice/{id}")]
        public async Task<ActionResult<Singlesalesinvoice>> Get_salesinvoice_by_id(long id)
        {
            Singlesalesinvoice? sales = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_salesinvoice_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@sales_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                sales = new Singlesalesinvoice
                                {
                                    Sales_id = reader.GetInt64(0),
                                    Prefix = reader.GetString(1),
                                    Suffix = reader.GetString(2),
                                    Customer_id = reader.GetInt64(3),
                                    Sales_date = reader.GetDateTime(4),
                                    Gross_total = reader.GetDecimal(5),
                                    Sgst_total = reader.GetDecimal(6),
                                    Cgst_total = reader.GetDecimal(7),
                                    Igst_total = reader.GetDecimal(8),
                                    Discount_total = reader.GetDecimal(9),
                                    Roundoff_total = reader.GetDecimal(10),
                                    Net_total = reader.GetDecimal(11),
                                    Balance_total = reader.GetDecimal(12),
                                    Payment_status = reader.GetBoolean(13),
                                    Isactive = reader.GetBoolean(14),
                                    Fin_Year_Id = reader.GetInt64(15),
                                    Comp_Id = reader.GetInt64(16),
                                    Created_Date = reader.IsDBNull(17) ? (DateTime?)null : reader.GetDateTime(17),
                                    Updated_Date = reader.IsDBNull(18) ? (DateTime?)null : reader.GetDateTime(18),
                                    User_Id = reader.IsDBNull(19) ? 0 : reader.GetInt64(19)
                                };
                            }
                        }
                    }
                }

                if (sales == null)
                    return NotFound($"Sales Invoice with ID {id} not found");

                return Ok(sales);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        //[HttpGet("dropdown_salesinvoice_list")]
        //public async Task<ActionResult<IEnumerable<Drop_salesinvoice_List>>> Get_drop_salesinvoice_list()
        //{
        //    var sales_list = new List<Drop_salesinvoice_List>();
        //    var connectionstring = _configuration.GetConnectionString("DefaultConnection");

        //    try
        //    {
        //        using (var connection = new SqlConnection(connectionstring))
        //        {
        //            string spName = "sp_salesinvoice_mast_ins_upd_del";

        //            await connection.OpenAsync();

        //            using (var command = new SqlCommand(spName, connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddWithValue("@action", "salesinvoice_mastlist");

        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var sales = new Drop_salesinvoice_List
        //                        {
        //                            Sales_id = reader.GetInt64(0),
        //                        };

        //                        sales_list.Add(sales);
        //                    }
        //                }
        //            }
        //        }

        //        return Ok(sales_list);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error: {ex.Message}");
        //    }
        //}




        [HttpPost("insert_challan")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddChallan([FromBody] AddChallanRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_challan_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@challan_id", 0);
                        command.Parameters.AddWithValue("@sales_id", request.Sales_id);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_Year_Id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_Id);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Challan added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Challan." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Challan already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("delete_challan/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteChallan(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_challan_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@challan_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Challan deleted successfully." });
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





        [HttpPost("update_challan")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateChallan([FromBody] UpdateChallanRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_challan_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@challan_id", request.Challan_id);
                    parameters.Add("@sales_id", request.Sales_id);
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
                    return NotFound($"Challan with ID {request.Challan_id} not found");
                else
                    return Ok(new { message = "Challan updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }



        [HttpGet("challan_list")]
        public async Task<ActionResult<IEnumerable<Challan_List>>> GetChallanList()
        {
            var challan_list = new List<Challan_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_challan_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var challan = new Challan_List
                                {
                                    Challan_id = reader.GetInt64(0),
                                    Sales_id = reader.GetInt64(1),
                                    Fin_Year_Name = reader.GetString(2),
                                    Comp_Name = reader.GetString(3),
                                    Created_Date = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(5) ? "" : reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(6) ? "" : reader.GetString(6)
                                };

                                challan_list.Add(challan);
                            }
                        }
                    }
                }

                return Ok(challan_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("challan/{id}")]
        public async Task<ActionResult<SingleChallaninvoice>> GetChallanById(long id)
        {
            SingleChallaninvoice? challan = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_challan_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@challan_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                challan = new SingleChallaninvoice
                                {
                                    Challan_id = reader.GetInt64(0),
                                    Sales_id = reader.GetInt64(1),
                                    Fin_Year_Id = reader.GetInt64(2),
                                    Comp_Id = reader.GetInt64(3),
                                    Created_Date = reader.GetDateTime(4),
                                    Updated_Date = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                                    User_Id = reader.IsDBNull(6) ? 0 : reader.GetInt64(6)
                                };
                            }
                        }
                    }
                }

                if (challan == null)
                    return NotFound($"Challan with ID {id} not found");

                return Ok(challan);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("dropdown_challan_list")]
        public async Task<ActionResult<IEnumerable<Drop_Challan_List>>> GetDropChallanList()
        {
            var challan_list = new List<Drop_Challan_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_challan_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "challanlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var challan = new Drop_Challan_List
                                {
                                    Challan_id = reader.GetInt64(0)
                                };

                                challan_list.Add(challan);
                            }
                        }
                    }
                }

                return Ok(challan_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpPost("insert_receipt")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddReceipt([FromBody] AddReceiptRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_receipt_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@receipt_id", 0);
                        command.Parameters.AddWithValue("@sales_id", request.Sales_id);
                        command.Parameters.AddWithValue("@recepit_date", request.Recepit_date);
                        command.Parameters.AddWithValue("@net_total", request.Net_total);
                        command.Parameters.AddWithValue("@balance_amount", request.Balance_amount);
                        command.Parameters.AddWithValue("@receipt_status", request.Receipt_status);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_Year_Id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_Id);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Receipt added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Receipt." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Receipt already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_receipt/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteReceipt(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_receipt_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@receipt_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Receipt deleted successfully." });
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




        [HttpPost("update_receipt")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateReceipt([FromBody] UpdateReceiptRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_receipt_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@receipt_id", request.Receipt_id);
                    parameters.Add("@sales_id", request.Sales_id);
                    parameters.Add("@recepit_date", request.Recepit_date);
                    parameters.Add("@net_total", request.Net_total);
                    parameters.Add("@balance_amount", request.Balance_amount);
                    parameters.Add("@receipt_status", request.Receipt_status);
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
                    return NotFound($"Receipt with ID {request.Receipt_id} not found");
                else
                    return Ok(new { message = "Receipt updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        [HttpGet("receipt_list")]
        public async Task<ActionResult<IEnumerable<Receipt_List>>> GetReceiptList()
        {
            var receipt_list = new List<Receipt_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_receipt_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var receipt = new Receipt_List
                                {
                                    Receipt_id = reader.GetInt64(0),
                                    Sales_id = reader.GetInt64(1),
                                    Recepit_date = reader.GetDateTime(2),
                                    Net_total = reader.GetDecimal(3),
                                    Balance_amount = reader.GetDecimal(4),
                                    Receipt_status = reader.GetBoolean(5),
                                    Fin_Year_Name = reader.GetString(6),
                                    Comp_Name = reader.GetString(7),
                                    Created_Date = reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(9) ? "" : reader.GetDateTime(9).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(10) ? "" : reader.GetString(10)
                                };

                                receipt_list.Add(receipt);
                            }
                        }
                    }
                }

                return Ok(receipt_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("receipt/{id}")]
        public async Task<ActionResult<SingleReceiptinvoice>> GetReceiptById(long id)
        {
            SingleReceiptinvoice? receipt = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_receipt_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@receipt_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                receipt = new SingleReceiptinvoice
                                {
                                    Receipt_id = reader.GetInt64(0),
                                    Sales_id = reader.GetInt64(1),
                                    Recepit_date = reader.GetDateTime(2),
                                    Net_total = reader.GetDecimal(3),
                                    Balance_amount = reader.GetDecimal(4),
                                    Receipt_status = reader.GetBoolean(5),
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

                if (receipt == null)
                    return NotFound($"Receipt with ID {id} not found");

                return Ok(receipt);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("dropdown_receipt_list")]
        public async Task<ActionResult<IEnumerable<Drop_Receipt_List>>> GetDropReceiptList()
        {
            var receipt_list = new List<Drop_Receipt_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_receipt_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "receiptlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var receipt = new Drop_Receipt_List
                                {
                                    Receipt_id = reader.GetInt64(0),
                                    Sales_id = reader.GetInt64(1)
                                };

                                receipt_list.Add(receipt);
                            }
                        }
                    }
                }

                return Ok(receipt_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpPost("insert_purchaseinvoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPurchaseInvoice([FromBody] AddpurchaseinvoiceRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_purchaseinvoice_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@purchase_id", request.Purchase_id);
                        command.Parameters.AddWithValue("@prefix", request.Prefix);
                        command.Parameters.AddWithValue("@suffix", request.Suffix);
                        command.Parameters.AddWithValue("@invoice_no", request.Invoice_no);
                        command.Parameters.AddWithValue("@purchase_date", request.Purchase_date);
                        command.Parameters.AddWithValue("@vendor_id", request.Vendor_id);
                        command.Parameters.AddWithValue("@gross_total", request.Gross_total);
                        command.Parameters.AddWithValue("@sgst_total", request.Sgst_total);
                        command.Parameters.AddWithValue("@cgst_total", request.Cgst_total);
                        command.Parameters.AddWithValue("@igst_total", request.Igst_total);
                        command.Parameters.AddWithValue("@discount_total", request.Discount_total);
                        command.Parameters.AddWithValue("@roundoff_total", request.Roundoff_total);
                        command.Parameters.AddWithValue("@net_total", request.Net_total);
                        command.Parameters.AddWithValue("@balance_total", request.Balance_total);
                        command.Parameters.AddWithValue("@paymentstatus", request.Payment_status);
                        command.Parameters.AddWithValue("@is_active", request.Isactive);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_Year_Id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_Id);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Purchase Invoice Added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add Purchase Invoice." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Invoice number already exists." });
                }
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }




        [HttpDelete("Delete_Purchase_invoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Delete_Purchase_Ivoice(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_purchaseinvoice_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@purchase_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "PurchaseInvoice deleted successfully." });
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


        [HttpPost("UpdatePurchaseInvoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdatePurchaseInvoice([FromBody] UpdatepurchaseinvoiceRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_purchaseinvoice_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@purchase_id", request.Purchase_id);
                    parameters.Add("@prefix", request.Prefix);
                    parameters.Add("@suffix", request.Suffix);
                    parameters.Add("@invoice_no", request.Invoice_no);
                    parameters.Add("@purchase_date", request.Purchase_date);
                    parameters.Add("@vendor_id", request.Vendor_id);
                    parameters.Add("@gross_total", request.Gross_total);
                    parameters.Add("@sgst_total", request.Sgst_total);
                    parameters.Add("@cgst_total", request.Cgst_total);
                    parameters.Add("@igst_total", request.Igst_total);
                    parameters.Add("@discount_total", request.Discount_total);
                    parameters.Add("@roundoff_total", request.Roundoff_total);
                    parameters.Add("@net_total", request.Net_total);
                    parameters.Add("@balance_total", request.Balance_total);
                    parameters.Add("@paymentstatus", request.Payment_status);
                    parameters.Add("@is_active", request.Isactive);
                    parameters.Add("@fin_year_id", request.Fin_Year_Id);
                    parameters.Add("@comp_id", request.Comp_Id);
                    parameters.Add("@created_date", request.Created_Date);
                    parameters.Add("@updated_date", request.Updated_Date);
                    parameters.Add("@user_id", request.User_Id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"Purchase Invoice with ID {request.Purchase_id} not found");
                else
                    return Ok(new { message = "Purchase Invoice updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("purchaseinvoice_list")]
        public async Task<ActionResult<IEnumerable<purchaseinvoice_List>>> Get_purchaseinvoice_list()
        {
            var list = new List<purchaseinvoice_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_purchaseinvoice_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new purchaseinvoice_List
                                {
                                    Purchase_id = reader.GetInt64(0),
                                    Prefix = reader.GetString(1),
                                    Suffix = reader.GetString(2),
                                    Invoice_no = reader.GetString(3),
                                    Purchase_date = reader.GetDateTime(4),
                                    Vendor_name = reader.GetString(5),
                                    Gross_total = reader.GetDecimal(6),
                                    Sgst_total = reader.GetDecimal(7),
                                    Cgst_total = reader.GetDecimal(8),
                                    Igst_total = reader.GetDecimal(9),
                                    Discount_total = reader.GetDecimal(10),
                                    Roundoff_total = reader.GetDecimal(11),
                                    Net_total = reader.GetDecimal(12),
                                    Balance_total = reader.GetDecimal(13),
                                    Payment_status = reader.GetBoolean(14),
                                    Isactive = reader.GetBoolean(15),
                                    Fin_Year_Name = reader.GetString(16),
                                    Comp_Name = reader.GetString(17),
                                    Created_Date = reader.GetDateTime(18).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(19) ? "" : reader.GetDateTime(19).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(20) ? "" : reader.GetString(20)
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



        [HttpGet("purchaseinvoice/{id}")]
        public async Task<ActionResult<Singlepurchaseinvoice>> Get_purchaseinvoice_by_id(long id)
        {
            Singlepurchaseinvoice? invoice = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_purchaseinvoice_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@purchase_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                invoice = new Singlepurchaseinvoice
                                {
                                    Purchase_id = reader.GetInt64(0),
                                    Prefix = reader.GetString(1),
                                    Suffix = reader.GetString(2),
                                    Invoice_no = reader.GetString(3),
                                    Purchase_date = reader.GetDateTime(4),
                                    Vendor_id = reader.GetInt64(5),
                                    Gross_total = reader.GetDecimal(6),
                                    Sgst_total = reader.GetDecimal(7),
                                    Cgst_total = reader.GetDecimal(8),
                                    Igst_total = reader.GetDecimal(9),
                                    Discount_total = reader.GetDecimal(10),
                                    Roundoff_total = reader.GetDecimal(11),
                                    Net_total = reader.GetDecimal(12),
                                    Balance_total = reader.GetDecimal(13),
                                    Payment_status = reader.GetBoolean(14),
                                    Isactive = reader.GetBoolean(15),
                                    Fin_Year_Id = reader.GetInt64(16),
                                    Comp_Id = reader.GetInt64(17),
                                    Created_Date = reader.IsDBNull(18) ? null : reader.GetDateTime(18),
                                    Updated_Date = reader.IsDBNull(19) ? null : reader.GetDateTime(19),
                                    User_Id = reader.IsDBNull(20) ? 0 : reader.GetInt64(20)
                                };
                            }
                        }
                    }
                }

                if (invoice == null)
                    return NotFound($"Purchase Invoice with ID {id} not found");

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        //[HttpGet("dropdown_purchaseinvoice")]
        //public async Task<ActionResult<IEnumerable<Drop_purchaseinvoice_List>>> Get_drop_purchaseinvoice_list()
        //{
        //    var list = new List<Drop_purchaseinvoice_List>();
        //    var connectionstring = _configuration.GetConnectionString("DefaultConnection");

        //    try
        //    {
        //        using (var connection = new SqlConnection(connectionstring))
        //        {
        //            string spName = "sp_purchaseinvoice_mast_ins_upd_del";

        //            await connection.OpenAsync();

        //            using (var command = new SqlCommand(spName, connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddWithValue("@action", "purchaseinvoice_mastlist");

        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        list.Add(new Drop_purchaseinvoice_List
        //                        {
        //                            Sales_id = reader.GetInt64(0)
        //                        });
        //                    }
        //                }
        //            }
        //        }

        //        return Ok(list);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error: {ex.Message}");
        //    }
        //}


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


        [HttpPost("insert_user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_user_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@User_id", request.User_id);
                        command.Parameters.AddWithValue("@User_name", request.User_name);
                        command.Parameters.AddWithValue("@User_password", request.User_password);
                        command.Parameters.AddWithValue("@User_role", request.User_role);
                        command.Parameters.AddWithValue("@Is_login", request.Is_login);
                        command.Parameters.AddWithValue("@Created_Date", request.Created_Date);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "User added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add User." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "User name already exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_user_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@User_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "User deleted successfully." });
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


        [HttpPost("update_user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_user_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@User_id", request.User_id);
                    parameters.Add("@User_name", request.User_name);
                    parameters.Add("@User_password", request.User_password);
                    parameters.Add("@User_role", request.User_role);
                    parameters.Add("@Is_login", request.Is_login);
                    parameters.Add("@Created_Date", request.Created_Date);
                    parameters.Add("@Updated_Date", request.Updated_Date);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"User with ID {request.User_id} not found");
                else
                    return Ok(new { message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        [HttpGet("user_list")]
        public async Task<ActionResult<IEnumerable<User_List>>> GetUserList()
        {
            var list = new List<User_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_user_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new User_List
                                {
                                    User_id = reader.GetInt64(0),
                                    User_name = reader.GetString(1),
                                    User_password = reader.GetString(2),
                                    User_role = reader.GetString(3),
                                    Is_login = reader.GetBoolean(4),
                                    Created_Date = reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(6) ? "" : reader.GetDateTime(6).ToString("yyyy-MM-dd"),
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



        [HttpGet("user/{id}")]
        public async Task<ActionResult<SingleUser>> GetUserById(long id)
        {
            SingleUser? user = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_user_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@User_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                user = new SingleUser
                                {
                                    User_id = reader.GetInt64(0),
                                    User_name = reader.GetString(1),
                                    User_password = reader.GetString(2),
                                    User_role = reader.GetString(3),
                                    Is_login = reader.GetBoolean(4),
                                    Created_Date = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                                    Updated_Date = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                                };
                            }
                        }
                    }
                }

                if (user == null)
                    return NotFound($"User with ID {id} not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("dropdown_user_list")]
        public async Task<ActionResult<IEnumerable<Drop_User_List>>> GetDropdownUserList()
        {
            var list = new List<Drop_User_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_user_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "userlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new Drop_User_List
                                {
                                    User_id = reader.GetInt64(0),
                                    User_name = reader.GetString(1)
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




        [HttpPost("insert_emp_desg")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add_Emp_Desg([FromBody] Add_Emp_desg_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_employee_desg_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@desg_id", request.Desg_id);
                        command.Parameters.AddWithValue("@desg_name", request.Desg_name);
                        command.Parameters.AddWithValue("@desg_desc", request.Desg_desc);
                        command.Parameters.AddWithValue("@daily_wk_hr", request.Daily_wk_hr);
                        command.Parameters.AddWithValue("@created_date", request.Created_Date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Designation Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Designation." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Designation name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteEmpDesg/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmpDesg(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_employee_desg_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@desg_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Designation deleted successfully." });
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




        [HttpPost("UpdateEmpDesg")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update_Emp_Desg([FromBody] Update_Emp_desg_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_employee_desg_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@desg_id", request.Desg_id);
                    parameters.Add("@desg_name", request.Desg_name);
                    parameters.Add("@desg_desc", request.Desg_desc);
                    parameters.Add("@daily_wk_hr", request.Daily_wk_hr);
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
                    return NotFound($"Designation with ID {request.Desg_id} not found");
                else
                    return Ok(new { message = "Designation Updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("emp_desg_list")]
        public async Task<ActionResult<IEnumerable<Emp_desg_List>>> Get_emp_desg_list()
        {
            var desg_list = new List<Emp_desg_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_employee_desg_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var desg = new Emp_desg_List
                                {
                                    Desg_id = reader.GetInt64(0),
                                    Desg_name = reader.GetString(1),
                                    Desg_desc = reader.GetString(2),
                                    Daily_wk_hr = reader.GetDecimal(3),
                                    Created_Date = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(5) ? "" : reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                };

                                desg_list.Add(desg);
                            }
                        }
                    }
                }

                return Ok(desg_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("emp_desg/{id}")]
        public async Task<ActionResult<Single_Emp_desg_>> Get_emp_desg_by_id(long id)
        {
            Single_Emp_desg_? desg = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_employee_desg_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@desg_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                desg = new Single_Emp_desg_
                                {
                                    Desg_id = reader.GetInt64(0),
                                    Desg_name = reader.GetString(1),
                                    Desg_desc = reader.GetString(2),
                                    Daily_wk_hr = reader.GetDecimal(3),
                                    Created_Date = reader.GetDateTime(4),
                                    Updated_Date = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                                    User_Id = reader.IsDBNull(6) ? 0 : reader.GetInt64(6),
                                };
                            }
                        }
                    }
                }

                if (desg == null)
                    return NotFound($"Designation with ID {id} not found");

                return Ok(desg);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("dropdown_emp_desg_list")]
        public async Task<ActionResult<IEnumerable<Drop_Emp_desg_>>> Get_drop_emp_desg_list()
        {
            var desg_list = new List<Drop_Emp_desg_>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_employee_desg_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "employee_desg_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var desg = new Drop_Emp_desg_
                                {
                                    Desg_id = reader.GetInt64(0),
                                    Desg_name = reader.GetString(1),
                                };

                                desg_list.Add(desg);
                            }
                        }
                    }
                }

                return Ok(desg_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpPost("insert_employee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddEmployee([FromBody] Add_Employee_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_employee_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@employee_id", request.Employee_id);
                        command.Parameters.AddWithValue("@desg_id", request.Desg_id);
                        command.Parameters.AddWithValue("@first_name", request.First_name);
                        command.Parameters.AddWithValue("@last_name", request.Last_name);
                        command.Parameters.AddWithValue("@gender", request.Gender);
                        command.Parameters.AddWithValue("@dob", request.Dob);
                        command.Parameters.AddWithValue("@phone_number", request.Phone_number);
                        command.Parameters.AddWithValue("@emailid", request.Emailid);
                        command.Parameters.AddWithValue("@address", request.Address);
                        command.Parameters.AddWithValue("@city_id", request.City_id);
                        command.Parameters.AddWithValue("@aadhaar_number", request.Aadhaar_number);
                        command.Parameters.AddWithValue("@pan_number", request.Pan_number);
                        command.Parameters.AddWithValue("@bankaccount_no", request.Bankaccount_no);
                        command.Parameters.AddWithValue("@ifsc_code", request.Ifsc_code);
                        command.Parameters.AddWithValue("@joining_date", request.Joining_date);
                        command.Parameters.AddWithValue("@relieving_date", request.Relieving_date);
                        command.Parameters.AddWithValue("@education", request.Education);
                        command.Parameters.AddWithValue("@exp_year", request.Exp_year);
                        command.Parameters.AddWithValue("@annual_salary", request.Annual_salary);
                        command.Parameters.AddWithValue("@active_status", request.Active_status);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Employee added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add employee." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "Duplicate record exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteEmployee/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmployee(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_employee_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@employee_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Employee deleted successfully." });
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




        [HttpPost("update_employee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEmployee([FromBody] Update_Employee_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_employee_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@employee_id", request.Employee_id);
                    parameters.Add("@desg_id", request.Desg_id);
                    parameters.Add("@first_name", request.First_name);
                    parameters.Add("@last_name", request.Last_name);
                    parameters.Add("@gender", request.Gender);
                    parameters.Add("@dob", request.Dob);
                    parameters.Add("@phone_number", request.Phone_number);
                    parameters.Add("@emailid", request.Emailid);
                    parameters.Add("@address", request.Address);
                    parameters.Add("@city_id", request.City_id);
                    parameters.Add("@aadhaar_number", request.Aadhaar_number);
                    parameters.Add("@pan_number", request.Pan_number);
                    parameters.Add("@bankaccount_no", request.Bankaccount_no);
                    parameters.Add("@ifsc_code", request.Ifsc_code);
                    parameters.Add("@joining_date", request.Joining_date);
                    parameters.Add("@relieving_date", request.Relieving_date);
                    parameters.Add("@education", request.Education);
                    parameters.Add("@exp_year", request.Exp_year);
                    parameters.Add("@annual_salary", request.Annual_salary);
                    parameters.Add("@active_status", request.Active_status);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@comp_id", request.Comp_id);
                    parameters.Add("@user_id", request.User_id);

                    rows_affected = await connection.ExecuteAsync(spname, parameters, commandType: CommandType.StoredProcedure);
                }

                if (rows_affected == 0)
                    return NotFound($"Employee with ID {request.Employee_id} not found");
                else
                    return Ok(new { message = "Employee updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("employee_list")]
        public async Task<ActionResult<IEnumerable<Employee_List>>> GetEmployeeList()
        {
            var employee_list = new List<Employee_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_employee_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var emp = new Employee_List
                                {
                                    Employee_id = reader.GetInt64(0),
                                    Desg_name = reader.GetString(1),
                                    First_name = reader.GetString(2),
                                    Last_name = reader.GetString(3),
                                    Gender = reader.GetString(4),
                                    Dob = reader.IsDBNull(5) ? "" : reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    Phone_number = reader.GetString(6),
                                    Emailid = reader.GetString(7),
                                    Address = reader.GetString(8),
                                    City_name = reader.GetString(9),
                                    Aadhaar_number = reader.GetString(10),
                                    Pan_number = reader.GetString(11),
                                    Bankaccount_no = reader.GetString(12),
                                    Ifsc_code = reader.GetString(13),
                                    Joining_date = reader.IsDBNull(14) ? "" : reader.GetDateTime(14).ToString("yyyy-MM-dd"),
                                    Relieving_date = reader.IsDBNull(15) ? "" : reader.GetDateTime(15).ToString("yyyy-MM-dd"),
                                    Education = reader.GetString(16),
                                    Exp_year = reader.GetDecimal(17),
                                    Annual_salary = reader.GetDecimal(18),
                                    Active_status = reader.GetBoolean(19),
                                    Created_date = reader.IsDBNull(20) ? "" : reader.GetDateTime(20).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(21) ? "" : reader.GetDateTime(21).ToString("yyyy-MM-dd"),
                                    Fin_year_name = reader.GetString(22),
                                    Comp_name = reader.GetString(23),
                                    User_name = reader.IsDBNull(24) ? "" : reader.GetString(24),
                                };

                                employee_list.Add(emp);
                            }
                        }
                    }
                }

                return Ok(employee_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("employee/{id}")]
        public async Task<ActionResult<Single_Employee_>> GetEmployeeById(long id)
        {
            Single_Employee_? emp = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_employee_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@employee_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                emp = new Single_Employee_
                                {
                                    Employee_id = reader.GetInt64(0),
                                    Desg_id = reader.GetInt64(1),
                                    First_name = reader.GetString(2),
                                    Last_name = reader.GetString(3),
                                    Gender = reader.GetString(4),
                                    Dob = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                                    Phone_number = reader.GetString(6),
                                    Emailid = reader.GetString(7),
                                    Address = reader.GetString(8),
                                    City_id = reader.GetInt64(9),
                                    Aadhaar_number = reader.GetString(10),
                                    Pan_number = reader.GetString(11),
                                    Bankaccount_no = reader.GetString(12),
                                    Ifsc_code = reader.GetString(13),
                                    Joining_date = reader.IsDBNull(14) ? null : reader.GetDateTime(14),
                                    Relieving_date = reader.IsDBNull(15) ? null : reader.GetDateTime(15),
                                    Education = reader.GetString(16),
                                    Exp_year = reader.GetDecimal(17),
                                    Annual_salary = reader.GetDecimal(18),
                                    Active_status = reader.GetBoolean(19),
                                    Created_date = reader.IsDBNull(20) ? null : reader.GetDateTime(20),
                                    Updated_date = reader.IsDBNull(21) ? null : reader.GetDateTime(21),
                                    Fin_year_id = reader.GetInt64(22),
                                    Comp_id = reader.GetInt64(23),
                                    User_id = reader.GetInt64(24),
                                };
                            }
                        }
                    }
                }

                if (emp == null)
                    return NotFound($"Employee with ID {id} not found");

                return Ok(emp);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("dropdown_employee_list")]
        public async Task<ActionResult<IEnumerable<Drop_Employee_>>> GetDropEmployeeList()
        {
            var emp_list = new List<Drop_Employee_>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_employee_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "employee_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var emp = new Drop_Employee_
                                {
                                    Employee_id = reader.GetInt64(0),
                                    First_name = reader.GetString(1)
                                };

                                emp_list.Add(emp);
                            }
                        }
                    }
                }

                return Ok(emp_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpPost("insert_leavetype")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddLeaveType([FromBody] Add_LeaveType_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_leavetype_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@leavetype_id", request.Leavetype_id);
                        command.Parameters.AddWithValue("@yearsincompany", request.Yearsincompany);
                        command.Parameters.AddWithValue("@allocated_leaves", request.Allocated_leaves);
                        command.Parameters.AddWithValue("@leave_name", request.Leave_name);
                        command.Parameters.AddWithValue("@leave_desc", request.Leave_desc);
                        command.Parameters.AddWithValue("@casual_leaves", request.Casual_leaves);
                        command.Parameters.AddWithValue("@sick_leaves", request.Sick_leaves);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Leave Type added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add Leave Type." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Leave Type name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }




        [HttpDelete("DeleteLeaveType/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteLeaveType(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_leavetype_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@leavetype_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Leave Type deleted successfully." });
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




        [HttpPost("UpdateLeaveType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateLeaveType([FromBody] Update_LeaveType_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_leavetype_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@leavetype_id", request.Leavetype_id);
                    parameters.Add("@yearsincompany", request.Yearsincompany);
                    parameters.Add("@allocated_leaves", request.Allocated_leaves);
                    parameters.Add("@leave_name", request.Leave_name);
                    parameters.Add("@leave_desc", request.Leave_desc);
                    parameters.Add("@casual_leaves", request.Casual_leaves);
                    parameters.Add("@sick_leaves", request.Sick_leaves);
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
                    return NotFound($"Leave Type with ID {request.Leavetype_id} not found");
                else
                    return Ok(new { message = "Leave Type updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("leavetype_list")]
        public async Task<ActionResult<IEnumerable<LeaveType_List>>> Get_LeaveType_List()
        {
            var leavelist = new List<LeaveType_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_leavetype_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var leavetype = new LeaveType_List
                                {
                                    Leavetype_id = reader.GetInt64(0),
                                    Yearsincompany = reader.GetDecimal(1),
                                    Allocated_leaves = reader.GetDecimal(2),
                                    Leave_name = reader.GetString(3),
                                    Leave_desc = reader.GetString(4),
                                    Casual_leaves = reader.GetDecimal(5),
                                    Sick_leaves = reader.GetDecimal(6),
                                    Created_date = reader.GetDateTime(7).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(8) ? "" : reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(9) ? "" : reader.GetString(9)
                                };

                                leavelist.Add(leavetype);
                            }
                        }
                    }
                }

                return Ok(leavelist);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("leavetype/{id}")]
        public async Task<ActionResult<Single_LeaveType_>> Get_LeaveType_By_Id(long id)
        {
            Single_LeaveType_? leavetype = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_leavetype_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@leavetype_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                leavetype = new Single_LeaveType_
                                {
                                    Leavetype_id = reader.GetInt64(0),
                                    Yearsincompany = reader.GetDecimal(1),
                                    Allocated_leaves = reader.GetDecimal(2),
                                    Leave_name = reader.GetString(3),
                                    Leave_desc = reader.GetString(4),
                                    Casual_leaves = reader.GetDecimal(5),
                                    Sick_leaves = reader.GetDecimal(6),
                                    Created_date = reader.GetDateTime(7),
                                    Updated_date = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                                    User_id = reader.IsDBNull(9) ? 0 : reader.GetInt64(9)
                                };
                            }
                        }
                    }
                }

                if (leavetype == null)
                    return NotFound($"Leave Type with ID {id} not found");

                return Ok(leavetype);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("dropdown_leavetype_list")]
        public async Task<ActionResult<IEnumerable<Drop_LeaveType_>>> Get_Drop_LeaveType_List()
        {
            var leavelist = new List<Drop_LeaveType_>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_leavetype_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "leavetype_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var leave = new Drop_LeaveType_
                                {
                                    Leavetype_id = reader.GetInt64(0),
                                    Leave_name = reader.GetString(1)
                                };

                                leavelist.Add(leave);
                            }
                        }
                    }
                }

                return Ok(leavelist);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpPost("insert_emp_leave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddEmpLeave([FromBody] Add_Emp_Leave_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_emp_leave_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@emp_leave_id", request.Emp_leave_id);
                        command.Parameters.AddWithValue("@employee_id", request.Employee_id);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                        command.Parameters.AddWithValue("@month_id", request.Month_id);
                        command.Parameters.AddWithValue("@emp_leave_date", request.Emp_leave_date);
                        command.Parameters.AddWithValue("@leavetype_id", request.Leavetype_id);
                        command.Parameters.AddWithValue("@total_allocated_leaves", request.Total_allocated_leaves);
                        command.Parameters.AddWithValue("@leaves_used", request.Leaves_used);
                        command.Parameters.AddWithValue("@leaves_balance", request.Leaves_balance);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Emp Leave added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add Emp Leave." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Duplicate Emp Leave entry already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }




        [HttpDelete("delete_emp_leave/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmpLeave(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_emp_leave_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@emp_leave_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Emp Leave deleted successfully." });
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



        [HttpPost("update_emp_leave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEmpLeave([FromBody] Update_Emp_Leave_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_emp_leave_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@emp_leave_id", request.Emp_leave_id);
                    parameters.Add("@employee_id", request.Employee_id);
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@month_id", request.Month_id);
                    parameters.Add("@emp_leave_date", request.Emp_leave_date);
                    parameters.Add("@leavetype_id", request.Leavetype_id);
                    parameters.Add("@total_allocated_leaves", request.Total_allocated_leaves);
                    parameters.Add("@leaves_used", request.Leaves_used);
                    parameters.Add("@leaves_balance", request.Leaves_balance);
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
                    return NotFound($"Emp Leave with ID {request.Emp_leave_id} not found");
                else
                    return Ok(new { message = "Emp Leave updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("emp_leave_list")]
        public async Task<ActionResult<IEnumerable<Emp_Leave_List>>> Get_Emp_Leave_List()
        {
            var list = new List<Emp_Leave_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_emp_leave_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new Emp_Leave_List
                                {
                                    Emp_leave_id = reader.GetInt64(0),
                                    First_name = reader.GetString(1),
                                    Last_name = reader.GetString(2),
                                    Fin_name = reader.GetString(3),
                                    Month_name = reader.GetString(4),
                                    Emp_leave_date = reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    Leave_name = reader.GetString(6),
                                    Total_allocated_leaves = reader.GetDecimal(7),
                                    Leaves_used = reader.GetDecimal(8),
                                    Leaves_balance = reader.GetDecimal(9),
                                    Created_date = reader.GetDateTime(10).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(11) ? "" : reader.GetDateTime(11).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(12) ? "" : reader.GetString(12)
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





        [HttpGet("emp_leave/{id}")]
        public async Task<ActionResult<Single_Emp_Leave>> Get_Emp_Leave_By_Id(long id)
        {
            Single_Emp_Leave? item = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_emp_leave_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@emp_leave_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                item = new Single_Emp_Leave
                                {
                                    Emp_leave_id = reader.GetInt64(0),
                                    Employee_id = reader.GetInt64(1),
                                    Fin_year_id = reader.GetInt64(2),
                                    Month_id = reader.GetInt64(3),
                                    Emp_leave_date = reader.GetDateTime(4),
                                    Leavetype_id = reader.GetInt64(5),
                                    Total_allocated_leaves = reader.GetDecimal(6),
                                    Leaves_used = reader.GetDecimal(7),
                                    Leaves_balance = reader.GetDecimal(8),
                                    Created_date = reader.GetDateTime(9),
                                    Updated_date = reader.IsDBNull(10) ? null : reader.GetDateTime(10),
                                    User_id = reader.IsDBNull(11) ? 0 : reader.GetInt64(11)
                                };
                            }
                        }
                    }
                }

                if (item == null)
                    return NotFound($"Emp Leave with ID {id} not found");

                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("dropdown_emp_leave_list")]
        public async Task<ActionResult<IEnumerable<Drop_Emp_Leave>>> Get_Drop_Emp_Leave_List()
        {
            var list = new List<Drop_Emp_Leave>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_emp_leave_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "emp_leavelist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new Drop_Emp_Leave
                                {
                                    Emp_leave_id = reader.GetInt64(0),
                                    Employee_id = reader.GetInt64(1)
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



        [HttpPost("insert_payment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPayment([FromBody] Add_Payment_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_payment_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@payment_id", request.Payment_id);
                        command.Parameters.AddWithValue("@trans_id", request.Trans_id);
                        command.Parameters.AddWithValue("@purchase_id", request.Purchase_id);
                        command.Parameters.AddWithValue("@emp_payslip_id", request.Emp_payslip_id);
                        command.Parameters.AddWithValue("@payment_date", request.Payment_date);
                        command.Parameters.AddWithValue("@net_total", request.Net_total);
                        command.Parameters.AddWithValue("@balance_total", request.Balance_total);
                        command.Parameters.AddWithValue("@payment_status", request.Payment_status);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Payment added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Payment." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }




        [HttpDelete("delete_payment/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePayment(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_payment_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@payment_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Payment deleted successfully." });
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





        [HttpPost("update_payment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdatePayment([FromBody] Update_Payment_Request request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_payment_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@payment_id", request.Payment_id);
                    parameters.Add("@trans_id", request.Trans_id);
                    parameters.Add("@purchase_id", request.Purchase_id);
                    parameters.Add("@emp_payslip_id", request.Emp_payslip_id);
                    parameters.Add("@payment_date", request.Payment_date);
                    parameters.Add("@net_total", request.Net_total);
                    parameters.Add("@balance_total", request.Balance_total);
                    parameters.Add("@payment_status", request.Payment_status);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@comp_id", request.Comp_id);
                    parameters.Add("@user_id", request.User_id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"Payment with ID {request.Payment_id} not found");
                else
                    return Ok(new { message = "Payment updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("payment_list")]
        public async Task<ActionResult<IEnumerable<Payment_List>>> Get_payment_list()
        {
            var payment_list = new List<Payment_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_payment_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var payment = new Payment_List
                                {
                                    Payment_id = reader.GetInt64(0),
                                    Trans_id = reader.GetInt64(1),
                                    Purchase_id = reader.GetInt64(2),
                                    Emp_payslip_id = reader.GetInt64(3),
                                    Payment_date = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    Net_total = reader.GetDecimal(5),
                                    Balance_total = reader.GetDecimal(6),
                                    Payment_status = reader.GetBoolean(7),
                                    Created_date = reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(9) ? "" : reader.GetDateTime(9).ToString("yyyy-MM-dd"),
                                    Fin_year_name = reader.GetString(10),
                                    Comp_name = reader.GetString(11),
                                    User_name = reader.IsDBNull(12) ? "" : reader.GetString(12)
                                };

                                payment_list.Add(payment);
                            }
                        }
                    }
                }

                return Ok(payment_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("payment/{id}")]
        public async Task<ActionResult<Single_Payment>> Get_payment_by_id(long id)
        {
            Single_Payment? payment = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_payment_mast_ins_upd_del"; 

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@payment_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                payment = new Single_Payment
                                {
                                    Payment_id = reader.GetInt64(0),
                                    Trans_id = reader.GetInt64(1),
                                    Purchase_id = reader.GetInt64(2),
                                    Emp_payslip_id = reader.GetInt64(3),
                                    Payment_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                    Net_total = reader.GetDecimal(5),
                                    Balance_total = reader.GetDecimal(6),
                                    Payment_status = reader.GetBoolean(7),
                                    Created_date = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                                    Updated_date = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                                    Fin_year_id = reader.GetInt64(10),
                                    Comp_id = reader.GetInt64(11),
                                    User_id = reader.IsDBNull(12) ? 0 : reader.GetInt64(12)
                                };
                            }
                        }
                    }
                }

                if (payment == null)
                    return NotFound($"Payment with ID {id} not found");

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }





        [HttpGet("dropdown_payment_list")]
        public async Task<ActionResult<IEnumerable<Drop_Payment>>> Get_drop_payment_list()
        {
            var payment_list = new List<Drop_Payment>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_payment_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "payment_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var payment = new Drop_Payment
                                {
                                    Payment_id = reader.GetInt64(0),
                                    User_id = reader.GetInt64(1)
                                };

                                payment_list.Add(payment);
                            }
                        }
                    }
                }

                return Ok(payment_list);
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


        public class AddColourRequest
        {
            public long ColourId { get; set; } = 0;
            public string ColourName { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long? User_id { get; set; } = 0;
        }


        public class UpdateColourRequest
        {
            public long ColourId { get; set; } = 0;
            public string ColourName { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }

        public class Colour_list
        {
            public long ColourId { get; set; } = 0;
            public string ColourName { get; set; } = "";
            public bool? IsActive { get; set; }
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";

        }


        public class Single_Colour_list
        {
            public long ColourId { get; set; } = 0;
            public string ColourName { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class drop_Colour_list
        {
            public long ColourId { get; set; }
            public string ColourName { get; set; } = "";

        }


        public class AddUnitRequest
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";
            public string UnitDesc { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long? User_id { get; set; } = 0;
        }


        public class UpdateUnitRequest
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";
            public string UnitDesc { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }

        public class Unit_list
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";
            public string UnitDesc { get; set; } = "";
            public bool? IsActive { get; set; }
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";

        }


        public class Single_Unit_list
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";
            public string UnitDesc { get; set; } = "";
            public bool? IsActive { get; set; }
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class drop_Unit_list
        {
            public long UnitId { get; set; } = 0;
            public string UnitName { get; set; } = "";

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
            public long? User_id { get; set; } = 0;
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
            public long User_id { get; set; } = 0;
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
            public string User_name { get; set; } = "";

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
            public long User_id { get; set; } = 0;

        }


        public class drop_Hsn_list
        {
            public long HsnId { get; set; } = 0;
            public string HsnCode { get; set; } = "";

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


        public class AddBrandRequest
        {
            public long Brand_Id { get; set; } = 0;
            public string Brand_Name { get; set; } = "";
            public string Brand_Desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long? User_id { get; set; } = 0;
        }


        public class UpdateBrandRequest
        {
            public long Brand_Id { get; set; } = 0;
            public string Brand_Name { get; set; } = "";
            public string Brand_Desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }

        public class Brand_list
        {
            public long Brand_Id { get; set; } = 0;
            public string Brand_Name { get; set; } = "";
            public string Brand_Desc { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";

        }


        public class Single_Brand_list
        {
            public long Brand_Id { get; set; } = 0;
            public string Brand_Name { get; set; } = "";
            public string Brand_Desc { get; set; } = "";
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class drop_Brand_list
        {
            public long Brand_Id { get; set; } = 0;
            public string Brand_Name { get; set; } = "";

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


        public class AddCustomerRequest
        {
            public string Customer_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public long City_Id { get; set; } = 0;
            public string Cust_Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; } = true;
            public string Customer_Notes { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class UpdateCustomerRequest
        {
            public long Customer_Id { get; set; } = 0;
            public string Customer_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public long City_Id { get; set; } = 0;
            public string Cust_Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; } = true;
            public string Customer_Notes { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Customer_List
        {
            public long Customer_Id { get; set; } = 0;
            public string Customer_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public string City_Name { get; set; } = "";
            public string Cust_Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public string Dob { get; set; } = "";
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; }
            public string Customer_Notes { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }


        public class Single_Customer_List
        {
            public long Customer_Id { get; set; } = 0;
            public string Customer_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public long City_Id { get; set; } = 0;
            public string Cust_Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; }
            public string Customer_Notes { get; set; } = "";
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_Customer_List
        {
            public long Customer_Id { get; set; } = 0;
            public string Customer_Name { get; set; } = "";
        }



        public class AddVendorRequest
        {
            public string Vendor_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public long City_Id { get; set; } = 0;
            public string Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; } = true;
            public string Vendor_Notes { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_Id { get; set; } = 0;
        }



        public class UpdateVendorRequest
        {
            public long Vendor_Id { get; set; } = 0;
            public string Vendor_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public long City_Id { get; set; } = 0;
            public string Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; } = true;
            public string Vendor_Notes { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_Id { get; set; } = 0;
        }




        public class Vendor_List
        {
            public long Vendor_Id { get; set; } = 0;
            public string Vendor_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public string City_Name { get; set; } = "";
            public string Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public string Dob { get; set; } = "";
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; }
            public string Vendor_Notes { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }


        public class Single_Vendor_List
        {
            public long Vendor_Id { get; set; } = 0;
            public string Vendor_Name { get; set; } = "";
            public string Prefix { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Phonenumber { get; set; } = "";
            public long City_Id { get; set; } = 0;
            public string Address { get; set; } = "";
            public string Email_Id { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Aadhaar_Number { get; set; } = "";
            public string License_Number { get; set; } = "";
            public string Pan_Number { get; set; } = "";
            public string Gst_Number { get; set; } = "";
            public bool Is_Active { get; set; }
            public string Vendor_Notes { get; set; } = "";
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_Vendor_List
        {
            public long Vendor_Id { get; set; } = 0;
            public string Vendor_Name { get; set; } = "";
        }


        public class AddProductRequest
        {

            public long Product_Id { get; set; } = 0;
            public long Prodtype_id { get; set; } = 0;
            public long Brand_id { get; set; } = 0;
            public long Hsn_id { get; set; } = 0;
            public long Unit_id { get; set; } = 0;
            public string Product_name { get; set; } = "";
            public string Product_desc { get; set; } = "";
            public decimal Rate { get; set; } = 0;
            public decimal Opening_stock { get; set; } = 0;
            public decimal Purchase { get; set; } = 0;
            public decimal Sales { get; set; } = 0;
            public decimal Return { get; set; } = 0;
            public decimal Current_stock { get; set; } = 0;
            public decimal Reorder_threshold { get; set; } = 0;
            public string Reorder_desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_Id { get; set; } = 0;

        }


        public class UpdateProductRequest
        {
            public long Product_Id { get; set; } = 0;
            public long Prodtype_id { get; set; } = 0;
            public long Brand_id { get; set; } = 0;
            public long Hsn_id { get; set; } = 0;
            public long Unit_id { get; set; } = 0;
            public string Product_name { get; set; } = "";
            public string Product_desc { get; set; } = "";
            public decimal Rate { get; set; } = 0;
            public decimal Opening_stock { get; set; } = 0;
            public decimal Purchase { get; set; } = 0;
            public decimal Sales { get; set; } = 0;
            public decimal Return { get; set; } = 0;
            public decimal Current_stock { get; set; } = 0;
            public decimal Reorder_threshold { get; set; } = 0;
            public string Reorder_desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_Id { get; set; } = 0;

        }

        public class Product_list
        {

            public long Product_Id { get; set; } = 0;
            public string Prodtype_name { get; set; } = "";
            public string Brand_name { get; set; } = "";
            public string Hsn_name { get; set; } = "";
            public string Unit_name { get; set; } = "";
            public string Product_name { get; set; } = "";
            public string Product_desc { get; set; } = "";
            public decimal Rate { get; set; } = 0;
            public decimal Opening_stock { get; set; } = 0;
            public decimal Purchase { get; set; } = 0;
            public decimal Sales { get; set; } = 0;
            public decimal Return { get; set; } = 0;
            public decimal Current_stock { get; set; } = 0;
            public decimal Reorder_threshold { get; set; } = 0;
            public string Reorder_desc { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }

        public class SingleProductList
        {
            public long Product_Id { get; set; } = 0;
            public long Prodtype_id { get; set; } = 0;
            public long Brand_id { get; set; } = 0;
            public long Hsn_id { get; set; } = 0;
            public long Unit_id { get; set; } = 0;
            public string Product_name { get; set; } = "";
            public string Product_desc { get; set; } = "";
            public decimal Rate { get; set; } = 0;
            public decimal Opening_stock { get; set; } = 0;
            public decimal Purchase { get; set; } = 0;
            public decimal Sales { get; set; } = 0;
            public decimal Return { get; set; } = 0;
            public decimal Current_stock { get; set; } = 0;
            public decimal Reorder_threshold { get; set; } = 0;
            public string Reorder_desc { get; set; } = "";
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_Product_List
        {
            public long Product_Id { get; set; } = 0;
            public string Product_name { get; set; } = "";
        }

        public class AddInwardRequest
        {
            public long Inward_Id { get; set; } = 0;
            public long Customer_Id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public decimal TotalQuantity { get; set; } = 0;
            public decimal Balance { get; set; } = 0;
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
            public decimal Balance { get; set; } = 0;
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
            public decimal Balance { get; set; } = 0;
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
            public decimal Balance { get; set; } = 0;
            public bool Inward_Status { get; set; } = false;
            public string Remarks { get; set; } = "";
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
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


        public class AddsalesinvoiceRequest
        {
            public long Sales_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public long Customer_id { get; set; } = 0;
            public DateTime Sales_date { get; set; }
            public decimal Gross_total { get; set; } = 0;
            public decimal Sgst_total { get; set; } = 0;
            public decimal Cgst_total { get; set; } = 0;
            public decimal Igst_total { get; set; } = 0;
            public decimal Discount_total { get; set; } = 0;
            public decimal Roundoff_total { get; set; } = 0;
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public bool Isactive { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class UpdatesalesinvoiceRequest
        {
            public long Sales_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public long Customer_id { get; set; } = 0;
            public DateTime Sales_date { get; set; }
            public decimal Gross_total { get; set; } = 0;
            public decimal Sgst_total { get; set; } = 0;
            public decimal Cgst_total { get; set; } = 0;
            public decimal Igst_total { get; set; } = 0;
            public decimal Discount_total { get; set; } = 0;
            public decimal Roundoff_total { get; set; } = 0;
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public bool Isactive { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class salesinvoice_List
        {
            public long Sales_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public string Customer_name { get; set; } = "";
            public DateTime Sales_date { get; set; }
            public decimal Gross_total { get; set; } = 0;
            public decimal Sgst_total { get; set; } = 0;
            public decimal Cgst_total { get; set; } = 0;
            public decimal Igst_total { get; set; } = 0;
            public decimal Discount_total { get; set; } = 0;
            public decimal Roundoff_total { get; set; } = 0;
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public bool Isactive { get; set; } = false;
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }

        public class Singlesalesinvoice
        {
            public long Sales_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public long Customer_id { get; set; } = 0;
            public DateTime Sales_date { get; set; }
            public decimal Gross_total { get; set; } = 0;
            public decimal Sgst_total { get; set; } = 0;
            public decimal Cgst_total { get; set; } = 0;
            public decimal Igst_total { get; set; } = 0;
            public decimal Discount_total { get; set; } = 0;
            public decimal Roundoff_total { get; set; } = 0;
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public bool Isactive { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_salesinvoice_List
        {
            public long Sales_id { get; set; } = 0;
        }


        public class AddChallanRequest
        {
            public long Challan_id { get; set; } = 0;
            public long Sales_id { get; set; } = 0;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class UpdateChallanRequest
        {
            public long Challan_id { get; set; } = 0;
            public long Sales_id { get; set; } = 0;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class Challan_List
        { 
            public long Challan_id { get; set; } = 0;
            public long Sales_id { get; set; } = 0;
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }

        public class SingleChallaninvoice
        {
            public long Challan_id { get; set; } = 0;
            public long Sales_id { get; set; } = 0;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_Challan_List
        {
            public long Challan_id { get; set; } = 0;
        }


        public class AddReceiptRequest
        {

            public long Receipt_id { get; set; } = 0;
            public long Sales_id { get; set; } = 0;
            public DateTime Recepit_date { get; set; }
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_amount { get; set; } = 0;
            public bool Receipt_status { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class UpdateReceiptRequest
        {
            public long Receipt_id { get; set; } = 0;
            public long Sales_id { get; set; } = 0;
            public DateTime Recepit_date { get; set; }
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_amount { get; set; } = 0;
            public bool Receipt_status { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class Receipt_List
        {
            public long Receipt_id { get; set; } = 0;
            public long Sales_id { get; set; } = 0;
            public DateTime Recepit_date { get; set; }
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_amount { get; set; } = 0;
            public bool Receipt_status { get; set; } = false;
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }

        public class SingleReceiptinvoice
        {
            public long Receipt_id { get; set; } = 0;
            public long Sales_id { get; set; } = 0;
            public DateTime Recepit_date { get; set; }
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_amount { get; set; } = 0;
            public bool Receipt_status { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_Receipt_List
        {
            public long Receipt_id { get; set; } = 0;
            public long Sales_id { get; set; } = 0;
        }


        public class AddpurchaseinvoiceRequest
        {
            public long Purchase_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public string Invoice_no { get; set; } = "";
            public DateTime Purchase_date { get; set; }
            public long Vendor_id { get; set; } = 0;
            public decimal Gross_total { get; set; } = 0;
            public decimal Sgst_total { get; set; } = 0;
            public decimal Cgst_total { get; set; } = 0;
            public decimal Igst_total { get; set; } = 0;
            public decimal Discount_total { get; set; } = 0;
            public decimal Roundoff_total { get; set; } = 0;
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public bool Isactive { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class UpdatepurchaseinvoiceRequest
        {
            public long Purchase_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public string Invoice_no { get; set; } = "";
            public DateTime Purchase_date { get; set; }
            public long Vendor_id { get; set; } = 0;
            public decimal Gross_total { get; set; } = 0;
            public decimal Sgst_total { get; set; } = 0;
            public decimal Cgst_total { get; set; } = 0;
            public decimal Igst_total { get; set; } = 0;
            public decimal Discount_total { get; set; } = 0;
            public decimal Roundoff_total { get; set; } = 0;
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public bool Isactive { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class purchaseinvoice_List
        {
            public long Purchase_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public string Invoice_no { get; set; } = "";
            public DateTime Purchase_date { get; set; }
            public string Vendor_name { get; set; } = "";
            public decimal Gross_total { get; set; } = 0;
            public decimal Sgst_total { get; set; } = 0;
            public decimal Cgst_total { get; set; } = 0;
            public decimal Igst_total { get; set; } = 0;
            public decimal Discount_total { get; set; } = 0;
            public decimal Roundoff_total { get; set; } = 0;
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public bool Isactive { get; set; } = false;
            public string Fin_Year_Name { get; set; } = "";
            public string Comp_Name { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }

        public class Singlepurchaseinvoice
        {
            public long Purchase_id { get; set; } = 0;
            public string Prefix { get; set; } = "";
            public string Suffix { get; set; } = "";
            public string Invoice_no { get; set; } = "";
            public DateTime Purchase_date { get; set; }
            public long Vendor_id { get; set; } = 0;
            public decimal Gross_total { get; set; } = 0;
            public decimal Sgst_total { get; set; } = 0;
            public decimal Cgst_total { get; set; } = 0;
            public decimal Igst_total { get; set; } = 0;
            public decimal Discount_total { get; set; } = 0;
            public decimal Roundoff_total { get; set; } = 0;
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public bool Isactive { get; set; } = false;
            public long Fin_Year_Id { get; set; } = 0;
            public long Comp_Id { get; set; } = 0;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_purchaseinvoice_List
        {
            public long Sales_id { get; set; } = 0;
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
            public string product_name{ get; set; } = "";
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


        public class AddUserRequest
        {
            public long User_id { get; set; } = 0;
            public string User_name { get; set; } = "";
            public string User_password { get; set; } = "";
            public string User_role { get; set; } = "";
            public bool Is_login { get; set; } = false;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }

        }

        public class UpdateUserRequest
        {
            public long User_id { get; set; } = 0;
            public string User_name { get; set; } = "";
            public string User_password { get; set; } = "";
            public string User_role { get; set; } = "";
            public bool Is_login { get; set; } = false;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
        }

        public class User_List
        {
            public long User_id { get; set; } = 0;
            public string User_name { get; set; } = "";
            public string User_password { get; set; } = "";
            public string User_role { get; set; } = "";
            public bool Is_login { get; set; } = false;
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
        }

        public class SingleUser
        {
            public long User_id { get; set; } = 0;
            public string User_name { get; set; } = "";
            public string User_password { get; set; } = "";
            public string User_role { get; set; } = "";
            public bool Is_login { get; set; } = false;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
        }


        public class Drop_User_List
        {
            public long User_id { get; set; } = 0;
            public string User_name { get; set; } = "";
        }




        public class Add_Emp_desg_Request
        {
            public long Desg_id { get; set; } = 0;
            public string Desg_name { get; set; } = "";
            public string Desg_desc { get; set; } = "";
            public decimal Daily_wk_hr { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;

        }

        public class Update_Emp_desg_Request
        {
            public long Desg_id { get; set; } = 0;
            public string Desg_name { get; set; } = "";
            public string Desg_desc { get; set; } = "";
            public decimal Daily_wk_hr { get; set; } = 0;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }

        public class Emp_desg_List
        {
            public long Desg_id { get; set; } = 0;
            public string Desg_name { get; set; } = "";
            public string Desg_desc { get; set; } = "";
            public decimal Daily_wk_hr { get; set; } = 0;
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_name { get; set; } = "";
        }

        public class Single_Emp_desg_
        {
            public long Desg_id { get; set; } = 0;
            public string Desg_name { get; set; } = "";
            public string Desg_desc { get; set; } = "";
            public decimal Daily_wk_hr { get; set; } = 0;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_Emp_desg_
        {
            public long Desg_id { get; set; } = 0;
            public string Desg_name { get; set; } = "";

        }



        public class Add_Employee_Request
        {
            public long Employee_id { get; set; } = 0;
            public long Desg_id { get; set; } = 0;
            public string First_name { get; set; } = "";
            public string Last_name { get; set; } = "";
            public string Gender { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Phone_number { get; set; } = "";
            public string Emailid { get; set; } = "";
            public string Address { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Aadhaar_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Bankaccount_no { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public DateTime? Joining_date { get; set; }
            public DateTime? Relieving_date { get; set; }
            public string Education { get; set; } = "";
            public decimal Exp_year { get; set; } = 0;
            public decimal Annual_salary { get; set; } = 0;
            public bool Active_status { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }


        public class Update_Employee_Request
        {
            public long Employee_id { get; set; } = 0;
            public long Desg_id { get; set; } = 0;
            public string First_name { get; set; } = "";
            public string Last_name { get; set; } = "";
            public string Gender { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Phone_number { get; set; } = "";
            public string Emailid { get; set; } = "";
            public string Address { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Aadhaar_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Bankaccount_no { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public DateTime? Joining_date { get; set; }
            public DateTime? Relieving_date { get; set; }
            public string Education { get; set; } = "";
            public decimal Exp_year { get; set; } = 0;
            public decimal Annual_salary { get; set; } = 0;
            public bool Active_status { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }




        public class Employee_List
        {
            public long Employee_id { get; set; } = 0;
            public string Desg_name { get; set; } = "";
            public string First_name { get; set; } = "";
            public string Last_name { get; set; } = "";
            public string Gender { get; set; } = "";
            public string Dob { get; set; } = "";
            public string Phone_number { get; set; } = "";
            public string Emailid { get; set; } = "";
            public string Address { get; set; } = "";
            public string City_name { get; set; } = "";
            public string Aadhaar_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Bankaccount_no { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public string Joining_date { get; set; } = "";
            public string Relieving_date { get; set; } = "";
            public string Education { get; set; } = "";
            public decimal Exp_year { get; set; } = 0;
            public decimal Annual_salary { get; set; } = 0;
            public bool Active_status { get; set; } = false;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string Fin_year_name { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string User_name { get; set; } = "";
        }




        public class Single_Employee_
        {
            public long Employee_id { get; set; } = 0;
            public long Desg_id { get; set; } = 0;
            public string First_name { get; set; } = "";
            public string Last_name { get; set; } = "";
            public string Gender { get; set; } = "";
            public DateTime? Dob { get; set; }
            public string Phone_number { get; set; } = "";
            public string Emailid { get; set; } = "";
            public string Address { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Aadhaar_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Bankaccount_no { get; set; } = "";
            public string Ifsc_code { get; set; } = "";
            public DateTime? Joining_date { get; set; }
            public DateTime? Relieving_date { get; set; }
            public string Education { get; set; } = "";
            public decimal Exp_year { get; set; } = 0;
            public decimal Annual_salary { get; set; } = 0;
            public bool Active_status { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }



        public class Drop_Employee_
        {
            public long Employee_id { get; set; } = 0;
            public string First_name { get; set; } = "";
        }


        public class Add_LeaveType_Request
        {
            public long Leavetype_id { get; set; } = 0;
            public decimal Yearsincompany { get; set; } = 0;
            public decimal Allocated_leaves { get; set; } = 0;
            public string Leave_name { get; set; } = "";
            public string Leave_desc { get; set; } = "";
            public decimal Casual_leaves { get; set; } = 0;
            public decimal Sick_leaves { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }




        public class Update_LeaveType_Request
        {
            public long Leavetype_id { get; set; } = 0;
            public decimal Yearsincompany { get; set; } = 0;
            public decimal Allocated_leaves { get; set; } = 0;
            public string Leave_name { get; set; } = "";
            public string Leave_desc { get; set; } = "";
            public decimal Casual_leaves { get; set; } = 0;
            public decimal Sick_leaves { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }



        public class LeaveType_List
        {
            public long Leavetype_id { get; set; } = 0;
            public decimal Yearsincompany { get; set; } = 0;
            public decimal Allocated_leaves { get; set; } = 0;
            public string Leave_name { get; set; } = "";
            public string Leave_desc { get; set; } = "";
            public decimal Casual_leaves { get; set; } = 0;
            public decimal Sick_leaves { get; set; } = 0;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";
        }



        public class Single_LeaveType_
        {
            public long Leavetype_id { get; set; } = 0;
            public decimal Yearsincompany { get; set; } = 0;
            public decimal Allocated_leaves { get; set; } = 0;
            public string Leave_name { get; set; } = "";
            public string Leave_desc { get; set; } = "";
            public decimal Casual_leaves { get; set; } = 0;
            public decimal Sick_leaves { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }



        public class Drop_LeaveType_
        {
            public long Leavetype_id { get; set; } = 0;
            public string Leave_name { get; set; } = "";
        }


        public class Add_Emp_Leave_Request
        {
            public long Emp_leave_id { get; set; } = 0;
            public long Employee_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public DateTime? Emp_leave_date { get; set; }
            public long Leavetype_id { get; set; } = 0;
            public decimal Total_allocated_leaves { get; set; } = 0;
            public decimal Leaves_used { get; set; } = 0;
            public decimal Leaves_balance { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }


        public class Update_Emp_Leave_Request
        {
            public long Emp_leave_id { get; set; } = 0;
            public long Employee_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public DateTime? Emp_leave_date { get; set; }
            public long Leavetype_id { get; set; } = 0;
            public decimal Total_allocated_leaves { get; set; } = 0;
            public decimal Leaves_used { get; set; } = 0;
            public decimal Leaves_balance { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }


        public class Emp_Leave_List
        {
            public long Emp_leave_id { get; set; } = 0;
            public string First_name { get; set; } = "";
            public string Last_name { get; set; } = "";
            public string Fin_name { get; set; } = "";
            public string Month_name { get; set; } = "";
            public string Emp_leave_date { get; set; } = "";
            public string Leave_name { get; set; } = "";
            public decimal Total_allocated_leaves { get; set; } = 0;
            public decimal Leaves_used { get; set; } = 0;
            public decimal Leaves_balance { get; set; } = 0;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";
        }


        public class Single_Emp_Leave
        {
            public long Emp_leave_id { get; set; } = 0;
            public long Employee_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public long Month_id { get; set; } = 0;
            public DateTime? Emp_leave_date { get; set; }
            public long Leavetype_id { get; set; } = 0;
            public decimal Total_allocated_leaves { get; set; } = 0;
            public decimal Leaves_used { get; set; } = 0;
            public decimal Leaves_balance { get; set; } = 0;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }


        public class Drop_Emp_Leave
        {
            public long Emp_leave_id { get; set; } = 0;
            public long Employee_id { get; set; } = 0;
        }


        public class Add_Payment_Request
        {
            public long Payment_id { get; set; } = 0;
            public long Trans_id { get; set; } = 0;
            public long Purchase_id { get; set; } = 0;
            public long Emp_payslip_id { get; set; } = 0;
            public DateTime? Payment_date { get; set; }
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }

        public class Update_Payment_Request
        {
            public long Payment_id { get; set; } = 0;
            public long Trans_id { get; set; } = 0;
            public long Purchase_id { get; set; } = 0;
            public long Emp_payslip_id { get; set; } = 0;
            public DateTime? Payment_date { get; set; }
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }

        public class Payment_List
        {
            public long Payment_id { get; set; } = 0;
            public long Trans_id { get; set; } = 0;
            public long Purchase_id { get; set; } = 0;
            public long Emp_payslip_id { get; set; } = 0;
            public string Payment_date { get; set; } = "";
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string Fin_year_name { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string User_name { get; set; } = "";
        }

        public class Single_Payment
        {
            public long Payment_id { get; set; } = 0;
            public long Trans_id { get; set; } = 0;
            public long Purchase_id { get; set; } = 0;
            public long Emp_payslip_id { get; set; } = 0;
            public DateTime? Payment_date { get; set; }
            public decimal Net_total { get; set; } = 0;
            public decimal Balance_total { get; set; } = 0;
            public bool Payment_status { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }

        public class Drop_Payment
        {
            public long Payment_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
        }


    }
}
