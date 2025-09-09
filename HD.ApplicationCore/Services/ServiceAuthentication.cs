using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HD.ApplicationCore.Services
{
    public class ServiceAuthentication : IServiceAuthentication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public ServiceAuthentication(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public AuthResult Authenticate(string username, string password)
        {
            // 1. Vérifier si c'est un client
            var client = _unitOfWork.Repository<Client>()
                .Get(c => c.clientName == username && c.Pwd == password);
            if (client != null)
            {
                var token = GenerateJwtToken("Client", client.clientId, username);
                return new AuthResult { Token = token, Role = "Client", UserId = client.clientId };
            }

            // 2. Vérifier si c'est un admin
            var admin = _unitOfWork.Repository<Admin>()
                .Get(a => a.Name == username && a.Pwd == password);
            if (admin != null)
            {
                var token = GenerateJwtToken("Admin", admin.AgentId, username);
                return new AuthResult { Token = token, Role = "Admin", UserId = admin.AgentId };
            }

            // 3. Vérifier si c'est un agent
            var agent = _unitOfWork.Repository<Agent>()
                .Get(a => a.Name == username && a.Pwd == password);
            if (agent != null)
            {
                var token = GenerateJwtToken("Agent", agent.AgentId, username);
                return new AuthResult { Token = token, Role = "Agent", UserId = agent.AgentId };
            }

            throw new UnauthorizedAccessException("Invalid credentials");
        }

        private string GenerateJwtToken(string role, int userId, string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("UserId", userId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
