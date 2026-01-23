using Dapper;
using System.Data;
using System.Data.SqlClient;
using dhara_pvd_decor_webapi_proj.Controllers;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public class StateService : IStateService
    {
        private readonly IConfiguration _configuration;

        public StateService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string ConnStr =>
            _configuration.GetConnectionString("DefaultConnection");

        public async Task<bool> AddState(StateController.AddStateRequest request)
        {
            using var con = new SqlConnection(ConnStr);
            using var cmd = new SqlCommand("sp_state_mast_ins_upd_del", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "insert");
            cmd.Parameters.AddWithValue("@state_id", request.State_id);
            cmd.Parameters.AddWithValue("@state_name", request.State_name);
            cmd.Parameters.AddWithValue("@country_id", request.Country_id);
            cmd.Parameters.AddWithValue("@created_date", request.Created_date);
            cmd.Parameters.AddWithValue("@created_by", request.Created_by);
            cmd.Parameters.AddWithValue("@modified_by", request.Modified_by);

            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteState(long id)
        {
            using var con = new SqlConnection(ConnStr);
            using var cmd = new SqlCommand("sp_state_mast_ins_upd_del", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "delete");
            cmd.Parameters.AddWithValue("@state_id", id);

            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateState(StateController.UpdateStateRequest request)
        {
            using var con = new SqlConnection(ConnStr);

            var param = new DynamicParameters();
            param.Add("@action", "update");
            param.Add("@state_id", request.State_id);
            param.Add("@state_name", request.State_name);
            param.Add("@country_id", request.Country_id);
            param.Add("@created_date", request.Created_date);
            param.Add("@updated_date", request.Updated_date);
            param.Add("@created_by", request.Created_by);
            param.Add("@modified_by", request.Modified_by);

            var rows = await con.ExecuteAsync(
                "sp_state_mast_ins_upd_del",
                param,
                commandType: CommandType.StoredProcedure);

            return rows > 0;
        }

        public async Task<List<StateController.state_list>> GetStateList()
        {
            var list = new List<StateController.state_list>();

            using var con = new SqlConnection(ConnStr);
            using var cmd = new SqlCommand("sp_state_mast_ins_upd_del", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "selectall");

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync())
            {
                list.Add(new StateController.state_list
                {
                    State_id = rd.GetInt64(0),
                    State_name = rd.GetString(1),
                    Country_name = rd.GetString(2),
                    Created_date = rd.GetDateTime(3).ToString("yyyy-MM-dd"),
                    Updated_date = rd.IsDBNull(4) ? "" : rd.GetDateTime(4).ToString("yyyy-MM-dd"),
                    Created_by = rd.GetInt64(5),
                    Created_by_name = rd.GetString(6),
                    Modified_by = rd.IsDBNull(7) ? 0 : rd.GetInt64(7),
                    Modified_by_name = rd.IsDBNull(8) ? "" : rd.GetString(8)
                });
            }

            return list;
        }

        public async Task<StateController.Single_state_list?> GetStateById(long id)
        {
            using var con = new SqlConnection(ConnStr);
            using var cmd = new SqlCommand("sp_state_mast_ins_upd_del", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "selectone");
            cmd.Parameters.AddWithValue("@state_id", id);

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            if (await rd.ReadAsync())
            {
                return new StateController.Single_state_list
                {
                    State_id = rd.GetInt64(0),
                    State_name = rd.GetString(1),
                    Country_id = rd.GetInt64(2),
                    Created_date = rd.GetDateTime(3),
                    Updated_date = rd.IsDBNull(4) ? null : rd.GetDateTime(4),
                    Created_by = rd.IsDBNull(5) ? 0 : rd.GetInt64(5),
                    Modified_by = rd.IsDBNull(6) ? 0 : rd.GetInt64(6)
                };
            }

            return null;
        }

        public async Task<List<StateController.drop_state_list>> GetDropStateList(long country_id)
        {
            var list = new List<StateController.drop_state_list>();

            using var con = new SqlConnection(ConnStr);
            using var cmd = new SqlCommand("sp_state_mast_ins_upd_del", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "state_mastlist");
            cmd.Parameters.AddWithValue("@country_id", country_id);

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync())
            {
                list.Add(new StateController.drop_state_list
                {
                    State_id = rd.GetInt64(0),
                    State_name = rd.GetString(1)
                });
            }

            return list;
        }
    }
}
