using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Bicikli_Admin.EntityFramework;
using Bicikli_Admin.EntityFramework.linq;
using Bicikli_Admin.Models;

namespace Bicikli_Admin.CommonClasses
{
    public class DataRepository
    {
        public static IEnumerable<LenderModel> GetAssignedLenders(string username)
        {
            var dc = new BicikliDataClassesDataContext();
            return from l in dc.Lenders
                   join lu in dc.LenderUsers
                   on l equals lu.Lender
                   where (lu.User.UserId == (Guid)Membership.GetUser(username).ProviderUserKey)
                   select new LenderModel
                   {
                       id = l.id,
                       latitude = l.latitude,
                       longitude = l.longitude,
                       name = l.name,
                       address = l.address,
                       description = l.description,
                       printer_ip = l.printer_ip
                   };
        }

        public static IEnumerable<LenderModel> GetLenders()
        {
            var dc = new BicikliDataClassesDataContext();
            return from l in dc.Lenders
                   select new LenderModel
                   {
                       id = l.id,
                       latitude = l.latitude,
                       longitude = l.longitude,
                       name = l.name,
                       address = l.address,
                       description = l.description,
                       printer_ip = l.printer_ip
                   };
        }

        public static IEnumerable<UserModel> GetUsers()
        {
            var dc = new BicikliDataClassesDataContext();
            return from u in dc.Users
                   select new UserModel
                   {
                       guid = u.UserId,
                       username = u.UserName
                   };
        }

        public static LenderModel GetLender(int id)
        {
            var dc = new BicikliDataClassesDataContext();
            return (from l in dc.Lenders
                    where l.id == id
                    select new LenderModel
                    {
                        id = l.id,
                        latitude = l.latitude,
                        longitude = l.longitude,
                        name = l.name,
                        address = l.address,
                        description = l.description,
                        printer_ip = l.printer_ip
                    }).Single();
        }

        public static IEnumerable<UserModel> GetLenderAssignedUsers(int lender_id)
        {
            var dc = new BicikliDataClassesDataContext();
            return from lug in dc.LenderUsers
                   where lug.lender_id == lender_id
                   select new UserModel
                   {
                       username = lug.User.UserName,
                       guid = lug.User.UserId
                   };
        }

        public static IEnumerable<Guid> GetLenderAssignedGuids(int lender_id)
        {
            var dc = new BicikliDataClassesDataContext();
            return from lug in dc.LenderUsers
                   where lug.lender_id == lender_id
                   select lug.user_id;
        }
    }
}