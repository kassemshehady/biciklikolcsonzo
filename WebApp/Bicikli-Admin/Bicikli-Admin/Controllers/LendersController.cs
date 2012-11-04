using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bicikli_Admin.CommonClasses;
using Bicikli_Admin.EntityFramework;
using Bicikli_Admin.Models;

namespace Bicikli_Admin.Controllers
{
    public class LendersController : Controller
    {
        //
        // GET: /Lenders/

        public ActionResult Index()
        {
            ViewBag.active_menu_item_id = "menu-btn-lenders";
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
            return View();
        }

        //
        // GET: /Lenders/Create

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
        public ActionResult Create(LenderModel model, string[] users)
        {
            try
            {
                // TODO: Add insert logic here
                // users: bepipált felhasználónevek tömbje vagy null

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-lenders";
                return View(model);
            }
        }

        //
        // GET: /Lenders/Edit/5

        public ActionResult Edit(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-lenders";
            return View();
        }

        //
        // POST: /Lenders/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-lenders";
                return View();
            }
        }

        //
        // GET: /Lenders/Delete/5

        public ActionResult Delete(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-lenders";
            return View();
        }

        //
        // POST: /Lenders/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-lenders";
                return View();
            }
        }
    }
}
