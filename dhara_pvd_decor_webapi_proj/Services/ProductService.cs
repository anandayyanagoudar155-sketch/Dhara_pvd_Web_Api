using Dapper;
using dhara_pvd_decor_webapi_proj.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using static dhara_pvd_decor_webapi_proj.Controllers.ProdTypeController;
using static dhara_pvd_decor_webapi_proj.Controllers.ProductController;
using static dhara_pvd_decor_webapi_proj.Controllers.TranTypeController;


namespace dhara_pvd_decor_webapi_proj.Services
{

    public class ProductService : IProductServices
    {
        private readonly IConfiguration _configuration;

        public ProductService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<int> Add_Product(ProductController.AddProductRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_product_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "insert");
                    command.Parameters.AddWithValue("@product_id", 0);
                    command.Parameters.AddWithValue("@prodtype_id", request.Prodtype_id);
                    command.Parameters.AddWithValue("@brand_id", request.Brand_id);
                    command.Parameters.AddWithValue("@hsn_id", request.Hsn_id);
                    command.Parameters.AddWithValue("@unit_id", request.Unit_id);
                    command.Parameters.AddWithValue("@product_name", request.Product_name);
                    command.Parameters.AddWithValue("@product_desc", request.Product_desc);
                    command.Parameters.AddWithValue("@rate", request.Rate);
                    command.Parameters.AddWithValue("@created_date", request.Created_date);
                    command.Parameters.AddWithValue("@created_by", request.Created_by);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<int> DeleteProduct(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_product_mast_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@product_id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> UpdateProduct(ProductController.UpdateProductRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spname = "sp_product_mast_ins_upd_del";

                var parameters = new DynamicParameters();
                parameters.Add("@action", "update");
                parameters.Add("@product_id", request.Product_Id);
                parameters.Add("@prodtype_id", request.Prodtype_id);
                parameters.Add("@brand_id", request.Brand_id);
                parameters.Add("@hsn_id", request.Hsn_id);
                parameters.Add("@unit_id", request.Unit_id);
                parameters.Add("@product_name", request.Product_name);
                parameters.Add("@product_desc", request.Product_desc);
                parameters.Add("@rate", request.Rate);
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

        public async Task<List<ProductController.Product_list>> Get_product_list()
        {
            var list = new List<Product_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_product_mast_ins_upd_del";

                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectall");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Product_list
                            {
                                Product_Id = reader.GetInt64(0),
                                Prodtype_Id = reader.GetInt64(1),
                                Prodtype_name = reader.GetString(2),
                                Brand_Id = reader.GetInt64(3),
                                Brand_name = reader.GetString(4),
                                Hsn_Id = reader.GetInt64(5),
                                Hsn_name = reader.GetString(6),
                                Unit_Id = reader.GetInt64(7),
                                Unit_name = reader.GetString(8),
                                Product_name = reader.GetString(9),
                                Product_desc = reader.GetString(10),
                                Rate = reader.GetDecimal(11),
                                Created_Date = reader.GetDateTime(12).ToString("yyyy-MM-dd"),
                                Updated_Date = reader.IsDBNull(13) ? "" : reader.GetDateTime(13).ToString("yyyy-MM-dd"),
                                Created_by = reader.GetInt64(14),
                                Modified_by = reader.IsDBNull(15) ? 0 : reader.GetInt64(15),
                                Created_by_name = reader.GetString(16),
                                Modified_by_name = reader.IsDBNull(17) ? "" : reader.GetString(17)
                            });
                        }
                    }
                }
            }

            return list;
        }

        public async Task<ProductController.SingleProductList?> Get_product_by_id(long id)
        {
            SingleProductList? product = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_product_mast_ins_upd_del";

                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectone");
                    command.Parameters.AddWithValue("@product_id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            product = new SingleProductList
                            {
                                Product_Id = reader.GetInt64(0),
                                Prodtype_id = reader.GetInt64(1),
                                Brand_id = reader.GetInt64(2),
                                Hsn_id = reader.GetInt64(3),
                                Unit_id = reader.GetInt64(4),
                                Product_name = reader.GetString(5),
                                Product_desc = reader.GetString(6),
                                Rate = reader.GetDecimal(7),
                                Created_Date = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                                Updated_Date = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9),
                                Created_by = reader.IsDBNull(10) ? 0 : reader.GetInt64(10),
                                Modified_by = reader.IsDBNull(11) ? 0 : reader.GetInt64(11)
                            };
                        }
                    }
                }
            }

            return product;
        }


        public async Task<List<ProductController.Drop_Product_List>> Get_drop_productlist()
        {
            var list = new List<Drop_Product_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_product_mast_ins_upd_del";

                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "productlist");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Drop_Product_List
                            {
                                Product_Id = reader.GetInt64(0),
                                Product_name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return list;
        }


        //-------------------------------------------------------------

        public async Task<int> Add_ProductDetail(ProductController.Add_ProductDetail_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_product_details_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "insert");
                    command.Parameters.AddWithValue("@product_details_id", request.Product_detail_id);
                    command.Parameters.AddWithValue("@product_id", request.Product_Id);
                    command.Parameters.AddWithValue("@opening_stock", request.Opening_stock);
                    command.Parameters.AddWithValue("@purchase", request.Purchase);
                    command.Parameters.AddWithValue("@sales", request.Sales);
                    command.Parameters.AddWithValue("@return", request.Return);
                    command.Parameters.AddWithValue("@current_stock", request.Current_stock);
                    command.Parameters.AddWithValue("@reorder_threshold", request.reorder_threshold);
                    command.Parameters.AddWithValue("@reorder_desc", request.reorder_desc);
                    command.Parameters.AddWithValue("@created_date", request.Created_date);
                    command.Parameters.AddWithValue("@fin_year_id", request.Fin_year_id);
                    command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                    command.Parameters.AddWithValue("@created_by", request.Created_by);
                    command.Parameters.AddWithValue("@modified_by", request.Modified_by);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> Delete_ProductDetail(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_product_details_ins_upd_del", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "delete");
                    command.Parameters.AddWithValue("@product_details_id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> Update_ProductDetail(ProductController.Update_ProductDetail_Request request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spname = "sp_product_details_ins_upd_del";

                var parameters = new DynamicParameters();
                parameters.Add("@action", "update");
                parameters.Add("@product_details_id", request.Product_detail_id);
                parameters.Add("@product_id", request.Product_Id);
                parameters.Add("@opening_stock", request.Opening_stock);
                parameters.Add("@purchase", request.Purchase);
                parameters.Add("@sales", request.Sales);
                parameters.Add("@return", request.Return);
                parameters.Add("@current_stock", request.Current_stock);
                parameters.Add("@reorder_threshold", request.reorder_threshold);
                parameters.Add("@reorder_desc", request.reorder_desc);
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

        public async Task<List<ProductController.ProductDetail_List>> Get_ProductDetail_List()
        {
            var list = new List<ProductDetail_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_product_details_ins_upd_del";

                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectall");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new ProductDetail_List
                            {
                                Product_detail_id = reader.GetInt64(0),
                                Product_Id = reader.GetInt64(1),
                                Product_name = reader.GetString(2),
                                Opening_stock = reader.GetDecimal(3),
                                Purchase = reader.GetDecimal(4),
                                Sales = reader.GetDecimal(5),
                                Return = reader.GetDecimal(6),
                                Current_stock = reader.GetDecimal(7),
                                reorder_threshold = reader.GetDecimal(8),
                                reorder_desc = reader.GetString(9),
                                Created_date = reader.GetDateTime(10).ToString("yyyy-MM-dd"),
                                Updated_date = reader.IsDBNull(11) ? "" : reader.GetDateTime(11).ToString("yyyy-MM-dd"),
                                Fin_year_id = reader.GetInt64(12),
                                Fin_year_name = reader.GetString(13),
                                Comp_id = reader.GetInt64(14),
                                Comp_name = reader.GetString(15),
                                Created_by = reader.GetInt64(16),
                                Modified_by = reader.IsDBNull(17) ? 0 : reader.GetInt64(17),
                                Created_by_name = reader.GetString(18),
                                Modified_by_name = reader.IsDBNull(19) ? "" : reader.GetString(19)
                            });
                        }
                    }
                }
            }

            return list;

        }

        public async Task<List<ProductController.Single_ProductDetail>> Get_ProductDetail_By_Id(long id)
        {
            var details = new List<Single_ProductDetail>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionstring))
            {
                string spName = "sp_product_details_ins_upd_del";
                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@action", "selectone");
                    command.Parameters.AddWithValue("@product_id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync()) 
                        {
                            details.Add(new Single_ProductDetail
                            {
                                Product_detail_id = reader.GetInt64(0),
                                Product_Id = reader.GetInt64(1),
                                Opening_stock = reader.GetDecimal(2),
                                Purchase = reader.GetDecimal(3),
                                Sales = reader.GetDecimal(4),
                                Return = reader.GetDecimal(5),
                                Current_stock = reader.GetDecimal(6),
                                reorder_threshold = reader.GetDecimal(7),
                                reorder_desc = reader.GetString(8),
                                Created_date = reader.GetDateTime(9),
                                Updated_date = reader.IsDBNull(10) ? null : reader.GetDateTime(10),
                                Fin_year_id = reader.GetInt64(11),
                                Fin_year_name = reader.GetString(12),
                                Comp_id = reader.GetInt64(13),
                                Created_by = reader.IsDBNull(14) ? 0 : reader.GetInt64(14),
                                Modified_by = reader.IsDBNull(15) ? 0 : reader.GetInt64(15)
                            });
                        }
                    }
                }
            }

            return details; 
        }

        public async Task<List<ProductController.Drop_ProductDetail>> Get_Drop_ProductDetailList()
        {
            var list = new List<Drop_ProductDetail>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

                using (var connection = new SqlConnection(connectionstring))
                {
                    string spName = "sp_product_details_ins_upd_del";

                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "productdetail_mastlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add(new Drop_ProductDetail
                                {
                                    Product_detail_id = reader.GetInt64(0)
                                });
                            }
                        }
                    }
                }

                return list;
            }

       
    }
}