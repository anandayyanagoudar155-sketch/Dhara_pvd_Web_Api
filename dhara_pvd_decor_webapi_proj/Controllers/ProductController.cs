using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {

            _configuration = configuration;

        }


        [HttpPost("insert_product")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Add_Product([FromBody] AddProductRequest request)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
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
                        command.Parameters.AddWithValue("@opening_stock", request.Opening_stock);
                        command.Parameters.AddWithValue("@purchase", request.Purchase);
                        command.Parameters.AddWithValue("@sales", request.Sales);
                        command.Parameters.AddWithValue("@return", request.Return);
                        command.Parameters.AddWithValue("@current_stock", request.Current_stock);
                        command.Parameters.AddWithValue("@reorder_threshold", request.Reorder_threshold);
                        command.Parameters.AddWithValue("@reorder_desc", request.Reorder_desc);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Product Added successfully." });
                        }
                        else
                        {
                            return StatusCode(500, new { errorMessage = "Failed to add Product." });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Product name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpDelete("DeleteProduct/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_product_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@product_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Product deleted successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "No record deleted." });
                    }
                }
            }
            catch (SqlException ex)
            {

                return BadRequest(new { errorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }



        [HttpPost("UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateProduct([FromBody] UpdateProductRequest request)
        {
            int rows_affected;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
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
                    parameters.Add("@opening_stock", request.Opening_stock);
                    parameters.Add("@purchase", request.Purchase);
                    parameters.Add("@sales", request.Sales);
                    parameters.Add("@return", request.Return);
                    parameters.Add("@current_stock", request.Current_stock);
                    parameters.Add("@reorder_threshold", request.Reorder_threshold);
                    parameters.Add("@reorder_desc", request.Reorder_desc);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_Id);

                    rows_affected = await connection.ExecuteAsync(
                        spname,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                }

                if (rows_affected == 0)
                    return NotFound($"Product with ID {request.Product_Id} not found");
                else
                    return Ok(new { message = "Product updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("product_list")]
        public async Task<ActionResult<IEnumerable<Product_list>>> Get_product_list()
        {
            var product_list = new List<Product_list>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
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
                                var product = new Product_list
                                {
                                    Product_Id = reader.GetInt64(0),
                                    Prodtype_name = reader.GetString(1),
                                    Brand_name = reader.GetString(2),
                                    Hsn_name = reader.GetString(3),
                                    Unit_name = reader.GetString(4),
                                    Product_name = reader.GetString(5),
                                    Product_desc = reader.GetString(6),
                                    Rate = reader.GetDecimal(7),
                                    Opening_stock = reader.GetDecimal(8),
                                    Purchase = reader.GetDecimal(9),
                                    Sales = reader.GetDecimal(10),
                                    Return = reader.GetDecimal(11),
                                    Current_stock = reader.GetDecimal(12),
                                    Reorder_threshold = reader.GetDecimal(13),
                                    Reorder_desc = reader.GetString(14),
                                    Created_Date = reader.GetDateTime(15).ToString("yyyy-MM-dd"),
                                    Updated_Date = reader.IsDBNull(16) ? "" : reader.GetDateTime(16).ToString("yyyy-MM-dd"),
                                    User_Name = reader.IsDBNull(17) ? "" : reader.GetString(17),
                                };

                                product_list.Add(product);
                            }
                        }
                    }
                }

                return Ok(product_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("product/{id}")]
        public async Task<ActionResult<SingleProductList>> Get_product_by_id(long id)
        {
            SingleProductList? product = null;
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
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
                                    Opening_stock = reader.GetDecimal(8),
                                    Purchase = reader.GetDecimal(9),
                                    Sales = reader.GetDecimal(10),
                                    Return = reader.GetDecimal(11),
                                    Current_stock = reader.GetDecimal(12),
                                    Reorder_threshold = reader.GetDecimal(13),
                                    Reorder_desc = reader.GetString(14),
                                    Created_Date = reader.IsDBNull(15) ? (DateTime?)null : reader.GetDateTime(15),
                                    Updated_Date = reader.IsDBNull(16) ? (DateTime?)null : reader.GetDateTime(16),
                                    User_Id = reader.IsDBNull(17) ? 0 : reader.GetInt64(17),
                                };
                            }
                        }
                    }
                }

                if (product == null)
                    return NotFound($"Product with ID {id} not found");

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("dropdown_product_list")]
        public async Task<ActionResult<IEnumerable<Drop_Product_List>>> Get_drop_productlist()
        {
            var product_list = new List<Drop_Product_List>();
            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {
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
                                var product = new Drop_Product_List
                                {
                                    Product_Id = reader.GetInt64(0),
                                    Product_name = reader.GetString(1)
                                };

                                product_list.Add(product);
                            }
                        }
                    }
                }

                return Ok(product_list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        public class AddProductRequest
        {

            public long Product_Id { get; set; } = 0;
            public long Prodtype_id { get; set; } = 0;
            public long Brand_id { get; set; } = 0;
            public long Hsn_id { get; set; } = 0;
            public long Unit_id { get; set; } = 0;
            public string Product_name { get; set; } = "";
            public string Product_desc { get; set; } = "";
            public decimal Rate { get; set; } = 0;
            public decimal Opening_stock { get; set; } = 0;
            public decimal Purchase { get; set; } = 0;
            public decimal Sales { get; set; } = 0;
            public decimal Return { get; set; } = 0;
            public decimal Current_stock { get; set; } = 0;
            public decimal Reorder_threshold { get; set; } = 0;
            public string Reorder_desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_Id { get; set; } = 0;

        }


        public class UpdateProductRequest
        {
            public long Product_Id { get; set; } = 0;
            public long Prodtype_id { get; set; } = 0;
            public long Brand_id { get; set; } = 0;
            public long Hsn_id { get; set; } = 0;
            public long Unit_id { get; set; } = 0;
            public string Product_name { get; set; } = "";
            public string Product_desc { get; set; } = "";
            public decimal Rate { get; set; } = 0;
            public decimal Opening_stock { get; set; } = 0;
            public decimal Purchase { get; set; } = 0;
            public decimal Sales { get; set; } = 0;
            public decimal Return { get; set; } = 0;
            public decimal Current_stock { get; set; } = 0;
            public decimal Reorder_threshold { get; set; } = 0;
            public string Reorder_desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_Id { get; set; } = 0;

        }

        public class Product_list
        {

            public long Product_Id { get; set; } = 0;
            public string Prodtype_name { get; set; } = "";
            public string Brand_name { get; set; } = "";
            public string Hsn_name { get; set; } = "";
            public string Unit_name { get; set; } = "";
            public string Product_name { get; set; } = "";
            public string Product_desc { get; set; } = "";
            public decimal Rate { get; set; } = 0;
            public decimal Opening_stock { get; set; } = 0;
            public decimal Purchase { get; set; } = 0;
            public decimal Sales { get; set; } = 0;
            public decimal Return { get; set; } = 0;
            public decimal Current_stock { get; set; } = 0;
            public decimal Reorder_threshold { get; set; } = 0;
            public string Reorder_desc { get; set; } = "";
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public string User_Name { get; set; } = "";
        }

        public class SingleProductList
        {
            public long Product_Id { get; set; } = 0;
            public long Prodtype_id { get; set; } = 0;
            public long Brand_id { get; set; } = 0;
            public long Hsn_id { get; set; } = 0;
            public long Unit_id { get; set; } = 0;
            public string Product_name { get; set; } = "";
            public string Product_desc { get; set; } = "";
            public decimal Rate { get; set; } = 0;
            public decimal Opening_stock { get; set; } = 0;
            public decimal Purchase { get; set; } = 0;
            public decimal Sales { get; set; } = 0;
            public decimal Return { get; set; } = 0;
            public decimal Current_stock { get; set; } = 0;
            public decimal Reorder_threshold { get; set; } = 0;
            public string Reorder_desc { get; set; } = "";
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long User_Id { get; set; } = 0;
        }


        public class Drop_Product_List
        {
            public long Product_Id { get; set; } = 0;
            public string Product_name { get; set; } = "";
        }
    }
}
