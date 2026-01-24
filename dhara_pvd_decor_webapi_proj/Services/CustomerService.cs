using Dapper;
using dhara_pvd_decor_webapi_proj.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using static dhara_pvd_decor_webapi_proj.Controllers.CustomerController;
using static dhara_pvd_decor_webapi_proj.Controllers.ProdTypeController;
using static dhara_pvd_decor_webapi_proj.Controllers.ProductController;
using static dhara_pvd_decor_webapi_proj.Controllers.TranTypeController;


namespace dhara_pvd_decor_webapi_proj.Services
{

    public class CustomerService : ICustomerService
    {
        private readonly IConfiguration _configuration;

        public CustomerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public async Task<int> AddCustomer(CustomerController.AddCustomerRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_customer_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "insert");
                    command.Parameters.AddWithValue("@customer_id", 0);
                    command.Parameters.AddWithValue("@customer_name", request.Customer_Name);
                    command.Parameters.AddWithValue("@prefix", request.Prefix);
                    command.Parameters.AddWithValue("@gender", request.Gender);
                    command.Parameters.AddWithValue("@phonenumber", request.Phonenumber);
                    command.Parameters.AddWithValue("@city_id", request.City_Id);
                    command.Parameters.AddWithValue("@cust_address", request.Cust_Address);
                    command.Parameters.AddWithValue("@email_id", request.Email_Id);
                    command.Parameters.AddWithValue("@dob", request.Dob);
                    command.Parameters.AddWithValue("@aadhaar_number", request.Aadhaar_Number);
                    command.Parameters.AddWithValue("@license_number", request.License_Number);
                    command.Parameters.AddWithValue("@pan_number", request.Pan_Number);
                    command.Parameters.AddWithValue("@gst_number", request.Gst_Number);
                    command.Parameters.AddWithValue("@is_active", request.Is_Active);
                    command.Parameters.AddWithValue("@customer_notes", request.Customer_Notes);
                    command.Parameters.AddWithValue("@created_date", request.Created_date);
                    command.Parameters.AddWithValue("@created_by", request.Created_by);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                    return await command.ExecuteNonQueryAsync();
                }

            }
        }


        public async Task<int> DeleteCustomer(long id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_customer_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@customer_id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> UpdateCustomer(CustomerController.UpdateCustomerRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@action", "update");
                parameters.Add("@customer_id", request.Customer_Id);
                parameters.Add("@customer_name", request.Customer_Name);
                parameters.Add("@prefix", request.Prefix);
                parameters.Add("@gender", request.Gender);
                parameters.Add("@phonenumber", request.Phonenumber);
                parameters.Add("@city_id", request.City_Id);
                parameters.Add("@cust_address", request.Cust_Address);
                parameters.Add("@email_id", request.Email_Id);
                parameters.Add("@dob", request.Dob);
                parameters.Add("@aadhaar_number", request.Aadhaar_Number);
                parameters.Add("@license_number", request.License_Number);
                parameters.Add("@pan_number", request.Pan_Number);
                parameters.Add("@gst_number", request.Gst_Number);
                parameters.Add("@is_active", request.Is_Active);
                parameters.Add("@customer_notes", request.Customer_Notes);
                parameters.Add("@created_date", request.Created_date);
                parameters.Add("@updated_date", request.Updated_date);
                parameters.Add("@created_by", request.Created_by);
                parameters.Add("@modified_by", request.Modified_by);

                return await connection.ExecuteAsync(
                    "sp_customer_mast_ins_upd_del",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public async Task<List<CustomerController.Customer_List>> Get_Customer_List()
        {
            var list = new List<Customer_List>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_customer_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectall");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Customer_List
                            {
                                Customer_Id = reader.GetInt64(0),
                                Customer_Name = reader.GetString(1),
                                Prefix = reader.GetString(2),
                                Gender = reader.GetString(3),
                                Phonenumber = reader.GetString(4),
                                City_Id = reader.GetInt64(5),
                                City_Name = reader.GetString(6),
                                Cust_Address = reader.GetString(7),
                                Email_Id = reader.GetString(8),
                                Dob = reader.IsDBNull(9) ? "" : reader.GetDateTime(9).ToString("yyyy-MM-dd"),
                                Aadhaar_Number = reader.GetString(10),
                                License_Number = reader.GetString(11),
                                Pan_Number = reader.GetString(12),
                                Gst_Number = reader.GetString(13),
                                Is_Active = reader.GetBoolean(14),
                                Customer_Notes = reader.GetString(15),
                                Created_Date = reader.GetDateTime(16).ToString("yyyy-MM-dd"),
                                Updated_Date = reader.IsDBNull(17) ? "" : reader.GetDateTime(17).ToString("yyyy-MM-dd"),
                                Created_by = reader.GetInt64(18),
                                Modified_by = reader.IsDBNull(19) ? 0 : reader.GetInt64(19),
                                Created_by_name = reader.GetString(20),
                                Modified_by_name = reader.IsDBNull(21) ? "" : reader.GetString(21)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public async Task<CustomerController.Single_Customer_List?> GetCustomerById(long id)
        {
            Single_Customer_List? customer = null;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_customer_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectone");
                    command.Parameters.AddWithValue("@customer_id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            customer = new Single_Customer_List
                            {
                                Customer_Id = reader.GetInt64(0),
                                Customer_Name = reader.GetString(1),
                                Prefix = reader.GetString(2),
                                Gender = reader.GetString(3),
                                Phonenumber = reader.GetString(4),
                                City_Id = reader.GetInt64(5),
                                Cust_Address = reader.GetString(6),
                                Email_Id = reader.GetString(7),
                                Dob = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                                Aadhaar_Number = reader.GetString(9),
                                License_Number = reader.GetString(10),
                                Pan_Number = reader.GetString(11),
                                Gst_Number = reader.GetString(12),
                                Is_Active = reader.GetBoolean(13),
                                Customer_Notes = reader.GetString(14),
                                Created_Date = reader.IsDBNull(15) ? null : reader.GetDateTime(15),
                                Updated_Date = reader.IsDBNull(16) ? null : reader.GetDateTime(16),
                                Created_by = reader.IsDBNull(17) ? 0 : reader.GetInt64(17),
                                Modified_by = reader.IsDBNull(18) ? 0 : reader.GetInt64(18)
                            };
                        }
                    }
                }
            }

            return customer;
        }

        public async Task<List<CustomerController.Drop_Customer_List>> Get_drop_customerlist()
        {
            var list = new List<Drop_Customer_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_customer_mast_ins_upd_del";

                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "customerlist");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Drop_Customer_List
                            {
                                Customer_Id = reader.GetInt64(0),
                                Customer_Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return list;
        }

        //-----------------------------------

        public async Task<int> Add_CustDetail_Request(CustomerController.Add_CustDetail_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_customer_details_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "insert");
                    command.Parameters.AddWithValue("@customer_details_id", request.Cust_detail_id);
                    command.Parameters.AddWithValue("@customer_id", request.Customer_id);
                    command.Parameters.AddWithValue("@opening_balance", request.Opening_balance);
                    command.Parameters.AddWithValue("@invoice_balance", request.Invoice_balance);
                    command.Parameters.AddWithValue("@outstanding_balance", request.Outstanding_balance);
                    command.Parameters.AddWithValue("@created_date", request.Created_date);
                    command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                    command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                    command.Parameters.AddWithValue("@created_by", request.Created_by);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteCustDetail(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_customer_details_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@customer_details_id", id);

                    return await command.ExecuteNonQueryAsync();

                }
            }
        }

        public async Task<int> UpdateCustDetail(CustomerController.Update_CustDetail_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spname = "sp_customer_details_ins_upd_del";

                var parameters = new DynamicParameters();
                parameters.Add("@action", "update");
                parameters.Add("@customer_details_id", request.Cust_detail_id);
                parameters.Add("@customer_id", request.Customer_id);
                parameters.Add("@opening_balance", request.Opening_balance);
                parameters.Add("@invoice_balance", request.Invoice_balance);
                parameters.Add("@outstanding_balance", request.Outstanding_balance);
                parameters.Add("@created_date", request.Created_date);
                parameters.Add("@updated_date", request.Updated_date);
                parameters.Add("@fin_year_id", request.Fin_year_id);
                parameters.Add("@comp_id", request.Comp_id);
                parameters.Add("@created_by", request.Created_by);
                parameters.Add("@modified_by", request.Modified_by);

                return await connection.ExecuteAsync(
                    spname,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
        }


        public async Task<List<CustomerController.CustDetail_List>> Get_CustDetail_list()
        {
            var list = new List<CustDetail_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_customer_details_ins_upd_del";

                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectall");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new CustDetail_List
                            {
                                Cust_detail_id = reader.GetInt64(0),
                                Customer_name = reader.GetInt64(1),
                                Opening_balance = reader.GetDecimal(2),
                                Invoice_balance = reader.GetDecimal(3),
                                Outstanding_balance = reader.GetDecimal(4),
                                Created_Date = reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                Updated_Date = reader.IsDBNull(6) ? "" : reader.GetDateTime(6).ToString("yyyy-MM-dd"),
                                Fin_year_id = reader.GetInt64(7),
                                Fin_year_name = reader.GetString(8),
                                Comp_id = reader.GetInt64(9),
                                Comp_name = reader.GetString(10),
                                Created_by = reader.GetInt64(11),
                                Modified_by = reader.IsDBNull(12) ? 0 : reader.GetInt64(12),
                                Created_by_name = reader.GetString(13),
                                Modified_by_name = reader.IsDBNull(14) ? "" : reader.GetString(14)
                            });
                        }
                    }
                }
            }

            return list;
        }

        public async Task<List<CustomerController.Single_CustDetail>> Get_CustDetail_by_id(long id)
        {
            var details = new List<Single_CustDetail>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_customer_details_ins_upd_del";
                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectone");
                    command.Parameters.AddWithValue("@customer_id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            details.Add(new Single_CustDetail
                            {
                                Cust_detail_id = reader.GetInt64(0),
                                Customer_id = reader.GetInt64(1),
                                Opening_balance = reader.GetDecimal(2),
                                Invoice_balance = reader.GetDecimal(3),
                                Outstanding_balance = reader.GetDecimal(4),
                                Created_date = reader.GetDateTime(5),
                                Updated_date = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                                Fin_year_id = reader.GetInt64(7),
                                Fin_year_name = reader.GetString(8),
                                Comp_id = reader.GetInt64(9),
                                Created_by = reader.IsDBNull(10) ? 0 : reader.GetInt64(10),
                                Modified_by = reader.IsDBNull(11) ? 0 : reader.GetInt64(11)
                            });
                        }
                    }
                }
            }

            return details;
        }

        public async Task<List<CustomerController.Drop_CustDetail>> Get_drop_custdetail_list()
        {
            var list = new List<Drop_CustDetail>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_customer_details_ins_upd_del";

                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "cust_detail_list");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Drop_CustDetail
                            {
                                Cust_detail_id = reader.GetInt64(0)
                            });
                        }
                    }
                }
            }

            return list;
        }

    }
}

