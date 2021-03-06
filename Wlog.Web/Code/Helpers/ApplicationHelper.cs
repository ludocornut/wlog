﻿using PagedList;
using System;
using System.Collections.Generic;
using Wlog.Web.Models;
using Wlog.Library.BLL.Reporitories;
using Wlog.BLL.Entities;
using Wlog.Library.BLL.Classes;

namespace Wlog.Web.Code.Helpers
{
    public class ApplicationHelper
    {
        /// <summary>
        /// Get Application by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ApplicationModel GetById(Guid id)
        {
            ApplicationEntity app = RepositoryContext.Current.Applications.GetById(id);
            if (app == null) return null;
            return EntityToModel(app);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serchFilter"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public static IPagedList<ApplicationModel> FilterApplicationList(string serchFilter, int pagenumber, int pagesize)
        {
            ApplicationSearchSettings settings = new ApplicationSearchSettings()
            {
                Orderby=Library.BLL.Enums.ApplicationFields.ApplicationName,
                PageNumber=pagenumber,
                PageSize=pagesize,
                SerchFilter=serchFilter
            };
                
            List<ApplicationModel> data = new List<ApplicationModel>();

            IPagedList<ApplicationEntity> queryResult = RepositoryContext.Current.Applications.Search(settings);

            foreach (ApplicationEntity ae in queryResult)
            {
                data.Add(EntityToModel(ae));
            }

            return new StaticPagedList<ApplicationModel>(data, pagenumber, pagesize,queryResult.TotalItemCount);
        }

        public static bool DeleteById(Guid id)
        {
            try
            {
                ApplicationEntity app = RepositoryContext.Current.Applications.GetById(id);
                RepositoryContext.Current.Applications.Delete(app);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private static ApplicationModel EntityToModel(ApplicationEntity entity)
        {
            ApplicationModel result = new ApplicationModel();
            result.ApplicationName = entity.ApplicationName;
            result.EndDate = entity.EndDate;
            result.IdApplication = entity.IdApplication;
            result.IsActive = entity.IsActive;
            result.StartDate = entity.StartDate;
            result.PublicKey = entity.PublicKey;
            return result;
        }
    }
}