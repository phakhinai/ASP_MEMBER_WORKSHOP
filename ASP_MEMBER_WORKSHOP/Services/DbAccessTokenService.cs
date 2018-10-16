using ASP_MEMBER_WORKSHOP.Entity;
using ASP_MEMBER_WORKSHOP.Interfaces;
using ASP_MEMBER_WORKSHOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASP_MEMBER_WORKSHOP.Services
{
    public class DbAccessTokenService : IAccessTokenService
    {
        private DbEntities db = new DbEntities();

        public string GenerateAccessToken(string email, int minute = 60)
        {
            try
            {
                var memberItem = this.db.Members.SingleOrDefault(m => m.email.Equals(email));
                if (memberItem == null) throw new Exception("Not found member.");
                var accessTokenCreate = new AccessToken
                {
                    token = Guid.NewGuid().ToString(),
                    exprise = DateTime.Now.AddMinutes(minute),
                    memberId = memberItem.id
                };
                this.db.AccessTokens.Add(accessTokenCreate);
                this.db.SaveChanges();
                return accessTokenCreate.token;
            }
            catch (Exception ex)
            {
                throw ex.GetErrorException();
            }
        }

        public Member VerifyAccessToken(string accessToken)
        {
            try
            {
                var accessTokenItem = this.db.AccessTokens.SingleOrDefault(item => item.token.Equals(accessToken));
                if (accessTokenItem == null) return null;
                if (accessTokenItem.exprise < DateTime.Now) return null;
                return accessTokenItem.Member;
            }
            catch
            {
                return null;
            }
        }
    }
}