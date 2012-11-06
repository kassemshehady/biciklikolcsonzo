using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Bicikli_Admin.CommonClasses;
using Bicikli_Admin.EntityFramework.linq;
using Bicikli_Admin.Models;

namespace Bicikli_Admin.Controllers
{
    public class LendersController : Controller
    {
        //
        // GET: /Lenders/

        public ActionResult Index()
        {
            ViewBag.active_menu_item_id = "menu-btn-lenders";
            return View(DataRepository.GetLenders());
        }

        //
        // GET: /Lenders/MyLenders

        public ActionResult MyLenders()
        {
            ViewBag.active_menu_item_id = "menu-btn-lenders";
            return View(DataRepository.GetAssignedLenders(User.Identity.Name));
        }

        //
        // GET: /Lenders/Details/5

        public ActionResult Details(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-lenders";
            try
            {
                var lenderToShow = DataRepository.GetLender(id);

                #region Get selected users from db

                var selectedUsers = new List<string>();
                foreach (UserModel usermodel in DataRepository.GetLenderAssignedUsers(id))
                {
                    selectedUsers.Add(usermodel.username);
                }
                ViewBag.SelectedUsers = selectedUsers;

                #endregion

                return View(lenderToShow);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Lenders/Create

        public ActionResult Create()
        {
            ViewBag.active_menu_item_id = "menu-btn-lenders";
            var users = DataRepository.GetUsers();
            ViewBag.Users = users;
            return View(new LenderModel());
        }

        //
        // POST: /Lenders/Create

        [HttpPost]
        public ActionResult Create(LenderModel model, string[] users)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var dbUsers = DataRepository.GetUsers();
                    ViewBag.Users = dbUsers;
                    ViewBag.SelectedUsers = users;
                    ViewBag.active_menu_item_id = "menu-btn-lenders";
                    return View(model);
                }

                #region Insert Logic

                // Step 1: Extract selected user guids
                var guidsToInsert = new HashSet<Guid>();
                if (users != null)
                {
                    foreach (string username in users)
                    {
                        var msUser = Membership.GetUser(username);
                        if (msUser != null)
                        {
                            Guid msUserGuid = (Guid)msUser.ProviderUserKey;
                            if (!guidsToInsert.Contains(msUserGuid))
                            {
                                guidsToInsert.Add(msUserGuid);
                            }
                        }
                    }
                }

                // Step 2: Create Lender
                var db = new BicikliDataClassesDataContext();
                var lenderToInsert = new Lender();
                lenderToInsert.name = model.name;
                lenderToInsert.address = model.address;
                lenderToInsert.description = model.description;
                lenderToInsert.printer_ip = model.printer_ip;
                lenderToInsert.latitude = model.latitude;
                lenderToInsert.longitude = model.longitude;
                db.Lenders.InsertOnSubmit(lenderToInsert);
                db.SubmitChanges();

                // Step 3: Add users to Lender
                foreach (Guid guid in guidsToInsert)
                {
                    var assignment = new LenderUser();
                    assignment.lender_id = lenderToInsert.id;
                    assignment.user_id = guid;
                    db.LenderUsers.InsertOnSubmit(assignment);
                }
                db.SubmitChanges();

                // Step 4: Return to list of Lenders
                return RedirectToAction("Index");

                #endregion
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-lenders";
                ModelState.AddModelError("", "A kölcsönző létrehozása belső hiba miatt sikertelen volt.");

                var dbUsers = DataRepository.GetUsers();
                ViewBag.Users = dbUsers;
                ViewBag.SelectedUsers = users;
                return View(model);
            }
        }

        //
        // GET: /Lenders/Edit/5

