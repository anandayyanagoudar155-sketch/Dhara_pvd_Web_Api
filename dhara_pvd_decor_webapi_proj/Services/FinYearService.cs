using Dapper;
using System.Data;
using System.Data.SqlClient;
using static dhara_pvd_decor_webapi_proj.Controllers.FinYearController;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public class FinYearService : IFinYearService
    {
        private readonly IConfiguration _configuration;

        public FinYearService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> AddFinYear(AddFinYearRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_fin_year_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "insert");
                    command.Parameters.AddWithValue("@fin_year_id", 0);
                    command.Parameters.AddWithValue("@fin_name", request.Fin_name);
                    command.Parameters.AddWithValue("@short_fin_year", request.Short_fin_year);
                    command.Parameters.AddWithValue("@year_start", request.Year_start);
                    command.Parameters.AddWithValue("@year_end", request.Year_end);
                    command.Parameters.AddWithValue("@created_date", request.Created_date);
                    command.Parameters.AddWithValue("@created_by", request.Created_by);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteFinYear(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_fin_year_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@fin_year_id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> UpdateFinYear(UpdateFinYearRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@action", "update");
                parameters.Add("@fin_year_id", request.Fin_year_id);
                parameters.Add("@fin_name", request.Fin_name);
                parameters.Add("@short_fin_year", request.Short_fin_year);
                parameters.Add("@year_start", request.Year_start);
                parameters.Add("@year_end", request.Year_end);
                parameters.Add("@created_date", request.Created_date);
                parameters.Add("@updated_date", request.Updated_date);
                parameters.Add("@created_by", request.Created_by);
                parameters.Add("@modified_by", request.Modified_by);

                return await connection.ExecuteAsync(
                    "sp_fin_year_mast_ins_upd_del",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public async Task<List<FinYearlist>> GetFinYearList()
        {
            var list = new List<FinYearlist>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

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
                            list.Add(new FinYearlist
                            {
                                Fin_year_id = reader.GetInt64(0),
                                Fin_name = reader.GetString(1),
                                Short_fin_year = reader.GetString(2),
                                Year_start = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                Year_end = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                Created_date = reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                Updated_date = reader.IsDBNull(6) ? "" : reader.GetDateTime(6).ToString("yyyy-MM-dd"),
                                Created_by = reader.GetInt64(7),
                                Modified_by = reader.IsDBNull(8) ? 0 : reader.GetInt64(8),
                                Created_by_name = reader.GetString(9),
                                Modified_by_name = reader.IsDBNull(10) ? "" : reader.GetString(10)
                            });
                        }
                    }
                }
            }

            return list;
        }

        public async Task<Single_FinYear_list?> GetFinYearById(long id)
        {
            Single_FinYear_list? finyear = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_fin_year_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectone");
                    command.Parameters.AddWithValue("@fin_year_id", id);

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
                                Created_by = reader.IsDBNull(7) ? 0 : reader.GetInt64(7),
                                Modified_by = reader.IsDBNull(8) ? 0 : reader.GetInt64(8)
                            };
                        }
                    }
                }
            }

            return finyear;
        }

        public async Task<List<drop_FinYear_list>> GetDropFinYearList(long userId)
        {
            var list = new List<drop_FinYear_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_fin_year_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "fin_year_mastlist");
                    command.Parameters.AddWithValue("@user_id", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new drop_FinYear_list
                            {
                                Fin_year_id = reader.GetInt64(0),
                                Fin_name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return list;
        }
    }
}
