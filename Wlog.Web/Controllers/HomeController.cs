﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wlog.Web.Models;
using Wlog.Web.Code.Authentication;
using Wlog.Web.Code.Helpers;
using System.Web.Security;
using Wlog.BLL.Entities;
using Wlog.DAL.NHibernate.Helpers;
using Wlog.BLL.Classes;
using Wlog.Library.BLL.Reporitories;

namespace Wlog.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
    
            List<ApplicationHomeModel> result = new List<ApplicationHomeModel>();
            if (UserProfileContext.Current.User != null)
            {
                WLogRoleProvider roleProvider = new WLogRoleProvider();

                UserEntity currentUser=RepositoryContext.Current.Users.GetByUsername(Membership.GetUser().UserName);
                List<ApplicationEntity> apps = RepositoryContext.Current.Applications.GetAppplicationForUser(currentUser);
                result.AddRange(ConversionHelper.ConvertListEntityToListApplicationHome(apps));

            }
            return View(result);
        }

        //
        // GET: /Home/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Home/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Home/Create

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
        // GET: /Home/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Home/Edit/5

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
        // GET: /Home/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Home/Delete/5

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
