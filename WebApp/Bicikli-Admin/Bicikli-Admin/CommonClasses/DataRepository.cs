using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Bicikli_Admin.EntityFramework;
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
                       description = l.description
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
                       description = l.description
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
    }
}