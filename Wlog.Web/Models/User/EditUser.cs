﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Wlog.BLL.Entities;
using Wlog.Library.BLL.Reporitories;

namespace Wlog.Web.Models.User
{
    public class UserApps
    {
        //public int Id { get; set; }
        //public ApplicationEntity Application { get; set; }
        //public RolesEntity Role { get; set; }
        public virtual Guid IdApplication { get; set; }
        [Display(Name = "Application name")]
        public virtual string ApplicationName { get; set; }
        public virtual Guid RoleId { get; set; }
        public virtual string RoleName { get; set; }

    }

    public class EditUser
    {

        public UserEntity DataUser { get; set; }
        public List<UserApps> Apps { get; set; }
        public IEnumerable<RolesEntity> RoleList
        { 
            get 
            {
                List<RolesEntity> role = RepositoryContext.Current.Roles.GetAllRoles();
                role.Add(new RolesEntity { RoleName = "No Role" });
                return role;
            }
        }

        public EditUser()
        {
            this.Apps = new List<UserApps>();
        }
    }
}