﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using Arsenalcn.CasinoSys.Entity;

namespace Arsenalcn.CasinoSys.Web
{
    public partial class CasinoPortal : Common.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Assign Control Property

            ctrlLeftPanel.UserID = userid;
            ctrlLeftPanel.UserName = username;

            ctrlFieldTooBar.UserID = userid;

            ctrlMenuTabBar.CurrentMenu = Arsenalcn.CasinoSys.Web.Control.CasinoMenuType.CasinoPortal;

            #endregion

            if (userid == -1)
            {
                gvMatch.Columns[gvMatch.Columns.Count - 1].Visible = false;
            }
            else if (CurrentGambler.Cash <= 0)
            {
                gvMatch.Columns[gvMatch.Columns.Count - 1].Visible = false;
            }

            var dtMatch = Entity.CasinoItem.GetMatchCasinoItemView(true);

            gvMatch.DataSource = dtMatch;
            gvMatch.DataBind();
        }

        protected void gvMatch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var drv = e.Row.DataItem as DataRowView;

                var m = new Match((Guid)drv["MatchGuid"]);

                var ltrlLeagueInfo = e.Row.FindControl("ltrlLeagueInfo") as Literal;

                if (ltrlLeagueInfo != null)
                {
                    var _strLeague = "<a href=\"CasinoGame.aspx?League={0}\" title=\"{1}\"><img src=\"{2}\" alt=\"{1}\" class=\"CasinoSys_CategoryImg\" /></a>";

                    var _strLeagueName = string.Format("{0}{1}", m.LeagueName, m.Round.HasValue ?
                        string.Format(" 第{0}轮", m.Round.ToString()) : string.Empty);

                    ltrlLeagueInfo.Text = string.Format(_strLeague, m.LeagueGuid.ToString(), _strLeagueName,
                        League.Cache.Load(m.LeagueGuid).LeagueLogo);
                }

                var lblHome = e.Row.FindControl("lblHome") as Label;
                var lblAway = e.Row.FindControl("lblAway") as Label;
                var hlVersus = e.Row.FindControl("hlVersus") as HyperLink;

                if (lblHome != null && lblAway != null && hlVersus != null)
                {
                    var tHome = Team.Cache.Load(m.Home);
                    var tAway = Team.Cache.Load(m.Away);

                    var _strTeamName = "<a class=\"StrongLink\" href=\"CasinoTeam.aspx?Team={0}\"  title=\"{1}\">{2}</a> ";
                    var _strTeamLogo = "<img src=\"{3}\" alt=\"{1}\" /> ";

                    lblHome.Text = string.Format(_strTeamName + _strTeamLogo,
                        tHome.ID.ToString(), tHome.TeamEnglishName, tHome.TeamDisplayName, tHome.TeamLogo);
                    lblAway.Text = string.Format(_strTeamLogo + _strTeamName,
                        tAway.ID.ToString(), tAway.TeamEnglishName, tAway.TeamDisplayName, tAway.TeamLogo);

                    hlVersus.NavigateUrl = string.Format("CasinoTeam.aspx?Match={0}", m.MatchGuid.ToString());
                    hlVersus.Text = string.Format("<em title=\"{0}{1}\">vs</em>", tHome.Ground,
                        tHome.Capacity.HasValue ? ("(" + tHome.Capacity.Value.ToString() + ")") : string.Empty);
                }

                var guid = Entity.CasinoItem.GetCasinoItemGuidByMatch(m.MatchGuid, CasinoType.SingleChoice);

                if (guid.HasValue)
                {
                    var item = Entity.CasinoItem.GetCasinoItem(guid.Value);

                    if (item != null)
                    {
                        var options = ((SingleChoice)item).Options;

                        var winOption = options.Find(delegate(ChoiceOption option) { return option.OptionValue == MatchChoiceOption.HomeWinValue; });
                        var drawOption = options.Find(delegate(ChoiceOption option) { return option.OptionValue == MatchChoiceOption.DrawValue; });
                        var loseOption = options.Find(delegate(ChoiceOption option) { return option.OptionValue == MatchChoiceOption.AwayWinValue; });

                        if (!string.IsNullOrEmpty(winOption.OptionValue) && !string.IsNullOrEmpty(drawOption.OptionValue) && !string.IsNullOrEmpty(loseOption.OptionValue))
                        {
                            var ltrlWinRate = e.Row.FindControl("ltrlWinRate") as Literal;
                            var ltrlDrawRate = e.Row.FindControl("ltrlDrawRate") as Literal;
                            var ltrlLoseRate = e.Row.FindControl("ltrlLoseRate") as Literal;

                            ltrlWinRate.Text = string.Format("<em title=\"主队胜赔率\">{0}</em>", Convert.ToSingle(winOption.OptionRate.Value).ToString("f2"));
                            ltrlDrawRate.Text = string.Format("<em title=\"双方平赔率\">{0}</em>", Convert.ToSingle(drawOption.OptionRate.Value).ToString("f2"));
                            ltrlLoseRate.Text = string.Format("<em title=\"客队胜赔率\">{0}</em>", Convert.ToSingle(loseOption.OptionRate.Value).ToString("f2"));

                            var lbWinInfo = e.Row.FindControl("lbWinInfo") as Label;
                            var lbDrawInfo = e.Row.FindControl("lbDrawInfo") as Label;
                            var lbLoseInfo = e.Row.FindControl("lbLoseInfo") as Label;

                            lbWinInfo.Text = string.Format("{0} | {1}", ChoiceOption.GetOptionTotalCount(guid.Value, winOption.OptionValue).ToString(), ChoiceOption.GetOptionTotalBet(guid.Value, winOption.OptionValue).ToString("N0"));
                            lbDrawInfo.Text = string.Format("{0} | {1}", ChoiceOption.GetOptionTotalCount(guid.Value, drawOption.OptionValue).ToString(), ChoiceOption.GetOptionTotalBet(guid.Value, drawOption.OptionValue).ToString("N0"));
                            lbLoseInfo.Text = string.Format("{0} | {1}", ChoiceOption.GetOptionTotalCount(guid.Value, loseOption.OptionValue).ToString(), ChoiceOption.GetOptionTotalBet(guid.Value, loseOption.OptionValue).ToString("N0"));

                            //Literal ltrlWinTotalBet = e.Row.FindControl("ltrlWinTotalBet") as Literal;
                            //Literal ltrlDrawTotalBet = e.Row.FindControl("ltrlDrawTotalBet") as Literal;
                            //Literal ltrlLoseTotalBet = e.Row.FindControl("ltrlLoseTotalBet") as Literal;

                            //ltrlWinTotalBet.Text = Convert.ToSingle(Entity.ChoiceOption.GetOptionTotalBet(guid.Value, winOption.OptionValue)).ToString("N0");
                            //ltrlDrawTotalBet.Text = Convert.ToSingle(Entity.ChoiceOption.GetOptionTotalBet(guid.Value, drawOption.OptionValue)).ToString("N0");
                            //ltrlLoseTotalBet.Text = Convert.ToSingle(Entity.ChoiceOption.GetOptionTotalBet(guid.Value, loseOption.OptionValue)).ToString("N0");

                            //Literal ltrlWinBetCount = e.Row.FindControl("ltrlWinBetCount") as Literal;
                            //Literal ltrlDrawBetCount = e.Row.FindControl("ltrlDrawBetCount") as Literal;
                            //Literal ltrlLoseBetCount = e.Row.FindControl("ltrlLoseBetCount") as Literal;

                            //ltrlWinBetCount.Text = Entity.ChoiceOption.GetOptionTotalCount(guid.Value, winOption.OptionValue).ToString();
                            //ltrlDrawBetCount.Text = Entity.ChoiceOption.GetOptionTotalCount(guid.Value, drawOption.OptionValue).ToString();
                            //ltrlLoseBetCount.Text = Entity.ChoiceOption.GetOptionTotalCount(guid.Value, loseOption.OptionValue).ToString();

                            var btnBet = e.Row.FindControl("btnBet") as HyperLink;

                            if (btnBet != null)
                            {
                                var betList = Entity.Bet.GetUserMatchAllBet(this.userid, m.MatchGuid);
                                var betCount = int.MinValue;

                                if (betList != null && betList.Count > 0)
                                    betCount = betList.Count;
                                else
                                    betCount = 0;

                                btnBet.Text = string.Format("投注 <span class=\"CasinoSys_BetInfo\">{0} | {1}</span>",
                                    betCount.ToString(), Bet.GetUserMatchTotalBet(this.userid, m.MatchGuid).ToString("N0"));
                                btnBet.NavigateUrl = string.Format("CasinoGameBet.aspx?Match={0}", m.MatchGuid.ToString());
                            }
                            else
                            {
                                btnBet.Visible = false;
                            }
                        }

                        // Adv Bodog Bet Button

                        //HyperLink btnBetBodog = e.Row.FindControl("btnBet_Bodog") as HyperLink;

                        ////管理人员或100积分以上会员看不到广告
                        //if (btnBetBodog == null || this.useradminid > 0 || this.usergroupid > 12)
                        //{
                        //    btnBetBodog.Visible = false;
                        //}

                        //if (btnBetBodog != null && this.CurrentGambler != null)
                        //{
                        //    btnBetBodog.Text = "博狗投注 <span class=\"CasinoSys_BetInfo\">Bodog</span>";
                        //    btnBetBodog.NavigateUrl = "http://record.slk61.com/_S9AEnQlJCqtPt_LV3gWenWNd7ZgqdRLk/68/?tc=olm36230";
                        //    btnBetBodog.Target = "_blank";
                        //}
                    }
                }
            }
        }
    }
}
