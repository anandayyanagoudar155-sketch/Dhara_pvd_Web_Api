//namespace dhara_pvd_decor_webapi_proj.Services
//{
//    public class CountryService
//    {
//    }
//}


using Dapper;
using System.Data;
using System.Data.SqlClient;
using dhara_pvd_decor_webapi_proj.Controllers;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public class CountryService : ICountryService
    {
        private readonly IConfiguration _configuration;

        public CountryService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string ConnStr =>
            _configuration.GetConnectionString("DefaultConnection");

        public async Task<bool> AddCountry(CountryController.AddCountryRequest request)
        {
            using var con = new SqlConnection(ConnStr);
            using var cmd = new SqlCommand("sp_country_mast_ins_upd_del", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "insert");
            cmd.Parameters.AddWithValue("@country_id", 0);
            cmd.Parameters.AddWithValue("@country_name", request.Country_name);
            cmd.Parameters.AddWithValue("@created_date", request.Created_date.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@created_by", request.Created_by);
            cmd.Parameters.AddWithValue("@modified_by", request.Modified_by);

            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteCountry(long id)
        {
            using var con = new SqlConnection(ConnStr);
            using var cmd = new SqlCommand("sp_country_mast_ins_upd_del", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "delete");
            cmd.Parameters.AddWithValue("@country_id", id);

            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateCountry(CountryController.UpdateCountryRequest request)
        {
            using var con = new SqlConnection(ConnStr);

            var param = new DynamicParameters();
            param.Add("@action", "update");
            param.Add("@country_id", request.Country_id);
            param.Add("@country_name", request.Country_name);
            param.Add("@created_date", request.Created_date);
            param.Add("@updated_date", request.Updated_date);
            param.Add("@created_by", request.Created_by);
            param.Add("@modified_by", request.Modified_by);

            var rows = await con.ExecuteAsync(
                "sp_country_mast_ins_upd_del",
                param,
                commandType: CommandType.StoredProcedure);

            return rows > 0;
        }

        public async Task<List<CountryController.country_list>> GetCountryList()
        {
            var list = new List<CountryController.country_list>();

            using var con = new SqlConnection(ConnStr);
            using var cmd = new SqlCommand("sp_country_mast_ins_upd_del", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "selectall");

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync())
            {
                list.Add(new CountryController.country_list
                {
                    Country_id = rd.GetInt64(0),
                    Country_name = rd.GetString(1),
                    Created_date = rd.GetDateTime(2).ToString("yyyy-MM-dd"),
                    Updated_date = rd.IsDBNull(3) ? "" : rd.GetDateTime(3).ToString("yyyy-MM-dd"),
                    Created_by = rd.GetInt64(4),
                    Created_by_name = rd.GetString(5),
                    Modified_by = rd.IsDBNull(6) ? 0 : rd.GetInt64(6),
                    Modified_by_name = rd.IsDBNull(7) ? "" : rd.GetString(7)
                });
            }

            return list;
        }

        public async Task<CountryController.Single_country_list?> GetCountryById(long id)
        {
            using var con = new SqlConnection(ConnStr);
            using var cmd = new SqlCommand("sp_country_mast_ins_upd_del", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "selectone");
            cmd.Parameters.AddWithValue("@country_id", id);

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            if (await rd.ReadAsync())
            {
                return new CountryController.Single_country_list
                {
                    Country_id = rd.GetInt64(0),
                    Country_name = rd.GetString(1),
                    Created_date = rd.GetDateTime(2),
                    Updated_date = rd.IsDBNull(3) ? null : rd.GetDateTime(3),
                    Created_by = rd.IsDBNull(4) ? 0 : rd.GetInt64(4),
                    Modified_by = rd.IsDBNull(5) ? 0 : rd.GetInt64(5)
                };
            }

            return null;
        }

        public async Task<List<CountryController.drop_country_list>> GetDropCountryList()
        {
            var list = new List<CountryController.drop_country_list>();

            using var con = new SqlConnection(ConnStr);
            using var cmd = new SqlCommand("sp_country_mast_ins_upd_del", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "countrylist");

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync())
            {
                list.Add(new CountryController.drop_country_list
                {
                    Country_id = rd.GetInt64(0),
                    Country_name = rd.GetString(1)
                });
            }

            return list;
        }
    }
}
