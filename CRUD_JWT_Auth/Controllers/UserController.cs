using CRUD_JWT_Auth.Models;
using CRUD_WO_ORM.DataSource;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Xml.Linq;

namespace CRUD_JWT_Auth.Controllers
{
    public class UserController : Controller
    {
        private readonly DataSource _dataSource;
        private readonly IJWTAuthenticationManager jwtAuthenticationManager;
        public UserController(IJWTAuthenticationManager jwtAuthenticationManager)
        {
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            _dataSource = new DataSource();
        }

        [Authorize]
        [HttpGet("get-all-users")]
        public ActionResult GetAllUsers()
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(Request.Headers["Authorization"][0].Split(" ")[1]);
            var tokenclaims = jwt.Claims.ToList();
            var username = tokenclaims[0].Value;
            var role = tokenclaims[1].Value;
            if (role != "admin") return Unauthorized();
            NpgsqlCommand command = _dataSource.source.CreateCommand($"Select * from users");
            NpgsqlDataReader reader = command.ExecuteReader();
            List<User> users = new List<User>();

            while (reader.Read())
            {
                users.Add(new User() { Name = (string)reader["name"], Age = (int)reader["age"], Username = (string)reader["username"], Password = (string)reader["password"], Role = (string)reader["role"] });
            
            }
            List<Dictionary<string, object>> lst = new List<Dictionary<string,object>>();
            foreach(User u in users) 
            {

                lst.Add(new Dictionary<string, object> { { "Name", u.Name }, { "Age", u.Age }, { "Username", u.Username } });
            }
            return Ok(lst);
        }

        [Authorize]
        [HttpGet("me")]
        public ActionResult GetUser()
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(Request.Headers["Authorization"][0].Split(" ")[1]);
            var tokenClaims = jwt.Claims.ToList();
            var username = tokenClaims[0].Value;
            
            NpgsqlCommand command = _dataSource.source.CreateCommand($"Select * from users where username='{username}'");
            NpgsqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return NotFound();
            }
            return Ok(new User() { Name = (string)reader["name"], Age = (int)reader["age"], Username = (string)reader["username"], Password = (string)reader["password"], Role = (string)reader["role"] });

        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] UserCred userCred)
        {
            var token = jwtAuthenticationManager.Authenticate(userCred.username, userCred.password, out string? name);
            
            if(token == null)
            {
                return NotFound();
            }
            return Ok(new Dictionary<string, string?>() { { "user", name}, { "token", token } });
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public IActionResult SignUp([FromBody] User user) 
        {
            UserCred userCred = new UserCred();
            userCred.username = user.Username;
            userCred.password = user.Password;

            if (ModelState.IsValid)
            {
                var existing = _dataSource.source.CreateCommand($"Select * from users where username='{userCred.username}'").ExecuteReader();
                if (existing.Read())
                {
                    return BadRequest("Username Already Exists!");
                }
                NpgsqlCommand cmd = _dataSource.source.CreateCommand($"Insert into users(name,age,username,password,role) values('{user.Name}',{user.Age},'{user.Username}','{user.Password}','{user.Role}')");
                cmd.ExecuteNonQuery();
                var token = jwtAuthenticationManager.Authenticate(userCred.username,userCred.password, out String? name);
                return Ok(new Dictionary<string, string?>() { {"user",name},{ "token", token } });
            }
            return BadRequest();
        }

    }
}
