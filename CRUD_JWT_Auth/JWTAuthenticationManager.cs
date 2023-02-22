using CRUD_JWT_Auth.DataSource;
using CRUD_WO_ORM.DataSource;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CRUD_JWT_Auth
{
    public class JWTAuthenticationManager : IJWTAuthenticationManager
    {
       
        private readonly IDataSource _dataSource;
        private readonly string key;
        
        public JWTAuthenticationManager(string key, IDataSource dataSource)
        {
            this._dataSource= dataSource;
            this.key = key;
        }
        
        public string? Authenticate(string username, string password, out string? name)
        {
            NpgsqlCommand command =_dataSource.source.CreateCommand($"Select * from users where username='{username}' and password='{password}'");
            NpgsqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                name = null;
                return null;
            }
            name = Convert.ToString(reader["name"]);
            var role = Convert.ToString(reader["role"]);
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Expires = DateTime.Now.AddMinutes(1),
                Claims = new Dictionary<string,object?>(){ { "username", username }, { "role", role } },
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
