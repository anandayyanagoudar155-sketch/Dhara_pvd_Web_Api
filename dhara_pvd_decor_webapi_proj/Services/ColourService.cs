using Dapper;
using System.Data;
using System.Data.SqlClient;
using static dhara_pvd_decor_webapi_proj.Controllers.ColourController;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public class ColourService : IColourService
    {
        private readonly IConfiguration _configuration;

        public ColourService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> AddColour(AddColourRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_colour_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "insert");
                    command.Parameters.AddWithValue("@colour_id", 0);
                    command.Parameters.AddWithValue("@colour_name", request.ColourName);
                    command.Parameters.AddWithValue("@is_active", request.IsActive);
                    command.Parameters.AddWithValue("@created_date", request.Created_date);
                    command.Parameters.AddWithValue("@created_by", request.Created_by);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteColour(long id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_colour_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@colour_id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> UpdateColour(UpdateColourRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@action", "update");
                parameters.Add("@colour_id", request.ColourId);
                parameters.Add("@colour_name", request.ColourName);
                parameters.Add("@is_active", request.IsActive);
                parameters.Add("@created_date", request.Created_date);
                parameters.Add("@updated_date", request.Updated_date);
                parameters.Add("@created_by", request.Created_by);
                parameters.Add("@modified_by", request.Modified_by);

                return await connection.ExecuteAsync(
                    "sp_colour_mast_ins_upd_del",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public async Task<List<Colour_list>> GetColourList()
        {
            var list = new List<Colour_list>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_colour_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectall");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Colour_list
                            {
                                ColourId = reader.GetInt64(0),
                                ColourName = reader.GetString(1),
                                IsActive = reader.GetBoolean(2),
                                Created_date = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                Updated_date = reader.IsDBNull(4) ? "" : reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                Created_by = reader.GetInt64(5),
                                Modified_by = reader.IsDBNull(6) ? 0 : reader.GetInt64(6),
                                Created_by_name = reader.GetString(7),
                                Modified_by_name = reader.IsDBNull(8) ? "" : reader.GetString(8)
                            });
                        }
                    }
                }
            }

            return list;
        }

        public async Task<Single_Colour_list?> GetColourById(long id)
        {
            Single_Colour_list? colour = null;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_colour_mast_ins_upd_del", connection))
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
                                Created_date = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                                Updated_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                Created_by = reader.IsDBNull(5) ? 0 : reader.GetInt64(5),
                                Modified_by = reader.IsDBNull(6) ? 0 : reader.GetInt64(6)
                            };
                        }
                    }
                }
            }

            return colour;
        }

        public async Task<List<drop_Colour_list>> GetDropColourList()
        {
            var list = new List<drop_Colour_list>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_colour_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "colourlist");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new drop_Colour_list
                            {
                                ColourId = reader.GetInt64(0),
                                ColourName = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return list;
        }
    }
}
