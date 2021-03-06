﻿using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Arsenalcn.ClubSys.Entity;
using Arsenalcn.ClubSys.Service;
using Arsenalcn.ClubSys.Web.Common;

namespace Arsenalcn.ClubSys.Web
{
    public partial class AdminHistory : AdminBasePage
    {
        private List<Entity.ClubHistory> history;

        private int ClubID
        {
            get
            {
                var clubID = -1;

                var ddlKey = string.Empty;
                foreach (var key in Request.Form.AllKeys)
                {
                    if (key.IndexOf("ddlClub") >= 0)
                        ddlKey = key;
                }

                if (ddlKey != string.Empty)
                {
                    int.TryParse(Request.Form[ddlKey], out clubID);
                }
                else
                {
                    int.TryParse(ddlClub.SelectedValue, out clubID);
                }

                return clubID;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var item = new ListItem("--请选择球会--", Guid.Empty.ToString());

            ddlClub.DataSource = ClubLogic.GetActiveClubs();
            ddlClub.DataTextField = "FullName";
            ddlClub.DataValueField = "ID";
            ddlClub.DataBind();

            ddlClub.Items.Insert(0, item);

            if (ClubID != -1)
                ddlClub.SelectedValue = ClubID.ToString();

            ctrlAdminFieldToolBar.AdminUserName = username;

            BindClubHistory();
        }

        private void BindClubHistory()
        {
            if (history == null)
            {
                if (ClubID > 0)
                    history = ClubLogic.GetClubHistory(ClubID);
                else
                    history = ClubLogic.GetClubHistory();

                foreach (var ch in history)
                {
                    var actionType = (ClubHistoryActionType) Enum.Parse(typeof (ClubHistoryActionType), ch.ActionType);
                    switch (actionType)
                    {
                        case ClubHistoryActionType.JoinClub:
                            ch.AdditionalData = "ClubSys_Agree";
                            break;
                        case ClubHistoryActionType.RejectJoinClub:
                            ch.AdditionalData = "ClubSys_Disagree";
                            break;
                        case ClubHistoryActionType.LeaveClub:
                            ch.AdditionalData = "ClubSys_Disagree";
                            break;
                        case ClubHistoryActionType.MandatoryLeaveClub:
                            ch.AdditionalData = "ClubSys_Disagree";
                            break;
                        case ClubHistoryActionType.Nominated:
                            ch.AdditionalData = "ClubSys_Agree";
                            break;
                        case ClubHistoryActionType.Dismiss:
                            ch.AdditionalData = "ClubSys_Disagree";
                            break;
                        case ClubHistoryActionType.LuckyPlayer:
                            ch.AdditionalData = "ClubSys_Disagree";
                            break;
                        default:
                            ch.AdditionalData = "ClubSys_Agree";
                            break;
                    }

                    ch.AdditionalData2 = ClubLogic.TranslateClubHistoryActionType(actionType);
                }
            }

            gvHistory.DataSource = history;
            gvHistory.DataBind();
        }

        protected void gvHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvHistory.PageIndex = e.NewPageIndex;

            BindClubHistory();
        }
    }
}