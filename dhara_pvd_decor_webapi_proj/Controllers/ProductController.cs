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
    public class ProductController : Controller
    {
        private readonly IProductServices _service;
        private readonly IDistributedCache _cache;

        public ProductController(IProductServices service, IDistributedCache cache)
        {
            _service = service;
            _cache = cache;
        }


        [HttpPost("insert_product")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Add_Product([FromBody] AddProductRequest request)
        {
            try
            {
                int rows = await _service.Add_Product(request);

                if (rows > 0)
                {
                    return Ok(new { message = "Product Added successfully." });
                }
                else
                {
                    return StatusCode(500, new { errorMessage = "Failed to add Product." });
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
            try
            {
                int rows = await _service.DeleteProduct(id);

                if (rows > 0)
                    return Ok(new { message = "Product deleted successfully." });
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



        [HttpPost("UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateProduct([FromBody] UpdateProductRequest request)
        {

            try
            {
                int rows = await _service.UpdateProduct(request);

                if (rows == 0)
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
            try
            {
                return Ok(await _service.Get_product_list());

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("product/{id}")]
        public async Task<ActionResult<SingleProductList>> Get_product_by_id(long id)
        {
            try
            {
                var data = await _service.Get_product_by_id(id);

                if (data == null)
                    return NotFound($"Product with ID {id} not found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("dropdown_product_list")]
        public async Task<ActionResult<IEnumerable<Drop_Product_List>>> Get_drop_productlist()
        {
            try
            {
                return Ok(await _service.Get_drop_productlist());

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpPost("insert_ProductDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add_ProductDetail([FromBody] Add_ProductDetail_Request request)
        {

            try
            {
                int rows = await _service.Add_ProductDetail(request);

                if (rows > 0)
                    return Ok(new { message = "Product detail added successfully." });
                else
                    return StatusCode(500, new { errorMessage = "Failed to add product detail." });

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




        [HttpDelete("Delete_ProductDetail/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete_ProductDetail(long id)
        {
            try
            {
                int rows = await _service.Delete_ProductDetail(id);

                if (rows > 0)
                    return Ok(new { message = "Product detail deleted successfully." });
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



        [HttpPost("Update_ProductDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update_ProductDetail([FromBody] Update_ProductDetail_Request request)
        {
    
            try
            {
                int rows = await _service.Update_ProductDetail(request);

                if (rows == 0)
                    return NotFound($"Product detail with ID {request.Product_detail_id} not found");
                else
                    return Ok(new { message = "Product detail updated successfully." });
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



        [HttpGet("productdetail_list")]
        public async Task<ActionResult<IEnumerable<ProductDetail_List>>> Get_ProductDetail_List()
        {
       
            try
            {
                return Ok(await _service.Get_ProductDetail_List());

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("productdetail/{id}")]
        public async Task<ActionResult<List<Single_ProductDetail>>> Get_ProductDetail_By_Id(long id)
        {
           
            try
            {
                var data = await _service.Get_ProductDetail_By_Id(id);

                if (data.Count == 0)
                    return NotFound($"No product details found for product_id {id}");

                return Ok(data);

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        [HttpGet("dropdown_productdetail_list")]
        public async Task<ActionResult<IEnumerable<Drop_ProductDetail>>> Get_Drop_ProductDetailList()
        {
      
            try
            {
                return Ok(await _service.Get_Drop_ProductDetailList());

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
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;

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
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;

        }

        public class Product_list
        {

            public long Product_Id { get; set; } = 0;
            public long Prodtype_Id { get; set; } = 0;
            public string Prodtype_name { get; set; } = "";
            public long Brand_Id { get; set; } = 0;
            public string Brand_name { get; set; } = "";
            public long Hsn_Id { get; set; } = 0;
            public string Hsn_name { get; set; } = "";
            public long Unit_Id { get; set; } = 0;
            public string Unit_name { get; set; } = "";
            public string Product_name { get; set; } = "";
            public string Product_desc { get; set; } = "";
            public decimal Rate { get; set; } = 0;
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";
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
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
        }


        public class Drop_Product_List
        {
            public long Product_Id { get; set; } = 0;
            public string Product_name { get; set; } = "";
        }



        public class Add_ProductDetail_Request
        {
            public long Product_detail_id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public decimal Opening_stock { get; set; } = 0;
            public decimal Purchase { get; set; } = 0;
            public decimal Sales { get; set; } = 0;
            public decimal Return { get; set; } = 0;
            public decimal Current_stock { get; set; } = 0;
            public decimal reorder_threshold { get; set; } = 0;
            public string reorder_desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }




        public class Update_ProductDetail_Request
        {
            public long Product_detail_id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public decimal Opening_stock { get; set; } = 0;
            public decimal Purchase { get; set; } = 0;
            public decimal Sales { get; set; } = 0;
            public decimal Return { get; set; } = 0;
            public decimal Current_stock { get; set; } = 0;
            public decimal reorder_threshold { get; set; } = 0;
            public string reorder_desc { get; set; } = "";
            public DateTime Created_date { get; set; }
            public DateTime Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }



        public class ProductDetail_List
        {
            public long Product_detail_id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public string Product_name { get; set; } = "";
            public decimal Opening_stock { get; set; } = 0;
            public decimal Purchase { get; set; } = 0;
            public decimal Sales { get; set; } = 0;
            public decimal Return { get; set; } = 0;
            public decimal Current_stock { get; set; } = 0;
            public decimal reorder_threshold { get; set; } = 0;
            public string reorder_desc { get; set; } = "";
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public long Fin_year_id { get; set; } = 0;
            public string Fin_year_name { get; set; } = "";
            public long Comp_id { get; set; } = 0;
            public string Comp_name { get; set; } = "";
            public long Created_by { get; set; } = 0;
            public long? Modified_by { get; set; } = 0;
            public string Created_by_name { get; set; } = "";
            public string? Modified_by_name { get; set; } = "";
        }



        public class Single_ProductDetail
        {
            public long Product_detail_id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
            public decimal Opening_stock { get; set; } = 0;
            public decimal Purchase { get; set; } = 0;
            public decimal Sales { get; set; } = 0;
            public decimal Return { get; set; } = 0;
            public decimal Current_stock { get; set; } = 0;
            public decimal reorder_threshold { get; set; } = 0;
            public string reorder_desc { get; set; } = "";
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Fin_year_id { get; set; } = 0;
            public string Fin_year_name { get; set; } = "";
            public long Comp_id { get; set; } = 0;
            public long Created_by { get; set; } = 0;
            public long Modified_by { get; set; } = 0;
        }


        public class Drop_ProductDetail
        {
            public long Product_detail_id { get; set; } = 0;
            public long Product_Id { get; set; } = 0;
        }


    }
}
