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
using System.Collections.Generic;

using Arsenalcn.ClubSys.DataAccess;
using Arsenalcn.ClubSys.Entity;

using Discuz.Entity;
using Discuz.Forum;

namespace Arsenalcn.ClubSys.Web
{
    public partial class ClubMember : Common.BasePage
    {
        private int ClubID
        {
            get
            {
                int tmp;
                if (int.TryParse(Request.QueryString["ClubID"], out tmp))
                    return tmp;
                else
                {
                    Response.Redirect("ClubPortal.aspx");

                    return -1;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Club club = ClubLogic.GetClubInfo(ClubID);

            if (club != null && this.Title.IndexOf("{0}") >= 0)
                this.Title = string.Format(this.Title, club.FullName);

            #region SetControlProperty

            ctrlLeftPanel.UserID = this.userid;
            ctrlLeftPanel.UserName = this.username;
            ctrlLeftPanel.UserKey = this.userkey;

            ctrlFieldToolBar.UserID = this.userid;
            ctrlFieldToolBar.UserName = this.username;

            ctrlMenuTabBar.CurrentMenu = Arsenalcn.ClubSys.Web.Control.ClubMenuItem.ClubMemeber;
            ctrlMenuTabBar.ClubID = ClubID;

            ctrlClubSysHeader.UserID = this.userid;
            ctrlClubSysHeader.ClubID = ClubID;
            ctrlClubSysHeader.UserName = this.username;

            #endregion

            BindMemberList();
        }

        private List<UserClub> clubMemberList = null;
        private void BindMemberList()
        {
            if (clubMemberList == null)
                clubMemberList = ClubLogic.GetClubMembers(ClubID);

            gvClubMemberList.DataSource = clubMemberList;
            gvClubMemberList.DataBind();
        }

        protected void gvClubMemberList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UserClub uc = e.Row.DataItem as UserClub;
                if (uc != null)
                {
                    UserInfo userInfo = AdminUsers.GetUserInfo(uc.Userid.Value);
                    if (userInfo != null)
                    {
                        #region set avatar

                        Image imgAvatar = e.Row.FindControl("imgAvatar") as Image;

                        if (imgAvatar != null)
                        {
                            //if (userInfo.Avatar == string.Empty)
                            //{
                            //    imgAvatar.ImageUrl = "/avatars/common/0.gif";
                            //}
                            //else if (userInfo.Avatar.ToLower().IndexOf(@"/") == 0)
                            //{
                            //    imgAvatar.ImageUrl = userInfo.Avatar;
                            //}
                            //else if (userInfo.Avatar.ToLower().IndexOf("http") >= 0)
                            //{
                            //    imgAvatar.ImageUrl = userInfo.Avatar;
                            //}
                            //else
                            //{
                            //    imgAvatar.ImageUrl = string.Format("/{0}", userInfo.Avatar);
                            //}

                            string myAvatar = Avatars.GetAvatarUrl(uc.Userid.Value, AvatarSize.Small);
                            imgAvatar.ImageUrl = myAvatar;

                            imgAvatar.AlternateText = userInfo.Username.Trim();
                        }

                        #endregion

                        #region set responsibility

                        Literal ltrlResponsibility = e.Row.FindControl("ltrlResponsibility") as Literal;
                        if (ltrlResponsibility != null)
                        {
                            if (uc.Responsibility.Value == (int)Responsibility.Member)
                                ltrlResponsibility.Text = string.Empty;
                            else
                                ltrlResponsibility.Text = string.Format("<em>({0})</em>", ClubLogic.TranslateResponsibility(uc.Responsibility.Value));
                        }

                        #endregion

                        #region set user group

                        Literal ltrlUserGroup = e.Row.FindControl("ltrlUserGroup") as Literal;
                        if (ltrlUserGroup != null)
                        {
                            UserGroupInfo groupInfo = UserGroups.GetUserGroupInfo(userInfo.Groupid);

                            if( groupInfo != null )
                                ltrlUserGroup.Text = string.Format("<span title=\"积分:{0}\">{1}</span>", userInfo.Credits.ToString("N0"), groupInfo.Grouptitle);
                        }

                        #endregion

                        #region set user fortune

                        Literal ltrlUserFortune = e.Row.FindControl("ltrlUserFortune") as Literal;
                        if (ltrlUserFortune != null)
                        {
                            ltrlUserFortune.Text = userInfo.Extcredits2.ToString("N2");
                        }

                        #endregion

                        #region set user posts

                        Literal ltrlUserPosts = e.Row.FindControl("ltrlUserPosts") as Literal;
                        if (ltrlUserPosts != null)
                        {
                            ltrlUserPosts.Text = userInfo.Posts.ToString("N0");
                        }

                        #endregion

                        #region set user days

                        Literal ltrlDays = e.Row.FindControl("ltrlDays") as Literal;
                        if (ltrlDays != null)
                        {
                            ltrlDays.Text = ((int)((DateTime.Now - uc.JoinClubDate.Value).TotalDays)).ToString();
                        }

                        #endregion

                        #region contribute value

                        Literal ltrlContributeValue = e.Row.FindControl("ltrlContributeValue") as Literal;

                        try
                        {
                            int contribution = FortuneContributeAlgorithm.CalcContributeFortune(userInfo, false);

                            float bonusRate = PlayerStrip.CalcPlayerContributionBonusRate(uc.Userid.Value);

                            if( bonusRate != 0 )
                                ltrlContributeValue.Text = string.Format("<em>{0}(*{1}) 枪手币</em>", contribution, 1 + bonusRate);
                            else
                                ltrlContributeValue.Text = string.Format("<em>{0} 枪手币</em>", contribution);

                            _totalContribution += (int)(contribution * (1 + bonusRate));
                        }
                        catch { }

                        #endregion
                    }
                }
            }
        }

        protected void gvClubMemberList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvClubMemberList.PageIndex = e.NewPageIndex;

            BindMemberList();
        }

        private long _totalContribution = 0;
        protected override void OnPreRender(EventArgs e)
        {
            ltrlMemberCount.Text = ClubLogic.GetClubMemberCount(ClubID).ToString();
            ltrlTotalContribution.Text = _totalContribution.ToString();

            base.OnPreRender(e);
        }
    }
}
