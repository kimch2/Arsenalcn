﻿using System;

using iArsenal.Entity;
using ArsenalTeam = iArsenal.Entity.Arsenal.Team;

namespace iArsenal.Web.Control
{
    public partial class PortalMatchInfo : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (MatchGuid != Guid.Empty)
            {
                MatchTicket mt = MatchTicket.Cache.Load(MatchGuid);
                ArsenalTeam tHome = Arsenal_Team.Cache.Load(ConfigGlobal.ArsenalTeamGuid);
                ArsenalTeam tAway = Arsenal_Team.Cache.Load(mt.TeamGuid);

                if (mt != null)
                {
                    string _strLeagueInfo = mt.LeagueName.ToString();

                    if (mt.Round.HasValue)
                        _strLeagueInfo += string.Format(" 第{0}轮", mt.Round.Value.ToString());

                    lblLeagueSeason.Text = _strLeagueInfo;

                    string _strTeamInfo = "<div class=\"MatchTicket_MatchInfo\"><a class=\"StrongLink\" title=\"{1}\">{0}</a><img src=\"{2}\" alt=\"{0}\" /><a><em>vs</em></a><img src=\"{5}\" alt=\"{3}\" /><a class=\"StrongLink\" title=\"{4}\">{3}</a></div>";

                    ltrlTeamInfo.Text = string.Format(_strTeamInfo, tHome.TeamDisplayName, tHome.TeamEnglishName, ConfigGlobal.AcnCasinoURL + tHome.TeamLogo, tAway.TeamDisplayName, tAway.TeamEnglishName, ConfigGlobal.AcnCasinoURL + tAway.TeamLogo);

                    lblTicketInfo.Text = mt.ProductInfo;
                    lblGameTime.Text = mt.PlayTimeLocal.ToString("yyyy-MM-dd HH:mm");
                }
            }
            else
            {
                lblLeagueSeason.Visible = false;
                ltrlTeamInfo.Visible = false;
                lblTicketInfo.Visible = false;
                lblGameTime.Visible = false;
            }
        }

        public Guid MatchGuid
        { get; set; }
    }
}