using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Bicikli_Admin.CommonClasses;
using Bicikli_Admin.EntityFramework.linq;
using Bicikli_Admin.Models;

namespace Bicikli_Admin.Controllers
{
    [Authorize(Roles = "SiteAdmin")]
    public class UsersController : Controller
    {
        //
        // GET: /Users/

        public ActionResult Index()
        {
            ViewBag.active_menu_item_id = "menu-btn-users";
            return View(DataRepository.GetUsersWithDetails());
        }

        //
        // GET: /Users/SiteAdminsList

        public ActionResult SiteAdminsList()
        {
            ViewBag.active_menu_item_id = "menu-btn-users";
            return View(DataRepository.GetUsersWithDetails().Where(u => u.isSiteAdmin));
        }

        //
        // GET: /Users/UnassignedList

        public ActionResult UnassignedList()
        {
            ViewBag.active_menu_item_id = "menu-btn-users";
            return View(DataRepository.GetUsersWithDetails().Where(u => u.countOfLenders == 0));
        }

        //
        // GET: /Users/UnapprovedList

        public ActionResult UnapprovedList()
        {
            ViewBag.active_menu_item_id = "menu-btn-users";
            return View(DataRepository.GetUsersWithDetails().Where(u => !u.isApproved));
        }

        //
        // GET: /Users/BannedList

        public ActionResult BannedList()
        {
            ViewBag.active_menu_item_id = "menu-btn-users";
            return View(DataRepository.GetUsersWithDetails().Where(u => u.isLockedOut));
        }

        //
        // GET: /Users/Details/5

        public ActionResult Details(string id)
        {
            try
            {
                ViewBag.active_menu_item_id = "menu-btn-users";
                var model = DataRepository.GetUsersWithDetails().Single(u => u.username == id);
                ViewBag.SelectedLenders = DataRepository.GetLendersOfUser(model.guid);
                return View(model);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Users/Edit/5

        public ActionResult Edit(string id)
        {
            try
            {
                ViewBag.active_menu_item_id = "menu-btn-users";
                var model = DataRepository.GetUsersWithDetails().Single(u => u.username == id);
                ViewBag.SelectedLenders = DataRepository.GetLendersOfUser(model.guid);
                ViewBag.Lenders = DataRepository.GetLenders();
                return View(model);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Users/Edit/5

        [HttpPost]
        public ActionResult Edit(UserModel model, int[] lenders)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                #region Update user profile

                var mUser = Membership.GetUser(model.username);
                if (mUser == null)
                {
                    return RedirectToAction("Index");
                }

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

                #endregion

                #region Update assigned lenders

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
                    var assignedLenders = DataRepository.GetLendersOfUser((Guid)mUser.ProviderUserKey);
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
                        db.LenderUsers.DeleteOnSubmit(db.LenderUsers.First(s => ((s.lender_id == l) && (s.user_id == (Guid)mUser.ProviderUserKey))));
                    }
                    foreach (int l in lendersToInsert)
                    {
                        db.LenderUsers.InsertOnSubmit(new LenderUser() { lender_id = l, user_id = (Guid)mUser.ProviderUserKey });
                    }
                    db.SubmitChanges();

                #endregion

                }
                else
                {
                    var db = new BicikliDataClassesDataContext();
                    db.LenderUsers.DeleteAllOnSubmit(db.LenderUsers.Where(s => (s.user_id == (Guid)mUser.ProviderUserKey)));
                    db.SubmitChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                #region Redisplay page

                ViewBag.active_menu_item_id = "menu-btn-users";
                var user = DataRepository.GetUsersWithDetails().Single(u => u.username == model.username);
                model.lastLogin = user.lastLogin;
                var dbLenders = DataRepository.GetLenders();
                ViewBag.Lenders = dbLenders;

                var selectedLenders = new List<LenderModel>();
                foreach (int lid in lenders)
                {
                    selectedLenders.Add(
                        new LenderModel
                        {
                            name = dbLenders.Where(dbl => dbl.id == lid).First().name,
                            id = lid
                        });
                }
                ViewBag.SelectedLenders = selectedLenders;

                #endregion

                return View(model);
            }
        }

    }
}
