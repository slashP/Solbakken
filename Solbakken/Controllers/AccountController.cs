using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CodeFirstMembershipSharp;
using Solbakken.Models;
using Solbakken.Util;

namespace Solbakken.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {

        private readonly CodeFirstMembershipProvider _membershipProvider = new CodeFirstMembershipProvider();
        //
        // GET: /Account/LogOn

        [AllowAnonymous]
        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [AllowAnonymous]
        [HttpPost]
        public ActionResult LogOn(string ReturnUrl, LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var req = Request;
                if (_membershipProvider.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Brukernavnet eller passordet er feil.");
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
        // POST: /Account/Register

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                _membershipProvider.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
                    MailUtil.SendRegisteredEmail(model.Email, model.UserName);
                    return RedirectToAction("Index", "Home");
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
                    MembershipUser currentUser = _membershipProvider.GetUser(User.Identity.Name, userIsOnline: true);
                    changePasswordSucceeded = _membershipProvider.ChangePassword(currentUser.UserName, model.OldPassword, model.NewPassword);
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
                    ModelState.AddModelError("", "Enten er det nåværende passordet feil eller så er det nye passordet ugyldig.");
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
            return ModelState.SelectMany(x => x.Value.Errors
                .Select(error => error.ErrorMessage));
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Brukernavnet finnes allerede. Velg et annet.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Denne e-postadressen finnes allerede. Forsøk å logge inn eller resette passordet.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Passordet er ikke gyldig. Forsøk å legge inn et gyldig passord.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Dette er ikke en gyldig e-postadresse. Sikker på at du har skrevet rett?";

                case MembershipCreateStatus.InvalidUserName:
                    return "Brukernavnet er ugyldig. Prøv å ikke bruke avanserte tegn.";

                default:
                    return "En ukjent feil oppsto. Hvis feilen fortsetter kan du kontakte perkristianhelland (at) gmail.com";
            }
        }
        #endregion

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(RegisterModel model)
        {
            var db = new DataContext();
            var user = db.Users.FirstOrDefault(x => x.Email == model.Email);
            
            if(user != null)
            {
                var password = RandomPassword.Generate();
                var membership = new CodeFirstMembershipProvider();
                if (Request.Url != null)
                {
                    MailUtil.SendNewPassword(password, model.Email, Request.Url.Host + Url.Action("ChangePassword"), user.Username);
                    if(membership.ChangePasswordForce(user.Email, password))
                    {
                        return View("ResetSuccess", model);                        
                    }
                }
            }
            ViewBag.Fail = "Fant ikke noen bruker med den epost-adressen. Har du skrevet feil?";
            return View();
        }
    }
}
