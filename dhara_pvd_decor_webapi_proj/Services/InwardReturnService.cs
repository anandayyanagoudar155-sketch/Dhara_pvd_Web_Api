using Dapper;
using dhara_pvd_decor_webapi_proj.Controllers;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Data;
using System.Data.SqlClient;
using static dhara_pvd_decor_webapi_proj.Controllers.InwardController;
using static dhara_pvd_decor_webapi_proj.Controllers.InwardReturnController;



namespace dhara_pvd_decor_webapi_proj.Services
{
    public class InwardReturnService:IInwardReturnService
    {
        private readonly IConfiguration _configuration;

        public InwardReturnService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> Add_inwardreturn(AddInwardreturnRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_inward_return_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "insert");
                    command.Parameters.AddWithValue("@inwardreturn_id", 0);
                    command.Parameters.AddWithValue("@inward_id", request.Inward_Id);
                    command.Parameters.AddWithValue("@customer_id", request.Customer_Id);
                    command.Parameters.AddWithValue("@product_id", request.Product_Id);
                    command.Parameters.AddWithValue("@returnquantity", request.ReturnQuantity);
                    command.Parameters.AddWithValue("@return_date", request.Return_Date);
                    command.Parameters.AddWithValue("@remarks", request.Remarks);
                    command.Parameters.AddWithValue("@fin_year_id", request.Fin_Year_Id);
                    command.Parameters.AddWithValue("@comp_id", request.Comp_Id);
                    command.Parameters.AddWithValue("@created_date", request.Created_Date);
                    command.Parameters.AddWithValue("@user_id", request.User_Id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteInwardReturn(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_inward_return_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@inwardreturn_id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<int> UpdateInwardReturn(UpdateInwardreturnRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {

                string spname = "sp_inward_return_ins_upd_del";

                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@action", "update");
                parameters.Add("@inwardreturn_id", request.Inwardreturn_Id);
                parameters.Add("@inward_id", request.Inward_Id);
                parameters.Add("@customer_id", request.Customer_Id);
                parameters.Add("@product_id", request.Product_Id);
                parameters.Add("@returnquantity", request.ReturnQuantity);
                parameters.Add("@return_date", request.Return_Date);
                parameters.Add("@remarks", request.Remarks);
                parameters.Add("@fin_year_id", request.Fin_Year_Id);
                parameters.Add("@comp_id", request.Comp_Id);
                parameters.Add("@created_date", request.Created_Date);
                parameters.Add("@updated_date", request.Updated_Date);
                parameters.Add("@user_id", request.User_Id);

                return await connection.ExecuteAsync(
                    spname,
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
        }


        public async Task<List<Inwardreturn_List>> Get_inwardreturn_list()
        {
            var list = new List<Inwardreturn_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_inward_return_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectall");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Inwardreturn_List
                            {
                                Inwardreturn_Id = reader.GetInt64(0),
                                Inward_Id = reader.GetInt64(1),
                                Customer_Name = reader.GetString(2),
                                Product_Name = reader.GetString(3),
                                ReturnQuantity = reader.GetDecimal(4),
                                Return_Date = reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                Remarks = reader.GetString(6),
                                Fin_Year_Name = reader.GetString(7),
                                Comp_Name = reader.GetString(8),
                                Created_Date = reader.GetDateTime(9).ToString("yyyy-MM-dd"),
                                Updated_Date = reader.IsDBNull(10) ? "" : reader.GetDateTime(10).ToString("yyyy-MM-dd"),
                                User_Name = reader.IsDBNull(11) ? "" : reader.GetString(11),
                            });

                        }
                    }
                }
            }

            return list;
        }



        public async Task<List<SingleInwardreturn>> Get_InwardReturn_by_id(long id)
        {
            List<SingleInwardreturn> list = new List<SingleInwardreturn>();

            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                using (var command = new SqlCommand("sp_inward_return_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectone");
                    command.Parameters.AddWithValue("@inward_id", id);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new SingleInwardreturn
                            {
                                Inwardreturn_Id = reader.GetInt64(0),
                                Inward_Id = reader.GetInt64(1),
                                Customer_Id = reader.GetInt64(2),
                                Product_Id = reader.GetInt64(3),
                                ReturnQuantity = reader.GetDecimal(4),
                                Return_Date = reader.GetDateTime(5),
                                Remarks = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                Fin_Year_Id = reader.GetInt64(7),
                                Comp_Id = reader.GetInt64(8),
                                Created_Date = reader.GetDateTime(9),
                                Updated_Date = reader.IsDBNull(10) ? null : reader.GetDateTime(10),
                                User_Id = reader.IsDBNull(11) ? 0 : reader.GetInt64(11)
                            });
                        }
                    }
                }
            }

            return list;
        }


        public async Task<List<Drop_InwardDetail>> Get_inward_for_return(long customer_id, long comp_id, long fin_year_id)
        {
            var list = new List<Drop_InwardDetail>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_inward_return_ins_upd_del";

                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "get_inward_for_return");
                    command.Parameters.AddWithValue("@customer_id", customer_id);
                    command.Parameters.AddWithValue("@comp_id", comp_id);
                    command.Parameters.AddWithValue("@fin_year_id", fin_year_id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Drop_InwardDetail
                            {
                                Inward_Id = reader.GetInt64(0),
                                Inward_Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return list;
        }



        public async Task<List<Drop_ProductDetail>> Get_products_for_return(long inward_id)
        {
            var list = new List<Drop_ProductDetail>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_inward_return_ins_upd_del";

                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "get_products_for_return");
                    command.Parameters.AddWithValue("@inward_id", inward_id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Drop_ProductDetail
                            {
                                Inward_Details_Id = reader.GetInt64(0),
                                Product_Id = reader.GetInt64(1),
                                Product_Name = reader.GetString(2)
                            });
                        }
                    }
                }
            }

            return list;
        }


        //public async Task<SingleInwardreturn?> Get_InwardReturn_by_id(long id)
        //{
        //    SingleInwardreturn? InwardReturn = null;
        //    var connectionstring = _configuration.GetConnectionString("DefaultConnection");

        //    using (var connection = new SqlConnection(connectionstring))
        //    {
        //        string spName = "sp_inward_return_ins_upd_del";

        //        await connection.OpenAsync();

        //        using (var command = new SqlCommand(spName, connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.AddWithValue("@action", "selectone");
        //            command.Parameters.AddWithValue("@inward_id", id);

        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    InwardReturn = new SingleInwardreturn
        //                    {
        //                        Inwardreturn_Id = reader.GetInt64(0),
        //                        Inward_Id = reader.GetInt64(1),
        //                        Customer_Id = reader.GetInt64(2),
        //                        Product_Id = reader.GetInt64(3),
        //                        ReturnQuantity = reader.GetDecimal(4),
        //                        Remarks = reader.GetString(5),
        //                        Fin_Year_Id = reader.GetInt64(6),
        //                        Comp_Id = reader.GetInt64(7),
        //                        Created_Date = reader.GetDateTime(8),
        //                        Updated_Date = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
        //                        User_Id = reader.IsDBNull(10) ? 0 : reader.GetInt64(10)
        //                    };
        //                }
        //            }
        //        }
        //    }

        //    return InwardReturn;
        //}









    }
}
