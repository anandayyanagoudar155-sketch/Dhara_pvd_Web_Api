using Dapper;
using System.Data;
using System.Data.SqlClient;
using static dhara_pvd_decor_webapi_proj.Controllers.MonthController;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public class MonthService : IMonthService
    {
        private readonly IConfiguration _configuration;

        public MonthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> AddMonth(AddMonthRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_month_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "insert");
                    command.Parameters.AddWithValue("@month_id", 0);
                    command.Parameters.AddWithValue("@month_name", request.Month_name);
                    command.Parameters.AddWithValue("@start_date", request.Start_date);
                    command.Parameters.AddWithValue("@end_date", request.End_date);
                    command.Parameters.AddWithValue("@created_date", request.Created_date);
                    command.Parameters.AddWithValue("@created_by", request.Created_by);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteMonth(long id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_month_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@month_id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> UpdateMonth(UpdateMonthRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@action", "update");
                parameters.Add("@month_id", request.Month_id);
                parameters.Add("@month_name", request.Month_name);
                parameters.Add("@start_date", request.Start_date);
                parameters.Add("@end_date", request.End_date);
                parameters.Add("@created_date", request.Created_date);
                parameters.Add("@updated_date", request.Updated_date);
                parameters.Add("@created_by", request.Created_by);
                parameters.Add("@modified_by", request.Modified_by);

                return await connection.ExecuteAsync(
                    "sp_month_mast_ins_upd_del",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public async Task<List<month_list>> GetMonthList()
        {
            var list = new List<month_list>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_month_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectall");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new month_list
                            {
                                Month_id = reader.GetInt64(0),
                                Month_name = reader.GetString(1),
                                Start_date = reader.GetDateTime(2).ToString("yyyy-MM-dd"),
                                End_date = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                Created_date = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                Updated_date = reader.IsDBNull(5) ? "" : reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                Created_by = reader.GetInt64(6),
                                Modified_by = reader.IsDBNull(7) ? 0 : reader.GetInt64(7),
                                Created_by_name = reader.GetString(8),
                                Modified_by_name = reader.IsDBNull(9) ? "" : reader.GetString(9)
                            });
                        }
                    }
                }
            }

            return list;
        }

        public async Task<Single_month_list?> GetMonthById(long id)
        {
            Single_month_list? month = null;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_month_mast_ins_upd_del", connection))
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
                                Created_by = reader.IsDBNull(6) ? 0 : reader.GetInt64(6),
                                Modified_by = reader.IsDBNull(7) ? 0 : reader.GetInt64(7)
                            };
                        }
                    }
                }
            }

            return month;
        }

        public async Task<List<drop_month_list>> GetDropMonthList()
        {
            var list = new List<drop_month_list>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_month_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "month_mastlist");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new drop_month_list
                            {
                                Month_id = reader.GetInt64(0),
                                Month_name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return list;
        }
    }
}
