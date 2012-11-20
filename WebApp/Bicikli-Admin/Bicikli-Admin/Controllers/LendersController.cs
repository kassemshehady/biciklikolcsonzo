using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Bicikli_Admin.CommonClasses;
using Bicikli_Admin.Models;

namespace Bicikli_Admin.Controllers
{
    [Authorize]
    public class LendersController : Controller
    {
        //
        // GET: /Lenders/

        public ActionResult Index()
        {
            ViewBag.active_menu_item_id = "menu-btn-lenders";
            ViewBag.MyLenders = DataRepository.GetAssignedLenders(User.Identity.Name);
            return View(DataRepository.GetLenders());
        }

        //
        // GET: /Lenders/MyLenders

        public ActionResult MyLenders()
        {
            ViewBag.active_menu_item_id = "menu-btn-lenders";
            return View(DataRepository.GetAssignedLenders(User.Identity.Name));
        }

        //
        // GET: /Lenders/Details/5

        public ActionResult Details(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-lenders";
            try
            {
                LenderModel lenderToShow;

                try
                {
                    lenderToShow = DataRepository.GetLender(id);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                GetSelectedUsers(id);

                #region Determine if it is favourite or not

                var favouriteLender = Request.Cookies.Get("favourite_lender");
                ViewBag.IsNotFavourite = ((favouriteLender == null) || (lenderToShow.id.ToString() != favouriteLender.Value));

                var myLenders = DataRepository.GetAssignedLenders(User.Identity.Name);
                try
                {
                    ViewBag.CanBeFavourite = (myLenders.SingleOrDefault(ml => ml.id == id) != null);
                }
                catch
                {
                    ViewBag.CanBeFavourite = false;
                }

                #endregion

                return View(lenderToShow);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Lenders/Create

        [Authorize(Roles = "SiteAdmin")]
        public ActionResult Create()
        {
            ViewBag.active_menu_item_id = "menu-btn-lenders";
            var users = DataRepository.GetUsers();
            ViewBag.Users = users;
            return View(new LenderModel());
        }

        //
        // POST: /Lenders/Create

        [HttpPost]
        [Authorize(Roles = "SiteAdmin")]
        public ActionResult Create(LenderModel model, string[] users)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                // Step 1: Create Lender
                var lenderModelToInsert = DataRepository.GetLender(DataRepository.InsertLender(model));

                // Step 2: Update Lender-User assignments
                DataRepository.UpdateAssignments((int)lenderModelToInsert.id, users);

                // Step 3: Return to list of Lenders
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-lenders";
                var dbUsers = DataRepository.GetUsers();
                ViewBag.Users = dbUsers;
                ViewBag.SelectedUsers = users;
                return View(model);
            }
        }

        //
        // GET: /Lenders/Edit/5

        public ActionResult Edit(int id)
        {
            try
            {
                DataRepository.GetLender(id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (!AppUtilities.IsSiteAdmin())
            {
                try
                {
                    DataRepository.GetLendersOfUser((Guid)Membership.GetUser().ProviderUserKey).Single(l => l.id == id);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            ViewBag.active_menu_item_id = "menu-btn-lenders";
            var users = DataRepository.GetUsers();
            ViewBag.Users = users;
            try
            {
                var lenderToEdit = DataRepository.GetLender(id);
                GetSelectedUsers(id);
                return View(lenderToEdit);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Lenders/Edit/5

        [HttpPost]
        public ActionResult Edit(LenderModel model, string[] users)
        {
            if (!AppUtilities.IsSiteAdmin())
            {
                try
                {
                    DataRepository.GetLendersOfUser((Guid)Membership.GetUser().ProviderUserKey).Single(l => l.id == model.id);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                DataRepository.UpdateLender(model);
                             
                if (AppUtilities.IsSiteAdmin())
                {
                    DataRepository.UpdateAssignments((int)model.id, users);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-lenders";

                var dbUsers = DataRepository.GetUsers();
                ViewBag.Users = dbUsers;
                ViewBag.SelectedUsers = users;
                return View(model);
            }
        }

        //
        // GET: /Lenders/Delete/5

        [Authorize(Roles = "SiteAdmin")]
        public ActionResult Delete(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-lenders";
            try
            {
                LenderModel lenderToShow;

                try
                {
                    lenderToShow = DataRepository.GetLender(id);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                GetSelectedUsers(id);

                return View(lenderToShow);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Lenders/Delete/5

        [HttpPost]
        [Authorize(Roles = "SiteAdmin")]
        public ActionResult Delete(LenderModel model)
        {
            try
            {
                DataRepository.DeleteLender((int)model.id);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Lenders/MakeFavourite/5

        public ActionResult MakeFavourite(int id)
        {
            try
            {
                var favouriteLender = DataRepository.GetAssignedLenders(User.Identity.Name).Single(l => l.id == id);
                Response.Cookies.Set(new HttpCookie("favourite_lender", favouriteLender.id.ToString()));
                return RedirectToAction("Details", new { id = id });
            }
            catch
            {
                return RedirectToAction("Details", new { id = id });
            }
        }

        /// <summary>
        /// Gets selected users from DB
        /// </summary>
        /// <param name="id">Lender ID</param>
        private void GetSelectedUsers(int id)
        {
            var selectedUsers = new List<string>();
            foreach (UserModel usermodel in DataRepository.GetLenderAssignedUsers(id))
            {
                selectedUsers.Add(usermodel.username);
            }
            ViewBag.SelectedUsers = selectedUsers;
        }
    }
}
