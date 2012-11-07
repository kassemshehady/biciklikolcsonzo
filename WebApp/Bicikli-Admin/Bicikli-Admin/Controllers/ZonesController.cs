using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bicikli_Admin.CommonClasses;

namespace Bicikli_Admin.Controllers
{
    public class ZonesController : Controller
    {
        //
        // GET: /Zones/

        public ActionResult Index()
        {
            ViewBag.active_menu_item_id = "menu-btn-zones";
            return View(DataRepository.GetDangerousZones());
        }

        //
        // GET: /Zones/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Zones/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Zones/Create

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
        // GET: /Zones/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Zones/Edit/5

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
        // GET: /Zones/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Zones/Delete/5

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
