using ASP_MEMBER_WORKSHOP.Interfaces;
using ASP_MEMBER_WORKSHOP.Models;
using ASP_MEMBER_WORKSHOP.Services;
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
        private IAccountService Account;
        private IAccessTokenService AccessToken;

        protected AccountController()
        {
            this.Account = new AccountService();
            this.AccessToken = new JwtAccessTokenService();
        }

        // การลงทะเบียน
        [Route("api/account/register")]
        public IHttpActionResult PostRegister([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.password = PasswordHashModel.Hash(model.password);

                    this.Account.Register(model);
                    return Ok("Successful.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Exception", ex.Message);
                }

            }
            return BadRequest(ModelState.GetErrorModelState());
        }

        // เข้าสู่ระบบ
        [Route("api/account/login")]
        public AccessTokenModel PostLogin([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (this.Account.Login(model))
                    {
                        return new AccessTokenModel
                        {
                            accessToken = this.AccessToken.GenerateAccessToken(model.email)
                        };
                    }
                    throw new Exception("Username or Password is invalid.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Exception", ex.Message);
                }
            }
            throw new HttpResponseException(Request.CreateResponse(
                HttpStatusCode.BadRequest,
                new { Message = ModelState.GetErrorModelState() }
            ));
        }
    }
}
