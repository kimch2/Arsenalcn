﻿using System;

using iArsenal.Entity;
using Arsenalcn.Common.Entity;

namespace iArsenal.Web.Control
{
    public partial class AdminFieldToolBar : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ltrlAdminInfo.Text = string.Format("<strong>欢迎<em>{0}</em>, 进入{1} 后台管理</strong>", AdminUserName, ConfigGlobal.PluginDisplayName);

            string _ip = IPLocation.GetIP();

            ltrlMyIPInfo.Text = string.Format("<a>IP: {0} - {1}</a>", _ip, IPLocation.GetIPInfo(_ip));
        }

        public string AdminUserName
        { get; set; }
    }
}