using Dapper;
using System.Data;
using System.Data.SqlClient;
using dhara_pvd_decor_webapi_proj.Controllers;
using static dhara_pvd_decor_webapi_proj.Controllers.CompanyController;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;


        public CompanyService(IConfiguration configuration , IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        //public async Task<bool> AddCompany(CompanyController.AddCompanyRequest request)
        //{
        //    var connectionstring = _configuration.GetConnectionString("DefaultConnection");
        //    string logoPath = "";

        //    if (request.Logo_File != null && request.Logo_File.Length > 0)
        //    {
        //        Console.WriteLine("ContentRootPath: " + _env.ContentRootPath);
        //        Console.WriteLine("WebRootPath: " + _env.WebRootPath);

        //        var folder = Path.Combine(
        //            _env.ContentRootPath,
        //            "wwwroot",
        //            "images",
        //            "company-logos"
        //        );

        //        Console.WriteLine("Upload Folder Path: " + folder);

        //        if (!Directory.Exists(folder))
        //            Directory.CreateDirectory(folder);

        //        var fileName = $"logo_{Guid.NewGuid()}{Path.GetExtension(request.Logo_File.FileName)}";

        //        var fullPath = Path.Combine(folder, fileName);

        //        Console.WriteLine("Saving File To: " + fullPath);

        //        using (var stream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            await request.Logo_File.CopyToAsync(stream);
        //        }

        //        logoPath = $"/images/company-logos/{fileName}";

        //        Console.WriteLine("Logo URL Saved In DB: " + logoPath);
        //    }


        //    using (SqlConnection connection = new SqlConnection(connectionstring))
        //    {
        //        await connection.OpenAsync();

        //        using (SqlCommand command = new SqlCommand("sp_company_mast_ins_upd_del", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.AddWithValue("@action", "insert");
        //            command.Parameters.AddWithValue("@comp_id", request.Comp_id);
        //            command.Parameters.AddWithValue("@comp_code", request.Comp_code);
        //            command.Parameters.AddWithValue("@comp_name", request.Comp_name);
        //            command.Parameters.AddWithValue("@comp_short_name", request.Comp_short_name);
        //            command.Parameters.AddWithValue("@comp_type", request.Comp_type);
        //            command.Parameters.AddWithValue("@comp_desc", request.Comp_desc);
        //            command.Parameters.AddWithValue("@cin_number", request.Cin_number);
        //            command.Parameters.AddWithValue("@gst_number", request.Gst_number);
        //            command.Parameters.AddWithValue("@pan_number", request.Pan_number);
        //            command.Parameters.AddWithValue("@contperson_name", request.Contperson_name);
        //            command.Parameters.AddWithValue("@contact_email", request.Contact_email);
        //            command.Parameters.AddWithValue("@contact_phone", request.Contact_phone);
        //            command.Parameters.AddWithValue("@address_line1", request.Address_line1);
        //            command.Parameters.AddWithValue("@address_line2", request.Address_line2);
        //            command.Parameters.AddWithValue("@city_id", request.City_id);
        //            command.Parameters.AddWithValue("@pincode", request.Pincode);
        //            command.Parameters.AddWithValue("@is_active", request.Is_active);
        //            command.Parameters.AddWithValue("@created_date", request.Created_date);
        //            command.Parameters.AddWithValue("@updated_date", request.Updated_date);
        //            command.Parameters.AddWithValue("@logo_path", logoPath);
        //            command.Parameters.AddWithValue("@created_by", request.Created_by);
        //            command.Parameters.AddWithValue("@modified_by", request.Modified_by);

        //            int rowsAffected = await command.ExecuteNonQueryAsync();
        //            return rowsAffected > 0;
        //        }
        //    }
        //}


        public async Task<bool> AddCompany(CompanyController.AddCompanyRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            byte[] logoBytes = null;

            if (!string.IsNullOrEmpty(request.Logo_path))
            {
                try
                {
                    logoBytes = Convert.FromBase64String(request.Logo_path);
                }
                catch
                {
                    throw new Exception("Invalid Base64 logo format.");
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_company_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "insert");
                    command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                    command.Parameters.AddWithValue("@comp_code", request.Comp_code);
                    command.Parameters.AddWithValue("@comp_name", request.Comp_name);
                    command.Parameters.AddWithValue("@comp_short_name", request.Comp_short_name);
                    command.Parameters.AddWithValue("@comp_type", request.Comp_type);
                    command.Parameters.AddWithValue("@comp_desc", request.Comp_desc);
                    command.Parameters.AddWithValue("@cin_number", request.Cin_number);
                    command.Parameters.AddWithValue("@gst_number", request.Gst_number);
                    command.Parameters.AddWithValue("@pan_number", request.Pan_number);
                    command.Parameters.AddWithValue("@contperson_name", request.Contperson_name);
                    command.Parameters.AddWithValue("@contact_email", request.Contact_email);
                    command.Parameters.AddWithValue("@contact_phone", request.Contact_phone);
                    command.Parameters.AddWithValue("@address_line1", request.Address_line1);
                    command.Parameters.AddWithValue("@address_line2", request.Address_line2);
                    command.Parameters.AddWithValue("@city_id", request.City_id);
                    command.Parameters.AddWithValue("@pincode", request.Pincode);
                    command.Parameters.AddWithValue("@is_active", request.Is_active);
                    command.Parameters.AddWithValue("@created_date", request.Created_date);
                    command.Parameters.AddWithValue("@updated_date", request.Updated_date);
                    //command.Parameters.AddWithValue("@logo_path", request.Logo_path);
                    command.Parameters.Add("@logo_path", SqlDbType.VarBinary).Value = (object)logoBytes ?? DBNull.Value;
                    command.Parameters.AddWithValue("@created_by", request.Created_by);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> DeleteCompany(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_company_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@comp_id", id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> UpdateCompany(CompanyController.UpdateCompanyRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            byte[] logoBytes = null;

            if (!string.IsNullOrEmpty(request.Logo_path))
            {
                try
                {
                    logoBytes = Convert.FromBase64String(request.Logo_path);
                }
                catch
                {
                    throw new Exception("Invalid Base64 logo format.");
                }
            }

            using (var connection = new SqlConnection(connectionstring))
            {
                string spname = "sp_company_mast_ins_upd_del";

                var parameters = new DynamicParameters();
                parameters.Add("@action", "update");
                parameters.Add("@comp_id", request.Comp_id);
                parameters.Add("@comp_code", request.Comp_code);
                parameters.Add("@comp_name", request.Comp_name);
                parameters.Add("@comp_short_name", request.Comp_short_name);
                parameters.Add("@comp_type", request.Comp_type);
                parameters.Add("@comp_desc", request.Comp_desc);
                parameters.Add("@cin_number", request.Cin_number);
                parameters.Add("@gst_number", request.Gst_number);
                parameters.Add("@pan_number", request.Pan_number);
                parameters.Add("@contperson_name", request.Contperson_name);
                parameters.Add("@contact_email", request.Contact_email);
                parameters.Add("@contact_phone", request.Contact_phone);
                parameters.Add("@address_line1", request.Address_line1);
                parameters.Add("@address_line2", request.Address_line2);
                parameters.Add("@city_id", request.City_id);
                parameters.Add("@pincode", request.Pincode);
                parameters.Add("@is_active", request.Is_active);
                parameters.Add("@created_date", request.Created_date);
                parameters.Add("@updated_date", request.Updated_date);
                //parameters.Add("@logo_path", request.Logo_path);
                parameters.Add("@logo_path", logoBytes, DbType.Binary);
                parameters.Add("@created_by", request.Created_by);
                parameters.Add("@modified_by", request.Modified_by);

                rows_affected = await connection.ExecuteAsync(
                    spname,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }

            return rows_affected > 0;
        }

        public async Task<List<CompanyController.company_list>> GetCompanyList()
        {
            var company_list = new List<CompanyController.company_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_company_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectall");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var company = new CompanyController.company_list
                            {
                                Comp_id = reader.GetInt64(0),
                                Comp_code = reader.GetString(1),
                                Comp_name = reader.GetString(2),
                                Comp_short_name = reader.GetString(3),
                                Comp_type = reader.GetString(4),
                                Comp_desc = reader.GetString(5),
                                Cin_number = reader.GetString(6),
                                Gst_number = reader.GetString(7),
                                Pan_number = reader.GetString(8),
                                Contperson_name = reader.GetString(9),
                                Contact_email = reader.GetString(10),
                                Contact_phone = reader.GetString(11),
                                Address_line1 = reader.GetString(12),
                                Address_line2 = reader.GetString(13),
                                City_id = reader.GetInt64(14),
                                City_name = reader.GetString(15),
                                Pincode = reader.GetString(16),
                                Is_active = reader.GetBoolean(17),
                                Created_date = reader.GetDateTime(18).ToString("yyyy-MM-dd"),
                                Updated_date = reader.IsDBNull(19) ? "" : reader.GetDateTime(19).ToString("yyyy-MM-dd"),
                                //Logo_path = reader.IsDBNull(20) ? "" : reader.GetString(20),
                                Created_by = reader.GetInt64(20),
                                Modified_by = reader.IsDBNull(21) ? 0 : reader.GetInt64(21),
                                Created_by_name = reader.GetString(22),
                                Modified_by_name = reader.IsDBNull(23) ? "" : reader.GetString(23)
                            };

                            company_list.Add(company);
                        }
                    }
                }
            }

            return company_list;
        }

        public async Task<CompanyController.single_company_list?> GetCompanyById(long id)
        {
            CompanyController.single_company_list? company = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_company_mast_ins_upd_del";

                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectone");
                    command.Parameters.AddWithValue("@comp_id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            company = new CompanyController.single_company_list
                            {
                                Comp_id = reader.GetInt64(0),
                                Comp_code = reader.GetString(1),
                                Comp_name = reader.GetString(2),
                                Comp_short_name = reader.GetString(3),
                                Comp_type = reader.GetString(4),
                                Comp_desc = reader.GetString(5),
                                Cin_number = reader.GetString(6),
                                Gst_number = reader.GetString(7),
                                Pan_number = reader.GetString(8),
                                Contperson_name = reader.GetString(9),
                                Contact_email = reader.GetString(10),
                                Contact_phone = reader.GetString(11),
                                Address_line1 = reader.GetString(12),
                                Address_line2 = reader.GetString(13),
                                City_id = reader.GetInt64(14),
                                Pincode = reader.GetString(15),
                                Is_active = reader.GetBoolean(16),
                                Created_date = reader.GetDateTime(17),
                                Updated_date = reader.IsDBNull(18) ? null : reader.GetDateTime(18),
                                Logo_path = reader.IsDBNull(19)? "": Convert.ToBase64String((byte[])reader[19]),
                                Created_by = reader.IsDBNull(20) ? 0 : reader.GetInt64(20),
                                Modified_by = reader.IsDBNull(21) ? 0 : reader.GetInt64(21)
                            };
                        }
                    }
                }
            }

            return company;
        }


        public async Task<CompanyController.single_company_list?> GetCompanylogoById(long id)
        {
            CompanyController.single_company_list? company = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_company_mast_ins_upd_del";

                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectone");
                    command.Parameters.AddWithValue("@comp_id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            company = new CompanyController.single_company_list
                            {
                                Comp_id = reader.GetInt64(0),
                                Comp_code = reader.GetString(1),
                                Comp_name = reader.GetString(2),
                                Comp_short_name = reader.GetString(3),
                                Comp_type = reader.GetString(4),
                                Comp_desc = reader.GetString(5),
                                Cin_number = reader.GetString(6),
                                Gst_number = reader.GetString(7),
                                Pan_number = reader.GetString(8),
                                Contperson_name = reader.GetString(9),
                                Contact_email = reader.GetString(10),
                                Contact_phone = reader.GetString(11),
                                Address_line1 = reader.GetString(12),
                                Address_line2 = reader.GetString(13),
                                City_id = reader.GetInt64(14),
                                Pincode = reader.GetString(15),
                                Is_active = reader.GetBoolean(16),
                                Created_date = reader.GetDateTime(17),
                                Updated_date = reader.IsDBNull(18) ? null : reader.GetDateTime(18),
                                Logo_path = reader.IsDBNull(19) ? "" : Convert.ToBase64String((byte[])reader[19]),
                                Created_by = reader.IsDBNull(20) ? 0 : reader.GetInt64(20),
                                Modified_by = reader.IsDBNull(21) ? 0 : reader.GetInt64(21)
                            };
                        }
                    }
                }
            }

            return company;
        }

        public async Task<List<CompanyController.drop_company_list>> GetDropCompanyList(long userId)
        {
            var company_list = new List<CompanyController.drop_company_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_company_mast_ins_upd_del";

                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "companylist");
                    command.Parameters.AddWithValue("@user_id", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var company = new CompanyController.drop_company_list
                            {
                                Comp_id = reader.GetInt64(0),
                                Comp_name = reader.GetString(1)
                            };

                            company_list.Add(company);
                        }
                    }
                }
            }

            return company_list;
        }


        public async Task<CompanyController.CompanyLogoResponse?> GetCompanyLogoById(long compId)
        {
            CompanyLogoResponse? result = null;

            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_company_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectlogo");
                    command.Parameters.AddWithValue("@comp_id", compId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                byte[] logoBytes = (byte[])reader["logo_path"];

                                result = new CompanyLogoResponse
                                {
                                    LogoBase64 = Convert.ToBase64String(logoBytes)
                                };
                            }
                            else
                            {
                                result = new CompanyLogoResponse
                                {
                                    LogoBase64 = ""
                                };
                            }
                        }
                    }
                }
            }

            return result;
        }


    }
}
