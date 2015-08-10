﻿using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;

using Arsenalcn.Core;

namespace Arsenal.Service.Casino
{
    [DbSchema("AcnCasino_Bet", Sort = "BetTime DESC")]
    public class Bet : Entity<int>
    {
        public Bet() : base() { }

        public static void CreateMap()
        {
            var map = AutoMapper.Mapper.CreateMap<IDataReader, Bet>();

            map.ForMember(d => d.BetAmount, opt => opt.MapFrom(s => s.GetValue("Bet")));
        }

        // Place Bet of SingleChoice
        public void Place(Guid matchGuid, string selectedOption)
        {
            using (SqlConnection conn = new SqlConnection(DataAccess.ConnectString))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    Contract.Requires(this.UserID > 0);
                    Contract.Requires(matchGuid != null);
                    Contract.Requires(!string.IsNullOrEmpty(selectedOption));
                    Contract.Requires(this.BetAmount.HasValue & this.BetAmount.Value > 0);

                    IRepository repo = new Repository();

                    #region Get CasinoItem & Check
                    var item = repo.Query<CasinoItem>(x =>
                        x.MatchGuid == matchGuid && x.ItemType == CasinoType.SingleChoice)[0];

                    if (item == null)
                    {
                        throw new Exception("对应投注项不存在(SingleChoice)");
                    }

                    if (item.CloseTime < DateTime.Now)
                    {
                        throw new Exception("已超出投注截止时间");
                    }
                    #endregion

                    #region Get Gambler & Check
                    var gambler = repo.Query<Gambler>(x => x.UserID == this.UserID)[0];

                    if (gambler == null)
                    {
                        throw new Exception("当前用户不存在博彩帐户(Gambler)");
                    }

                    if ((double)gambler.Cash < this.BetAmount.Value)
                    {
                        throw new Exception(string.Format("博彩帐户余额不足(博彩币余额: {0})", gambler.Cash.ToString("f2")));
                    }
                    #endregion

                    #region Get ChoiceOption & Check
                    var choiceOption = repo.Query<ChoiceOption>(x => x.CasinoItemGuid == item.ID)
                        .Find(x => x.OptionName.Equals(selectedOption, StringComparison.OrdinalIgnoreCase));

                    if (choiceOption == null)
                    {
                        throw new Exception("对应投注项不存在(ChoiceOption)");
                    }
                    #endregion

                    #region Get Banker & Check
                    var banker = repo.Single<Banker>(item.BankerID);

                    if (banker == null)
                    {
                        throw new Exception("对应庄家不存在(Banker)");
                    }
                    #endregion

                    //update gambler statistics
                    gambler.TotalBet += BetAmount.Value;
                    gambler.Cash -= BetAmount.Value;
                    repo.Update<Gambler>(gambler, trans);

                    banker.Cash += BetAmount.Value;
                    repo.Update<Banker>(banker, trans);

                    this.CasinoItemGuid = item.ID;
                    this.BetTime = DateTime.Now;
                    this.BetRate = (double)choiceOption.OptionRate;

                    object _key = null;
                    repo.Insert<Bet>(this, out _key, trans);
                    this.ID = Convert.ToInt32(_key);

                    var betDetail = new BetDetail();

                    betDetail.BetID = this.ID;

                    if (selectedOption.ToLower() == "home")
                        betDetail.DetailName = "Home";
                    else if (selectedOption.ToLower() == "away")
                        betDetail.DetailName = "Away";
                    else if (selectedOption.ToLower() == "draw")
                        betDetail.DetailName = "Draw";

                    repo.Insert<BetDetail>(betDetail, trans);

                    trans.Commit();

                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    throw ex;
                }
            }
        }

        // Place Bet of MatchResult
        public void Place(Guid matchGuid, short resultHome, short resultAway)
        {
            using (SqlConnection conn = new SqlConnection(DataAccess.ConnectString))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    Contract.Requires(this.UserID > 0);
                    Contract.Requires(matchGuid != null);
                    Contract.Requires(resultHome >= 0 & resultAway >= 0);

                    IRepository repo = new Repository();

                    #region Get CasinoItem & Check
                    var item = repo.Query<CasinoItem>(x =>
                        x.MatchGuid == matchGuid && x.ItemType == CasinoType.MatchResult)[0];

                    if (item == null)
                    {
                        throw new Exception("对应投注项不存在(MatchResult)");
                    }

                    if (item.CloseTime < DateTime.Now)
                    {
                        throw new Exception("已超出投注截止时间");
                    }
                    #endregion

                    #region Get Gambler & Check
                    var gambler = repo.Query<Gambler>(x => x.UserID == this.UserID)[0];

                    if (gambler == null)
                    {
                        throw new Exception("当前用户不存在博彩帐户(Gambler)");
                    }
                    #endregion

                    #region Get RepeatBet & Check
                    var historyBets = repo.Query<Bet>(x =>
                        x.CasinoItemGuid == item.ID && x.UserID == this.UserID);

                    if (historyBets.Count > 0)
                    {
                        throw new Exception("已经投过此注，不能重复猜比分");
                    }
                    #endregion

                    this.CasinoItemGuid = item.ID;
                    this.BetTime = DateTime.Now;

                    object _key = null;
                    repo.Insert<Bet>(this, out _key, trans);
                    this.ID = Convert.ToInt32(_key);

                    var betDetailHome = new BetDetail();

                    betDetailHome.BetID = this.ID;
                    betDetailHome.DetailName = "Home";
                    betDetailHome.DetailValue = resultHome.ToString();

                    repo.Insert<BetDetail>(betDetailHome, trans);

                    var betDetailAway = new BetDetail();

                    betDetailAway.BetID = this.ID;
                    betDetailAway.DetailName = "Away";
                    betDetailAway.DetailValue = resultAway.ToString();

                    repo.Insert<BetDetail>(betDetailAway, trans);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    throw ex;
                }
            }
        }

        public static void Clean(SqlTransaction trans = null)
        {
            //DELETE FROM dbo.AcnCasino_Bet WHERE (CasinoItemGuid NOT IN(SELECT CasinoItemGuid FROM dbo.AcnCasino_CasinoItem))
            string sql = string.Format(@"DELETE FROM {0} WHERE (CasinoItemGuid NOT IN (SELECT CasinoItemGuid FROM {1}))",
                   Repository.GetTableAttr<Bet>().Name,
                   Repository.GetTableAttr<CasinoItem>().Name);

            DataAccess.ExecuteNonQuery(sql, null, trans);
        }


        #region Members and Properties

        [DbColumn("UserID")]
        public int UserID
        { get; set; }

        [DbColumn("UserName")]
        public string UserName
        { get; set; }

        [DbColumn("CasinoItemGuid")]
        public Guid CasinoItemGuid
        { get; set; }

        [DbColumn("Bet")]
        public double? BetAmount
        { get; set; }

        [DbColumn("BetTime")]
        public DateTime BetTime
        { get; set; }

        [DbColumn("BetRate")]
        public double? BetRate
        { get; set; }

        [DbColumn("IsWin")]
        public bool? IsWin
        { get; set; }

        [DbColumn("Earning")]
        public double? Earning
        { get; set; }

        [DbColumn("EarningDesc")]
        public string EarningDesc
        { get; set; }

        #endregion
    }
}