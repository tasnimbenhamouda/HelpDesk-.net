using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Interfaces
{
    public interface IServiceAuthentication
    {
        AuthResult Authenticate(string username, string password);
    }
    public class AuthResult
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
    }
}
