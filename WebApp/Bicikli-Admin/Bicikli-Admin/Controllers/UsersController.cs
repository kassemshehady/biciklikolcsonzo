using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bicikli_Admin.CommonClasses;
using Bicikli_Admin.Models;

namespace Bicikli_Admin.Controllers
{
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
        public ActionResult Edit(UserModel model)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}
