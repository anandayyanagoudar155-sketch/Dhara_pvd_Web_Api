using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using static dhara_pvd_decor_webapi_proj.Controllers.ColourController;
using static dhara_pvd_decor_webapi_proj.Controllers.PayTypeController;


namespace dhara_pvd_decor_webapi_proj.Services
{

    public class PayTypeService : IPayTypeService
    {
        private readonly IConfiguration _configuration;

        public PayTypeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<int> AddPaytype(AddPaytypeRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

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
                    command.Parameters.AddWithValue("@created_by", request.Created_by);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                    return await command.ExecuteNonQueryAsync();


                }
            }

        }


        public async Task<int> DeletePaytype(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_paytype_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@paytype_id", id);

                    return await command.ExecuteNonQueryAsync();

                }
            }
        }


        public async Task<int> UpdatePaytype(UpdatePaytypeRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

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
                parameters.Add("@created_by", request.Created_by);
                parameters.Add("@modified_by", request.Modified_by);

                return await connection.ExecuteAsync
                (
                    spname,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

            }
        }


        public async Task<List<Paytype_list>> GetPaytypeList()
        {
            var list = new List<Paytype_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

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
                            list.Add(new Paytype_list
                            {
                                Paytype_Id = reader.GetInt64(0),
                                Paytype_Name = reader.GetString(1),
                                Paytype_Desc = reader.IsDBNull(2) ? "" : reader.GetString(2),
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



        public async Task<Single_Paytype_list?> GetPaytypeById(long id)
        {
            Single_Paytype_list? paytype = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

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
                                Created_by = reader.IsDBNull(5) ? 0 : reader.GetInt64(5),
                                Modified_by = reader.IsDBNull(6) ? 0 : reader.GetInt64(6)
                            };
                        }
                    }
                }
            }

            return paytype;

        }



        public async Task<List<drop_Paytype_list>> GetDropdownPaytypeList()
        {
            var list = new List<drop_Paytype_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

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
                            list.Add(new drop_Paytype_list
                            {
                                Paytype_Id = reader.GetInt64(0),
                                Paytype_Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return list;

        }




    }

}