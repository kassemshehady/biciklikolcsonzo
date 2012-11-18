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
    [Authorize]
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
            return View(DataRepository.GetSessions().Where(s => s.endTime == null));
        }

        //
        // GET: /Invoices/ClosedSessions

        public ActionResult ClosedSessions()
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            return View(DataRepository.GetSessions().Where(s => ((s.endTime != null) && !s.paid)));
        }

        //
        // GET: /Invoices/Details/5

        public ActionResult Details(int id, bool created_now = false)
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            ViewBag.Printers = DataRepository.GetLenders().Where(l => l.printer_ip != null);
            ViewBag.IsCreatedNow = created_now;
            return View(DataRepository.GetSession(id));
        }

        //
        // GET: /Invoices/Create/5

        public ActionResult Create(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            var bikeData = DataRepository.GetBike(id);
            ViewBag.BikeData = bikeData;
            ViewBag.LenderData = DataRepository.GetLender((int) bikeData.currentLenderId);
            return View(new SessionModel() { bike_id = id });
        }

        //
        // POST: /Invoices/Create

        [HttpPost]
        public ActionResult Create(SessionModel m)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                var db = new BicikliDataClassesDataContext();
                var invoiceToInsert = new Session()
                {
                    bike_id = m.bike_id,
                    name = m.name,              // user data (name)
                    address = m.address,        // user data (address)
                    dz_id = null,
                    dz_time = 0,
                    end_time = null,
                    last_report = null,
                    latitude = null,
                    longitude = null,
                    normal_time = 0,
                    paid = false,
                    start_time = DateTime.Now,
                    normal_price = DataRepository.GetNormalUnitPrice(),
                    normal_vat = DataRepository.GetNormalVAT(),
                    danger_price = DataRepository.GetDangerousUnitPrice(),
                    danger_vat = DataRepository.GetDangerousVAT()
                };
                db.Sessions.InsertOnSubmit(invoiceToInsert);
                db.Bikes.Single(b => b.is_active && (b.id == m.bike_id)).current_lender_id = null;
                db.SubmitChanges();

                return RedirectToAction("Details", new { id = invoiceToInsert.id, created_now = true });
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
        public ActionResult Edit(SessionModel m, string submitButton = "")
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                var db = new BicikliDataClassesDataContext();
                var invoiceToUpdate = db.Sessions.Single(s => s.id == m.id);

                if (invoiceToUpdate.paid)
                {
                    ModelState.AddModelError("", "A számla nem módosítható, ha már ki van fizetve!");

                    ViewBag.active_menu_item_id = "menu-btn-invoices";
                    ViewBag.Printers = DataRepository.GetLenders().Where(l => l.printer_ip != null);
                    return View(DataRepository.GetSession((int)m.id));
                }

                if (invoiceToUpdate.name != m.name)
                {
                    invoiceToUpdate.address = m.name;
                }
                if (invoiceToUpdate.address != m.address)
                {
                    invoiceToUpdate.address = m.address;
                }
                if ((submitButton == "ForceEndSession") && ((invoiceToUpdate.end_time == null) || (invoiceToUpdate.end_time == new DateTime())))
                {
                    invoiceToUpdate.end_time = DateTime.Now;
                }
                else if ((submitButton == "PaySession") && (!invoiceToUpdate.paid))
                {
                    invoiceToUpdate.paid = true;
                }
                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-invoices";
                return View(DataRepository.GetSession((int)m.id));
            }
        }

        //
        // GET: /Invoices/Configure

        [Authorize(Roles = "SiteAdmin")]
        public ActionResult Configure()
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            ViewBag.NormalPrice = DataRepository.GetNormalUnitPrice();
            ViewBag.NormalVAT = DataRepository.GetNormalVAT();
            ViewBag.DangerousPrice = DataRepository.GetDangerousUnitPrice();
            ViewBag.DangerousVAT = DataRepository.GetDangerousVAT();
            return View();
        }
        
        //
        // POST: /Invoices/Configure

        [HttpPost]
        [Authorize(Roles = "SiteAdmin")]
        public ActionResult Configure(int normal_price = 0, int danger_price = 0, float normal_vat = 0, float danger_vat = 0)
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";

            if (normal_price > 0 && danger_price > 0 && normal_vat > 0 && danger_vat > 0)
            {
                var db = new BicikliDataClassesDataContext();
                db.Configurations.Single(c => c.key == "normal_price").value = normal_price.ToString();
                db.Configurations.Single(c => c.key == "danger_price").value = danger_price.ToString();
                db.Configurations.Single(c => c.key == "normal_vat").value = normal_vat.ToString();
                db.Configurations.Single(c => c.key == "danger_vat").value = danger_vat.ToString();
                db.SubmitChanges();
                ViewBag.Message = "Sikeres mentés.";
            }
            else
            {
                ModelState.AddModelError("", "HIBA: Rossz paraméterek!");
            }

            ViewBag.NormalPrice = DataRepository.GetNormalUnitPrice();
            ViewBag.NormalVAT = DataRepository.GetNormalVAT();
            ViewBag.DangerousPrice = DataRepository.GetDangerousUnitPrice();
            ViewBag.DangerousVAT = DataRepository.GetDangerousVAT();
            return View();
        }

        //
        // GET: /Invoices/Print

        public ActionResult Print(int id, int lender_id = -1)
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            var session = DataRepository.GetSession(id);

            if (lender_id > -1)
            {
                try
                {
                    var lender = DataRepository.GetLender(lender_id);

                    if (PrintingSubscription.sendInvoice(session, lender))
                    {
                        return View("Print_Success");
                    }

                    return View("Print_Failed");
                }
                catch
                {
                    try
                    {
                        var db = new BicikliDataClassesDataContext();
                        var lenderWithPrinter = db.Lenders.Single(l => l.id == id);
                        lenderWithPrinter.printer_ip = null;
                        db.SubmitChanges();
                    }
                    catch
                    {
                        // dummy
                    }

                    return View("Print_Failed");
                }
            }

            ViewBag.Session = session;

            try
            {
                ViewBag.InvoiceId = id;
                var assignedLendersWithPrinter = DataRepository.GetAssignedLenders(User.Identity.Name).Where(l => l.printer_ip != null);
                if (assignedLendersWithPrinter.Count() < 1)
                {
                    throw new Exception();
                }
                return View(assignedLendersWithPrinter);
            }
            catch
            {
                return View("Print_NoPrinters");
            }
        }
    }
}
