using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authorization;
using dhara_pvd_decor_webapi_proj.Services;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : Controller
    {
        private readonly IBrandService _service;
        private readonly IDistributedCache _cache;

        public BrandController(IBrandService service, IDistributedCache cache)
        {
            _service = service;
            _cache = cache;
        }

        [HttpPost("insert_brand")]
        public async Task<IActionResult> AddBrand([FromBody] AddBrandRequest request)
        {
            try
            {
                int rows = await _service.AddBrand(request);

                if (rows > 0)
                    return Ok(new { message = "Brand added successfully." });

                return StatusCode(500, new { errorMessage = "Failed to add brand." });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "Brand name already exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }

        [HttpDelete("delete_brand/{id}")]
        public async Task<IActionResult> DeleteBrand(long id)
        {
            try
            {
                int rows = await _service.DeleteBrand(id);

                if (rows > 0)
                    return Ok(new { message = "Brand deleted successfully." });

                return StatusCode(500, new { errorMessage = "No record deleted." });
            }
            catch (SqlException ex)
            {

                return BadRequest(new { errorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpPost("update_brand")]
        public async Task<IActionResult> UpdateBrand([FromBody] UpdateBrandRequest request)
        {
            try
            {
                int rows = await _service.UpdateBrand(request);

                if (rows == 0)
                    return NotFound($"Brand with ID {request.Brand_Id} not found");

                return Ok(new { message = "Brand updated successfully." });
            }
            catch (SqlException ex)
            {

                return BadRequest(new { errorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpGet("brand_list")]
        public async Task<IActionResult> GetBrandList()
        {
            try
            {
                return Ok(await _service.GetBrandList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpGet("brand/{id}")]
        public async Task<IActionResult> GetBrandById(long id)
        {
            try
            {
                var data = await _service.GetBrandById(id);

                if (data == null)
                    return NotFound($"Brand with Id {id} not found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }

        [HttpGet("dropdown_brand_list")]
        public async Task<IActionResult> GetDropBrandList()
        {
            try
            {
                return Ok(await _service.GetDropBrandList());
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
        public long Created_by { get; set; } = 0;
        public long Modified_by { get; set; } = 0;
    }


    public class UpdateBrandRequest
    {
        public long Brand_Id { get; set; } = 0;
        public string Brand_Name { get; set; } = "";
        public string Brand_Desc { get; set; } = "";
        public DateTime Created_date { get; set; }
        public DateTime Updated_date { get; set; }
        public long Created_by { get; set; } = 0;
        public long Modified_by { get; set; } = 0;
    }

    public class Brand_list
    {
        public long Brand_Id { get; set; } = 0;
        public string Brand_Name { get; set; } = "";
        public string Brand_Desc { get; set; } = "";
        public string Created_date { get; set; } = "";
        public string Updated_date { get; set; } = "";
        public long Created_by { get; set; } = 0;
        public long? Modified_by { get; set; } = 0;
        public string Created_by_name { get; set; } = "";
        public string? Modified_by_name { get; set; } = "";

    }


    public class Single_Brand_list
    {
        public long Brand_Id { get; set; } = 0;
        public string Brand_Name { get; set; } = "";
        public string Brand_Desc { get; set; } = "";
        public DateTime? Created_date { get; set; }
        public DateTime? Updated_date { get; set; }
        public long Created_by { get; set; } = 0;
        public long? Modified_by { get; set; } = 0;

    }


    public class drop_Brand_list
    {
        public long Brand_Id { get; set; } = 0;
        public string Brand_Name { get; set; } = "";

    }
    }
}
