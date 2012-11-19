using System;
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

            try
            {
                CreateSelectableLenders(int.Parse(favouriteLender.Value));
            }
            catch
            {
                CreateSelectableLenders(null);
            }

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

                var bikeModelToInsert = new BikeModel(m);

                if (MyLenders > -1)
                {
                    try
                    {
                        DataRepository.GetLendersOfUser((Guid)Membership.GetUser().ProviderUserKey).Single(l => l.id == MyLenders);
                        bikeModelToInsert.currentLenderId = MyLenders;
                    }
                    catch
                    {
                        ModelState.AddModelError("", "A választott kölcsönző nincs Önhöz rendelve.");
                        throw new Exception();
                    }
                }

                bikeModelToInsert = DataRepository.GetBike(DataRepository.InsertBike(bikeModelToInsert));
                bikeModelToInsert.imageUrl = UploadImage(ImgFile, (int)bikeModelToInsert.id);
                DataRepository.UpdateBike(bikeModelToInsert);

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-bikes";
                var favouriteLender = Request.Cookies.Get("favourite_lender");

                try
                {
                    CreateSelectableLenders(int.Parse(favouriteLender.Value));
                }
                catch
                {
                    CreateSelectableLenders(null);
                }

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

            CreateSelectableLenders(bike.currentLenderId);
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

                var bikeModelToUpdate = DataRepository.GetBike((int)m.id);
                bikeModelToUpdate.name = m.name;
                bikeModelToUpdate.description = m.description;
                bikeModelToUpdate.isActive = m.isActive;

                if (bikeModelToUpdate.session == null)
                {
                    if (MyLenders < 0)
                    {
                        bikeModelToUpdate.currentLenderId = null;
                    }
                    else if (bikeModelToUpdate.currentLenderId != MyLenders)
                    {
                        try
                        {
                            DataRepository.GetLendersOfUser((Guid)Membership.GetUser().ProviderUserKey).Single(l => l.id == MyLenders);
                            bikeModelToUpdate.currentLenderId = MyLenders;
                        }
                        catch
                        {
                            ModelState.AddModelError("", "A választott kölcsönző nincs Önhöz rendelve.");
                            throw new Exception();
                        }
                    }
                }
                bikeModelToUpdate.imageUrl = UploadImage(ImgFile, (int)m.id);

                DataRepository.UpdateBike(bikeModelToUpdate);
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-bikes";
                CreateSelectableLenders(m.currentLenderId);
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

                DataRepository.DeleteBike((int)m.id);

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

        /// <summary>
        /// Creates a DDL for selectable lenders
        /// </summary>
        private void CreateSelectableLenders(int? selectedId)
        {
            var myLenders = new List<SelectListItem>();
            myLenders.Add(new SelectListItem()
            {
                Text = "-- Használaton kívül --",
                Value = "-1",
                Selected = (selectedId == null)
            });
            foreach (var item in DataRepository.GetAssignedLenders(User.Identity.Name))
            {
                myLenders.Add(new SelectListItem()
                {
                    Text = item.name,
                    Value = item.id.ToString(),
                    Selected = (selectedId == item.id)
                });
            }
            ViewBag.MyLenders = myLenders;
        }

        /// <summary>
        /// Uploads an image
        /// </summary>
        /// <param name="ImgFile"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private String UploadImage(HttpPostedFileBase ImgFile, int id)
        {
            if (ImgFile != null)
            {
                if (ImgFile != null && ImgFile.ContentLength > 0)
                {
                    var fileName = DateTime.Now.Ticks.ToString() + "---" + id.ToString() + "---" + Path.GetFileName(ImgFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/uploads"), fileName);
                    ImgFile.SaveAs(path);
                    return fileName;
                }
            }

            return null;
        }
    }
}
