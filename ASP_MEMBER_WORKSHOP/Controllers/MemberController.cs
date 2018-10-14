using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ASP_MEMBER_WORKSHOP.Controllers
{
    public class MemberController : ApiController
    {
        [Route("api/member/data")]
        public IHttpActionResult GetMemberLogin()
        {
            return Json(new
            {
                isLogin = User.Identity.IsAuthenticated,
                emailLogin = User.Identity.Name
            });
        }
    }
}
