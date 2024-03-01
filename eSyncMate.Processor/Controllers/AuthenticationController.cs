using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Drawing;
using EdiEngine.Runtime;
using EdiEngine;
using JUST;
using eSyncMate.DB.Entities;
using System.Reflection;
using eSyncMate.Processor.Models;
using eSyncMate.Processor.Managers;
using System.Data;
using eSyncMate.DB;
using Nancy;
using Hangfire;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using AuthenticationandAuthorization.Controllers;
using System.Security.Claims;

namespace eSyncMate.Processor.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _config;

        public AuthenticationController(IConfiguration config)
        {
            _config = config;
        }

        private string GenerateJSONWebToken(LoginModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("mobile", user.Mobile),
                new Claim("email", user.Email),
                new Claim("status", user.Status.ToString()),
                new Claim("createdDate", user.CreatedDate.ToString()),
                new Claim("userType", user.UserType.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<LoginModel> AuthenticateUser(string email, string password)
        {
            LoginModel user = null;
            var l_DataTable = new DataTable();
            Users l_users = new Users();

            l_users.UseConnection(CommonUtils.ConnectionString);

            string criteria = $"Email = '{email}' AND Password = '{l_users.Encrypt(password)}' AND Status = 'ACTIVE'";

            if (l_users.GetViewList(criteria, "", ref l_DataTable))
            {
                if (l_DataTable.Rows.Count > 0)
                {
                    DataRow row = l_DataTable.Rows[0];
                    user = new LoginModel
                    {
                        Id = int.Parse(row["Id"].ToString()),
                        FirstName = row["FirstName"].ToString(),
                        LastName = row["LastName"].ToString(),
                        Mobile = row["Mobile"].ToString(),
                        Email = row["Email"].ToString(),
                        Status = row["Status"].ToString(),
                        CreatedDate = DateTime.Parse(row["createdDate"].ToString()), 
                        UserType = row["UserType"].ToString(),                                
                    };
                }
            }

            return user;
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            IActionResult response = Unauthorized();
            var user = await AuthenticateUser(email, password);
           
            if (user !=null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { Token = tokenString, Message = "Success" });
            }
            else
            {
                response = Ok(new { Token = "Invalid", Message = "Either you dont have permission or Invalid Credentials!" });
            }
            return response;
        }

        [HttpPost]
        [Route("Get")]
        public async Task<IEnumerable<string>> Get()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            return new string[] { accessToken };
        }
    }
}
