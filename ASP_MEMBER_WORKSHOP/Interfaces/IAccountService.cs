using ASP_MEMBER_WORKSHOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP_MEMBER_WORKSHOP.Interfaces
{
    interface IAccountService
    {
        void Register(RegisterModel model);

        bool Login(LoginModel model);
    }
}
