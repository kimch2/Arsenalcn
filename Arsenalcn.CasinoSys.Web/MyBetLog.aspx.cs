﻿using System;
using System.Data;
using System.Web.UI.WebControls;

using Arsenalcn.CasinoSys.Entity;

using Discuz.Entity;
using Discuz.Forum;

namespace Arsenalcn.CasinoSys.Web
{
    public partial class MyBetLog : Common.BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            AnonymousRedirect = true;

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Assign Control Property

            ctrlLeftPanel.UserID = userid;
            ctrlLeftPanel.UserName = username;

            ctrlFieldTooBar.UserID = userid;

            ctrlMenuTabBar.CurrentMenu = Arsenalcn.CasinoSys.Web.Control.CasinoMenuType.CasinoPortal;

            ctrlGamblerHeader.UserID = CurrentUserID;
            ctrlGamblerHeader.UserName = CurrentUserName;

            #endregion

            BindData();
        }

        private void BindData()
        {
            gvBetLog.DataSource = Entity.Bet.GetUserBetHistoryView(CurrentUserID);
            gvBetLog.DataBind();
        }

        protected void gvBetLog_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Literal ltrlResult = e.Row.FindControl("ltrlResult") as Literal;
                Literal ltrlBetResult = e.Row.FindControl("ltrlBetResult") as Literal;
                Literal ltrlBetRate = e.Row.FindControl("ltrlBetRate") as Literal;
                LinkButton btnReturnBet = e.Row.FindControl("btnReturnBet") as LinkButton;

                DataRowView drv = e.Row.DataItem as DataRowView;

                Guid itemGuid = (Guid)drv["CasinoItemGuid"];

                Entity.CasinoItem item = Entity.CasinoItem.GetCasinoItem(itemGuid);
                DataTable dt = Entity.BetDetail.GetBetDetailByBetID((int)drv["ID"]);

                if (dt != null)
                {
                    DataRow dr = dt.Rows[0];

                    switch (item.ItemType)
                    {
                        case CasinoItem.CasinoType.SingleChoice:
                            if (dr["DetailName"].ToString() == MatchChoiceOption.HomeWinValue)
                                ltrlResult.Text = "主队胜";
                            else if (dr["DetailName"].ToString() == MatchChoiceOption.DrawValue)
                                ltrlResult.Text = "双方平";
                            else if (dr["DetailName"].ToString() == MatchChoiceOption.AwayWinValue)
                                ltrlResult.Text = "客队胜";

                            break;
                        case CasinoItem.CasinoType.MatchResult:
                            Entity.MatchResultBetDetail betDetail = new MatchResultBetDetail(dt);
                            ltrlResult.Text = string.Format("{0}：{1}", betDetail.Home, betDetail.Away);

                            break;
                    }
                }

                if (!Convert.IsDBNull(drv["IsWin"]))
                {
                    ltrlBetResult.Visible = true;
                    btnReturnBet.Visible = false;
                    if (Convert.ToBoolean(drv["IsWin"]))
                    {
                        if (item.ItemType == CasinoItem.CasinoType.SingleChoice)
                            ltrlBetResult.Text = "<span class=\"CasinoSys_True\" title=\"猜对输赢\"></span>";
                        else if (item.ItemType == CasinoItem.CasinoType.MatchResult)
                            ltrlBetResult.Text = "<span class=\"CasinoSys_Good\" title=\"猜对比分\"></span>";

                        e.Row.CssClass = "RowCasinoSys_True";
                    }
                    else
                    {
                        ltrlBetResult.Text = "<span class=\"CasinoSys_False\" title=\"失败\"></span>";
                    }
                }
                else if (Convert.IsDBNull(drv["IsWin"]) && (CurrentUserID == userid) && (item.CloseTime > DateTime.Now))
                {
                    btnReturnBet.Visible = true;
                    btnReturnBet.CommandArgument = drv["ID"].ToString();
                }
                else
                {
                    ltrlBetResult.Visible = false;
                    btnReturnBet.Visible = false;
                }

                if (Convert.IsDBNull(drv["BetRate"]))
                {
                    ltrlBetRate.Text = "/";
                }
                else
                {
                    ltrlBetRate.Text = Convert.ToSingle(drv["BetRate"]).ToString("f2");
                }
            }
        }

        protected void gvBetLog_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBetLog.PageIndex = e.NewPageIndex;
            gvBetLog.DataBind();

        }

        private int CurrentUserID
        {
            get
            {
                int _userid;
                if (!string.IsNullOrEmpty(Request.QueryString["UserID"]) && int.TryParse(Request.QueryString["UserID"], out _userid))
                {
                    return _userid;
                }
                else
                    return this.userid;
            }
        }

        private string CurrentUserName
        {
            get
            {
                ShortUserInfo sUser = AdminUsers.GetShortUserInfo(CurrentUserID);
                return sUser.Username.Trim();
            }
        }

        protected void gvBetLog_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ReturnBet")
            {
                Bet bet = new Bet(Convert.ToInt32(e.CommandArgument.ToString()));

                try
                {
                    if (bet.UserID == userid)
                    {
                        Bet.ReturnBet(bet.ID);
                        this.ClientScript.RegisterClientScriptBlock(typeof(string), "success", "alert('投注退还成功');window.location.href=window.location.href", true);
                    }
                    else
                        throw new Exception("非本人投注项");
                }
                catch (Exception ex)
                {
                    this.ClientScript.RegisterClientScriptBlock(typeof(string), "failed", "alert('投注退还失败');", true);
                    throw ex;
                }

                BindData();
            }
        }
    }
}
