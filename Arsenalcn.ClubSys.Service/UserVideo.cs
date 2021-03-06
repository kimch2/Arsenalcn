﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Arsenalcn.ClubSys.Entity;
using Arsenalcn.Common;
using Microsoft.ApplicationBlocks.Data;

namespace Arsenalcn.ClubSys.Service
{
    public class UserVideo
    {
        public static DataRow GetUserVideoByID(int uvID)
        {
            var sql = "SELECT * FROM dbo.AcnClub_RelationUserVideo WHERE ID = @uvID";

            var ds = SqlHelper.ExecuteDataset(SQLConn.GetConnection(), CommandType.Text, sql,
                new SqlParameter("@uvID", uvID));

            if (ds.Tables[0].Rows.Count == 0)
                return null;
            return ds.Tables[0].Rows[0];
        }

        public static void UpdateUserVideo(int uvID, int userID, string userName, Guid videoGuid, DateTime activeDate,
            string userDesc, bool isPublic, SqlTransaction trans = null)
        {
            var sql =
                @"UPDATE dbo.AcnClub_RelationUserVideo SET UserID = @userID, UserName = @userName, VideoGuid = @videoGuid, 
                                  ActiveDate = @activeDate, UserDesc = @userDesc, IsPublic = @isPublic WHERE ID = @uvID";

            SqlParameter[] para =
            {
                new SqlParameter("@uvID", uvID),
                new SqlParameter("@userID", userID),
                new SqlParameter("@userName", userName),
                new SqlParameter("@videoGuid", videoGuid),
                new SqlParameter("@activeDate", activeDate),
                new SqlParameter("@userDesc", userDesc),
                new SqlParameter("@isPublic", isPublic)
            };

            if (trans == null)
            {
                SqlHelper.ExecuteNonQuery(SQLConn.GetConnection(), CommandType.Text, sql, para);
            }
            else
            {
                SqlHelper.ExecuteNonQuery(trans, CommandType.Text, sql, para);
            }
        }

        public static void InsertUserVideo(int uvID, int userID, string userName, Guid videoGuid, DateTime activeDate,
            string userDesc, bool isPublic, SqlTransaction trans = null)
        {
            var sql =
                @"INSERT INTO dbo.AcnClub_RelationUserVideo (UserID, UserName, VideoGuid, ActiveDate, UserDesc, IsPublic)  
                                 VALUES (@userID, @userName, @videoGuid, @activeDate, @userDesc, @isPublic)";

            SqlParameter[] para =
            {
                new SqlParameter(),
                new SqlParameter("@userID", userID),
                new SqlParameter("@userName", userName),
                new SqlParameter("@videoGuid", videoGuid),
                new SqlParameter("@activeDate", activeDate),
                new SqlParameter("@userDesc", userDesc),
                new SqlParameter("@isPublic", isPublic)
            };

            if (trans == null)
            {
                SqlHelper.ExecuteNonQuery(SQLConn.GetConnection(), CommandType.Text, sql, para);
            }
            else
            {
                SqlHelper.ExecuteNonQuery(trans, CommandType.Text, sql, para);
            }
        }

        public static void DeleteUserVideo(int uvID, SqlTransaction trans = null)
        {
            var sql = "DELETE dbo.AcnClub_RelationUserVideo WHERE ID = @uvID";

            SqlParameter[] para = {new SqlParameter("@uvID", uvID)};

            if (trans == null)
            {
                SqlHelper.ExecuteNonQuery(SQLConn.GetConnection(), CommandType.Text, sql, para);
            }
            else
            {
                SqlHelper.ExecuteNonQuery(trans, CommandType.Text, sql, para);
            }
        }

        public static DataTable GetUserVideos()
        {
            var sql = @"SELECT  ID, UserID, UserName, VideoGuid, ActiveDate, UserDesc, IsPublic  
                                  FROM AcnClub_RelationUserVideo ORDER BY ActiveDate DESC";

            var ds = SqlHelper.ExecuteDataset(SQLConn.GetConnection(), CommandType.Text, sql);

            if (ds.Tables[0].Rows.Count == 0)
                return null;
            return ds.Tables[0];
        }

