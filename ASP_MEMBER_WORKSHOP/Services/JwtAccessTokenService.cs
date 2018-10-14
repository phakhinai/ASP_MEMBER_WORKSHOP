using ASP_MEMBER_WORKSHOP.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jose;
using System.Text;
using ASP_MEMBER_WORKSHOP.Entity;

namespace ASP_MEMBER_WORKSHOP.Services
{
    public class JwtAccessTokenService : IAccessTokenService
    {
        private byte[] secretKey = Encoding.UTF8.GetBytes("C# ASP.NET MEMBER WORKSHOP");
        private DbEntities db;

        public JwtAccessTokenService()
        {
            this.db = new DbEntities();
        }

        public string GenerateAccessToken(string email, int minute = 60)
        {
            JwtPayload payload = new JwtPayload
            {
                email = email,
                exp = DateTime.UtcNow.AddMinutes(minute)
            };
            return JWT.Encode(payload, this.secretKey, JwsAlgorithm.HS256);
        }

        public Member VerifyAccessToken(string accessToken)
        {
            try
            {
                JwtPayload payload = JWT.Decode<JwtPayload>(accessToken, this.secretKey);
                if (payload == null) return null;
                if (payload.exp < DateTime.UtcNow) return null;
                return this.db.Members.SingleOrDefault(item => item.email.Equals(payload.email));
            }
            catch
            {
                return null;
            }
        }
    }

    public class JwtPayload
    {
        public string email { get; set; }
        public DateTime exp { get; set; }
    }
}