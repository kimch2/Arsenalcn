﻿using System;

using Arsenalcn.CasinoSys.Entity;

namespace Arsenalcn.CasinoSys.Web.Control
{
    public partial class DNTFooter : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            pluginName = ConfigGlobal.PluginName;
            pluginVersion = ConfigGlobal.PluginVersion;
        }

        public string pluginName = string.Empty;
        public string pluginVersion = string.Empty;
    }
}