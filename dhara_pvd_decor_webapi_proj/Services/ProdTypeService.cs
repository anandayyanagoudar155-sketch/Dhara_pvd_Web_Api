using Dapper;
using dhara_pvd_decor_webapi_proj.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using static dhara_pvd_decor_webapi_proj.Controllers.ProdTypeController;
using static dhara_pvd_decor_webapi_proj.Controllers.TranTypeController;


namespace dhara_pvd_decor_webapi_proj.Services
{

    public class ProdTypeService : IProdTypeService
    {
        private readonly IConfiguration _configuration;

        public ProdTypeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> AddProdtype(ProdTypeController.AddProdtypeRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_prodtype_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "insert");
                    command.Parameters.AddWithValue("@prodtype_id", request.Prodtype_Id);
                    command.Parameters.AddWithValue("@prodtype_name", request.Prodtype_Name);
                    command.Parameters.AddWithValue("@prodtype_desc", request.Prodtype_Desc);
                    command.Parameters.AddWithValue("@created_date", request.Created_date);
                    command.Parameters.AddWithValue("@created_by", request.Created_by);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                    return await command.ExecuteNonQueryAsync();
                }

            }

        }

        public async Task<int> DeleteProdtype(long id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_prodtype_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@prodtype_id", id);

                    return await command.ExecuteNonQueryAsync();
                }

            }

        }

        public async Task<int> UpdateProdtype(ProdTypeController.UpdateProdtypeRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_prodtype_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@prodtype_id", request.Prodtype_Id);
                    parameters.Add("@prodtype_name", request.Prodtype_Name);
                    parameters.Add("@prodtype_desc", request.Prodtype_Desc);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@created_by", request.Created_by);
                    parameters.Add("@modified_by", request.Modified_by); ;

                    return await connection.ExecuteAsync(
                        spName,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }
        }


        public async Task<List<ProdTypeController.Prodtype_list>> GetProdtypeList()
        {
            var list = new List<Prodtype_list>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_prodtype_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add( new Prodtype_list
                                {
                                    Prodtype_Id = reader.GetInt64(0),
                                    Prodtype_Name = reader.GetString(1),
                                    Prodtype_Desc = reader.IsDBNull(2) ? "" : reader.GetString(2),
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


        public async Task<ProdTypeController.Single_Prodtype_list?> GetProdtypeById(long id)
        {
            Single_Prodtype_list? item = null;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                string spName = "sp_prodtype_mast_ins_upd_del";
                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectone");
                    command.Parameters.AddWithValue("@prodtype_id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new Single_Prodtype_list
                            {
                                Prodtype_Id = reader.GetInt64(0),
                                Prodtype_Name = reader.GetString(1),
                                Prodtype_Desc = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Created_date = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                                Updated_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                Created_by = reader.IsDBNull(5) ? 0 : reader.GetInt64(5),
                                Modified_by = reader.IsDBNull(6) ? 0 : reader.GetInt64(6)
                            };
                        }
                    }
                }
            }

            return item;
        }


        public async Task<List<ProdTypeController.drop_Prodtype_list>> GetDropProdtypeList()
        {
            var list = new List<drop_Prodtype_list>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                string spName = "sp_prodtype_mast_ins_upd_del";
                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "prodtypelist");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new drop_Prodtype_list
                            {
                                Prodtype_Id = reader.GetInt64(0),
                                Prodtype_Name = reader.GetString(1)
                            });

                        }
                    }
                }
            }

            return list;
        }
    }

}