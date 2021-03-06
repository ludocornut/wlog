﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Wlog.BLL.Classes;
using Wlog.BLL.Entities;
using Wlog.Library.BLL.Reporitories;
using Wlog.Web.Code.Authentication;
using Wlog.Web.Code.Helpers;
using Wlog.Web.Models;
using Wlog.Web.Models.User;


namespace Wlog.Web.Controllers
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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            WLogMembershipProvider provider = new WLogMembershipProvider();
            if (provider.ValidateUser(model.UserName, model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.UserName, true);
                if (returnUrl == null) returnUrl = "~/private";
                return RedirectToLocal(returnUrl);
            }

            // Se si arriva a questo punto, significa che si è verificato un errore, rivisualizzare il form
            ModelState.AddModelError("", "Il nome utente o la password fornita non è corretta.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Account");
        }


        //Get: /Account/Manage

        [HttpGet]
        public ActionResult Manage()
        {
            LocalPasswordModel model = new LocalPasswordModel();
            model.email.address = UserProfileContext.Current.User.Email;
            return View(model);
        }

        //Post: /Account/ChangePassword

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(LocalPasswordModel.Password model)
        {
            WLogMembershipProvider provider = new WLogMembershipProvider();
            if (!provider.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
            {
                UserProfileContext.Current.Refresh();
                ModelState.AddModelError("", "Error");
                RedirectToAction("Manage");
            }
            else
            {
                return RedirectToAction("Index", "Private");
            }
            return View(new LocalPasswordModel());
        }

        //Post: /Account/ChangeEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEmail(LocalPasswordModel.Email model)
        {
            if (ModelState.IsValid)
            {
               
                    UserEntity entity = RepositoryContext.Current.Users.GetByUsername(User.Identity.Name);
                    entity.Email = model.address;
                    entity.LastActivityDate = DateTime.Now;
                RepositoryContext.Current.Users.Save(entity);

              UserProfileContext.Current.Refresh();
                return RedirectToAction("Index", "Private");
            }
            else
            {
                ModelState.AddModelError("", "Error");
            }
            
            return View();
        }

        //Post: /Account/Info
        [HttpGet]
        public ActionResult Info()
        {
            ViewBag.Title = UserProfileContext.Current.User.Username;
            EditUser model = new EditUser();
            model.DataUser = UserProfileContext.Current.User;
            model.Apps = UserHelper.GetApp(UserProfileContext.Current.User.Id);
            return View(model);
        }

        //
        // GET: /Account/Register

        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        //
        // POST: /Account/Register

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult Register(RegisterModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Tentare di registrare l'utente
        //        try
        //        {
        //            MembershipCreateStatus status;
        //            WLogMembershipProvider provider = new WLogMembershipProvider();
        //            provider.CreateUser(model.UserName, model.Password,null, null, null, true, null, out status);
        //            if (status == MembershipCreateStatus.Success)
        //            {
        //                return RedirectToAction("Index", "Private");
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", ErrorCodeToString(status));
        //            }
        //           // WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
        //           // WebSecurity.Login(model.UserName, model.Password);
         
        //        }
        //        catch (MembershipCreateUserException e)
        //        {
        //            ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
        //        }
        //    }

        //    // Se si arriva a questo punto, significa che si è verificato un errore, rivisualizzare il form
        //    return View(model);
        //}

        

        #region Helper
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Private");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }


        //private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        //{
        //    // Vedere http://go.microsoft.com/fwlink/?LinkID=177550 per
        //    // un elenco completo di codici di stato.
        //    switch (createStatus)
        //    {
        //        case MembershipCreateStatus.DuplicateUserName:
        //            return "Il nome utente esiste già. Immettere un nome utente differente.";

        //        case MembershipCreateStatus.DuplicateEmail:
        //            return "Un nome utente per l'indirizzo di posta elettronica esiste già. Immettere un nome utente differente.";

        //        case MembershipCreateStatus.InvalidPassword:
        //            return "La password fornita non è valida. Immettere un valore valido per la password.";

        //        case MembershipCreateStatus.InvalidEmail:
        //            return "L'indirizzo di posta elettronica fornito non è valido. Controllare il valore e riprovare.";

        //        case MembershipCreateStatus.InvalidAnswer:
        //            return "La risposa fornita per il recupero della password non è valida. Controllare il valore e riprovare.";

        //        case MembershipCreateStatus.InvalidQuestion:
        //            return "La domanda fornita per il recupero della password non è valida. Controllare il valore e riprovare.";

        //        case MembershipCreateStatus.InvalidUserName:
        //            return "Il nome utente fornito non è valido. Controllare il valore e riprovare.";

        //        case MembershipCreateStatus.ProviderError:
        //            return "Il provider di autenticazione ha restituito un errore. Verificare l'immissione e riprovare. Se il problema persiste, contattare l'amministratore di sistema.";

        //        case MembershipCreateStatus.UserRejected:
        //            return "La richiesta di creazione dell'utente è stata annullata. Verificare l'immissione e riprovare. Se il problema persiste, contattare l'amministratore di sistema.";

        //        default:
        //            return "Si è verificato un errore sconosciuto. Verificare l'immissione e riprovare. Se il problema persiste, contattare l'amministratore di sistema.";
        //    }
        //}
        #endregion
    }
}
