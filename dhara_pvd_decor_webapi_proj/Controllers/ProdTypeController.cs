using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using dhara_pvd_decor_webapi_proj.Services;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProdTypeController : Controller
    {
        private readonly IProdTypeService _service;
        private readonly IDistributedCache _cache;

        public ProdTypeController(IProdTypeService service, IDistributedCache cache)
        {
            _service = service;
            _cache = cache;
        }

        [HttpPost("insert_prodtype")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddProdtype([FromBody] AddProdtypeRequest request)
        {

            try
            {
                int rows = await _service.AddProdtype(request);

                if (rows > 0)
                    return Ok(new { message = "Product type added successfully." });
                else
                    return StatusCode(500, new { errorMessage = "Failed to add product type." });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "Product type name already exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_prodtype/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProdtype(long id)
        {
            try
            {
                int rows = await _service.DeleteProdtype(id);

                if (rows > 0)
                    return Ok(new { message = "Product type deleted successfully." });
                else
                    return StatusCode(500, new { errorMessage = "No record deleted." });

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


        [HttpPost("update_prodtype")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateProdtype([FromBody] UpdateProdtypeRequest request)
        {
            try
            {
                int rows = await _service.UpdateProdtype(request);

                if (rows == 0)
                    return NotFound($"Product type with Id {request.Prodtype_Id} not found");
                else
                    return Ok(new { message = "Product type updated successfully." });
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


        [HttpGet("prodtype_list")]
        public async Task<ActionResult<IEnumerable<Prodtype_list>>> GetProdtypeList()
        {
            try
            {
                return Ok(await _service.GetProdtypeList());

            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        [HttpGet("prodtype/{id}")]
        public async Task<ActionResult<Single_Prodtype_list>> GetProdtypeById(long id)
        {
            try
            {
                var data = await _service.GetProdtypeById(id);

                if (data == null)
                    return NotFound($"Product type with Id {id} not found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        [HttpGet("dropdown_prodtype_list")]
        public async Task<ActionResult<IEnumerable<drop_Prodtype_list>>> GetDropProdtypeList()
        {
            try
            {
                return Ok(await _service.GetDropProdtypeList());

            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }



        public class AddProdtypeRequest
        {
            public long Prodtype_Id { get; set; } = 0;
            public string Prodtype_Name { get; set; } = "";
            public string Prodtype_Desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }


        public class UpdateProdtypeRequest
        {
            public long Prodtype_Id { get; set; } = 0;
            public string Prodtype_Name { get; set; } = "";
            public string Prodtype_Desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }

        public class Prodtype_list
        {
            public long Prodtype_Id { get; set; } = 0;
            public string Prodtype_Name { get; set; } = "";
            public string Prodtype_Desc { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";

        }


        public class Single_Prodtype_list
        {
            public long Prodtype_Id { get; set; } = 0;
            public string Prodtype_Name { get; set; } = "";
            public string Prodtype_Desc { get; set; } = "";
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;

        }


        public class drop_Prodtype_list
        {
            public long Prodtype_Id { get; set; } = 0;
            public string Prodtype_Name { get; set; } = "";

        }
    }
}
