using Dapper;
using dhara_pvd_decor_webapi_proj.Controllers;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Data;
using System.Data.SqlClient;
using static dhara_pvd_decor_webapi_proj.Controllers.InwardController;


namespace dhara_pvd_decor_webapi_proj.Services
{

    public class InwardService : IInwardService
    {
        private readonly IConfiguration _configuration;

        public InwardService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> AddInward(AddInwardRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {

                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_inward_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "insert");
                    command.Parameters.AddWithValue("@inward_id", 0);
                    command.Parameters.AddWithValue("@inward_name", request.Inward_name);
                    command.Parameters.AddWithValue("@customer_id", request.Customer_Id);
                    command.Parameters.AddWithValue("@Inward_Date", request.Inward_Date);
                    command.Parameters.AddWithValue("@created_date", request.Created_date);
                    command.Parameters.AddWithValue("@created_by", request.Created_by);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteInward(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_inward_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@inward_id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> Updateinward(UpdateInwardRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spname = "sp_inward_mast_ins_upd_del";

                var parameters = new DynamicParameters();
                parameters.Add("@action", "update");
                parameters.Add("@inward_id", request.Inward_Id);
                parameters.Add("inward_name", request.Inward_name);
                parameters.Add("@customer_id", request.Customer_Id);
                parameters.Add("@Inward_Date", request.Inward_Date);
                parameters.Add("@created_date", request.Created_date);
                parameters.Add("@updated_date", request.Updated_date);
                parameters.Add("@created_by", request.Created_by);
                parameters.Add("@modified_by", request.Modified_by);

                return await connection.ExecuteAsync(
                    spname,
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
        }

        public async Task<List<Inward_List>> GetInwardList()
        {
            var list = new List<Inward_List>();

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_inward_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectall");

                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Inward_List
                            {
                                Inward_Id = reader.GetInt64(0),
                                Inward_name = reader.GetString(1),
                                Customer_Id = reader.GetInt64(2),
                                Customer_Name = reader.GetString(3),
                                Inward_Date = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                Created_Date = reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                Updated_Date = reader.IsDBNull(6) ? "" : reader.GetDateTime(6).ToString("yyyy-MM-dd"),
                                Created_by = reader.GetInt64(7),
                                Modified_by = reader.IsDBNull(8) ? 0 : reader.GetInt64(8),
                                Created_by_name = reader.GetString(9),
                                Modified_by_name = reader.IsDBNull(10) ? "" : reader.GetString(10)
                            });

                        }
                    }
                }
            }
            return (list);
        }

        public async Task<SingleInwardList?> GetInwardById(long id)
        {
            SingleInwardList? inward = null;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_inward_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectone");
                    command.Parameters.AddWithValue("@inward_id", id);

                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            inward = new SingleInwardList
                            {
                                Inward_Id = reader.GetInt64(0),
                                Inward_name = reader.GetString(1),
                                Customer_Id = reader.GetInt64(2),
                                Inward_Date = reader.GetDateTime(3),
                                Created_Date = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                Updated_Date = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                Created_by = reader.IsDBNull(6) ? 0 : reader.GetInt64(6),
                                Modified_by = reader.IsDBNull(7) ? 0 : reader.GetInt64(7)
                            };
                        }
                    }
                }
            }

            return (inward);
        }

        public async Task<List<Drop_Inward_List>> Get_drop_inwardlist()
        {
            var list = new List<Drop_Inward_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_inward_mast_ins_upd_del";

                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "inwardlist");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Drop_Inward_List
                            {
                                Inward_Id = reader.GetInt64(0),
                                Inward_name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return (list);
        }

        public async Task<int> AddInwardDetails(AddInwardDetailsRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_inward_details_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;


                    command.Parameters.AddWithValue("@action", "insert");
                    command.Parameters.AddWithValue("@inward_details_id", 0);
                    command.Parameters.AddWithValue("@inward_id", request.Inward_Id);
                    command.Parameters.AddWithValue("@product_id", request.Product_Id);
                    command.Parameters.AddWithValue("@totalquantity", request.TotalQuantity);
                    command.Parameters.AddWithValue("@balance_quantity", request.Balance_Quantity);
                    command.Parameters.AddWithValue("@inward_status", request.Inward_Status);
                    command.Parameters.AddWithValue("@remarks", request.Remarks);
                    command.Parameters.AddWithValue("@fin_year_id", request.Fin_Year_Id);
                    command.Parameters.AddWithValue("@comp_id", request.Comp_Id);
                    command.Parameters.AddWithValue("@created_date", request.Created_Date);
                    command.Parameters.AddWithValue("@updated_date", request.Updated_Date);
                    command.Parameters.AddWithValue("@created_by", request.Created_By);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_By);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> UpdateInwardDetails(UpdateInwardDetailsRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spname = "sp_inward_details_ins_upd_del";

                var parameters = new DynamicParameters();


                parameters.Add("@action", "update");
                parameters.Add("@inward_details_id", request.Inward_Details_Id);
                parameters.Add("@inward_id", request.Inward_Id);
                parameters.Add("@product_id", request.Product_Id);
                parameters.Add("@totalquantity", request.TotalQuantity);
                parameters.Add("@balance_quantity", request.Balance_Quantity);
                parameters.Add("@inward_status", request.Inward_Status);
                parameters.Add("@remarks", request.Remarks);
                parameters.Add("@fin_year_id", request.Fin_Year_Id);
                parameters.Add("@comp_id", request.Comp_Id);
                parameters.Add("@created_date", request.Created_Date);
                parameters.Add("@updated_date", request.Updated_Date);
                parameters.Add("@created_by", request.Created_By);
                parameters.Add("@modified_by", request.Modified_By);

                return await connection.ExecuteAsync(
                    spname,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public async Task<int> DeleteInwardDetails(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_inward_details_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@inward_details_id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Inward_Details_List>> GetInwardDetailsList()
        {
            var list = new List<Inward_Details_List>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_inward_details_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectall");

                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Inward_Details_List
                            {
                                Inward_Details_Id = reader.GetInt64(0),
                                Inward_Id = reader.GetInt64(1),
                                Inward_name = reader.GetString(2),
                                Product_Id = reader.GetInt64(3),
                                Product_Name = reader.GetString(4),
                                TotalQuantity = reader.GetDecimal(5),
                                Balance_Quantity = reader.GetDecimal(6),
                                Inward_Status = reader.GetBoolean(7),
                                Remarks = reader.GetString(8),
                                Fin_Year_Id = reader.GetInt64(9),
                                Fin_Name = reader.GetString(10),
                                Comp_Id = reader.GetInt64(11),
                                Comp_Name = reader.GetString(12),
                                Created_Date = reader.GetDateTime(13).ToString("yyyy-MM-dd"),
                                Updated_Date = reader.IsDBNull(14) ? "" : reader.GetDateTime(14).ToString("yyyy-MM-dd"),
                                Created_By = reader.GetInt64(15),
                                Modified_By = reader.IsDBNull(16) ? 0 : reader.GetInt64(16),
                                Created_By_Name = reader.GetString(17),
                                Modified_By_Name = reader.IsDBNull(18) ? "" : reader.GetString(18)
                            });
                        }
                    }
                }
            }

            return list;
        }

        public async Task<List<SingleInwardDetailsList>> GetInwardDetailsByInwardId(long id)
        {
            List<SingleInwardDetailsList> list = new List<SingleInwardDetailsList>();

            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_inward_details_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectone");
                    command.Parameters.AddWithValue("@inward_id", id);

                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new SingleInwardDetailsList
                            {
                                Inward_Details_Id = reader.GetInt64(0),
                                Inward_Id = reader.GetInt64(1),
                                Product_Id = reader.GetInt64(2),
                                Product_Name = reader.GetString(3),
                                TotalQuantity = reader.GetDecimal(4),
                                Balance_Quantity = reader.GetDecimal(5),
                                Inward_Status = reader.GetBoolean(6),
                                Remarks = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                Fin_Year_Id = reader.GetInt64(8),
                                Fin_Name = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                Comp_Id = reader.GetInt64(10),
                                Created_Date = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                                Updated_Date = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                                Created_By = reader.IsDBNull(13) ? 0 : reader.GetInt64(13),
                                Modified_By = reader.IsDBNull(14) ? 0 : reader.GetInt64(14)
                            });
                        }
                    }
                }
            }

            return list;
        }



        public async Task<List<InwardQuantitySummary>> GetInwardQuantitySummary(long inwardId, long productId)
        {
            var list = new List<InwardQuantitySummary>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_inward_details_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@action", "get_quantity_summary");
                    command.Parameters.AddWithValue("@inward_id", inwardId);
                    command.Parameters.AddWithValue("@product_id", productId);

                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new InwardQuantitySummary
                            {
                                Inward_Id = reader.GetInt64(0),
                                Product_Id = reader.GetInt64(1),
                                Total_Sold_Quantity = reader.GetDecimal(2),
                                Total_Return_Quantity = reader.GetDecimal(3)
                            });
                        }
                    }
                }
            }

            return list;
        }

    }
}
