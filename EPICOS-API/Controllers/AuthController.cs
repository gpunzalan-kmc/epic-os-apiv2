using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using EPICOS_API;
using EPICOS_API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EPICOS_API.Models;
using System.Linq;
using Managers;
using EPICOS_API.Managers;
using EPICOS_API.Models.Entities;
using EPICOS_API.Models.Wrappers;
using EPICOS_API.Repositories;

namespace EPICOS_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtAuthenticationManager jwtAuthenticationManager;
        private readonly IHttpCallManager httpCallManager;
        private IConfiguration configuration;
        public AuthController(IJwtAuthenticationManager jwtAuthenticationManager, IHttpCallManager httpCallManager)
        {
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            this.httpCallManager = httpCallManager;
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult CurrentUser()
        {
            var Id = User.Identity.Name;
            using (var context = new EpicOSContext())
            {
                var user = context.User.Where(e => e.ID == Int32.Parse(Id)).FirstOrDefault();
                if(user == null){
                    var response = new Response<User>();
                    response.Succeeded = false;
                    response.Message = "User not found";
                    return StatusCode(404, response);
                }
                return Ok(new {user = user});
            }
        }
                
        [HttpPost("login")]
        public IActionResult Authenticate([FromBody] UserCred userCred)
        {
            var user = new UserRepository();
            var userList =  user.ValidateUsername(userCred.Username);
            var response = new Response<User>();
            if (userList == null)
            {
                response.Succeeded = false;
                response.Message = "User not found";
                return StatusCode(404, response);
            }
            string url = "/external/ldap/auth";
            var credentials = new
            {
                username = userCred.Username,
                password = userCred.Password,
            };
            var payload = JsonSerializer.Serialize(credentials);
            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
            var t = Task.Run(() => this.httpCallManager.PostAuthURI(url, c));
            t.Wait();
      
            if (t.Result == false){
                response.Succeeded = false;
                response.Message = "Invalid username or password";
                return StatusCode(401, response);
            }
            var token = jwtAuthenticationManager.Authenticate(userList.ID);
            if (token == null){
                response.Succeeded = false;
                response.Message = "Something went wrong";
                return StatusCode(500, response);
            }
            return Ok(new { userToken = token, user = userList });
        }      
    }
}