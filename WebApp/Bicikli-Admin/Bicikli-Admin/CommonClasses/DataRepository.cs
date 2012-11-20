using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
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

        /// <summary>
        /// Updates a user profile
        /// </summary>
        /// <param name="model"></param>
        public static void UpdateUser(UserModel model)
        {
            var mUser = Membership.GetUser(model.username);

            if (mUser.Email != model.email)
            {
                mUser.Email = model.email;
            }
            if (mUser.IsApproved != model.isApproved)
            {
                mUser.IsApproved = model.isApproved;
            }
            if (mUser.IsLockedOut != model.isLockedOut)
            {
                if (model.isLockedOut)
                {
                    try
                    {
                        for (int i = 0; i < Membership.MaxInvalidPasswordAttempts; i++)
                        {
                            Membership.ValidateUser(mUser.UserName, "98zfd8vbd9fvbdfv9d8vz9b8dz9a8z89z9d8z9da8za98fdzd");
                        }
                    }
                    catch
                    {
                        //dummy
                    }
                }
                else
                {
                    mUser.UnlockUser();
                }
            }
            if (Roles.IsUserInRole(mUser.UserName, "SiteAdmin") != model.isSiteAdmin)
            {
                if (model.isSiteAdmin)
                {
                    Roles.AddUserToRole(mUser.UserName, "SiteAdmin");
                }
                else
                {
                    Roles.RemoveUserFromRole(mUser.UserName, "SiteAdmin");
                }
            }
            Membership.UpdateUser(mUser);
        }

        /// <summary>
        /// Updates user assignments
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="lenders"></param>
        public static void UpdateUserAssignments(string username, int[] lenders)
        {
            Guid guid = (Guid)Membership.GetUser(username).ProviderUserKey;

            if (lenders != null)
            {
                var lendersToInsert = new List<int>();
                foreach (int l in lenders)
                {
                    if (!lendersToInsert.Contains(l))
                    {
                        lendersToInsert.Add(l);
                    }
                }
                var assignedLenders = DataRepository.GetLendersOfUser(guid);
                var lendersToRemove = new List<int>();
                foreach (LenderModel lm in assignedLenders)
                {
                    if (lendersToInsert.Contains((int)lm.id))
                    {
                        lendersToInsert.Remove((int)lm.id);
                    }
                    else
                    {
                        lendersToRemove.Add((int)lm.id);
                    }
                }

                var db = new BicikliDataClassesDataContext();
                foreach (int l in lendersToRemove)
                {
                    db.LenderUsers.DeleteOnSubmit(db.LenderUsers.First(s => ((s.lender_id == l) && (s.user_id == guid))));
                }
                foreach (int l in lendersToInsert)
                {
                    db.LenderUsers.InsertOnSubmit(new LenderUser() { lender_id = l, user_id = guid });
                }
                db.SubmitChanges();
            }
            else
            {
                var db = new BicikliDataClassesDataContext();
                db.LenderUsers.DeleteAllOnSubmit(db.LenderUsers.Where(s => (s.user_id == guid)));
                db.SubmitChanges();
            }
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

        /// <summary>
        /// Inserts a Lender, then returns the generated primary key
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertLender(LenderModel model)
        {
            var db = new BicikliDataClassesDataContext();
            var lenderToInsert = new Lender();
            lenderToInsert.name = model.name;
            lenderToInsert.address = model.address;
            lenderToInsert.description = model.description;
            lenderToInsert.printer_ip = model.printer_ip;
            lenderToInsert.latitude = model.latitude;
            lenderToInsert.longitude = model.longitude;

            if ((model.printer_password != null) && (model.printer_password != ""))
            {
                lenderToInsert.printer_password = model.printer_password;
            }
            else
            {
                lenderToInsert.printer_password = ((int)(new Random().NextDouble() * 899999) + 100000).ToString();
            }

            db.Lenders.InsertOnSubmit(lenderToInsert);
            db.SubmitChanges();

            return lenderToInsert.id;
        }

        /// <summary>
        /// Updates a Lender
        /// </summary>
        /// <param name="model"></param>
        public static void UpdateLender(LenderModel model)
        {
            var db = new BicikliDataClassesDataContext();
            var lenderToUpdate = db.Lenders.First(l => l.id == model.id);

            lenderToUpdate.address = model.address;
            lenderToUpdate.description = model.description;
            lenderToUpdate.latitude = model.latitude;
            lenderToUpdate.longitude = model.longitude;
            lenderToUpdate.name = model.name;
            lenderToUpdate.printer_ip = model.printer_ip;

            if ((model.printer_password != null) && (model.printer_password != ""))
            {
                lenderToUpdate.printer_password = model.printer_password;
            }
            else
            {
                lenderToUpdate.printer_password = ((int)(new Random().NextDouble() * 899999) + 100000).ToString();
            }

            db.SubmitChanges();
        }

        /// <summary>
        /// Updates Lender-User assignments
        /// </summary>
        /// <param name="lender_id">Lender ID</param>
        /// <param name="users">username list</param>
        public static void UpdateAssignments(int lender_id, string[] users)
        {
            var db = new BicikliDataClassesDataContext();

            #region Step 1: Extract selected user guids

            var guidsToInsert = new HashSet<Guid>();
            if (users != null)
            {
                foreach (string username in users)
                {
                    var msUser = Membership.GetUser(username);
                    if (msUser != null)
                    {
                        Guid msUserGuid = (Guid)msUser.ProviderUserKey;
                        if (!guidsToInsert.Contains(msUserGuid))
                        {
                            guidsToInsert.Add(msUserGuid);
                        }
                    }
                }
            }

            #endregion

            #region Step 2: Apply changes to assignments

            #region Insert new assignments

            var lenderGuidAssignments = DataRepository.GetLenderAssignedGuids(lender_id);
            foreach (Guid guid in guidsToInsert)
            {
                if (!lenderGuidAssignments.Contains(guid))
                {
                    var luToInsert = new LenderUser();
                    luToInsert.user_id = guid;
                    luToInsert.lender_id = lender_id;
                    db.LenderUsers.InsertOnSubmit(luToInsert);
                }
            }

            #endregion

            #region Delete removed assignments

            lenderGuidAssignments = DataRepository.GetLenderAssignedGuids(lender_id);
            foreach (Guid guid in lenderGuidAssignments)
            {
                if (!guidsToInsert.Contains(guid))
                {
                    var luToDelete = db.LenderUsers.Where(lu => (lu.lender_id == lender_id) && (lu.user_id == guid)).First();
                    db.LenderUsers.DeleteOnSubmit(luToDelete);
                }
            }

            #endregion
            
            db.SubmitChanges();

            #endregion
        }

        /// <summary>
        /// Removes a printer from a lender
        /// </summary>
        /// <param name="lender_id"></param>
        public static void RemovePrinter(int lender_id)
        {
            var db = new BicikliDataClassesDataContext();
            var lenderWithPrinter = db.Lenders.Single(l => l.id == lender_id);
            lenderWithPrinter.printer_ip = null;
            db.SubmitChanges();
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
                       radius = z.radius,
                       bikesInThisZone = (from s in dc.Sessions
                                          where ((s.end_time == null) && (s.dz_id == z.id))
                                          select s.dz_id).Count()
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
                        radius = z.radius,
                        bikesInThisZone = (from s in dc.Sessions
                                           where ((s.end_time == null) && (s.dz_id == z.id))
                                           select s.dz_id).Count()
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

        /// <summary>
        /// Inserts a new Zone into the database
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static int InsertZone(ZoneModel m)
        {
            var db = new BicikliDataClassesDataContext();
            var zoneToInsert = new DangerousZone()
            {
                description = m.description,
                latitude = m.latitude,
                longitude = m.longitude,
                name = m.name,
                radius = m.radius
            };
            db.DangerousZones.InsertOnSubmit(zoneToInsert);
            db.SubmitChanges();

            return zoneToInsert.id;
        }

        /// <summary>
        /// Updates a dangerous zone
        /// </summary>
        /// <param name="m"></param>
        public static void UpdateZone(ZoneModel m)
        {
            var db = new BicikliDataClassesDataContext();
            var zoneToUpdate = db.DangerousZones.Single(z => z.id == m.id);

            if (zoneToUpdate.description != m.description)
            {
                zoneToUpdate.description = m.description;
            }
            if (zoneToUpdate.latitude != m.latitude)
            {
                zoneToUpdate.latitude = m.latitude;
            }
            if (zoneToUpdate.longitude != m.longitude)
            {
                zoneToUpdate.longitude = m.longitude;
            }
            if (zoneToUpdate.name != m.name)
            {
                zoneToUpdate.name = m.name;
            }
            if (zoneToUpdate.radius != m.radius)
            {
                zoneToUpdate.radius = m.radius;
            }

            db.SubmitChanges();
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

        /// <summary>
        /// Inserts a bike into the database
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static int InsertBike(BikeModel m)
        {
            var db = new BicikliDataClassesDataContext();
            var bike = new Bike()
                {
                    description = m.description,
                    is_active = m.isActive,
                    name = m.name,
                    current_lender_id = m.currentLenderId
                };
            db.Bikes.InsertOnSubmit(bike);
            db.SubmitChanges();

            return bike.id;
        }

        /// <summary>
        /// Deletes a bike
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteBike(int id)
        {
            var db = new BicikliDataClassesDataContext();
            db.Bikes.DeleteOnSubmit(db.Bikes.Single(b => b.id == id));
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
                invoiceToUpdate.paid = m.paid;
            }
            db.SubmitChanges();
        }

        /// <summary>
        /// Reports a session
        /// </summary>
        /// <param name="m"></param>
        public static void ReportSession(SessionModel m)
        {
            var db = new BicikliDataClassesDataContext();
            var report = db.Sessions.Single(s => s.id == m.id);

            report.dz_id = m.dangerousZoneId;
            report.dz_time = m.dangerousZoneTime;
            report.normal_time = m.normalTime;
            report.last_report = m.lastReport;
            report.latitude = m.latitude;
            report.longitude = m.longitude;

            if (report.end_time != m.endTime)
            {
                report.end_time = m.endTime;
            }

            db.SubmitChanges();
        }

        /// <summary>
        /// Creates a new session
        /// </summary>
        /// <param name="m"></param>
        public static int CreateSession(SessionModel m)
        {
            var db = new BicikliDataClassesDataContext();
            var invoiceToInsert = new Session()
            {
                bike_id = m.bike_id,
                name = m.name,              // user data (name)
                address = m.address,        // user data (address)
                dz_id = null,
                dz_time = 0,
                end_time = null,
                last_report = null,
                latitude = null,
                longitude = null,
                normal_time = 0,
                paid = false,
                start_time = DateTime.Now,
                normal_price = DataRepository.GetNormalUnitPrice(),
                normal_vat = DataRepository.GetNormalVAT(),
                danger_price = DataRepository.GetDangerousUnitPrice(),
                danger_vat = DataRepository.GetDangerousVAT()
            };
            db.Sessions.InsertOnSubmit(invoiceToInsert);
            db.Bikes.Single(b => b.is_active && (b.id == m.bike_id)).current_lender_id = null;
            db.SubmitChanges();

            return invoiceToInsert.id;
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

        /// <summary>
        /// Updates the Invoice configuration
        /// </summary>
        /// <param name="normal_price"></param>
        /// <param name="danger_price"></param>
        /// <param name="normal_vat"></param>
        /// <param name="danger_vat"></param>
        public static void UpdateInvoiceConfig(int normal_price, int danger_price, float normal_vat, float danger_vat)
        {
            var db = new BicikliDataClassesDataContext();
            db.Configurations.Single(c => c.key == "normal_price").value = normal_price.ToString();
            db.Configurations.Single(c => c.key == "danger_price").value = danger_price.ToString();
            db.Configurations.Single(c => c.key == "normal_vat").value = normal_vat.ToString();
            db.Configurations.Single(c => c.key == "danger_vat").value = danger_vat.ToString();
            db.SubmitChanges();
        }

        #endregion
    }
}