﻿using ASP_MEMBER_WORKSHOP.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP_MEMBER_WORKSHOP.Interfaces
{
    interface IAccessTokenService
    {
        string GenerateAccessToken(string email, int minute = 60);

        Member VerifyAccessToken(string accessToken);
    }
}
