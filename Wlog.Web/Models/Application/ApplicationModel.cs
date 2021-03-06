﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Wlog.Web.Models
{
    public class ApplicationModel
    {
        [Required]
        [Display(Name = "Application Id")]
        public virtual Guid IdApplication { get; set; }
        [Required]
        [Display(Name = "Application name")]
        public virtual string ApplicationName { get; set; }
        [Required]
        [Display(Name = "Active")]
        public virtual bool IsActive { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public virtual DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public virtual System.Nullable<DateTime> EndDate { get; set; }
        [Required]
        [Display(Name="Public Key")]
        public virtual Guid PublicKey { get; set; }
    }
}