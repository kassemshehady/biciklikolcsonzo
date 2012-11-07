using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bicikli_Admin.Controllers
{
    public class PrintersController : Controller
    {
        //
        // GET: /Printers/

        public ActionResult Index()
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            return View();
        }

        //
        // GET: /Printers/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Printers/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Printers/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Printers/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Printers/Edit/5

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
                return View();
            }
        }

        //
        // GET: /Printers/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Printers/Delete/5

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
                return View();
            }
        }
    }
}
