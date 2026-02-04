using Dapper;
using dhara_pvd_decor_webapi_proj.Controllers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        private readonly PasswordHasher<AuthController.User> _passwordHasher;
        private readonly IEmailService _emailService;

        public AuthService(
            IConfiguration configuration,
            IDistributedCache cache,
            IEmailService emailService
        )
        {
            _configuration = configuration;
            _cache = cache;
            _emailService = emailService;
            _passwordHasher = new PasswordHasher<AuthController.User>();
        }

        public async Task<int> RegisterUser(AuthController.RegisterRequest request)
        {
            var connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command =
                    new SqlCommand("sp_user_register", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var tempUser = new AuthController.User
                    {
                        user_name = request.User_name
                    };

                    string hashedPassword = _passwordHasher.HashPassword(tempUser, request.User_password);

                    command.Parameters.AddWithValue("@action", "register");
                    command.Parameters.AddWithValue("@user_name", request.User_name);
                    command.Parameters.AddWithValue("@user_password", hashedPassword);
                    command.Parameters.AddWithValue("@user_role", request.User_role);
                    command.Parameters.AddWithValue("@comp_ids", request.Comp_ids ?? "");
                    command.Parameters.AddWithValue("@finyear_ids", request.Finyear_ids ?? "");

                    //return await command.ExecuteNonQueryAsync();

                    int result = await command.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        await _emailService.SendWelcomeEmail(
                            request.User_name,
                            request.User_name,
                            request.User_password 
                        );
                    }

                    return result;

                }
            }
        }

        public async Task<AuthController.User?> ValidateLogin(AuthController.UservalidateLoginRequest request )
        {
            var connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@action", "verifylogin");
                parameters.Add("@user_name", request.Email);
                //parameters.Add("@user_password", request.Password);

                var user = await connection.QueryFirstOrDefaultAsync<AuthController.User>(
                    "sp_user_register",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (user == null)
                    return null;

                var result = _passwordHasher.VerifyHashedPassword(
                    user,
                    user.user_password,   // hash from DB
                    request.Password      // plain password from user
                );

                return result == PasswordVerificationResult.Success
                    ? user
                    : null;
            }
        }


        public async Task<AuthLoginResponse?> SaveLogin(
            AuthController.UsersaveLoginRequest request
        )
        {
            var connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@action", "savelogin");
                parameters.Add("@user_name", request.Email);
                parameters.Add("@user_password", request.Password);
                parameters.Add("@comp_ids", request.comp_id);
                parameters.Add("@finyear_ids", request.fin_year_id);

                var user = await connection.QueryFirstOrDefaultAsync<AuthController.User>(
                    "sp_user_register",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (user == null)
                    return null;

                var token = await GenerateJwtToken(user);

                return new AuthLoginResponse
                {
                    Token = token,
                    UserId = user.user_id,
                    UserName = user.user_name,
                    UserRole = user.user_role,
                    CompId = user.comp_id,
                    CompName = user.comp_name,
                    FinYearId = user.fin_year_id,
                    FinName = user.fin_name,
                    YearStart = user.year_start,
                    YearEnd = user.year_end
                };
            }
        }

        public async Task<int> Logout(string jti)
        {
            if (string.IsNullOrEmpty(jti))
                return 0;

            await _cache.RemoveAsync($"jwt:{jti}");
            return 1;
        }


        public HealthResponse GetHealth(int port)
        {
            return new HealthResponse
            {
                Status = "Healthy",
                Port = port,
                Server = Environment.MachineName,
                Time = DateTime.UtcNow
            };
        }


        private async Task<string> GenerateJwtToken(AuthController.User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var jti = Guid.NewGuid().ToString();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.user_id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.user_name),
                new Claim("role", user.user_role),
                new Claim("comp_id", user.comp_id.ToString()),
                new Claim("fin_year_id", user.fin_year_id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            );

            var creds =
                new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(jwtSettings["ExpireMinutes"])
                ),
                signingCredentials: creds
            );

            var tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            // STORE TOKEN IN REDIS
            await _cache.SetStringAsync(
                $"jwt:{jti}",
                tokenString,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(
                            Convert.ToDouble(jwtSettings["ExpireMinutes"])
                        )
                }
            );

            return tokenString;
        }
    }
}
