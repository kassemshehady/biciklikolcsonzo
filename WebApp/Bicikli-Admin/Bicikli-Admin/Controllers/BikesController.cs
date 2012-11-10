using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bicikli_Admin.CommonClasses;
using Bicikli_Admin.Models;

namespace Bicikli_Admin.Controllers
{
    public class BikesController : Controller
    {
        //
        // GET: /Bikes/

        public ActionResult Index()
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            return View(DataRepository.GetBikes());
        }

        //
        // GET: /Bikes/FreeList/5
        // GET: /Bikes/FreeList

        public ActionResult FreeList(int id = -1)
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            ViewBag.MyLenders = DataRepository.GetAssignedLenders(User.Identity.Name);
            if (id != -1)
            {
                ViewBag.SelectedLenderId = id;
                return View(DataRepository.GetBikes().Where(b => b.isActive && (b.currentLenderId == id)));
            }

            var favouriteLender = Request.Cookies.Get("favourite_lender");
            int favouriteLenderId;

            if ((favouriteLender != null) && int.TryParse(favouriteLender.Value, out favouriteLenderId))
            {
                ViewBag.SelectedLenderId = favouriteLenderId;
                return View(DataRepository.GetBikes().Where(b => b.isActive && (b.currentLenderId == favouriteLenderId)));
            }
            else
            {
                return View(DataRepository.GetBikes().Where(b => b.isActive && (b.currentLenderId != null)));
            }
        }

        //
        // GET: /Bikes/DangerousList

        public ActionResult DangerousList(int id = -1)
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            if (id != -1)
            {
                return View(DataRepository.GetBikes().Where(b => b.isActive && (b.session.dangerousZoneId == id)));
            }

            return View(DataRepository.GetBikes().Where(b => b.isActive && b.isInDangerousZone));
        }

        //
        // GET: /Bikes/UnusedList

        public ActionResult UnusedList()
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            return View(DataRepository.GetBikes().Where(b => !b.isActive || ((b.currentLenderId == null) && (b.session == null))));
        }

        //
        // GET: /Bikes/BusyList

        public ActionResult BusyList()
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            return View(DataRepository.GetBikes().Where(b => (b.currentLenderId == null) && (b.session != null)));
        }

        //
        // GET: /Bikes/Details/5

        public ActionResult Details(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            ViewBag.CanILend = false;

            #region Determine if I can lend it?

            var bike = DataRepository.GetBike(id);
            if ((bike.session == null) && bike.isActive && (bike.currentLenderId != null))
            {
                var myLenders = DataRepository.GetAssignedLenders(User.Identity.Name);
                if ((myLenders != null) && (myLenders.Count() > 0))
                {
                    foreach (var l in myLenders)
                    {
                        if (l.id == bike.currentLenderId)
                        {
                            ViewBag.CanILend = true;
                            break;
                        }
                    }
                }
            }

            #endregion

            return View(bike);
        }

        //
        // GET: /Bikes/Create

        public ActionResult Create()
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            ViewBag.MyLenders = DataRepository.GetAssignedLenders(User.Identity.Name);

            var favouriteLender = Request.Cookies.Get("favourite_lender");
            int favouriteLenderId;

            if ((favouriteLender != null) && int.TryParse(favouriteLender.Value, out favouriteLenderId))
            {
                ViewBag.SelectedLenderId = favouriteLenderId;
            }

            return View(new BikeModel());
        }

        //
        // POST: /Bikes/Create

        [HttpPost]
        public ActionResult Create(BikeModel m)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-bikes";
                ViewBag.MyLenders = DataRepository.GetAssignedLenders(User.Identity.Name);

                var favouriteLender = Request.Cookies.Get("favourite_lender");
                int favouriteLenderId;

                if ((favouriteLender != null) && int.TryParse(favouriteLender.Value, out favouriteLenderId))
                {
                    ViewBag.SelectedLenderId = favouriteLenderId;
                }
                return View(m);
            }
        }

        //
        // GET: /Bikes/Edit/5

        public ActionResult Edit(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            ViewBag.MyLenders = DataRepository.GetAssignedLenders(User.Identity.Name);
            return View(DataRepository.GetBike(id));
        }

        //
        // POST: /Bikes/Edit/5

        [HttpPost]
        public ActionResult Edit(BikeModel m)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-bikes";
                ViewBag.MyLenders = DataRepository.GetAssignedLenders(User.Identity.Name);
                return View(m);
            }
        }

        //
        // GET: /Bikes/Delete/5

        public ActionResult Delete(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            return View(DataRepository.GetBike(id));
        }

        //
        // POST: /Bikes/Delete/5

        [HttpPost]
        public ActionResult Delete(BikeModel m)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-bikes";
                return View(m);
            }
        }

        //
        // GET: /Bikes/Lend
        // TODO: ÁTHELYEZNI A SZÁMLÁKHOZ!!!!
        // TODO: Kölcsönzők listája nem kell, mert a bicikliről tudjuk, hogy hol van!

        public ActionResult Lend(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            ViewBag.BikeToLend = DataRepository.GetBike(id);

            var favouriteLender = Request.Cookies.Get("favourite_lender");
            int favouriteLenderId;

            if ((favouriteLender != null) && int.TryParse(favouriteLender.Value, out favouriteLenderId))
            {
                ViewBag.SelectedLenderId = favouriteLenderId;
            }
            return View(DataRepository.GetAssignedLenders(User.Identity.Name));
        }

        //
        // GET: /Bikes/ShowMap

        public ActionResult ShowMap()
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            ViewBag.DangerousZones = DataRepository.GetDangerousZones();
            return View(DataRepository.GetBikes().Where(b => (b.currentLenderId == null) && (b.session != null) && (b.session.latitude != null) && (b.session.longitude != null)));
        }
    }
}
