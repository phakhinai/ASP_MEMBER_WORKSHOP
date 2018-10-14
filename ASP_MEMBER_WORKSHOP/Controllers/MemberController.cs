using ASP_MEMBER_WORKSHOP.Entity;
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
    [Authorize]
    public class MemberController : ApiController
    {
        private IMemberService memberService;

        public MemberController()
        {
            this.memberService = new MemberService();
        }

        // แสดงข้อมูลผู้ใช้งานที่เข้าสู่ระบบ
        [Route("api/member/data")]
        public MemberModel GetMemberData()
        {
            var member = this.memberService
                .MemberItems
                .SingleOrDefault(item => item.email.Equals(User.Identity.Name));
            if (member == null) return null;
            return new MemberModel
            {
                Id = member.Id,
                firstname = member.firstname,
                lastname = member.lastname,
                email = member.email,
                position = member.position,
                image_type = member.image_type,
                image_byte = member.image,
                role = member.role,
                created = member.created,
                updated = member.updated
            };
        }

        // บันทึกข้อมูลโปรไฟล์
        [Route("api/member/profile")]
        public IHttpActionResult PostUpdateProfile([FromBody] ProfileModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    this.memberService.UpdateProfile(User.Identity.Name, model);
                    return Ok(this.GetMemberData());
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Exception", ex.Message);
                }
            }
            return BadRequest(ModelState.GetErrorModelState());
        }

        // เปลี่ยนรหัสผ่าน
        [Route("api/member/change-password")]
        public IHttpActionResult PostChangePassword([FromBody] ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(model);
            }
            return BadRequest(ModelState.GetErrorModelState());
        }
    }
}
