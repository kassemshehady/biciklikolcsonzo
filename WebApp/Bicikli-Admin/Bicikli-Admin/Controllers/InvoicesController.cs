using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bicikli_Admin.CommonClasses;
using Bicikli_Admin.Models;

namespace Bicikli_Admin.Controllers
{
    public class InvoicesController : Controller
    {
        //
        // GET: /Invoices/

        public ActionResult Index()
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            return View(DataRepository.GetSessions());
        }

        //
        // GET: /Invoices/ActiveSessions

        public ActionResult ActiveSessions()
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            return View(DataRepository.GetSessions().Where(s => s.endTime != null));
        }

        //
        // GET: /Invoices/ClosedSessions

        public ActionResult ClosedSessions()
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            return View(DataRepository.GetSessions().Where(s => ((s.endTime == null) && !s.paid)));
        }

        //
        // GET: /Invoices/Details/5

        public ActionResult Details(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            ViewBag.Printers = DataRepository.GetLenders().Where(l => l.printer_ip != null);
            return View(DataRepository.GetSession(id));
        }

        //
        // GET: /Invoices/Create/5

        public ActionResult Create(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            return View(new SessionModel());
        }

        //
        // POST: /Invoices/Create

        [HttpPost]
        public ActionResult Create(SessionModel m)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-invoices";
                return View(m);
            }
        }

        //
        // GET: /Invoices/Edit/5

        public ActionResult Edit(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            ViewBag.Printers = DataRepository.GetLenders().Where(l => l.printer_ip != null);
            return View(DataRepository.GetSession(id));
        }

        //
        // POST: /Invoices/Edit/5

        [HttpPost]
        public ActionResult Edit(SessionModel m)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-invoices";
                return View(m);
            }
        }

        //
        // GET: /Invoices/Print

        public ActionResult Print(int session_id, int lender_id)
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            var lender = DataRepository.GetLender(lender_id);
            var session = DataRepository.GetSession(session_id);

            // TODO: nyomtatóra küldő logika

            ViewBag.Lender = lender;
            ViewBag.Session = session;
            return View();
        }
    }
}
