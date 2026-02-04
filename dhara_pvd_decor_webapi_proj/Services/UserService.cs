using dhara_pvd_decor_webapi_proj.Controllers;
using dhara_pvd_decor_webapi_proj.Services;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using static dhara_pvd_decor_webapi_proj.Controllers.UserController;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public class UserService : IUserServices
    {

        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> AddUser(UserController.AddUserRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

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

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteUser(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_user_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@User_id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> UpdateUser(UserController.UpdateUserRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

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

                return await connection.ExecuteAsync(
                    spname,
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
        }



        public async Task<List<UserController.User_List>> GetUserList()
        {
            var list = new List<User_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

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
                            list.Add(new User_List
                            {
                                User_id = reader.GetInt64(0),
                                User_name = reader.GetString(1),
                               User_password = reader.GetString(2),
                                User_role = reader.GetString(3),
                                Is_login = reader.GetBoolean(4),
                                Created_Date = reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                Updated_Date = reader.IsDBNull(6) ? "" : reader.GetDateTime(6).ToString("yyyy-MM-dd"),
                            });
                        }
                    }
                }
            }

            return list;
        }

        public async Task<UserController.SingleUser?> GetUserById(long id)
        {
            SingleUser? user = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

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

                return user;
            }


        public async Task<List<UserController.Drop_User_List>> GetDropdownUserList()
        {
            var list = new List<Drop_User_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

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
                                list.Add( new Drop_User_List
                                {
                                    User_id = reader.GetInt64(0),
                                    User_name = reader.GetString(1)
                                });

                            }
                        }
                    }
                }

                return list;
            }



        public async Task<int> AddUserDetails(UserController.AddUserDetailsRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_user_details_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@action", "insert");
                    command.Parameters.AddWithValue("@user_details_id", request.User_details_id);
                    command.Parameters.AddWithValue("@user_id", request.User_id);
                    command.Parameters.AddWithValue("@comp_id", request.Comp_id ?? "");
                    command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id ?? "");
                    command.Parameters.AddWithValue("@is_active", request.Is_active);
                    command.Parameters.AddWithValue("@created_date", request.Created_date);
                    command.Parameters.AddWithValue("@updated_date", request.Updated_date);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> AddMultipleUserDetails(UserController.AddUserDetailsRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_user_details_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@action", "multipleinsert");
                    command.Parameters.AddWithValue("@user_details_id", request.User_details_id);
                    command.Parameters.AddWithValue("@user_id", request.User_id);
                    command.Parameters.AddWithValue("@comp_id", request.Comp_id ?? "");
                    command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id ?? "");
                    command.Parameters.AddWithValue("@is_active", request.Is_active);
                    command.Parameters.AddWithValue("@created_date", request.Created_date);
                    command.Parameters.AddWithValue("@updated_date", request.Updated_date);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<int> DeleteUserDetails(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_user_details_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@user_id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> UpdateUserDetails(UserController.UpdateUserDetailsRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

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

                return await connection.ExecuteAsync(
                     spname,
                     parameters,
                     commandType: CommandType.StoredProcedure
                );
            }
        }


        public async Task<List<UserController.UserDetails_List>> GetUserDetailsList()
        {
            var ud_list = new List<UserDetails_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

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
                            ud_list.Add(new UserDetails_List
                            {
                                User_details_id = reader.GetInt64(0),
                                User_name = reader.GetString(1),
                                Comp_name = reader.GetString(2),
                                Fin_year_name = reader.GetString(3),
                                Is_active = reader.GetBoolean(4),
                                Created_date = reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                Updated_date = reader.IsDBNull(6) ? "" : reader.GetDateTime(6).ToString("yyyy-MM-dd"),
                                Modified_by = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            });

                        }
                    }
                }
            }

            return ud_list;
        }

        public async Task<List<UserController.Multiple_UserDetails_List>> GetMultipleUserDetailsByUserId(long userId)
        {
            var results = new List<Multiple_UserDetails_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_user_details_ins_upd_del";
                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectupdateall");
                    command.Parameters.AddWithValue("@user_id", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new Multiple_UserDetails_List
                            {
                                User_details_id = reader.IsDBNull(0) ? 0 : reader.GetInt64(0),
                                User_id = reader.IsDBNull(1) ? 0 : reader.GetInt64(1),
                                Comp_id = reader.IsDBNull(2) ? "" : reader.GetInt64(2).ToString(),
                                Comp_name = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Fin_year_id = reader.IsDBNull(4) ? "" : reader.GetInt64(4).ToString(),
                                Fin_year_name = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                Is_active = reader.IsDBNull(6) ? true : reader.GetBoolean(6),
                                Created_date = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7),
                                Updated_date = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                                Modified_by = reader.IsDBNull(9) ? 0 : reader.GetInt64(9),
                            });
                        }
                    }
                }
            }

            return results;
        }



    }

}

