using ASP_MEMBER_WORKSHOP.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jose;
using System.Text;

namespace ASP_MEMBER_WORKSHOP.Services
{
    public class JwtAccessTokenService : IAccessTokenService
    {
        private byte[] secretKey = Encoding.UTF8.GetBytes("C# ASP.NET MEMBER WORKSHOP");

        public string GenerateAccessToken(string email, int minute = 60)
        {
            JwtPayload payload = new JwtPayload
            {
                email = email,
                exp = DateTime.UtcNow.AddMinutes(minute)
            };
            return JWT.Encode(payload, this.secretKey, JwsAlgorithm.HS256);
        }
    }

    public class JwtPayload
    {
        public string email { get; set; }
        public DateTime exp { get; set; }
    }
}