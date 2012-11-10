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
            if (id != -1)
            {
                return View(DataRepository.GetBikes().Where(b => b.currentLenderId == id));
            }

            var favouriteLender = Request.Cookies.Get("favourite_lender");
            int favouriteLenderId;

            if ((favouriteLender != null) && int.TryParse(favouriteLender.Value, out favouriteLenderId))
            {
                return View(DataRepository.GetBikes().Where(b => b.currentLenderId == favouriteLenderId));
            }
            else
            {
                return View(DataRepository.GetBikes().Where(b => b.currentLenderId != null));
            }
        }

        //
        // GET: /Bikes/DangerousList

        public ActionResult DangerousList(int id = -1)
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            if (id != -1)
            {
                return View(DataRepository.GetBikes().Where(b => b.session.dangerousZoneId == id));
            }

            return View(DataRepository.GetBikes().Where(b => b.isInDangerousZone));
        }

        //
        // GET: /Bikes/UnusedList

        public ActionResult UnusedList()
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            return View(DataRepository.GetBikes().Where(b => ((b.currentLenderId == null) && (b.session == null))));
        }

        //
        // GET: /Bikes/BusyList

        public ActionResult BusyList()
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            return View(DataRepository.GetBikes().Where(b => b.currentLenderId == null));
        }

        //
        // GET: /Bikes/Details/5

        public ActionResult Details(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            return View(DataRepository.GetBike(id));
        }

        //
        // GET: /Bikes/Create

        public ActionResult Create()
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
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
                return View(m);
            }
        }

        //
        // GET: /Bikes/Edit/5

        public ActionResult Edit(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
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

        public ActionResult Lend()
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            return View(DataRepository.GetAssignedLenders(User.Identity.Name));
        }

        //
        // GET: /Bikes/LendFrom

        public ActionResult LendFrom(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            return View(DataRepository.GetBikes().Where(b => b.currentLenderId == id));
        }

        //
        // GET: /Bikes/ShowMap

        public ActionResult ShowMap()
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            return View(DataRepository.GetBikes().Where(b => b.currentLenderId == null));
        }
    }
}
