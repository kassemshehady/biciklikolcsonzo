using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
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
            SessionModel session;
            try
            {
                session = DataRepository.GetSession(id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            ViewBag.Printers = DataRepository.GetLenders().Where(l => l.printer_ip != null);
            ViewBag.IsCreatedNow = created_now;
            return View(session);
        }

        //
        // GET: /Invoices/Create/5

        public ActionResult Create(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            var bikeData = DataRepository.GetBike(id);

            try
            {
                DataRepository.GetLendersOfUser((Guid)Membership.GetUser().ProviderUserKey).Single(l => l.id == bikeData.currentLenderId);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            ViewBag.BikeData = bikeData;
            ViewBag.LenderData = DataRepository.GetLender((int) bikeData.currentLenderId);
            return View(new SessionModel() { bike_id = id });
        }

        //
        // POST: /Invoices/Create

        [HttpPost]
        public ActionResult Create(SessionModel m)
        {
            {
                var bike = DataRepository.GetBike(m.bike_id);

                try
                {
                    DataRepository.GetLendersOfUser((Guid)Membership.GetUser().ProviderUserKey).Single(l => l.id == bike.currentLenderId);
                }
                catch
                {
                    ModelState.AddModelError("", "Ezt a kerékpárt Ön nem adhatja ki: " + bike.name);
                    throw new Exception();
                }
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                var invoiceId = DataRepository.CreateSession(m);

                return RedirectToAction("Details", new { id = invoiceId, created_now = true });
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
            SessionModel session;
            try
            {
                session = DataRepository.GetSession(id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            ViewBag.active_menu_item_id = "menu-btn-invoices";
            ViewBag.Printers = DataRepository.GetLenders().Where(l => l.printer_ip != null);
            return View(session);
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

                var invoiceModelToUpdate = DataRepository.GetSession((int)m.id);

                if (invoiceModelToUpdate.paid)
                {
                    ModelState.AddModelError("", "A számla nem módosítható, ha már ki van fizetve!");

                    ViewBag.active_menu_item_id = "menu-btn-invoices";
                    ViewBag.Printers = DataRepository.GetLenders().Where(l => l.printer_ip != null);
                    return View(invoiceModelToUpdate);
                }

                invoiceModelToUpdate.name = m.name;
                invoiceModelToUpdate.address = m.address;

                if ((submitButton == "ForceEndSession") && ((invoiceModelToUpdate.endTime == null) || (invoiceModelToUpdate.endTime == new DateTime())))
                {
                    invoiceModelToUpdate.endTime = DateTime.Now;
                }
                else if ((submitButton == "PaySession") && (!invoiceModelToUpdate.paid))
                {
                    invoiceModelToUpdate.paid = true;
                }

                DataRepository.UpdateSession(invoiceModelToUpdate);

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
                DataRepository.UpdateInvoiceConfig(normal_price, danger_price, normal_vat, danger_vat);
                ViewBag.Message = "Sikeres mentés.";
            }
            else
            {
                ModelState.AddModelError("", "HIBA: Rossz paraméterek!");
            }

            return Configure();
        }

        //
        // GET: /Invoices/Print

        public ActionResult Print(int id, int lender_id = -1)
        {
            SessionModel session;
            try
            {
                session = DataRepository.GetSession(id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            ViewBag.active_menu_item_id = "menu-btn-invoices";

            if (lender_id > -1)
            {
                LenderModel lender = null;
                try
                {
                    lender = DataRepository.GetLender(lender_id);
                }
                catch { }

                if (lender != null)
                {
                    try
                    {
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
