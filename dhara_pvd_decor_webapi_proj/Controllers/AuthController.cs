using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace dhara_pvd_decor_webapi_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public AuthController(IConfiguration configuration, IMemoryCache cache)
        {

            _configuration = configuration;
            _cache = cache;

        }



        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_user_register", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@action", "register");
                        command.Parameters.AddWithValue("@user_name", request.User_name);
                        command.Parameters.AddWithValue("@user_password", request.User_password);
                        command.Parameters.AddWithValue("@user_role", request.User_role);
                        command.Parameters.AddWithValue("@comp_id", request.Comp_id);
                        command.Parameters.AddWithValue("@finyear_id", request.Finyear_id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "User registered successfully." });
                        }

                        return StatusCode(500, new { errorMessage = "Failed to register user." });
                    }
                }

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



        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {

            var connectionstring = _configuration.GetConnectionString("DefaultConnection");

            try
            {

                using (var connection = new SqlConnection(connectionstring))
                {

                    string spName = "sp_user_register";

                    var parameters = new DynamicParameters();
                    parameters.Add("@action", "login");
                    parameters.Add("@user_name", request.Email);
                    parameters.Add("@user_password", request.Password);
                    parameters.Add("@comp_id", request.comp_id);
                    parameters.Add("@finyear_id", request.fin_year_id);


                    var user = await connection.QueryFirstOrDefaultAsync<User>(
                    spName,
                    parameters,
                    commandType: CommandType.StoredProcedure
                    );

                    if (user != null)
                    {
                        return Ok(new
                        {
                            user_id = user.user_id,
                            user_name = user.user_name,
                            user_role = user.user_role,
                            comp_id = user.comp_id,
                            comp_name = user.comp_name,
                            fin_year_id = user.fin_year_id,
                            fin_name = user.fin_name,
                            year_start = user.year_start,
                            year_end = user.year_end
                        });
                    }
                    else
                    {
                        return BadRequest(new { message = "Invalid Email Or Password." });
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




        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { message = "Email is required." });

            try
            {
                // Generate 6-digit OTP
                var otp = new Random().Next(100000, 999999).ToString();

                // Store OTP in memory for 5 minutes
                _cache.Set($"OTP_{request.Email}", otp, TimeSpan.FromMinutes(5));

                // Send email
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("youremail@gmail.com", "your-app-password"),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("youremail@gmail.com"),
                    Subject = "Your OTP Code",
                    Body = $"Your OTP is: {otp}",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(request.Email);
                await smtpClient.SendMailAsync(mailMessage);

                return Ok(new
                {
                    message = "OTP sent successfully",
                    email = request.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        public class RegisterRequest
        {
            public string User_name { get; set; }
            public string User_password { get; set; }
            public string User_role { get; set; }
            public int Comp_id { get; set; }
            public int Finyear_id { get; set; }
        }

        public class UserLoginRequest
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
