﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wlog.BLL.Classes;

namespace Wlog.Web.Models
{
    public class DashboardModel
    {
        public long ErrorCount { get; set; }
        public long LogCount { get; set; }
        public long WarnCount { get; set; }
        public long InfoCount { get; set; }

        public List<QueueLoad> QueueLoad { get; set; }

      public  List<LogMessage> LastTen { get; set; }
      public List<MessagesListModel> AppLastTen { get; set; }


    }
}