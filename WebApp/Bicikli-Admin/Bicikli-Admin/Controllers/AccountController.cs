using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Bicikli_Admin.Models;

namespace Bicikli_Admin.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = Membership.GetUser(model.UserName);
                if (user != null && user.IsLockedOut)
                {
                    ModelState.AddModelError("", "A felhasználó ki van tiltva.");
                }
                else if (user != null && !user.IsApproved)
                {
                    ModelState.AddModelError("", "A felhasználó nincs jóváhagyva.");
                }
                else if (Membership.ValidateUser(model.UserName, model.Password))
                {

                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "A felhasználónév vagy a jelszó nem megfelelő.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // GET: /Account/RegisterSuccess

        [AllowAnonymous]
        public ActionResult RegisterSuccess()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: false, providerUserKey: null, status: out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    return RedirectToAction("RegisterSuccess");
                }
                else if (createStatus == MembershipCreateStatus.DuplicateUserName)
                {
                    ModelState.AddModelError("", "A felhasználónév foglalt.");
                }
                else if (createStatus == MembershipCreateStatus.InvalidEmail)
                {
                    ModelState.AddModelError("", "Az E-Mail cím formátuma nem megfelelő.");
                }
                else if (createStatus == MembershipCreateStatus.InvalidPassword)
                {
                    ModelState.AddModelError("", "A jelszó nem megfelelő.");
                }
                else if (createStatus == MembershipCreateStatus.InvalidUserName)
                {
                    ModelState.AddModelError("", "A felhasználónév formátuma nem megfelelő.");
                }
                else if (createStatus == MembershipCreateStatus.UserRejected)
                {
                    ModelState.AddModelError("", "A felhasználó nem hozható létre, kérjük lépjen kapcsolatba az üzemeltetővel.");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, userIsOnline: true);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "A jelenlegi vagy az új jelszó nem megfelelő.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Ez a felhasználónév már foglalt. Válasszon másikat!";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Ez az e-mail cím már regisztrálva van.";

                case MembershipCreateStatus.InvalidPassword:
                    return "A beírt jelszó nem megfelelő.";

                case MembershipCreateStatus.InvalidEmail:
                    return "A beírt e-mail cím nem megfelelő.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "A biztonsági válasz nem megfelelő.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "A biztonsági kérdés nem megfelelő.";

                case MembershipCreateStatus.InvalidUserName:
                    return "A felhasználónév nem megfelelő.";

                case MembershipCreateStatus.ProviderError:
                    return "Az autentikációs folyamat során hiba történt. Kérjük próbálja újra később vagy vegye fel a kapcsolatot az üzemeltetővel!";

                case MembershipCreateStatus.UserRejected:
                    return "A felhasználó nem hozható létre. Kérjük próbálja újra később vagy vegye fel a kapcsolatot az üzemeltetővel!";

                default:
                    return "Meghatározhatatlan hiba történt. Kérjük próbálja újra később vagy vegye fel a kapcsolatot az üzemeltetővel!";
            }
        }
        #endregion
    }
}
