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
    public class UserController : Controller
    {
        private readonly IUserServices _service;
        private readonly IDistributedCache _cache;

        public UserController(IUserServices service, IDistributedCache cache)
        {
            _service = service;
            _cache = cache;
        }



        [HttpPost("insert_user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequest request)
        {
   
            try
            {
                int rows = await _service.AddUser(request);

                if (rows > 0)
                            return Ok(new { message = "User added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add User." });

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { errorMessage = "User name already exists." });

                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(long id)
        {
  
            try
            {
                int rows = await _service.DeleteUser(id);

                if (rows > 0)
                            return Ok(new { message = "User deleted successfully." });
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


        [HttpPost("update_user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
     
            try
            {
                int rows = await _service.UpdateUser(request);

                if (rows == 0)
                    return NotFound($"User with ID {request.User_id} not found");
                else
                    return Ok(new { message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }


        [HttpGet("user_list")]
        public async Task<ActionResult<IEnumerable<User_List>>> GetUserList()
        {
     
            try
            {
                return Ok(await _service.GetUserList());

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("user/{id}")]
        public async Task<ActionResult<SingleUser>> GetUserById(long id)
        {
      
            try
            {
                var user = await _service.GetUserById(id);

                if (user == null)
                    return NotFound($"User with ID {id} not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("dropdown_user_list")]
        public async Task<ActionResult<IEnumerable<Drop_User_List>>> GetDropdownUserList()
        {
   
            try
            {
                return Ok(await _service.GetDropdownUserList());

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpPost("insert_userdetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddUserDetails([FromBody] AddUserDetailsRequest request)
        {
  
            try
            {
                int rows = await _service.AddUserDetails(request);

                if (rows > 0)
                            return Ok(new { message = "User Details added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add User details." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpPost("insert_multipleuserdetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddMultipleUserDetails([FromBody] AddUserDetailsRequest request)
        {
    
            try
            {
                int rows = await _service.AddMultipleUserDetails(request);

                if (rows > 0)
                            return Ok(new { message = "Compnies and Finyears added successfully." });
                        else
                            return StatusCode(500, new { errorMessage = "Failed to add Compnies and Finyears." });

            }
            catch (SqlException sqlEx)
            {
                return BadRequest(new { errorMessage = sqlEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpDelete("delete_userdetails/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserDetails(long id)
        {
     
            try
            {
                int rows = await _service.DeleteUserDetails(id);

                if (rows > 0)
                            return Ok(new { message = "User Details deleted successfully." });
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




        [HttpPost("update_userdetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUserDetails([FromBody] UpdateUserDetailsRequest request)
        {
  
            try
            {
                int rows = await _service.UpdateUserDetails(request);


                if (rows == 0)
                    return NotFound($"User Details with ID {request.User_details_id} not found");
                else
                    return Ok(new { message = "User Details updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("userdetails_list")]
        public async Task<ActionResult<IEnumerable<UserDetails_List>>> Get_userdetails_list()
        {
  
            try
            {
                return Ok(await _service.GetUserDetailsList());

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("multiple_userdetails/{id}")]
        public async Task<ActionResult<List<Multiple_UserDetails_List>>> Get_multiple_userdetails_by_id(long id)
        {
 
            try
            {
                return Ok(await _service.GetMultipleUserDetailsByUserId(id)); 

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        public class AddUserRequest
        {
            public long User_id { get; set; } = 0;
            public string User_name { get; set; } = "";
            public string User_password { get; set; } = "";
            public string User_role { get; set; } = "";
            public bool Is_login { get; set; } = false;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }

        }

        public class UpdateUserRequest
        {
            public long User_id { get; set; } = 0;
            public string User_name { get; set; } = "";
            public string User_password { get; set; } = "";
            public string User_role { get; set; } = "";
            public bool Is_login { get; set; } = false;
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
        }

        public class User_List
        {
            public long User_id { get; set; } = 0;
            public string User_name { get; set; } = "";
            public string User_password { get; set; } = "";
            public string User_role { get; set; } = "";
            public bool Is_login { get; set; } = false;
            public string Created_Date { get; set; } = "";
            public string Updated_Date { get; set; } = "";
        }

        public class SingleUser
        {
            public long User_id { get; set; } = 0;
            public string User_name { get; set; } = "";
            public string User_password { get; set; } = "";
            public string User_role { get; set; } = "";
            public bool Is_login { get; set; } = false;
            public DateTime? Created_Date { get; set; }
            public DateTime? Updated_Date { get; set; }
        }


        public class Drop_User_List
        {
            public long User_id { get; set; } = 0;
            public string User_name { get; set; } = "";
        }

        public class AddUserDetailsRequest
        {
            public long User_details_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
            public string Comp_id { get; set; } = "";
            public string Fin_year_id { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Modified_by { get; set; } = 0;
        }



        public class UpdateUserDetailsRequest
        {
            public long User_details_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public bool Is_active { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Modified_by { get; set; } = 0;
        }


        public class UserDetails_List
        {
            public long User_details_id { get; set; } = 0;
            public string User_name { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Fin_year_name { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public string Created_date { get; set; } = "";
            public string Updated_date { get; set; } = "";
            public string Modified_by { get; set; } = "";
        }



        public class Single_UserDetails_List
        {
            public long User_details_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
            public long Comp_id { get; set; } = 0;
            public long Fin_year_id { get; set; } = 0;
            public bool Is_active { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Modified_by { get; set; } = 0;
        }

        public class Multiple_UserDetails_List
        {
            public long User_details_id { get; set; } = 0;
            public long User_id { get; set; } = 0;
            public string Comp_id { get; set; } = "";
            public string Comp_name { get; set; } = "";
            public string Fin_year_id { get; set; } = "";
            public string Fin_year_name { get; set; } = "";
            public bool Is_active { get; set; } = false;
            public DateTime? Created_date { get; set; }
            public DateTime? Updated_date { get; set; }
            public long Modified_by { get; set; } = 0;
        }



    }
}
