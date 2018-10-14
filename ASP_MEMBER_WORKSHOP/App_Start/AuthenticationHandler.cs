using ASP_MEMBER_WORKSHOP.Entity;
using ASP_MEMBER_WORKSHOP.Interfaces;
using ASP_MEMBER_WORKSHOP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ASP_MEMBER_WORKSHOP
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private IAccessTokenService accessTokenService;

        public AuthenticationHandler()
        {

        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var Authorization = request.Headers.Authorization;
            if (Authorization != null)
            {
                string AccessToken = Authorization.Parameter;
                string AccessTokenType = Authorization.Scheme;
                if (AccessTokenType.Equals("Bearer"))
                {
                    this.accessTokenService = new JwtAccessTokenService();
                    var memberItem = this.accessTokenService.VerifyAccessToken(AccessToken);
                    if (memberItem != null)
                    {
                        var userLogin = new UserLogin(new GenericIdentity(memberItem.email), memberItem.role);
                        userLogin.member = memberItem;
                        Thread.CurrentPrincipal = userLogin;
                        HttpContext.Current.User = userLogin;
                    }
                }
            }
            return base.SendAsync(request, cancellationToken);
        }
    }

    public class UserLogin : GenericPrincipal
    {
        public Member member { get; set; }

        public UserLogin(IIdentity identity, RoleAccount roles) : base(identity, new string[] { roles.ToString() })
        {
        }
    }
}