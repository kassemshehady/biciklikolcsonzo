using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bicikli_Admin.CommonClasses;
using Bicikli_Admin.EntityFramework.linq;
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
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                #region Insertion Logic

                var db = new BicikliDataClassesDataContext();
                db.DangerousZones.InsertOnSubmit(new DangerousZone()
                {
                    description = m.description,
                    latitude = m.latitude,
                    longitude = m.longitude,
                    name = m.name,
                    radius = m.radius
                });
                db.SubmitChanges();

                #endregion

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
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                #region Update Logic

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

                #endregion

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-zones";
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
                #region Deletion Logic

                var db = new BicikliDataClassesDataContext();
                var zoneToRemove = db.DangerousZones.Single(z => z.id == m.id);
                if (zoneToRemove.Sessions.Count() > 0)
                {
                    foreach (Session s in zoneToRemove.Sessions)
                    {
                        s.dz_id = null;
                    }
                }
                db.DangerousZones.DeleteOnSubmit(zoneToRemove);
                db.SubmitChanges();

                #endregion

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-zones";
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