        public ActionResult Edit(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-lenders";
            var users = DataRepository.GetUsers();
            ViewBag.Users = users;
            try
            {
                var lenderToEdit = DataRepository.GetLender(id);

                #region Get selected users from db

                var selectedUsers = new List<string>();
                foreach (UserModel usermodel in DataRepository.GetLenderAssignedUsers(id))
                {
                    selectedUsers.Add(usermodel.username);
                }
                ViewBag.SelectedUsers = selectedUsers;

                #endregion

                return View(lenderToEdit);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Lenders/Edit/5

        [HttpPost]
        public ActionResult Edit(LenderModel model, string[] users)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var dbUsers = DataRepository.GetUsers();
                    ViewBag.Users = dbUsers;
                    ViewBag.SelectedUsers = users;
                    ViewBag.active_menu_item_id = "menu-btn-lenders";
                    return View(model);
                }

                #region Update Logic

                var db = new BicikliDataClassesDataContext();
                var lenderToUpdate = db.Lenders.First(l => l.id == model.id);

                // Step 1: Extract selected user guids
                var guidsToInsert = new HashSet<Guid>();
                if (users != null)
                {
                    foreach (string username in users)
                    {
                        var msUser = Membership.GetUser(username);
                        if (msUser != null)
                        {
                            Guid msUserGuid = (Guid)msUser.ProviderUserKey;
                            if (!guidsToInsert.Contains(msUserGuid))
                            {
                                guidsToInsert.Add(msUserGuid);
                            }
                        }
                    }
                }

                // Step 2: Apply changes to lender data
                lenderToUpdate.address = model.address;
                lenderToUpdate.description = model.description;
                lenderToUpdate.latitude = model.latitude;
                lenderToUpdate.longitude = model.longitude;
                lenderToUpdate.name = model.name;
                lenderToUpdate.printer_ip = model.printer_ip;

                // Step 3: Apply changes to assignments

                // Insert new assignments
                var lenderGuidAssignments = DataRepository.GetLenderAssignedGuids(lenderToUpdate.id);
                foreach (Guid guid in guidsToInsert)
                {
                    if (!lenderGuidAssignments.Contains(guid))
                    {
                        var luToInsert = new LenderUser();
                        luToInsert.user_id = guid;
                        luToInsert.lender_id = lenderToUpdate.id;
                        db.LenderUsers.InsertOnSubmit(luToInsert);
                    }
                }

                // Delete removed assignments
                lenderGuidAssignments = DataRepository.GetLenderAssignedGuids(lenderToUpdate.id);
                foreach (Guid guid in lenderGuidAssignments)
                {
                    if (!guidsToInsert.Contains(guid))
                    {
                        var luToDelete = db.LenderUsers.Where(lu => (lu.lender_id == lenderToUpdate.id) && (lu.user_id == guid)).First();
                        db.LenderUsers.DeleteOnSubmit(luToDelete);
                    }
                }

                // Step 4: Commit changes
                db.SubmitChanges();

                return RedirectToAction("Index");

                #endregion
            }
            catch
            {
                ViewBag.active_menu_item_id = "menu-btn-lenders";
                ModelState.AddModelError("", "A kölcsönző létrehozása belső hiba miatt sikertelen volt.");

                var dbUsers = DataRepository.GetUsers();
                ViewBag.Users = dbUsers;
                ViewBag.SelectedUsers = users;
                return View(model);
            }
        }

        //
        // GET: /Lenders/Delete/5

        public ActionResult Delete(int id)
        {
            ViewBag.active_menu_item_id = "menu-btn-lenders";
            try
            {
                var lenderToShow = DataRepository.GetLender(id);

                #region Get selected users from db

                var selectedUsers = new List<string>();
                foreach (UserModel usermodel in DataRepository.GetLenderAssignedUsers(id))
                {
                    selectedUsers.Add(usermodel.username);
                }
                ViewBag.SelectedUsers = selectedUsers;

                #endregion

                return View(lenderToShow);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Lenders/Delete/5

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
                ViewBag.active_menu_item_id = "menu-btn-lenders";
                return View();
            }
        }
    }
}
