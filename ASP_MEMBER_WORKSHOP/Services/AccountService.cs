using ASP_MEMBER_WORKSHOP.Entity;
using ASP_MEMBER_WORKSHOP.Interfaces;
using ASP_MEMBER_WORKSHOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASP_MEMBER_WORKSHOP.Services
{
    public class AccountService : IAccountService
    {
        private DbEntities db = new DbEntities();

        // เข้าสู่ระบบ
        public bool Login(LoginModel model)
        {
            try
            {
                var memberItem = this.db.Members.SingleOrDefault();
                return false;
            }
            catch (Exception ex)
            {
                throw ex.GetErrorException();
            }
        }

        // ลงทะเบียน
        public void Register(RegisterModel model)
        {
            try
            {
                this.db.Members.Add(new Member
                {
                    firstname = model.firstname,
                    lastname = model.lastname,
                    email = model.email,
                    password = model.password,
                    position = "",
                    image = null,
                    role = RoleAccount.Member,
                    created = DateTime.Now,
                    updated = DateTime.Now
                });

                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex.GetErrorException();
            }
        }
    }
}