using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : Controller
    {
        private readonly IConfiguration _configuration;

        public BrandController(IConfiguration configuration)
        {

            _configuration = configuration;

        }

        [HttpPost("insert_brand")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddBrand([FromBody] AddBrandRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_brand_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "insert");
                        command.Parameters.AddWithValue("@brand_id", request.Brand_Id);
                        command.Parameters.AddWithValue("@brand_name", request.Brand_Name);
                        command.Parameters.AddWithValue("@brand_desc", request.Brand_Desc);
                        command.Parameters.AddWithValue("@created_date", request.Created_date);
                        command.Parameters.AddWithValue("@user_id", request.User_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Brand added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add brand." });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "Brand name already exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_brand/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBrand(long id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_brand_mast_ins_upd_del", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "delete");
                        command.Parameters.AddWithValue("@brand_id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Brand deleted successfully." });
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


        [HttpPost("update_brand")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateBrand([FromBody] UpdateBrandRequest request)
        {
            int rowsAffected;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_brand_mast_ins_upd_del";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "update");
                    parameters.Add("@brand_id", request.Brand_Id);
                    parameters.Add("@brand_name", request.Brand_Name);
                    parameters.Add("@brand_desc", request.Brand_Desc);
                    parameters.Add("@created_date", request.Created_date);
                    parameters.Add("@updated_date", request.Updated_date);
                    parameters.Add("@user_id", request.User_id);

                    rowsAffected = await connection.ExecuteAsync(
                        spName,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }

                if (rowsAffected == 0)
                    return NotFound($"Brand with ID {request.Brand_Id} not found");
                else
                    return Ok(new { message = "Brand updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }



        [HttpGet("brand_list")]
        public async Task<ActionResult<IEnumerable<Brand_list>>> GetBrandList()
        {
            var brandList = new List<Brand_list>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_brand_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectall");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var brand = new Brand_list
                                {
                                    Brand_Id = reader.GetInt64(0),
                                    Brand_Name = reader.GetString(1),
                                    Brand_Desc = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Created_date = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    Updated_date = reader.IsDBNull(4) ? "" : reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    User_name = reader.IsDBNull(5) ? "" : reader.GetString(5)
                                };

                                brandList.Add(brand);
                            }
                        }
                    }
                }

                return Ok(brandList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }




        [HttpGet("brand/{id}")]
        public async Task<ActionResult<Single_Brand_list>> GetBrandById(long id)
        {
            Single_Brand_list? brand = null;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_brand_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "selectone");
                        command.Parameters.AddWithValue("@brand_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                brand = new Single_Brand_list
                                {
                                    Brand_Id = reader.GetInt64(0),
                                    Brand_Name = reader.GetString(1),
                                    Brand_Desc = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Created_date = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                                    Updated_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                    User_id = reader.IsDBNull(5) ? 0 : reader.GetInt64(5)
                                };
                            }
                        }
                    }
                }

                if (brand == null)
                    return NotFound($"Brand with Id {id} not found");

                return Ok(brand);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }




        [HttpGet("dropdown_brand_list")]
        public async Task<ActionResult<IEnumerable<drop_Brand_list>>> GetDropBrandList()
        {
            var brandList = new List<drop_Brand_list>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string spName = "sp_brand_mast_ins_upd_del";
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "brandlist");

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var brand = new drop_Brand_list
                                {
                                    Brand_Id = reader.GetInt64(0),
                                    Brand_Name = reader.GetString(1)
                                };

                                brandList.Add(brand);
                            }
                        }
                    }
                }

                return Ok(brandList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }



        public class AddBrandRequest
        {
            public long Brand_Id { get; set; } = 0;
            public string Brand_Name { get; set; } = "";
            public string Brand_Desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long? User_id { get; set; } = 0;
        }


        public class UpdateBrandRequest
        {
            public long Brand_Id { get; set; } = 0;
            public string Brand_Name { get; set; } = "";
            public string Brand_Desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long User_id { get; set; } = 0;
        }

        public class Brand_list
        {
            public long Brand_Id { get; set; } = 0;
            public string Brand_Name { get; set; } = "";
            public string Brand_Desc { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string User_name { get; set; } = "";

        }


        public class Single_Brand_list
        {
            public long Brand_Id { get; set; } = 0;
            public string Brand_Name { get; set; } = "";
            public string Brand_Desc { get; set; } = "";
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long User_id { get; set; } = 0;

        }


        public class drop_Brand_list
        {
            public long Brand_Id { get; set; } = 0;
            public string Brand_Name { get; set; } = "";

        }
    }
}
