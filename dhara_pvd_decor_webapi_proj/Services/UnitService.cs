using Dapper;
using System.Data;
using System.Data.SqlClient;
using static dhara_pvd_decor_webapi_proj.Controllers.UnitController;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public class UnitService : IUnitService
    {
        private readonly IConfiguration _configuration;

        public UnitService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> AddUnit(AddUnitRequest request)
        {
            var cs = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(cs);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_unit_mast_ins_upd_del", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@action", "insert");
            command.Parameters.AddWithValue("@unit_id", 0);
            command.Parameters.AddWithValue("@unit_name", request.UnitName);
            command.Parameters.AddWithValue("@unit_desc", request.UnitDesc);
            command.Parameters.AddWithValue("@is_active", request.IsActive);
            command.Parameters.AddWithValue("@created_date", request.Created_date);
            command.Parameters.AddWithValue("@created_by", request.Created_by);
            command.Parameters.AddWithValue("@modified_by", request.Modified_by);

            return await command.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteUnit(long id)
        {
            var cs = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(cs);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_unit_mast_ins_upd_del", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@action", "delete");
            command.Parameters.AddWithValue("@unit_id", id);

            return await command.ExecuteNonQueryAsync();
        }

        public async Task<int> UpdateUnit(UpdateUnitRequest request)
        {
            var cs = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(cs);

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

            return await connection.ExecuteAsync(
                "sp_unit_mast_ins_upd_del",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<List<Unit_list>> GetUnitList()
        {
            var list = new List<Unit_list>();
            var cs = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(cs);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_unit_mast_ins_upd_del", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@action", "selectall");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Unit_list
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
                });
            }

            return list;
        }

        public async Task<Single_Unit_list?> GetUnitById(long id)
        {
            Single_Unit_list? unit = null;
            var cs = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(cs);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_unit_mast_ins_upd_del", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@action", "selectone");
            command.Parameters.AddWithValue("@unit_id", id);

            using var reader = await command.ExecuteReaderAsync();
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

            return unit;
        }

        public async Task<List<drop_Unit_list>> GetDropUnitList()
        {
            var list = new List<drop_Unit_list>();
            var cs = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(cs);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_unit_mast_ins_upd_del", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@action", "unitlist");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new drop_Unit_list
                {
                    UnitId = reader.GetInt64(0),
                    UnitName = reader.GetString(1)
                });
            }

            return list;
        }
    }
}

