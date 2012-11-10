using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bicikli_Admin.CommonClasses;
using Bicikli_Admin.Models;

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
            ViewBag.active_menu_item_id = "menu-btn-zones";
            return View(DataRepository.GetDangerousZone(id));
        }

        //
        // GET: /Zones/Create

        public ActionResult Create()
        {
            ViewBag.active_menu_item_id = "menu-btn-zones";
            return View(new ZoneModel());
        }

        //
        // POST: /Zones/Create

        [HttpPost]
        public ActionResult Create(ZoneModel m)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-zones";
                return View();
            }
        }

        //
        // GET: /Zones/Edit/5

        public ActionResult Edit(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-zones";
            return View(DataRepository.GetDangerousZone(id));
        }

        //
        // POST: /Zones/Edit/5

        [HttpPost]
        public ActionResult Edit(ZoneModel m)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View(m);
            }
        }

        //
        // GET: /Zones/Delete/5

        public ActionResult Delete(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-zones";
            return View(DataRepository.GetDangerousZone(id));
        }

        //
        // POST: /Zones/Delete/5

        [HttpPost]
        public ActionResult Delete(ZoneModel m)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View(m);
            }
        }

        //
        // GET: /Zones/ShowMap

        public ActionResult ShowMap()
        {
            ViewBag.active_menu_item_id = "menu-btn-zones";
            return View(DataRepository.GetDangerousZones());
        }
    }
}
