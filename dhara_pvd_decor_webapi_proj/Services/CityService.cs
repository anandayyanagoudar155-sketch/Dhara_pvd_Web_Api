using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using dhara_pvd_decor_webapi_proj.Services.Interfaces;
using dhara_pvd_decor_webapi_proj.Controllers;

namespace dhara_pvd_decor_webapi_proj.Services.Implementations
{
    public class CityService : ICityService
    {
        private readonly IConfiguration _configuration;

        public CityService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<bool> AddCity(CityController.AddCityRequest request)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_city_mast_ins_upd_del", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@action", "insert");
            command.Parameters.AddWithValue("@city_id", 0);
            command.Parameters.AddWithValue("@city_name", request.City_name);
            command.Parameters.AddWithValue("@state_id", request.State_id);
            command.Parameters.AddWithValue("@created_date", request.Created_date);
            command.Parameters.AddWithValue("@updated_date", request.Updated_date);
            command.Parameters.AddWithValue("@created_by", request.Created_by);
            command.Parameters.AddWithValue("@modified_by", request.Modified_by);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteCity(long id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_city_mast_ins_upd_del", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@action", "delete");
            command.Parameters.AddWithValue("@city_id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateCity(CityController.UpdatecityRequest request)
        {
            using var connection = GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@action", "update");
            parameters.Add("@city_id", request.City_id);
            parameters.Add("@city_name", request.City_name);
            parameters.Add("@state_id", request.State_id);
            parameters.Add("@created_date", request.Created_date);
            parameters.Add("@updated_date", request.Updated_date);
            parameters.Add("@created_by", request.Created_by);
            parameters.Add("@modified_by", request.Modified_by);

            var rows = await connection.ExecuteAsync(
                "sp_city_mast_ins_upd_del",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rows > 0;
        }

        public async Task<IEnumerable<CityController.city_list>> GetCityList()
        {
            var list = new List<CityController.city_list>();

            using var connection = GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_city_mast_ins_upd_del", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@action", "selectall");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new CityController.city_list
                {
                    City_id = reader.GetInt64(0),
                    City_name = reader.GetString(1),
                    State_name = reader.GetString(2),
                    Created_date = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                    Updated_date = reader.IsDBNull(4) ? "" : reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                    Created_by = reader.GetInt64(5),
                    Created_by_name = reader.GetString(6),
                    Modified_by = reader.IsDBNull(7) ? 0 : reader.GetInt64(7),
                    Modified_by_name = reader.IsDBNull(8) ? "" : reader.GetString(8)
                });
            }

            return list;
        }

        public async Task<CityController.Single_city_list?> GetCityById(long id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_city_mast_ins_upd_del", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@action", "selectone");
            command.Parameters.AddWithValue("@state_id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new CityController.Single_city_list
                {
                    City_id = reader.GetInt64(0),
                    City_name = reader.GetString(1),
                    State_id = reader.GetInt64(2),
                    Created_date = reader.GetDateTime(3),
                    Updated_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                    Created_by = reader.IsDBNull(5) ? 0 : reader.GetInt64(5),
                    Modified_by = reader.IsDBNull(6) ? 0 : reader.GetInt64(6)
                };
            }

            return null;
        }

        public async Task<IEnumerable<CityController.drop_city_list>> GetDropdownCityList(long stateId)
        {
            var list = new List<CityController.drop_city_list>();

            using var connection = GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_city_mast_ins_upd_del", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@action", "city_mastlist");
            command.Parameters.AddWithValue("@state_id", stateId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new CityController.drop_city_list
                {
                    City_id = reader.GetInt64(0),
                    City_name = reader.GetString(1)
                });
            }

            return list;
        }
    }
}
