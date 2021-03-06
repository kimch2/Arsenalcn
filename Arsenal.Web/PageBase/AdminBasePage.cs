﻿using System;
using Arsenal.Service;

namespace Arsenal.Web
{
    public class AdminPageBase : AcnPageBase
    {
        protected override void OnInitComplete(EventArgs e)
        {
            _adminPage = true;
            AnonymousRedirect = true;

            base.OnInitComplete(e);

            if (!ConfigGlobal_Arsenal.IsPluginAdmin(UID))
            {
                Response.Redirect("Default.aspx");
            }
        }
    }
}