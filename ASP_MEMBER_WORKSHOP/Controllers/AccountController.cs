using ASP_MEMBER_WORKSHOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ASP_MEMBER_WORKSHOP.Controllers
{
    public class AccountController : ApiController
    {

        // การลงทะเบียน
        [Route("api/account/register")]
        public IHttpActionResult PostRegister([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
                return Json(model);
            return BadRequest(ModelState);
        }
    }
}
