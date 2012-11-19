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
        #region User data

        /// <summary>
        /// Returns all lenders assigned to a certain user
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static IEnumerable<LenderModel> GetLendersOfUser(Guid guid)
        {
            var dc = new BicikliDataClassesDataContext();
            return from lu in dc.LenderUsers
                   join l in dc.Lenders
                   on lu.lender_id equals l.id
                   where (lu.user_id == guid)
                   orderby l.name ascending
                   select new LenderModel
                   {
                       id = l.id,
                       address = l.address,
                       description = l.description,
                       latitude = l.latitude,
                       longitude = l.longitude,
                       name = l.name,
                       printer_ip = l.printer_ip,
                       printer_password = l.printer_password
                   };
        }

        /// <summary>
        /// Returns all assigned lenders by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static IEnumerable<LenderModel> GetAssignedLenders(string username)
        {
            var dc = new BicikliDataClassesDataContext();
            return from l in dc.Lenders
                   join lu in dc.LenderUsers
                   on l equals lu.Lender
                   where (lu.User.UserId == (Guid)Membership.GetUser(username).ProviderUserKey)
                   orderby l.name ascending
                   select new LenderModel
                   {
                       id = l.id,
                       latitude = l.latitude,
                       longitude = l.longitude,
                       name = l.name,
                       address = l.address,
                       description = l.description,
                       printer_ip = l.printer_ip,
                       printer_password = l.printer_password
                   };
        }

        /// <summary>
        /// Returns all users from the database but only with guid and username
        /// fields filled out. (Performance...)
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<UserModel> GetUsers()
        {
            var dc = new BicikliDataClassesDataContext();
            return from u in dc.Users
                   orderby u.UserName ascending
                   select new UserModel
                   {
                       guid = u.UserId,
                       username = u.UserName
                   };
        }

        /// <summary>
        /// Returns all users from the database with every collectable detail
        /// </summary>
        /// <returns></returns>
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
                uModel.guid = (Guid)mUser.ProviderUserKey;
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

        #endregion

        #region Lender data

        /// <summary>
        /// Returns all lenders
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<LenderModel> GetLenders()
        {
            var dc = new BicikliDataClassesDataContext();
            return from l in dc.Lenders
                   orderby l.name ascending
                   select new LenderModel
                   {
                       id = l.id,
                       latitude = l.latitude,
                       longitude = l.longitude,
                       name = l.name,
                       address = l.address,
                       description = l.description,
                       printer_ip = l.printer_ip,
                       printer_password = l.printer_password
                   };
        }

        /// <summary>
        /// Returns all lenders for API
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiModels.LenderListItemModel> GetLendersForApi()
        {
            var dc = new BicikliDataClassesDataContext();
            return from li in dc.GetLendersList()
                   select new ApiModels.LenderListItemModel()
                   {
                       id = li.id,
                       latitude = li.latitude,
                       longitude = li.longitude,
                       name = li.name,
                       address = li.address
                   };
        }

        /// <summary>
        /// Returns a lender for API
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ApiModels.LenderModel GetLenderForApi(int id)
        {
            var dc = new BicikliDataClassesDataContext();
            return (from l in dc.GetLenderDetails(id)
                    select new ApiModels.LenderModel()
                    {
                        name = l.name,
                        address = l.address,
                        description = l.description,
                        latitude = l.latitude,
                        longitude = l.longitude
                    }).SingleOrDefault();
        }

        /// <summary>
        /// Returns a lender
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static LenderModel GetLender(int id)
        {
            var dc = new BicikliDataClassesDataContext();
            return (from l in dc.Lenders
                    where l.id == id
                    orderby l.name ascending
                    select new LenderModel
                    {
                        id = l.id,
                        latitude = l.latitude,
                        longitude = l.longitude,
                        name = l.name,
                        address = l.address,
                        description = l.description,
                        printer_ip = l.printer_ip,
                        printer_password = l.printer_password
                    }).Single();
        }

        /// <summary>
        /// Returns all users assigned to a certain lender
        /// </summary>
        /// <param name="lender_id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns all guids (user_id) assigned to a certain lender
        /// </summary>
        /// <param name="lender_id"></param>
        /// <returns></returns>
        public static IEnumerable<Guid> GetLenderAssignedGuids(int lender_id)
        {
            var dc = new BicikliDataClassesDataContext();
            return from lug in dc.LenderUsers
                   where lug.lender_id == lender_id
                   select lug.user_id;
        }

        /// <summary>
        /// Deletes a lender
        /// </summary>
        /// <param name="lender_id"></param>
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

        /// <summary>
        /// Returns the nearest lender within a specified radius
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static LenderModel GetNearestLenderInRadius(double latitude, double longitude, double radius)
        {
            var dc = new BicikliDataClassesDataContext();
            return (from l in dc.GetLendersByDistance(latitude, longitude, radius)
                    select new LenderModel()
                    {
                        id = l.id
                    }).FirstOrDefault();
        }

        #endregion

        #region Zone data

        /// <summary>
        /// Returns ALL dangerous zones
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ZoneModel> GetDangerousZones()
        {
            var dc = new BicikliDataClassesDataContext();
            return from z in dc.DangerousZones
                   orderby z.name ascending
                   select new ZoneModel()
                   {
                       id = z.id,
                       name = z.name,
                       description = z.description,
                       latitude = z.latitude,
                       longitude = z.longitude,
                       radius = z.radius
                   };
        }

        /// <summary>
        /// Returns a dangerous zone
        /// </summary>
        /// <returns></returns>
        public static ZoneModel GetDangerousZone(int zoneId)
        {
            var dc = new BicikliDataClassesDataContext();
            return (from z in dc.DangerousZones
                    where z.id == zoneId
                    orderby z.name ascending
                    select new ZoneModel()
                    {
                        id = z.id,
                        name = z.name,
                        description = z.description,
                        latitude = z.latitude,
                        longitude = z.longitude,
                        radius = z.radius
                    }).Single();
        }

        /// <summary>
        /// Deletes a dangerous zone
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteZone(int id)
        {
            var db = new BicikliDataClassesDataContext();
            var zoneToRemove = db.DangerousZones.Single(z => z.id == id);
            if (zoneToRemove.Sessions.Count() > 0)
            {
                foreach (Session s in zoneToRemove.Sessions)
                {
                    s.dz_id = null;
                }
            }
            db.DangerousZones.DeleteOnSubmit(zoneToRemove);
            db.SubmitChanges();
        }

        /// <summary>
        /// Returns the nearest dangerous zone to the specified coordinates
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static ZoneModel GetNearestDangerousZone(double latitude, double longitude)
        {
            var db = new BicikliDataClassesDataContext();
            return (from dz in db.GetDangerousZonesByDistance(latitude, longitude)
             select new ZoneModel()
             {
                 id = dz.id
             }).FirstOrDefault();
        }

        #endregion

        #region Bike data

        /// <summary>
        /// Returns the last lending data of a bike
        /// </summary>
        /// <param name="bike_id"></param>
        /// <returns></returns>
        public static LastLendingOfBike GetLastLendingOf(int bike_id)
        {
            var dc = new BicikliDataClassesDataContext();

            return (from s in dc.Sessions
                    where s.bike_id == bike_id
                    orderby s.start_time descending
                    select new LastLendingOfBike()
                    {
                        lastLendingDate = s.start_time,
                        lastSession = new SessionModel()
                        {
                            address = s.address,
                            bike_id = s.bike_id,
                            dangerousZoneId = s.dz_id,
                            dangerousZoneTime = s.dz_time,
                            endTime = s.end_time,
                            id = s.id,
                            lastReport = s.last_report,
                            latitude = s.latitude,
                            longitude = s.longitude,
                            name = s.name,
                            normalTime = s.normal_time,
                            paid = s.paid,
                            normal_price = s.normal_price,
                            danger_price = s.danger_price,
                            normal_vat = s.normal_vat,
                            danger_vat = s.danger_vat,
                            startTime = s.start_time
                        }
                    }).FirstOrDefault();
        }

        /// <summary>
        /// Returns ALL bikes from database with every collectable data
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<BikeModel> GetBikes()
        {
            // ide írni kell egy tárolt eljárást, hogy a többi mezőt is ki tudjam tölteni
            var dc = new BicikliDataClassesDataContext();

            #region Get all bikes...

            var allBikes = (from b in dc.Bikes
                            orderby b.name ascending
                            select new BikeModel()
                            {
                                id = b.id,
                                name = b.name,
                                description = b.description,
                                currentLenderId = b.current_lender_id,
                                imageUrl = b.image_url,
                                isActive = b.is_active
                            }).ToList();

            #endregion

            #region Get Active Sessions...

            var allActiveSessions = (from s in dc.Sessions
                                     where s.end_time == null
                                     orderby s.start_time descending
                                     select new SessionModel()
                                     {
                                         address = s.address,
                                         bike_id = s.bike_id,
                                         dangerousZoneId = s.dz_id,
                                         dangerousZoneTime = s.dz_time,
                                         endTime = s.end_time,
                                         id = s.id,
                                         lastReport = s.last_report,
                                         latitude = s.latitude,
                                         longitude = s.longitude,
                                         name = s.name,
                                         normalTime = s.normal_time,
                                         startTime = s.start_time,
                                         normal_price = s.normal_price,
                                         danger_price = s.danger_price,
                                         normal_vat = s.normal_vat,
                                         danger_vat = s.danger_vat
                                     }).ToList();

            #endregion

            #region Collect additional information...

            foreach (var bike in allBikes)
            {
                var sessionsWithThisBike = allActiveSessions.Where(s => s.bike_id == bike.id);

                if ((sessionsWithThisBike != null) && (sessionsWithThisBike.Count() > 0))
                {
                    bike.session = sessionsWithThisBike.First();
                    bike.isInDangerousZone = (bike.session.dangerousZoneId != null);
                }

                var lastLendingData = GetLastLendingOf((int)bike.id);

                if (lastLendingData != null)
                {
                    bike.lastLendingDate = lastLendingData.lastLendingDate;
                    bike.lastSession = lastLendingData.lastSession;
                }

                if (bike.lastLendingDate == new DateTime())
                {
                    bike.lastLendingDate = null;
                }

                if (bike.currentLenderId != null)
                {
                    bike.lender = (from l in dc.Lenders
                                   where l.id == bike.currentLenderId
                                   select new LenderModel()
                                   {
                                       address = l.address,
                                       description = l.description,
                                       id = l.id,
                                       latitude = l.latitude,
                                       longitude = l.longitude,
                                       name = l.name,
                                       printer_ip = l.printer_ip,
                                       printer_password = l.printer_password
                                   }).SingleOrDefault();
                }
            }

            #endregion

            return allBikes;
        }

        /// <summary>
        /// Returns only ONE bike from database with every collectable data
        /// </summary>
        /// <param name="bikeId"></param>
        /// <returns></returns>
        public static BikeModel GetBike(int bikeId)
        {
            var dc = new BicikliDataClassesDataContext();

            #region Get the bike...

            var bike = (from b in dc.Bikes
                        where b.id == bikeId
                        orderby b.name ascending
                        select new BikeModel()
                        {
                            id = b.id,
                            name = b.name,
                            description = b.description,
                            currentLenderId = b.current_lender_id,
                            imageUrl = b.image_url,
                            isActive = b.is_active
                        }).Single();

            #endregion

            #region Get Bike's Active Session if any...

            bike.session = (from s in dc.Sessions
                            where ((s.end_time == null) && (s.bike_id == bikeId))
                            orderby s.start_time descending
                            select new SessionModel()
                            {
                                address = s.address,
                                bike_id = s.bike_id,
                                dangerousZoneId = s.dz_id,
                                dangerousZoneTime = s.dz_time,
                                endTime = s.end_time,
                                id = s.id,
                                lastReport = s.last_report,
                                latitude = s.latitude,
                                longitude = s.longitude,
                                name = s.name,
                                normalTime = s.normal_time,
                                startTime = s.start_time,
                                normal_price = s.normal_price,
                                danger_price = s.danger_price,
                                normal_vat = s.normal_vat,
                                danger_vat = s.danger_vat
                            }).SingleOrDefault();

            #endregion

            #region Collect additional information...

            if (bike.session != null)
            {
                bike.isInDangerousZone = (bike.session.dangerousZoneId != null);
            }

            var lastLendingData = GetLastLendingOf((int)bike.id);

            if (lastLendingData != null)
            {
                bike.lastLendingDate = lastLendingData.lastLendingDate;
                bike.lastSession = lastLendingData.lastSession;
            }

            if (bike.lastLendingDate == new DateTime())
            {
                bike.lastLendingDate = null;
            }

            if (bike.currentLenderId != null)
            {
                bike.lender = (from l in dc.Lenders
                               where l.id == bike.currentLenderId
                               select new LenderModel()
                               {
                                   address = l.address,
                                   description = l.description,
                                   id = l.id,
                                   latitude = l.latitude,
                                   longitude = l.longitude,
                                   name = l.name,
                                   printer_ip = l.printer_ip,
                                   printer_password = l.printer_password
                               }).SingleOrDefault();
            }

            #endregion

            return bike;
        }

        /// <summary>
        /// Updates a bike in the database
        /// </summary>
        /// <param name="m"></param>
        public static void UpdateBike(BikeModel m)
        {
            var db = new BicikliDataClassesDataContext();
            var bikeToUpdate = db.Bikes.Single(b => b.id == m.id);
            if (bikeToUpdate.name != m.name)
            {
                bikeToUpdate.name = m.name;
            }
            if (bikeToUpdate.description != m.description)
            {
                bikeToUpdate.description = m.description;
            }
            if (bikeToUpdate.is_active != m.isActive)
            {
                bikeToUpdate.is_active = m.isActive;
            }
            if (bikeToUpdate.current_lender_id != m.currentLenderId)
            {
                bikeToUpdate.current_lender_id = m.currentLenderId;
            }
            if (bikeToUpdate.image_url != m.imageUrl)
            {
                bikeToUpdate.image_url = m.imageUrl;
            }
            db.SubmitChanges();
        }

        #endregion

        #region Session data

        /// <summary>
        /// Returns all sessions (invoices) from the database
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SessionModel> GetSessions()
        {
            var dc = new BicikliDataClassesDataContext();
            return from s in dc.Sessions
                   orderby s.start_time descending
                   select new SessionModel()
                   {
                       address = s.address,
                       bike_id = s.bike_id,
                       dangerousZoneId = s.dz_id,
                       dangerousZoneTime = s.dz_time,
                       endTime = s.end_time,
                       id = s.id,
                       lastReport = s.last_report,
                       latitude = s.latitude,
                       longitude = s.longitude,
                       name = s.name,
                       normalTime = s.normal_time,
                       startTime = s.start_time,
                       paid = s.paid,
                       normal_price = s.normal_price,
                       danger_price = s.danger_price,
                       normal_vat = s.normal_vat,
                       danger_vat = s.danger_vat,
                       bikeModel = DataRepository.GetBike(s.bike_id)
                   };
        }

        /// <summary>
        /// Returns a session (invoice) from the database
        /// </summary>
        /// <returns></returns>
        public static SessionModel GetSession(int id)
        {
            var dc = new BicikliDataClassesDataContext();
            return (from s in dc.Sessions
                    where s.id == id
                    select new SessionModel()
                    {
                        address = s.address,
                        bike_id = s.bike_id,
                        dangerousZoneId = s.dz_id,
                        dangerousZoneTime = s.dz_time,
                        endTime = s.end_time,
                        id = s.id,
                        lastReport = s.last_report,
                        latitude = s.latitude,
                        longitude = s.longitude,
                        name = s.name,
                        normalTime = s.normal_time,
                        startTime = s.start_time,
                        paid = s.paid,
                        normal_price = s.normal_price,
                        danger_price = s.danger_price,
                        normal_vat = s.normal_vat,
                        danger_vat = s.danger_vat,
                        bikeModel = DataRepository.GetBike(s.bike_id)
                    }).Single();
        }

        /// <summary>
        /// Updates a Session in the database
        /// </summary>
        /// <param name="m"></param>
        public static void UpdateSession(SessionModel m)
        {
            var db = new BicikliDataClassesDataContext();
            var invoiceToUpdate = db.Sessions.Single(s => s.id == m.id);

            if (invoiceToUpdate.name != m.name)
            {
                invoiceToUpdate.address = m.name;
            }
            if (invoiceToUpdate.address != m.address)
            {
                invoiceToUpdate.address = m.address;
            }
            if (invoiceToUpdate.end_time != m.endTime)
            {
                invoiceToUpdate.end_time = m.endTime;
            }
            if (invoiceToUpdate.paid != m.paid)
            {
                invoiceToUpdate.paid = true;
            }
            db.SubmitChanges();
        }

        #endregion

        #region Configuration data

        /// <summary>
        /// Returns the normal unit price from the database
        /// </summary>
        /// <returns></returns>
        public static int GetNormalUnitPrice()
        {
            var dc = new BicikliDataClassesDataContext();
            return int.Parse((from c in dc.Configurations
                              where c.key == "normal_price"
                              select c.value).Single());
        }

        /// <summary>
        /// Returns the dangerous zone unit price from the database
        /// </summary>
        /// <returns></returns>
        public static int GetDangerousUnitPrice()
        {
            var dc = new BicikliDataClassesDataContext();
            return int.Parse((from c in dc.Configurations
                              where c.key == "danger_price"
                              select c.value).Single());
        }

        /// <summary>
        /// Returns the normal VAT from the database
        /// </summary>
        /// <returns></returns>
        public static float GetNormalVAT()
        {
            var dc = new BicikliDataClassesDataContext();
            return float.Parse((from c in dc.Configurations
                                where c.key == "normal_vat"
                                select c.value).Single());
        }

        /// <summary>
        /// Returns the dangerous VAT from the database
        /// </summary>
        /// <returns></returns>
        public static float GetDangerousVAT()
        {
            var dc = new BicikliDataClassesDataContext();
            return float.Parse((from c in dc.Configurations
                                where c.key == "danger_vat"
                                select c.value).Single());
        }

        #endregion
    }
}