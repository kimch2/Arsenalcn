﻿using System;
using System.Web.UI.WebControls;
using Arsenal.Service;
using Arsenalcn.Core;

namespace Arsenal.Web
{
    public partial class AdminVideoView : AdminPageBase
    {
        private readonly IRepository _repo = new Repository();

        private Guid VideoGuid
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.QueryString["VideoGuid"]))
                {
                    try
                    {
                        return new Guid(Request.QueryString["VideoGuid"]);
                    }
                    catch
                    {
                        return Guid.Empty;
                    }
                }
                return Guid.Empty;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ctrlAdminFieldToolBar.AdminUserName = Username;

            if (!IsPostBack)
            {
                #region Bind ddlLeague, ddlMatch

                var leagueList = League.Cache.LeagueList.FindAll(l =>
                    Match.Cache.MatchList.FindAll(m => m.LeagueGuid.Equals(l.ID)).Count > 0);

                ddlLeague.DataSource = leagueList;
                ddlLeague.DataTextField = "LeagueNameInfo";
                ddlLeague.DataValueField = "ID";
                ddlLeague.DataBind();

                ddlLeague.Items.Insert(0, new ListItem("--请选择比赛分类--", string.Empty));

                #endregion

                #region Bind ddlGoalPlayer, ddlAssistPlayer

                var list = Player.Cache.PlayerList_HasSquadNumber;

                if (list != null && list.Count > 0)
                {
                    foreach (var p in list)
                    {
                        ddlGoalPlayer.Items.Add(new ListItem(
                            string.Format("NO.{0} - {1}", p.SquadNumber, p.DisplayName), p.ID.ToString()));
                        ddlAssistPlayer.Items.Add(
                            new ListItem(string.Format("NO.{0} - {1}", p.SquadNumber, p.DisplayName), p.ID.ToString()));
                    }
                }

                ddlGoalPlayer.Items.Insert(0, new ListItem("--请选择进球队员--", string.Empty));
                ddlAssistPlayer.Items.Insert(0, new ListItem("--请选择助攻队员--", string.Empty));

                #endregion

                InitForm();
            }
        }

        private void InitForm()
        {
            if (VideoGuid != Guid.Empty)
            {
                var v = _repo.Single<Video>(VideoGuid);

                tbVideoGuid.Text = VideoGuid.ToString();
                tbFileName.Text = v.FileName;

                #region Set Video ArsenalMatchGuid

                if (v.ArsenalMatchGuid.HasValue)
                {
                    var m = Match.Cache.Load(v.ArsenalMatchGuid.Value);

                    if (m != null)
                    {
                        if (m.LeagueGuid.HasValue)
                        {
                            ddlLeague.SelectedValue = m.LeagueGuid.Value.ToString();
                            BindMatchData(m.LeagueGuid.Value);
                            ddlMatch.SelectedValue = v.ArsenalMatchGuid.Value.ToString();
                        }
                    }
                }
                else
                {
                    ddlLeague.SelectedValue = string.Empty;
                    ddlMatch.Items.Clear();
                }

                #endregion

                if (v.GoalPlayerGuid.HasValue)
                    ddlGoalPlayer.SelectedValue = v.GoalPlayerGuid.Value.ToString();
                else
                    ddlGoalPlayer.SelectedValue = string.Empty;

                if (v.AssistPlayerGuid.HasValue)
                    ddlAssistPlayer.SelectedValue = v.AssistPlayerGuid.Value.ToString();
                else
                    ddlAssistPlayer.SelectedValue = string.Empty;

                tbGoalRank.Text = v.GoalRank;
                tbTeamworkRank.Text = v.TeamworkRank;
                tbGoalYear.Text = v.GoalYear;
                tbOpponent.Text = v.Opponent;
                ddlVideoType.SelectedValue = v.VideoType.ToString();
                tbVideoLength.Text = v.VideoLength.ToString();
                tbVideoWidth.Text = v.VideoWidth.ToString();
                tbVideoHeight.Text = v.VideoHeight.ToString();
            }
            else
            {
                tbVideoGuid.Text = Guid.NewGuid().ToString();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var v = new Video();

                if (!VideoGuid.Equals(Guid.Empty))
                {
                    v = _repo.Single<Video>(VideoGuid);
                }
                else
                {
                    v.ID = new Guid(tbVideoGuid.Text.Trim());
                }

                v.FileName = tbFileName.Text.Trim();

                if (!string.IsNullOrEmpty(ddlLeague.SelectedValue) && !string.IsNullOrEmpty(ddlMatch.SelectedValue))
                {
                    v.ArsenalMatchGuid = new Guid(ddlMatch.SelectedValue);
                }
                else
                {
                    v.ArsenalMatchGuid = null;
                }

                if (!string.IsNullOrEmpty(ddlGoalPlayer.SelectedValue))
                {
                    v.GoalPlayerGuid = new Guid(ddlGoalPlayer.SelectedValue);
                    v.GoalPlayerName = Player.Cache.Load(v.GoalPlayerGuid.Value).DisplayName;
                }
                else
                {
                    throw new Exception("请选择进球队员");
                }

                if (!string.IsNullOrEmpty(ddlAssistPlayer.SelectedValue))
                {
                    v.AssistPlayerGuid = new Guid(ddlAssistPlayer.SelectedValue);
                    v.AssistPlayerName = Player.Cache.Load(v.AssistPlayerGuid.Value).DisplayName;
                }
                else
                {
                    v.AssistPlayerGuid = null;
                    v.AssistPlayerName = null;
                }

                v.GoalRank = tbGoalRank.Text.Trim();
                v.TeamworkRank = tbTeamworkRank.Text.Trim();
                v.GoalYear = tbGoalYear.Text.Trim();
                v.Opponent = tbOpponent.Text.Trim();
                //v.VideoType = ddlVideoType.SelectedValue;
                v.VideoType = (VideoFileType) Enum.Parse(typeof (VideoFileType), ddlVideoType.SelectedValue);
                v.VideoLength = Convert.ToInt16(tbVideoLength.Text.Trim());
                v.VideoWidth = Convert.ToInt16(tbVideoWidth.Text.Trim());
                v.VideoHeight = Convert.ToInt16(tbVideoHeight.Text.Trim());

                if (VideoGuid != Guid.Empty)
                {
                    _repo.Update(v);
                    ClientScript.RegisterClientScriptBlock(typeof (string), "succeed",
                        "alert('更新成功');window.location.href = window.location.href", true);
                }
                else
                {
                    _repo.Insert(v);
                    ClientScript.RegisterClientScriptBlock(typeof (string), "succeed",
                        "alert('添加成功');window.location.href = window.location.href", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(typeof (string), "failed",
                    string.Format("alert('{0}')", ex.Message), true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (VideoGuid != Guid.Empty)
            {
                Response.Redirect("AdminVideo.aspx?VideoGuid=" + VideoGuid);
            }
            else
            {
                Response.Redirect("AdminVideo.aspx");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (VideoGuid != Guid.Empty)
                {
                    _repo.Delete<Video>(VideoGuid);

                    ClientScript.RegisterClientScriptBlock(typeof (string), "succeed",
                        "alert('删除成功');window.location.href='AdminVideo.aspx'", true);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                ClientScript.RegisterClientScriptBlock(typeof (string), "failed", "alert('删除失败')", true);
            }
        }

        private void BindMatchData(Guid leagueGuid)
        {
            ddlMatch.Items.Clear();

            var query = Match.Cache.MatchList.FindAll(x => x.IsActive && x.LeagueGuid.Equals(leagueGuid));

            if (query.Count > 0)
            {
                foreach (var m in query)
                {
                    string strRound;
                    if (m.Round.HasValue)
                        strRound = string.Format("【{0}】", m.Round.Value);
                    else
                        strRound = string.Empty;

                    ddlMatch.Items.Add(
                        new ListItem($"【{(m.IsHome ? "主" : "客")}】-{strRound}- {m.TeamName}",
                            m.ID.ToString()));
                }
            }
            else
            {
                ddlMatch.Items.Clear();
            }

            ddlMatch.Items.Insert(0, new ListItem("--请选择比赛对阵--", string.Empty));
        }

        private void BindOponnentData(Guid matchGuid)
        {
            var m = Match.Cache.Load(matchGuid);

            if (m != null)
            {
                tbOpponent.Text = m.TeamName;
                tbGoalYear.Text = m.PlayTime.Year.ToString();
            }
        }

        protected void ddlLeague_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlLeague.SelectedValue))
                BindMatchData(new Guid(ddlLeague.SelectedValue));
        }

        protected void ddlMatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlMatch.SelectedValue))
                BindOponnentData(new Guid(ddlMatch.SelectedValue));
        }
    }
}