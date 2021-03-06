﻿using Hangfire;
using Hangfire.MemoryStorage;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Wlog.Web.Code.Helpers;
using Wlog.BLL.Classes;
using Wlog.DAL.NHibernate.Helpers;
using Wlog.Library.BLL.Reporitories;
using Wlog.Library.BLL.Configuration;

namespace Wlog.Web
{
    // Nota: per istruzioni su come abilitare la modalità classica di IIS6 o IIS7, 
    // visitare il sito Web all'indirizzo http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        //Todo: Move nhibernate init stuff in a dedicated class. Implement a DataContext or Operation manager pattern to wrap data access
      
        private BackgroundJobServer _backgroundJobServer;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(System.Web.Http.GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            //Apply schema changes
            //RepositoryContext.Current.System.ApplySchemaChanges();

            //Setup infopage configuration

            InfoPageConfigurator.Configure(c => {
                c.ApplicationName = "Wlog";
                
            });

           
            JobStorage.Current=new MemoryStorage();
            //Hangfire.GlobalConfiguration.Configuration.UseNLogLogProvider();
            _backgroundJobServer = new BackgroundJobServer();



            RecurringJob.AddOrUpdate(() => LogQueue.Current.Run(), "*/1 * * * *");

            SystemDataHelper.InsertRoles();
            SystemDataHelper.EnsureSampleData();

 

        }

        protected void Application_End(object sender, EventArgs e)
        {
            _backgroundJobServer.Dispose();
        }
    }
}