﻿using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Arsenalcn.Common.Entity;

namespace Arsenalcn.ClubSys.Web
{
    public partial class AdminConfig : Common.AdminBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ctrlAdminFieldToolBar.AdminUserName = this.username;

            if (!IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            var list = Config.GetConfigs().FindAll(delegate(Config c) { return c.ConfigSystem.Equals(ConfigSystem.AcnClub); });

            gvSysConfig.DataSource = list;
            gvSysConfig.DataBind();
        }

        protected void gvSysConfig_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvSysConfig.EditIndex = e.NewEditIndex;

            BindData();
        }

        protected void gvSysConfig_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            var tbConfigValue = gvSysConfig.Rows[gvSysConfig.EditIndex].Cells[1].Controls[0] as TextBox;

            if (tbConfigValue != null)
            {
                try
                {
                    var c = new Config();

                    c.ConfigSystem = ConfigSystem.AcnClub;
                    c.ConfigKey = gvSysConfig.DataKeys[gvSysConfig.EditIndex].Value.ToString();
                    c.ConfigValue = tbConfigValue.Text.Trim();

                    c.Update();
                    Config.Cache.RefreshCache();
                }
                catch (Exception ex)
                {
                    this.ClientScript.RegisterClientScriptBlock(typeof(string), "failed",
                        $"alert('{ex.Message.ToString()}');", true);
                }
            }

            gvSysConfig.EditIndex = -1;

            BindData();
        }

        protected void gvSysConfig_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvSysConfig.EditIndex = -1;

            BindData();
        }

        protected void gvSysConfig_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSysConfig.PageIndex = e.NewPageIndex;

            BindData();
        }

        protected void btnRefreshCache_Click(object sender, EventArgs e)
        {
            try
            {
                Config.Cache.RefreshCache();

                Service.Match.Cache.RefreshCache();
                Service.Player.Cache.RefreshCache();
                Service.Team.Cache.RefreshCache();
                Service.Video.Cache.RefreshCache();

                ClientScript.RegisterClientScriptBlock(typeof(string), "succeed", "alert('更新全部缓存成功');window.location.href=window.location.href", true);
            }
            catch (Exception ex)
            {
                this.ClientScript.RegisterClientScriptBlock(typeof(string), "failed",
                    $"alert('{ex.Message.ToString()}');", true);
            }
        }
    }
}
