using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bicikli_Admin.CommonClasses;

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
        // GET: /Bikes/FreeList

        public ActionResult FreeList()
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
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
        // GET: /Bikes/FreeList/5

        public ActionResult FreeList(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            return View(DataRepository.GetBikes().Where(b => b.currentLenderId == id));
        }

        //
        // GET: /Bikes/DangerousList

        public ActionResult DangerousList()
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            return View(DataRepository.GetBikes().Where(b => b.isInDangerousZone));
        }

        //
        // GET: /Bikes/UnusedList

        public ActionResult DangerousList()
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
            return View();
        }

        //
        // GET: /Bikes/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Bikes/Create

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
        // GET: /Bikes/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Bikes/Edit/5

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
        // GET: /Bikes/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Bikes/Delete/5

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
