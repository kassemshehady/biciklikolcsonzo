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

        public static IEnumerable<LenderModel> GetLendersOfUser(Guid guid)
        {
            var dc = new BicikliDataClassesDataContext();
            return from lu in dc.LenderUsers
                   join l in dc.Lenders
                   on lu.lender_id equals l.id
                   where (lu.user_id == guid)
                   select new LenderModel
                   {
                       id = l.id,
                       address = l.address,
                       description = l.description,
                       latitude = l.latitude,
                       longitude = l.longitude,
                       name = l.name,
                       printer_ip = l.printer_ip
                   };
        }

        public static IEnumerable<UserModel> GetUsersWithDetails()
        {
            var result = new List<UserModel>();

            #region Get users data

            var dc = new BicikliDataClassesDataContext();
            var membershipUsers = Membership.GetAllUsers();
            foreach (MembershipUser mUser in membershipUsers)
            {
                var uModel = new UserModel();

                #region Get user data

                uModel.username = mUser.UserName;
                uModel.guid = (Guid) mUser.ProviderUserKey;
                uModel.email = mUser.Email;
                uModel.countOfLenders = GetLendersOfUser(uModel.guid).Count();
                uModel.lastLogin = mUser.LastLoginDate;
                uModel.isSiteAdmin = Roles.IsUserInRole(mUser.UserName, "SiteAdmin");
                uModel.isLockedOut = mUser.IsLockedOut;
                uModel.isApproved = mUser.IsApproved;

                #endregion

                result.Add(uModel);
            }

            #endregion

            return result;
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

        public static void DeleteLender(int lender_id)
        {
            var dc = new BicikliDataClassesDataContext();
            var lenderToRemove = dc.Lenders.Where(l => l.id == lender_id).Single();
            foreach (Bike bike in lenderToRemove.Bikes)
            {
                bike.current_lender_id = null;
            }
            dc.SubmitChanges();

            dc.LenderUsers.DeleteAllOnSubmit(lenderToRemove.LenderUsers);
            dc.SubmitChanges();

            dc.Lenders.DeleteOnSubmit(lenderToRemove);
            dc.SubmitChanges();
        }

        public static IEnumerable<ZoneModel> GetDangerousZones()
        {
            var dc = new BicikliDataClassesDataContext();
            return from z in dc.DangerousZones
                   select new ZoneModel() {
                       id = z.id,
                       name = z.name,
                       description = z.description,
                       latitude = z.latitude,
                       longitude = z.longitude,
                       radius = z.radius
                   };
        }

        public static IEnumerable<BikeModel> GetBikes()
        {
            // ide írni kell egy tárolt eljárást, hogy a többi mezőt is ki tudjam tölteni
            var dc = new BicikliDataClassesDataContext();
            return from b in dc.Bikes
                   select new BikeModel()
                   {
                       id = b.id,
                       name = b.name,
                       description = b.description,
                       currentLenderId = b.current_lender_id,
                       imageUrl = b.image_url,
                       isActive = b.is_active
                   };
        }
    }
}