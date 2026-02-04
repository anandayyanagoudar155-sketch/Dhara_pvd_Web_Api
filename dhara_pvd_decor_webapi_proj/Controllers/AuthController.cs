using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Caching.Memory;
using System;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using dhara_pvd_decor_webapi_proj.Services;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult Health()
        {
            var result =
                _authService.GetHealth(HttpContext.Connection.LocalPort);

            return Ok(result);
        }


        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterUser( [FromBody] RegisterRequest request)
        {
            try
            {
                var rows = await _authService.RegisterUser(request);

                if (rows > 0)
                    return Ok(new { message = "User registered successfully." });

                return StatusCode(500,
                    new { errorMessage = "Failed to register user." });
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50000 || ex.Message.Contains("exists"))
                {
                    return BadRequest(new { errorMessage = "Username already exists." });
                }

                return StatusCode(500, new { errorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = ex.Message });
            }
        }


        [HttpPost("validatelogin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidateLogin([FromBody] UservalidateLoginRequest request)
        {
            try
            {
                var user = await _authService.ValidateLogin(request);

                if (user == null)
                    return BadRequest(
                        new { message = "Invalid Email Or Password." });

                return Ok(new
                {
                    user_id = user.user_id,
                    user_name = user.user_name
                });
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


        [HttpPost("savelogin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SaveLogin([FromBody] UsersaveLoginRequest request)
        {
            try
            {
                var result = await _authService.SaveLogin(request);

                if (result == null)
                    return BadRequest(
                        new { message = "Invalid Email Or Password." });

                return Ok(new
                {
                    token = result.Token,
                    user_id = result.UserId,
                    user_name = result.UserName,
                    user_role = result.UserRole,
                    comp_id = result.CompId,
                    comp_name = result.CompName,
                    fin_year_id = result.FinYearId,
                    fin_name = result.FinName,
                    year_start = result.YearStart,
                    year_end = result.YearEnd
                });
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


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var jti = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)
                ?.Value;

            if (string.IsNullOrEmpty(jti))
                return Unauthorized("Invalid token");

            await _authService.Logout(jti);

            return Ok(new
            {
                message = "Logged out successfully"
            });
        }


        public class RegisterRequest
        {
            public string User_name { get; set; }
            public string User_password { get; set; }
            public string User_role { get; set; }
            public string Comp_ids { get; set; }
            public string Finyear_ids { get; set; }
        }

        public class UservalidateLoginRequest
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";

        }

        public class UsersaveLoginRequest
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
            public long comp_id { get; set; } = 0;
            public long fin_year_id { get; set; } = 0;

        }

        public class User
        {
            public long user_id { get; set; } = 0;
            public string user_name { get; set; } = "";

            public string user_password { get; set; } = "";
            public string user_role { get; set; } = "";
            public long comp_id { get; set; } = 0;
            public string comp_name { get; set; } = "";
            public long fin_year_id { get; set; } = 0;
            public string fin_name { get; set; } = "";
            public DateTime? year_start { get; set; } = null;
            public DateTime? year_end { get; set; } = null;

        }

        public class SendOtpRequest
        {
            public string Email { get; set; }
        }


        public class SendOtpResponse
        {
            public string Email { get; set; }
            public string Message { get; set; }
            public bool IsSent { get; set; }
        }


    }
}
