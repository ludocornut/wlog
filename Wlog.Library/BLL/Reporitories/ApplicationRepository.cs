﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using Wlog.BLL.Entities;
using Wlog.Library.BLL.Classes;
using Wlog.Library.BLL.Interfaces;
using Wlog.DAL.NHibernate.Helpers;
using Wlog.BLL.Classes;
using Wlog.Library.BLL.DataBase;

namespace Wlog.Library.BLL.Reporitories
{
    public class ApplicationRepository:IRepository
    {
        private static UnitFactory _UnitFactory;

        public ApplicationRepository()
        {
            _UnitFactory = new UnitFactory();
        }

        public ApplicationEntity GetById(Guid id)
        {
            ApplicationEntity app;
            using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
            {
                uow.BeginTransaction();
                app = uow.Query<ApplicationEntity>().Where(x => x.IdApplication.Equals(id)).FirstOrDefault();
                return app;
            }
        }

        public void ResetUserRoles(UserEntity entity, List<AppUserRoleEntity> role)
        {
            try
            {
                Guid idapp=role.Select(x=>x.ApplicationId).Distinct().First();
                using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
                {
                    uow.BeginTransaction();
                    List<AppUserRoleEntity> deleterole = uow.Query<AppUserRoleEntity>().Where(x => x.UserId.Equals(entity.Id) && x.ApplicationId.Equals(idapp)).ToList();
                    foreach (AppUserRoleEntity del in deleterole)
                    {
                        uow.Delete(del);
                    }

                    foreach (AppUserRoleEntity rol in role)
                    {
                        uow.SaveOrUpdate(rol);
                    }

                    uow.Commit();

                }
            }
            catch (Exception err)
            {
                //log here
            }
        }

        public IPagedList<ApplicationEntity> Search(ApplicationSearchSettings searchSettings)
        {
            List<ApplicationEntity> entity;
            using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
            {
                uow.BeginTransaction();
                if (string.IsNullOrEmpty(searchSettings.SerchFilter))
                {
                    entity = uow.Query<ApplicationEntity>().OrderBy(x => x.ApplicationName).ToList();
                }
                else
                {
                    entity = uow.Query<ApplicationEntity>().Where(x => x.ApplicationName.Contains(searchSettings.SerchFilter)).OrderBy(x => x.ApplicationName).ToList();
                }

                //foreach (ApplicationEntity e in entity)
                //{
                //    AppUserRoleEntity r = uow.Query<AppUserRoleEntity>().Where(x => (x.Role.RoleName == Constants.Roles.Admin || x.Role.RoleName == Constants.Roles.Write) && x.Application.IdApplication == e.IdApplication && x.User.Id == UserProfileContext.Current.User.Id).FirstOrDefault();
                //    if (UserProfileContext.Current.User.IsAdmin || r != null)
                //        data.Add(EntityToModel(e));
                //}
            }

            return new StaticPagedList<ApplicationEntity>(entity, searchSettings.PageNumber, searchSettings.PageSize, entity.Count);
        }

        public void Delete(ApplicationEntity app)
        {
            using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
            {
                uow.BeginTransaction();
                ApplicationEntity appToDelete = uow.Query<ApplicationEntity>().Where(x => x.IdApplication.Equals(app.IdApplication)).FirstOrDefault();

                List<LogEntity> logs = uow.Query<LogEntity>().Where(x => x.ApplictionId.Equals( app.IdApplication)).ToList();
                foreach (LogEntity e in logs)
                {
                    uow.Delete(e);
                }

                uow.Delete(app);

                uow.Commit();
            }
        }

        public List<ApplicationEntity> GetAppplicationsByUsername(string userName)
        {
            UserEntity user = RepositoryContext.Current.Users.GetByUsername(userName);
            List<Guid> Ids = GetAppplicationsIdsForUser(user);
            return RepositoryContext.Current.Applications.GetByIds(Ids);


        }

        public List<ApplicationEntity> GetByIds(List<Guid> ids)
        {
            List<ApplicationEntity> result = new List<ApplicationEntity>();
            using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
            {
                uow.BeginTransaction();
                foreach (Guid id in ids)
                {
                    result.Add(GetById(id));
                }
                
            }
            return result;
        }

        public void Save(ApplicationEntity app)
        {
            using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
            {
                uow.BeginTransaction();
                uow.SaveOrUpdate(app);
                uow.Commit();
            }
        }

        public List<Guid> GetAppplicationsIdsByUsername(string userName)
        {
            UserEntity user = RepositoryContext.Current.Users.GetByUsername(userName);
            List<Guid> Ids = GetAppplicationsIdsForUser(user);
            return Ids;
        }


        public List<Guid> GetAppplicationsIdsForUser(UserEntity user)
        {
            return GetAppplicationForUser(user).Select(x => x.IdApplication).ToList();

        }

        public List<ApplicationEntity> GetAppplicationForUser(UserEntity user)
        {
            using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
            {
                uow.BeginTransaction();
                List<ApplicationEntity> applications;
                if (UserProfileContext.Current.User.IsAdmin)
                {
                    applications = uow.Query<ApplicationEntity>().ToList();
                }
                else
                {
                    List<Guid> appLinks = uow
                        .Query<AppUserRoleEntity>()
                        .Where(x => x.UserId == UserProfileContext.Current.User.Id)
                        .Select(x => x.ApplicationId)
                        .ToList();

                    applications = uow
                        .Query<ApplicationEntity>()
                        .Where(x => appLinks.Contains(x.IdApplication))
                        .ToList();
                }
                return applications;
            }
        }



       
 
        public ApplicationEntity GetByApplicationKey(string applicationKey)
        {
            Guid pk = new Guid(applicationKey);
            using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
            {
                uow.BeginTransaction();
                return uow.Query<ApplicationEntity>().Where(x => x.IdApplication.Equals(pk) || x.PublicKey.Equals(pk)).FirstOrDefault();

            }
        }

        public bool AssignRoleToUser(ApplicationEntity application, UserEntity user, RolesEntity role)
        {
            try
            {
                using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
                {
                    uow.BeginTransaction();
                    if (!uow.Query<AppUserRoleEntity>().Any(x => x.ApplicationId.Equals(application.IdApplication) && x.RoleId.Equals(role.Id) && x.UserId.Equals(user.Id)))
                    {
                        AppUserRoleEntity app = new AppUserRoleEntity();
                        app.ApplicationId = app.ApplicationId;
                        app.RoleId = role.Id;
                        app.UserId = user.Id;
                        uow.SaveOrUpdate(app);

                        uow.Commit();
                        return true;
                    }

                }
            }
            catch (Exception err)
            {
                //TODO:Add log here
                return false;
            }
            return true;
        }
    }
}
