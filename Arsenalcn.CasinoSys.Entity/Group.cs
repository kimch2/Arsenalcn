﻿using System;
using System.Collections.Generic;
using System.Data;

namespace Arsenalcn.CasinoSys.Entity
{
    public class Group
    {
        public Group() { }

        private Group(DataRow dr)
        {
            InitGroup(dr);
        }

        public Group(Guid groupGuid)
        {
            DataRow dr = DataAccess.Group.GetGroupByID(groupGuid);

            if (dr != null)
                InitGroup(dr);
        }

        private void InitGroup(DataRow dr)
        {

            if (dr != null)
            {
                GroupGuid = (Guid)dr["GroupGuid"];
                GroupName = Convert.ToString(dr["GroupName"]);
                GroupOrder = Convert.ToInt32(dr["GroupOrder"]);
                LeagueGuid = (Guid)dr["LeagueGuid"];
                IsTable = Convert.ToBoolean(dr["IsTable"]);
            }
            else
                throw new Exception("Unable to init Group.");
        }

        public void Insert()
        {
            DataAccess.Group.InsertGroup(GroupGuid, GroupName, GroupOrder, LeagueGuid, IsTable);
        }

        public void Update()
        {
            DataAccess.Group.UpdateGroup(GroupGuid, GroupName, GroupOrder, LeagueGuid, IsTable);
        }

        public static List<Group> GetGroups()
        {
            DataTable dt = DataAccess.Group.GetGroups();
            List<Group> list = new List<Group>();

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new Group(dr));
                }
            }

            return list;
        }

        public static void RemoveGroup(Guid groupGuid)
        {
            DataAccess.Group.RemoveRelationGroupAllTeam(groupGuid);
            DataAccess.Match.RemoveMatchGroupGuid(groupGuid);
            DataAccess.Group.DeleteGroup(groupGuid);
        }

        public static void RemoveRelationGroupAllTeam(Guid groupGuid)
        {
            DataAccess.Group.RemoveRelationGroupAllTeam(groupGuid);
        }

        public static DataTable GetGroupByLeague(Guid leagueGuid)
        {
            return DataAccess.Group.GetLeagueGroup(leagueGuid);
        }

        public static DataTable GetGroupByLeague(Guid leagueGuid, bool isTable)
        {
            return DataAccess.Group.GetLeagueGroup(leagueGuid, isTable);
        }

        public static DataTable GetRelationGroupTeam(Guid groupGuid)
        {
            return DataAccess.Group.GetRelationGroupTeamByGroupGuid(groupGuid);
        }

        public static DataTable GetTableGroupTeam(Guid groupGuid)
        {
            return DataAccess.Group.GetTableGroupTeamByGroupGuid(groupGuid);
        }

        public static bool IsExistGroupByLeague(Guid leagueGuid, bool isTable)
        {
            DataTable dtGroup = DataAccess.Group.GetLeagueGroup(leagueGuid, isTable);

            return (dtGroup != null);
        }

        public static int GetResultMatchCount(Guid groupGuid)
        {
            Group group = new Group(groupGuid);
            DataTable dtGroupMatch = DataAccess.Match.GetResultMatchByGroupGuid(group.GroupGuid, group.IsTable);

            if (dtGroupMatch != null)
                return dtGroupMatch.Rows.Count;
            else
                return 0;
        }

        public static int GetAllMatchCount(Guid groupGuid)
        {
            Group group = new Group(groupGuid);
            DataTable dtGroupMatch = DataAccess.Match.GetAllMatchByGroupGuid(group.GroupGuid, group.IsTable);

            if (dtGroupMatch != null)
                return dtGroupMatch.Rows.Count;
            else
                return 0;
        }

        public static void SetGroupMatch(Guid groupGuid)
        {
            Group group = new Group(groupGuid);

            DataAccess.Match.UpdateMatchGroupGuid(group.GroupGuid, group.LeagueGuid);
        }

        public static void ActiveGroupTableStatistics()
        {
            List<Group> list = Group.GetGroups().FindAll(delegate(Group g)
            { return Arsenal_League.Cache.Load(g.LeagueGuid).IsActive; });

            if (list != null && list.Count > 0)
            {
                foreach (Group g in list)
                {
                    Entity.Group.GroupTableStatistics(g.GroupGuid);
                }
            }
        }

        public static void GroupTableStatistics(Guid groupGuid)
        {
            Group group = new Group(groupGuid);
            DataTable dtGroupTeam = DataAccess.Group.GetRelationGroupTeamByGroupGuid(groupGuid);
            DataTable dtGroupMatch = DataAccess.Match.GetResultMatchByGroupGuid(group.GroupGuid, group.IsTable);

            if (dtGroupTeam != null && dtGroupMatch != null)
            {
                foreach (DataRow dr in dtGroupTeam.Rows)
                {
                    GroupTeam gt = new GroupTeam(groupGuid, (Guid)dr["TeamGuid"], null);
                    GroupTeam.UpdateGroupTeamByGroupMatch(gt.GroupGuid, gt.TeamGuid, dtGroupMatch);
                }
            }

            //对球队进行排行
            dtGroupTeam = DataAccess.Group.GetRelationGroupTeamByGroupGuid(groupGuid);
            short positionNo = 0;

            if (dtGroupTeam != null)
            {
                foreach (DataRow dr in dtGroupTeam.Rows)
                {
                    GroupTeam gt = new GroupTeam(groupGuid, (Guid)dr["TeamGuid"], null);
                    gt.PositionNo = ++positionNo;
                    gt.Update();
                }
            }
        }

        public Guid GroupGuid
        { get; set; }

        public string GroupName
        { get; set; }

        public int GroupOrder
        { get; set; }

        public Guid LeagueGuid
        { get; set; }

        public bool IsTable
        { get; set; }
    }
}