        //public static DataTable GetUserVideo(int userID)
        //{
        //    string sql = "SELECT * FROM vw_AcnClub_UserVideoInfo WHERE [UserID] = @userID ORDER BY ActiveDate DESC";

        //    using (SqlConnection con = SQLConn.GetConnection())
        //    {
        //        SqlCommand com = new SqlCommand(sql, con);

        //        com.Parameters.Add(new SqlParameter("@userID", userID));

        //        SqlDataAdapter sda = new SqlDataAdapter(com);
        //        con.Open();

        //        DataTable dt = new DataTable();

        //        sda.Fill(dt);

        //        con.Close();

        //        return dt;
        //    }
        //}

        //public static DataView GetUserPublicVideo(int userID)
        //{
        //    DataTable dt = GetUserVideo(userID);

        //    DataView dv = dt.DefaultView;
        //    dv.RowFilter = "IsPublic = 1";

        //    return dv;
        //}

        public static DataTable GetUserVideoByClubID(int clubID)
        {
            var sql = @"SELECT * INTO #TmpUserClubVideo FROM AcnClub_RelationUserVideo
                                  WHERE (UserID IN (SELECT UserID FROM AcnClub_RelationUserClub 
                                  WHERE (ClubUid = @clubID) AND (IsActive = 1)))

                                  SELECT #TmpUserClubVideo.* FROM #TmpUserClubVideo INNER JOIN 
                                  (SELECT VideoGuid, MIN(ActiveDate) AS MinDate FROM #TmpUserClubVideo GROUP BY VideoGuid) #TmpUserClubVideoDup 
                                  ON #TmpUserClubVideo.VideoGuid = #TmpUserClubVideoDup.VideoGuid 
                                  AND #TmpUserClubVideo.ActiveDate = #TmpUserClubVideoDup.MinDate ORDER BY ActiveDate DESC

                                  DROP TABLE #TmpUserClubVideo;";

            SqlParameter[] para = {new SqlParameter("@clubID", clubID)};

            var ds = SqlHelper.ExecuteDataset(SQLConn.GetConnection(), CommandType.Text, sql, para);

            if (ds.Tables[0].Rows.Count == 0)
                return null;
            return ds.Tables[0];
        }

        //public static DataRow GetVideoInfoByUserVideoID(int userVideoID)
        //{
        //    string sql = "SELECT * FROM vw_AcnClub_UserVideoInfo WHERE [ID] = @userVideoID";

        //    using (SqlConnection con = SQLConn.GetConnection())
        //    {
        //        SqlCommand com = new SqlCommand(sql, con);

        //        com.Parameters.Add(new SqlParameter("@userVideoID", userVideoID));

        //        SqlDataAdapter sda = new SqlDataAdapter(com);
        //        con.Open();

        //        DataTable dt = new DataTable();

        //        sda.Fill(dt);

        //        con.Close();

        //        if (dt.Rows.Count == 0)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return dt.Rows[0];
        //        }
        //    }
        //}

        //public static DataRow GetVideoInfoByVideoGuid(Guid guid)
        //{
        //    string sql = "SELECT * FROM vw_Arsenal_VideoInfo WHERE [VideoGuid] = @videoGuid";

        //    using (SqlConnection con = SQLConn.GetConnection())
        //    {
        //        SqlCommand com = new SqlCommand(sql, con);

        //        com.Parameters.Add(new SqlParameter("@videoGuid", guid));

        //        SqlDataAdapter sda = new SqlDataAdapter(com);
        //        con.Open();

        //        DataTable dt = new DataTable();

        //        sda.Fill(dt);

        //        con.Close();

        //        if (dt.Rows.Count == 0)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return dt.Rows[0];
        //        }
        //    }
        //}

        public static Guid? GetRandomVideo(int userID, int minLv, int maxLv, bool? isLegend = null)
        {
            //string sql = "SELECT TOP 1 VideoGuid FROM dbo.vw_Arsenal_VideoInfo WHERE VideoGuid NOT IN (SELECT VideoGuid FROM AcnClub_RelationUserVideo WHERE UserID = @userID) AND GoalRank BETWEEN @minLv AND @maxLv AND IsLegend=@isLegend ORDER BY NEWID() ";

            //if (!isLegend.HasValue)
            //{
            //    sql = "SELECT TOP 1 VideoGuid FROM dbo.vw_Arsenal_VideoInfo WHERE GoalRank BETWEEN @minLv AND @maxLv ORDER BY NEWID()";
            //}

            //using (SqlConnection con = SQLConn.GetConnection())
            //{
            //    SqlCommand com = new SqlCommand(sql, con);

            //    com.Parameters.Add(new SqlParameter("@minLv", minLv));
            //    com.Parameters.Add(new SqlParameter("@maxLv", maxLv));

            //    if (isLegend.HasValue)
            //    {
            //        com.Parameters.Add(new SqlParameter("@isLegend", isLegend));
            //        com.Parameters.Add(new SqlParameter("@userID", userID));
            //    }

            //    con.Open();

            //    Guid? guid = null;

            //    try
            //    {
            //        guid = (Guid)com.ExecuteScalar();
            //    }
            //    catch
            //    { }

            //    con.Close();

            //    return guid;
            //}

            var list = new List<Video>();

            if (!isLegend.HasValue)
            {
                list = Video.Cache.VideoList;
            }
            else
            {
                list = Video.Cache.VideoList.FindAll(x =>
                    Player.Cache.Load(x.GoalPlayerGuid.Value).IsLegend.Equals(isLegend.Value));
            }

            if (minLv > 0 && maxLv > 0 && minLv <= maxLv)
            {
                list = list.FindAll(x => Convert.ToInt16(x.GoalRank) >= minLv && Convert.ToInt16(x.GoalRank) <= maxLv);
            }

            if (userID > 0)
            {
                var userList = Entity.UserVideo.GetUserVideosByUserID(userID);

                if (userList != null && userList.Count > 0)
                {
                    list.RemoveAll(x => userList.Exists(uv => uv.VideoGuid.Equals(x.ID)));
                }
            }

            if (list != null && list.Count > 0)
            {
                var rand = new Random(Guid.NewGuid().GetHashCode());
                return list[rand.Next(0, list.Count - 1)].ID;
            }
            return null;
        }

        public static void ConsolidateCards(int userID, string userName, int userNumID1, int userNumID2)
        {
            var sql =
                "DELETE FROM AcnClub_Card WHERE [ID] = @userNum1 OR [ID] = @userNum2 ; INSERT INTO dbo.AcnClub_Card (UserID, UserName, IsActive, IsInUse, GainDate, ActiveDate, ArsenalPlayerGuid) VALUES (@userID, @userName, 0, 0, getdate(), null, null);";

            using (var con = SQLConn.GetConnection())
            {
                var com = new SqlCommand(sql, con);

                com.Parameters.Add(new SqlParameter("@userID", userID));
                com.Parameters.Add(new SqlParameter("@userName", userName));
                com.Parameters.Add(new SqlParameter("@userNum1", userNumID1));
                com.Parameters.Add(new SqlParameter("@userNum2", userNumID2));

                con.Open();

                com.ExecuteNonQuery();

                //con.Close();
            }
        }

        //public static void InsertActiveVideo(int userid, string username, Guid videoGuid)
        //{
        //    string sql = "INSERT INTO AcnClub_RelationUserVideo (UserID, UserName, VideoGuid, ActiveDate, UserDesc, IsPublic) VALUES (@userID, @userName, @guid, getdate(), null, 0) ;";

        //    using (SqlConnection con = SQLConn.GetConnection())
        //    {
        //        SqlCommand com = new SqlCommand(sql, con);

        //        com.Parameters.Add(new SqlParameter("@userID", userid));
        //        com.Parameters.Add(new SqlParameter("@userName", username));
        //        com.Parameters.Add(new SqlParameter("@guid", videoGuid));

        //        con.Open();

        //        com.ExecuteNonQuery();

        //        con.Close();
        //    }
        //}

        public static Guid? SetDailyVideo()
        {
            var guid = GetRandomVideo(-1, 4, 5, null);
            if ((guid.HasValue) && ConfigGlobal.DailyVideoActive)
            {
                if (ConfigGlobal.DailyVideoGuid.Equals(guid.Value))
                    guid = GetRandomVideo(-1, 4, 5, null);

                return guid;
            }
            return null;
        }
    }
}