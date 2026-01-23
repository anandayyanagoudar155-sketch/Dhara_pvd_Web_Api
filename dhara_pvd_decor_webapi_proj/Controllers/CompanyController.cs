using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using dhara_pvd_decor_webapi_proj.Services;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        private readonly ICompanyService _companyService;

        public CompanyController(
            IConfiguration configuration,
            IDistributedCache cache,
            ICompanyService companyService)
        {
            _configuration = configuration;
            _cache = cache;
            _companyService = companyService;
        }


        [HttpPost("InsertCompany")]
        public async Task<IActionResult> AddCompany([FromBody] AddCompanyRequest request)
        {
            try
            {
                bool result = await _companyService.AddCompany(request);

                if (result)
                    return Ok(new { message = "Company Added successfully." });

                return StatusCode(500, new { errorMessage = "Failed to add Company." });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") ||
                    ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { errorMessage = "Company name already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("Deletecompany/{id}")]
        public async Task<IActionResult> Deletecompany(long id)
        {
            try
            {
                bool result = await _companyService.DeleteCompany(id);

                if (result)
                    return Ok(new { message = "company deleted successfully." });

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


        [HttpPost("Updatecompany")]
        public async Task<IActionResult> Updatecompany([FromBody] UpdateCompanyRequest request)
        {
            try
            {
                bool result = await _companyService.UpdateCompany(request);

                if (!result)
                    return NotFound($"company with ID {request.Comp_id} not found");

                return Ok(new { message = "company updated successfully." });
            }
            catch (SqlException ex)
            {

                return BadRequest(new { errorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("company_list")]
        public async Task<ActionResult<IEnumerable<company_list>>> Get_CompanyList()
        {
            try
            {
                var list = await _companyService.GetCompanyList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("company/{id}")]
        public async Task<ActionResult<single_company_list>> Gete_Company_by_id(long id)
        {
            try
            {
                var company = await _companyService.GetCompanyById(id);

                if (company == null)
                    return NotFound($"company with ID {id} not found");

                return Ok(company);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("dropdown_company_list")]
        public async Task<ActionResult<IEnumerable<drop_company_list>>> Get_drop_companylist(long userId = 0)
        {
            try
            {
                var list = await _companyService.GetDropCompanyList(userId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        public class AddCompanyRequest
        {

            public long Comp_id { get; set; } = 0;
            public string Comp_code { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Comp_short_name { get; set; } = "";
            public string Comp_type { get; set; } = "";
            public string Comp_desc { get; set; } = "";
            public string Cin_number { get; set; } = "";
            public string Gst_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Contperson_name { get; set; } = "";
            public string Contact_email { get; set; } = "";
            public string Contact_phone { get; set; } = "";
            public string Address_line1 { get; set; } = "";
            public string Address_line2 { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Pincode { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public string Logo_path { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;

        }


        public class UpdateCompanyRequest
        {

            public long Comp_id { get; set; } = 0;
            public string Comp_code { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Comp_short_name { get; set; } = "";
            public string Comp_type { get; set; } = "";
            public string Comp_desc { get; set; } = "";
            public string Cin_number { get; set; } = "";
            public string Gst_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Contperson_name { get; set; } = "";
            public string Contact_email { get; set; } = "";
            public string Contact_phone { get; set; } = "";
            public string Address_line1 { get; set; } = "";
            public string Address_line2 { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Pincode { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public string Logo_path { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;

        }


        public class company_list
        {
            public long Comp_id { get; set; } = 0;
            public string Comp_code { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Comp_short_name { get; set; } = "";
            public string Comp_type { get; set; } = "";
            public string Comp_desc { get; set; } = "";
            public string Cin_number { get; set; } = "";
            public string Gst_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Contperson_name { get; set; } = "";
            public string Contact_email { get; set; } = "";
            public string Contact_phone { get; set; } = "";
            public string Address_line1 { get; set; } = "";
            public string Address_line2 { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string City_name { get; set; } = "";
            public string Pincode { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string Logo_path { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";

        }


        public class single_company_list
        {

            public long Comp_id { get; set; } = 0;
            public string Comp_code { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Comp_short_name { get; set; } = "";
            public string Comp_type { get; set; } = "";
            public string Comp_desc { get; set; } = "";
            public string Cin_number { get; set; } = "";
            public string Gst_number { get; set; } = "";
            public string Pan_number { get; set; } = "";
            public string Contperson_name { get; set; } = "";
            public string Contact_email { get; set; } = "";
            public string Contact_phone { get; set; } = "";
            public string Address_line1 { get; set; } = "";
            public string Address_line2 { get; set; } = "";
            public long City_id { get; set; } = 0;
            public string Pincode { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public string Logo_path { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;

        }


        public class drop_company_list
        {
            public long Comp_id { get; set; } = 0;
            public string Comp_name { get; set; } = "";

        }


    }
}
