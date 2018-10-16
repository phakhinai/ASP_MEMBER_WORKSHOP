using ASP_MEMBER_WORKSHOP.Entity;
using ASP_MEMBER_WORKSHOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP_MEMBER_WORKSHOP.Interfaces
{
    interface IMemberService
    {
        IEnumerable<Member> MemberItems { get; }

        GetMemberModel GetMembers(MemberFilterOptions filters);

        void UpdateProfile(string email, ProfileModel model);

        void ChangePassword(string email, ChangePasswordModel model);

        void CreateMember(CreateMemberModel model);

        void DeleteMember(int id);

        void UpdateMember(int id, UpdateMemberModel model);
    }
}
