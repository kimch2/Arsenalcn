﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Arsenalcn.ClubSys.DataAccess;
using Arsenalcn.ClubSys.Entity;

namespace Arsenalcn.ClubSys.Web
{
    public partial class AdminRank : Common.AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RanksConfigSqlDataSource.ConnectionString = DataAccess.SQLConn.ConnectionString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ctrlAdminFieldToolBar.AdminUserName = this.username;
        }

        protected void gvRanks_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvRanks.EditIndex = e.NewEditIndex;
        }

        protected void gvRanks_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvRanks.EditIndex = -1;
        }

    }
}
