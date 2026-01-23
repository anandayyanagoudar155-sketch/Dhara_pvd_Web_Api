using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using static dhara_pvd_decor_webapi_proj.Controllers.TranTypeController;


namespace dhara_pvd_decor_webapi_proj.Services
{

    public class TranTypeService : ITranTypeService
    {
        private readonly IConfiguration _configuration;

        public TranTypeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> AddTransType(AddTrans_typeRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_trans_type_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "insert");
                    command.Parameters.AddWithValue("@trans_id", 0);
                    command.Parameters.AddWithValue("@transtype_name", request.Transtype_name);
                    command.Parameters.AddWithValue("@transtype_desc", request.Transtype_desc);
                    command.Parameters.AddWithValue("@created_date", request.Created_Date);
                    command.Parameters.AddWithValue("@created_by", request.Created_by);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                    return await command.ExecuteNonQueryAsync();
                }

            }

        }

        public async Task<int> DeleteTransType(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_trans_type_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@trans_id", id);

                        return await command.ExecuteNonQueryAsync();
                    }

                }

        }

        public async Task<int> UpdateTransType( UpdateTrans_typeRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

                using (var connection = new SqlConnection(connectionstring))
                {
                    string spname = "sp_trans_type_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@trans_id", request.Trans_id);
                    parameters.Add("@transtype_name", request.Transtype_name);
                    parameters.Add("@transtype_desc", request.Transtype_desc);
                    parameters.Add("@created_date", request.Created_Date);
                    parameters.Add("@updated_date", request.Updated_Date);
                    parameters.Add("@created_by", request.Created_by);
                    parameters.Add("@modified_by", request.Modified_by);


                    return await connection.ExecuteAsync(
                            spname,
                            parameters,
                            commandType: System.Data.CommandType.StoredProcedure
                    );
                }

        }


        public async Task<List<trans_type_List>> Get_trans_type_list()
        {
            var list = new List<trans_type_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_trans_type_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add(new trans_type_List
                                {
                                    Trans_id = reader.GetInt64(0),
                                    Transtype_name = reader.GetString(1),
                                    Transtype_desc = reader.GetString(2),
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

        public async Task<Singletrans_type?> Get_trans_type_by_id(long id)
        {
            Singletrans_type? item = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_trans_type_mast_ins_upd_del";

                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectone");
                    command.Parameters.AddWithValue("@trans_id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new Singletrans_type
                            {
                                Trans_id = reader.GetInt64(0),
                                Transtype_name = reader.GetString(1),
                                Transtype_desc = reader.GetString(2),
                                Created_Date = reader.GetDateTime(3),
                                Updated_Date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                Created_by = reader.IsDBNull(5) ? 0 : reader.GetInt64(5),
                                Modified_by = reader.IsDBNull(6) ? 0 : reader.GetInt64(6)
                            };
                        }
                    }
                }
            }


            return item;

        }

        public async Task<List<Drop_trans_type_List>> Get_drop_trans_type_list()
        {
            var list = new List<Drop_trans_type_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_trans_type_mast_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "trans_type_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add(new Drop_trans_type_List
                                {
                                    Trans_id = reader.GetInt64(0),
                                    Transtype_name = reader.GetString(1)
                                });

                            }
                        }
                    }
                }

                return list;

        }

    }

}
