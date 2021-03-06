﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using Wlog.BLL.Entities;
using Wlog.Library.BLL.Classes;
using Wlog.Library.BLL.Interfaces;
using Wlog.Library.BLL.DataBase;
using Wlog.DAL.NHibernate.Helpers;
using MongoDB.Driver.Linq;

namespace Wlog.Library.BLL.Reporitories
{
    public class UserRepository : IRepository
    {

        private static UnitFactory _UnitFactory;

        public UserRepository()
        {
            _UnitFactory = new UnitFactory();
        }
        public UserEntity GetById(Guid id)
        {
            UserEntity entity = null;
            using(IUnitOfWork uow=_UnitFactory.GetUnit(this))
            {
                uow.BeginTransaction();
                 entity = uow.Query<UserEntity>().Where(x => x.Id.Equals(id)).FirstOrDefault();
            }
            return entity;
        }

        public bool Delete(UserEntity user)
        {
            bool result = true;
            try
            {
                using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
                {
                    uow.BeginTransaction();
                    List<AppUserRoleEntity> entity = uow.Query<AppUserRoleEntity>().Where(x => x.UserId.Equals(user.Id)).ToList();
                    foreach (AppUserRoleEntity e in entity)
                    {
                        uow.Delete(e);
                    }



                    uow.Delete(user);

                    uow.Commit();
                }
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }

        public List<UserEntity> GetAll()
        {
            List<UserEntity> result = new List<UserEntity>();
            using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
            {
                uow.BeginTransaction();
                result = uow.Query<UserEntity>().ToList();
            }

            return result;
        }

        public bool Save(UserEntity usr)
        {
            bool result = false;

            using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
            {
                uow.BeginTransaction();
                try
                {
                    uow.SaveOrUpdate(usr);
                    uow.Commit();
                }
                catch (Exception ee)
                {

                    result = false;
                }
            }

            return result;
        }

        public IPagedList<UserEntity> SearchUsers(UserSearchSettings userSearchSettings)
        {
            using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
            {
                uow.BeginTransaction();
                List<UserEntity> entity;
                if (string.IsNullOrEmpty(userSearchSettings.Username))
                {
                    entity = uow.Query<UserEntity>().OrderBy(x => x.Username).ToList();
                }
                else
                {
                    entity = uow.Query<UserEntity>().Where(x => x.Username.Contains(userSearchSettings.Username)).OrderBy(x => x.Username).ToList();
                }

                return new
                      StaticPagedList<UserEntity>(entity, userSearchSettings.PageNumber, userSearchSettings.PageSize, 1000);
            }
        }

        public UserEntity GetByUsername(string userneame)
        {
            using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
            {
                uow.BeginTransaction();
                return uow.Query<UserEntity>().Where(x => x.Username == userneame).FirstOrDefault();
            }
        }

        public UserEntity GetByEmail(string email)
        {
            using (IUnitOfWork uow = _UnitFactory.GetUnit(this))
            {
                uow.BeginTransaction();
                return uow.Query<UserEntity>().Where(x => x.Email == email).FirstOrDefault();
            }
        }
    }
}
