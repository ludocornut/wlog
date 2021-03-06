﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PagedList;
using Wlog.Web.Models.User;
using Wlog.BLL.Entities;
using Wlog.Library.BLL.Reporitories;
using Wlog.Library.BLL.Classes;

namespace Wlog.Web.Code.Helpers
{
    public class UserHelper
    {
        /// <summary>
        /// Get User by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static UserEntity GetById(Guid id)
        {
            return RepositoryContext.Current.Users.GetById(id);
        }

        /// <summary>
        /// Get All User
        /// </summary>
        /// <returns></returns>
        public static List<UserEntity> GetAll()
        {
            return RepositoryContext.Current.Users.GetAll();
        }

        public static IPagedList<UserData> FilterUserList(string serchFilter, int pagenumber, int pagesize)
        {
         
            List<UserData> data = new List<UserData>();
            IPagedList<UserEntity> users = RepositoryContext.Current.Users.SearchUsers(new UserSearchSettings
            {
                OrderBy=Library.BLL.Enums.UserFields.Username,
                PageNumber=pagenumber,
                PageSize=pagesize,
                Username=serchFilter
            });

            foreach (UserEntity e in users)
            {
                data.Add(new UserData
                {
                    Id = e.Id,
                    Username = e.Username,
                    Email = e.Email,
                    IsAdmin = e.IsAdmin,
                    LastLoginDate = e.LastLoginDate,
                    CreationDate = e.CreationDate,
                    IsOnLine = e.IsOnLine
                });
            }

            return new PagedList<UserData>(data, pagenumber, pagesize);
        }

        /// <summary>
        /// Get User By Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static UserEntity GetByEmail(string email)
        {
            return RepositoryContext.Current.Users.GetByEmail(email);
        }

        public static List<ApplicationEntity> GetAppsForUser(string userName)
        {
            return RepositoryContext.Current.Applications.GetAppplicationsByUsername(userName);

        }

        public static List<Guid> GetAppsIdsForUser(string userName)
        {
            return RepositoryContext.Current.Applications.GetAppplicationsIdsByUsername(userName);
        }

        /// <summary>
        /// Get User by Username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static UserEntity GetByUsername(string username)
        {
            return RepositoryContext.Current.Users.GetByUsername(username);
        }

        /// <summary>
        /// Update the User
        /// </summary>
        /// <param name="usr"></param>
        /// <returns></returns>
        public static bool UpdateUser(UserEntity usr)
        {
           return RepositoryContext.Current.Users.Save(usr);
        }


        public static bool DeleteById(Guid Id)
        {
            UserEntity user = RepositoryContext.Current.Users.GetById(Id);
          return RepositoryContext.Current.Users.Delete(user);
          
        }
        /// <summary>
        /// given a user return list of app with role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<UserApps> GetApp(Guid id)
        {
            List<UserApps> result = new List<UserApps>();

            UserEntity user= RepositoryContext.Current.Users.GetById(id);
            List<ApplicationEntity> apps = RepositoryContext.Current.Applications.GetAppplicationForUser(user);
            List<RolesEntity> roles = RepositoryContext.Current.Roles.GetAllRoles();
            List<AppUserRoleEntity> appForUser = RepositoryContext.Current.Roles.GetApplicationRolesForUser(user);




            AppUserRoleEntity current;
            RolesEntity r;
                foreach (ApplicationEntity ae in apps)
                {

                    current = appForUser.FirstOrDefault(x => x.ApplicationId == ae.IdApplication); 
                    if (current!=null)
                    {
                        r = roles.FirstOrDefault(x => x.Id == current.RoleId);
                        result.Add(new UserApps
                        {
                            ApplicationName = ae.ApplicationName,
                             IdApplication=ae.IdApplication,
                            RoleId=r.Id,
                           RoleName=r.RoleName
                        });
                    }
                    else
                    {
                        result.Add(new UserApps
                        {
                            ApplicationName = ae.ApplicationName,
                            IdApplication = ae.IdApplication,
                            RoleId = Guid.Empty,
                            RoleName = "No Role"
                        });
                    }
                }
            
            return result;
        }

        public static string EncodePassword(string password)
        {
            using (SHA1CryptoServiceProvider Sha = new SHA1CryptoServiceProvider())
            {
                return Convert.ToBase64String(Sha.ComputeHash(Encoding.ASCII.GetBytes(password)));
            }
        }
    }
}