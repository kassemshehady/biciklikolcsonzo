﻿using System;
using System.Collections.Generic;
using System.IO;
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
    public class BikesController : Controller
    {
        //
        // GET: /Bikes/

        public ActionResult Index()
        {
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            ViewBag.MyLenders = DataRepository.GetAssignedLenders(User.Identity.Name);
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
                try
                {
                    DataRepository.GetLender(id);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                ViewBag.SelectedLenderId = id;
                return View(DataRepository.GetBikes().Where(b => b.isActive && (b.currentLenderId == id)));
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
                try
                {
                    DataRepository.GetLender(id);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
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
            BikeModel bike;
            try
            {
                bike = DataRepository.GetBike(id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            ViewBag.active_menu_item_id = "menu-btn-bikes";
            ViewBag.CanILend = false;

            #region Determine if I can lend it?

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
            var favouriteLender = Request.Cookies.Get("favourite_lender");

            #region Create dropdown list for selectable lenders

            var myLenders = new List<SelectListItem>();
            myLenders.Add(new SelectListItem()
            {
                Text = "-- Használaton kívül --",
                Value = "-1",
                Selected = ((favouriteLender == null) || (favouriteLender.Value == "-1"))
            });
            foreach (var item in DataRepository.GetAssignedLenders(User.Identity.Name))
            {
                myLenders.Add(new SelectListItem()
                {
                    Text = item.name,
                    Value = item.id.ToString(),
                    Selected = ((favouriteLender != null) && (favouriteLender.Value == item.id.ToString()))
                });
            }

            ViewBag.MyLenders = myLenders;

            #endregion

            return View(new BikeModel() { isActive = true });
        }

        //
        // POST: /Bikes/Create

        [HttpPost]
        public ActionResult Create(BikeModel m, int MyLenders = -1, HttpPostedFileBase ImgFile = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                var db = new BicikliDataClassesDataContext();
                var bikeToInsert = new Bike();

                bikeToInsert.description = m.description;
                bikeToInsert.is_active = m.isActive;
                bikeToInsert.name = m.name;

                if (MyLenders > -1)
                {
                    try
                    {
                        DataRepository.GetLendersOfUser((Guid)Membership.GetUser().ProviderUserKey).Single(l => l.id == MyLenders);
                        bikeToInsert.current_lender_id = MyLenders;
                    }
                    catch
                    {
                        ModelState.AddModelError("", "A választott kölcsönző nincs Önhöz rendelve.");
                        throw new Exception();
                    }
                }

                db.Bikes.InsertOnSubmit(bikeToInsert);
                db.SubmitChanges();

                if (ImgFile != null)
                {
                    if (ImgFile != null && ImgFile.ContentLength > 0)
                    {
                        var fileName = DateTime.Now.Ticks.ToString() + "---" + bikeToInsert.id.ToString() + "---" + Path.GetFileName(ImgFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/uploads"), fileName);
                        ImgFile.SaveAs(path);
                        bikeToInsert.image_url = fileName;
                    }
                }
                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-bikes";
                var favouriteLender = Request.Cookies.Get("favourite_lender");

                #region Create dropdown list for selectable lenders

                var myLenders = new List<SelectListItem>();
                myLenders.Add(new SelectListItem()
                {
                    Text = "-- Használaton kívül --",
                    Value = "-1",
                    Selected = ((favouriteLender == null) || (favouriteLender.Value == "-1"))
                });
                foreach (var item in DataRepository.GetAssignedLenders(User.Identity.Name))
                {
                    myLenders.Add(new SelectListItem()
                    {
                        Text = item.name,
                        Value = item.id.ToString(),
                        Selected = ((favouriteLender != null) && (favouriteLender.Value == item.id.ToString()))
                    });
                }

                ViewBag.MyLenders = myLenders;

                #endregion

                return View(m);
            }
        }

        //
        // GET: /Bikes/Edit/5

        public ActionResult Edit(int id)
        {
            BikeModel bike;
            try
            {
                bike = DataRepository.GetBike(id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            ViewBag.active_menu_item_id = "menu-btn-bikes";

            if (bike.currentLenderId != null)
            {
                try
                {
                    DataRepository.GetLendersOfUser((Guid)Membership.GetUser().ProviderUserKey).Single(l => l.id == bike.currentLenderId);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            #region Create dropdown list for selectable lenders

            var myLenders = new List<SelectListItem>();
            myLenders.Add(new SelectListItem()
            {
                Text = "-- Használaton kívül --",
                Value = "-1",
                Selected = (bike.currentLenderId == null)
            });
            foreach (var item in DataRepository.GetAssignedLenders(User.Identity.Name))
            {
                myLenders.Add(new SelectListItem()
                {
                    Text = item.name,
                    Value = item.id.ToString(),
                    Selected = (bike.currentLenderId == item.id)
                });
            }
            ViewBag.MyLenders = myLenders;

            #endregion

            return View(bike);
        }

        //
        // POST: /Bikes/Edit/5

        [HttpPost]
        public ActionResult Edit(BikeModel m, int MyLenders = -1, HttpPostedFileBase ImgFile = null)
        {
            var bike = DataRepository.GetBike((int)m.id);
            if (bike.currentLenderId != null)
            {
                try
                {
                    DataRepository.GetLendersOfUser((Guid)Membership.GetUser().ProviderUserKey).Single(l => l.id == bike.currentLenderId);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                var db = new BicikliDataClassesDataContext();
                var bikeToUpdate = db.Bikes.Single(b => b.id == m.id);

                if (bikeToUpdate.name != m.name)
                {
                    bikeToUpdate.name = m.name;
                }
                if (bikeToUpdate.description != m.description)
                {
                    bikeToUpdate.description = m.description;
                }
                if (DataRepository.GetBike((int)m.id).session == null)
                {
                    if (MyLenders < 0)
                    {
                        bikeToUpdate.current_lender_id = null;
                    }
                    else if (bikeToUpdate.current_lender_id != MyLenders)
                    {
                        try
                        {
                            DataRepository.GetLendersOfUser((Guid)Membership.GetUser().ProviderUserKey).Single(l => l.id == MyLenders);
                            bikeToUpdate.current_lender_id = MyLenders;
                        }
                        catch
                        {
                            ModelState.AddModelError("", "A választott kölcsönző nincs Önhöz rendelve.");
                            throw new Exception();
                        }
                    }
                }
                if (bikeToUpdate.is_active != m.isActive)
                {
                    bikeToUpdate.is_active = m.isActive;
                }
                if (ImgFile != null)
                {
                    if (ImgFile != null && ImgFile.ContentLength > 0)
                    {
                        var fileName = DateTime.Now.Ticks.ToString() + "---" + m.id.ToString() + "---" + Path.GetFileName(ImgFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/uploads"), fileName);
                        ImgFile.SaveAs(path);
                        bikeToUpdate.image_url = fileName;
                    }
                }
                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-bikes";

                #region Create dropdown list for selectable lenders

                var myLenders = new List<SelectListItem>();
                myLenders.Add(new SelectListItem()
                {
                    Text = "-- Használaton kívül --",
                    Value = "-1",
                    Selected = (m.currentLenderId == null)
                });
                foreach (var item in DataRepository.GetAssignedLenders(User.Identity.Name))
                {
                    myLenders.Add(new SelectListItem()
                    {
                        Text = item.name,
                        Value = item.id.ToString(),
                        Selected = (m.currentLenderId == item.id)
                    });
                }
                ViewBag.MyLenders = myLenders;

                #endregion

                return View(m);
            }
        }

        //
        // GET: /Bikes/Delete/5

        public ActionResult Delete(int id)
        {
            BikeModel bike;
            try
            {
                bike = DataRepository.GetBike(id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            ViewBag.active_menu_item_id = "menu-btn-bikes";

            if (bike.currentLenderId != null)
            {
                try
                {
                    DataRepository.GetLendersOfUser((Guid)Membership.GetUser().ProviderUserKey).Single(l => l.id == bike.currentLenderId);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
            return View(bike);
        }

        //
        // POST: /Bikes/Delete/5

        [HttpPost]
        public ActionResult Delete(BikeModel m)
        {
            try
            {
                var bikeToDelete = DataRepository.GetBike((int)m.id);

                if (bikeToDelete.currentLenderId != null)
                {
                    try
                    {
                        DataRepository.GetLendersOfUser((Guid)Membership.GetUser().ProviderUserKey).Single(l => l.id == bikeToDelete.currentLenderId);
                    }
                    catch
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                    }
                }

                if (bikeToDelete.lastLendingDate != null)
                {
                    throw new Exception();
                }

                var db = new BicikliDataClassesDataContext();
                db.Bikes.DeleteOnSubmit(db.Bikes.Single(b => b.id == m.id));
                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-bikes";
                return View(m);
            }
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