using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {

            _configuration = configuration;

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



        [HttpPost("insert_userdetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddUserDetails([FromBody] AddUserDetailsRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_user_details_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@user_details_id", request.User_details_id);
                        command.Parameters.AddWithValue("@user_id", request.User_id);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                        command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                        command.Parameters.AddWithValue("@is_active", request.Is_active);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@updated_date", request.Updated_date);
                        command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "User Details added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add User details." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_userdetails/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserDetails(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_user_details_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@user_details_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "User Details deleted successfully." });
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




        [HttpPost("update_userdetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUserDetails([FromBody] UpdateUserDetailsRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_user_details_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@user_details_id", request.User_details_id);
                    parameters.Add("@user_id", request.User_id);
                    parameters.Add("@comp_id", request.Comp_id);
                    parameters.Add("@fin_year_id", request.Fin_year_id);
                    parameters.Add("@is_active", request.Is_active);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@modified_by", request.Modified_by);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"User Details with ID {request.User_details_id} not found");
                else
                    return Ok(new { message = "User Details updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("userdetails_list")]
        public async Task<ActionResult<IEnumerable<UserDetails_List>>> Get_userdetails_list()
        {
            var ud_list = new List<UserDetails_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_user_details_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var ud = new UserDetails_List
                                {
                                    User_details_id = reader.GetInt64(0),
                                    User_name = reader.GetString(1),
                                    Comp_name = reader.GetString(2),
                                    Fin_year_name = reader.GetString(3),
                                    Is_active = reader.GetBoolean(4),
                                    Created_date = reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(6) ? "" : reader.GetDateTime(6).ToString("yyyy-MM-dd"),
                                    Modified_by = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                };

                                ud_list.Add(ud);
                            }
                        }
                    }
                }

                return Ok(ud_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("userdetails/{id}")]
        public async Task<ActionResult<Single_UserDetails_List>> Get_userdetails_by_id(long id)
        {
            Single_UserDetails_List? ud = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_user_details_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@user_details_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                ud = new Single_UserDetails_List
                                {
                                    User_details_id = reader.GetInt64(0),
                                    User_id = reader.GetInt64(1),
                                    Comp_id = reader.GetInt64(2),
                                    Fin_year_id = reader.GetInt64(3),
                                    Is_active = reader.GetBoolean(4),
                                    Created_date = reader.GetDateTime(5),
                                    Updated_date = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                                    Modified_by = reader.IsDBNull(7) ? 0 : reader.GetInt64(7),
                                };
                            }
                        }
                    }
                }

                if (ud == null)
                    return NotFound($"User Details with ID {id} not found");

                return Ok(ud);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
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

        public class AddUserDetailsRequest
        {
            public long User_details_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public bool Is_active { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Modified_by { get; set; } = 0;
        }



        public class UpdateUserDetailsRequest
        {
            public long User_details_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public bool Is_active { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Modified_by { get; set; } = 0;
        }


        public class UserDetails_List
        {
            public long User_details_id { get; set; } = 0;
            public string User_name { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Fin_year_name { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string Modified_by { get; set; } = "";
        }



        public class Single_UserDetails_List
        {
            public long User_details_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public bool Is_active { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Modified_by { get; set; } = 0;
        }



    }
}
