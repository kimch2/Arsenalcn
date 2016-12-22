﻿using System.Data.SqlClient;
using Arsenalcn.Core;

namespace Arsenal.Service.Casino
{
    [DbSchema("AcnCasino_BetDetail", Sort = "ID DESC")]
    public class BetDetail : Entity<int>
    {
        public static void Clean(SqlTransaction trans = null)
        {
            //DELETE FROM AcnCasino_BetDetail WHERE (BetID NOT IN (SELECT ID FROM AcnCasino_Bet))
            var sql =
                $@"DELETE FROM {Repository.GetTableAttr<BetDetail>().Name} 
                WHERE (BetID NOT IN (SELECT ID FROM {Repository.GetTableAttr<Bet>().Name}))";

            var dapper = new DapperHelper();

            dapper.Execute(sql, trans);
        }

        #region Members and Properties

        [DbColumn("BetID")]
        public int BetID { get; set; }

        [DbColumn("DetailName")]
        public string DetailName { get; set; }

        [DbColumn("DetailValue")]
        public string DetailValue { get; set; }

        #endregion
    }
}