﻿using ASP_MEMBER_WORKSHOP.Entity;
using ASP_MEMBER_WORKSHOP.Interfaces;
using ASP_MEMBER_WORKSHOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASP_MEMBER_WORKSHOP.Services
{
    public class MemberService : IMemberService
    {
        private DbEntities db = new DbEntities();

        // ข้อมูลสมาชิก
        public IEnumerable<Member> MemberItems => this.db.Members.ToList();

        // เปลี่ยนรหัสผ่าน
        public void ChangePassword(string email, ChangePasswordModel model)
        {
            try
            {
                var memberItem = this.db.Members.SingleOrDefault(item => item.email.Equals(email));
                if (memberItem == null) throw new Exception("Not found member.");
                if (!PasswordHashModel.Verify(model.old_pass, memberItem.password))
                    throw new Exception("รหัสผ่านเดิมของท่านไม่ถูกต้อง!!");
                this.db.Members.Attach(memberItem);
                memberItem.password = PasswordHashModel.Hash(model.new_pass);
                memberItem.updated = DateTime.Now;
                this.db.Entry(memberItem).State = System.Data.Entity.EntityState.Modified;
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex.GetErrorException();
            }
        }

        // แก้ไขข้อมูลส่วนตัว
        public void UpdateProfile(string email, ProfileModel model)
        {
            try
            {
                var memberItem = this.db.Members.SingleOrDefault(item => item.email.Equals(email));
                if (memberItem == null) throw new Exception("Not found member.");
                this.db.Members.Attach(memberItem);
                memberItem.firstname = model.firstname;
                memberItem.lastname = model.lastname;
                memberItem.position = model.position;
                memberItem.updated = DateTime.Now;

                // ตรวจสอบว่ามีภาพอัพโหลดมาหรือไม่
                this.onConvertBase64ToImage(memberItem, model.image);

                this.db.Entry(memberItem).State = System.Data.Entity.EntityState.Modified;
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex.GetErrorException();
            }
        }

        // แสดงรายการสมาชิก Pagination และ Filter
        public GetMemberModel GetMembers(MemberFilterOptions filters)
        {
            // ดึงข้อมูล และ คัดข้อมูลที่ต้องการแสดง
            var items = this.MemberItems.Select(m => new GetMember
            {
                id = m.id,
                firstname = m.firstname,
                lastname = m.lastname,
                email = m.email,
                position = m.position,
                role = m.role,
                updated = m.updated
            })
            .OrderByDescending(m => m.updated);

            // เรียบเรียงข้อมูลให้อยู่ในรูปแบบที่ Frontend ต้องการ
            var memberItems = new GetMemberModel
            {
                items = items
                            .Skip((filters.startPage - 1) * filters.limitPage)
                            .Take(filters.limitPage)
                            .ToArray(),
                totalItems = items.Count()
            };

            // ตรวจสอบการ ค้นหาข้อมูลจากวันที่ หากมีข้อมูลมาก็ทำการ เรียบเรียงข้อมูลใหม่
            if (!string.IsNullOrEmpty(filters.searchType) && filters.searchType.Equals("updated"))
            {
                var paramItem = HttpContext.Current.Request.Params;
                var fromDate = paramItem.Get("searchText[from]").Replace(" GMT+0700 (Indochina Time)", "");
                var toDate = paramItem.Get("searchText[to]").Replace(" GMT+0700 (Indochina Time)", "");
                filters.searchText = $"{fromDate},{toDate}";
            }

            // หากว่ามีการค้นหาข้อมูลเข้ามาในระบบ
            if (!string.IsNullOrEmpty(filters.searchType) && !string.IsNullOrEmpty(filters.searchText))
            {
                string searchText = filters.searchText;
                string searchType = filters.searchType;
                IEnumerable<GetMember> searchItem = new GetMember[] { };

                switch (searchType)
                {
                    // ค้นหาจากวันที่
                    case "updated":
                        var searchTexts = searchText.Split(',');
                        DateTime fromDate = DateTime.Parse(searchTexts[0]);
                        DateTime toDate = DateTime.Parse(searchTexts[1]);
                        searchItem = from m in items
                                     where m.updated >= fromDate && m.updated <= toDate
                                     select m;
                        break;

                    // ค้นหาจากสิทธิ์ผู้ใช้งาน
                    case "role":
                        searchItem = from m in items
                                     where Convert.ToInt16(m.GetType()
                                     .GetProperty(filters.searchType)
                                     .GetValue(m)) == Convert.ToInt16(searchText)
                                     select m;
                        break;

                    // ค้นหาทั่วไป
                    default:
                        searchItem = from m in items
                                     where m.GetType()
                                     .GetProperty(filters.searchType)
                                     .GetValue(m)
                                     .ToString()
                                     .ToUpper()
                                     .Contains(searchText.ToUpper())
                                     select m;
                        break;
                }

                memberItems.items = searchItem
                                        .Skip((filters.startPage - 1) * filters.limitPage)
                                        .Take(filters.limitPage)
                                        .ToArray();

                memberItems.totalItems = searchItem.Count();
            }

            return memberItems;
        }

        public void CreateMember(CreateMemberModel model)
        {
            try
            {
                Member memberCreate = new Member
                {
                    firstname = model.firstname,
                    lastname = model.lastname,
                    email = model.email,
                    password = PasswordHashModel.Hash(model.password),
                    position = model.position,
                    role = model.role,
                    created = DateTime.Now,
                    updated = DateTime.Now
                };
                this.onConvertBase64ToImage(memberCreate, model.image);
                this.db.Members.Add(memberCreate);
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex.GetErrorException();
            }
        }

        // แปลง Base64 Image เป็น Byte และ ชนิดของรูปภาพ
        private void onConvertBase64ToImage(Member memberItem, string image)
        {
            // ตรวจสอบว่ามีภาพอัพโหลดมาหรือไม่
            if (!string.IsNullOrEmpty(image))
            {
                string[] images = image.Split(',');
                if (images.Length == 2)
                {
                    if (images[0].IndexOf("image") >= 0)
                    {
                        memberItem.image_type = images[0];
                        memberItem.image = Convert.FromBase64String(images[1]);
                    }
                }
            }
            else if (image == null)
            {
                memberItem.image_type = null;
                memberItem.image = null;
            }
        }

        // ลบข้อมูลสมาชิก
        public void DeleteMember(int id)
        {
            try
            {
                var memberDelete = this.MemberItems.SingleOrDefault(m => m.id == id);
                if (memberDelete == null) throw new Exception("Not found member.");
                this.db.Members.Remove(memberDelete);
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex.GetErrorException();
            }
        }

        // แก้ไขข้อมูลสมาชิก
        public void UpdateMember(int id, UpdateMemberModel model)
        {
            try
            {
                var memberUpdate = this.MemberItems.SingleOrDefault(m => m.id == id);
                if (memberUpdate == null) throw new Exception("Not found member");
                this.db.Members.Attach(memberUpdate);
                memberUpdate.email = model.email;
                memberUpdate.firstname = model.firstname;
                memberUpdate.lastname = model.lastname;
                memberUpdate.position = model.position;
                memberUpdate.role = model.role;
                memberUpdate.updated = DateTime.Now;
                if (!string.IsNullOrEmpty(model.password))
                    memberUpdate.password = PasswordHashModel.Hash(model.password);
                this.onConvertBase64ToImage(memberUpdate, model.image);
                this.db.Entry(memberUpdate).State = System.Data.Entity.EntityState.Modified;
                this.db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex.GetErrorException();
            }

        }
    }
}