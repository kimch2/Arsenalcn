﻿using System;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using Arsenalcn.Core;

namespace Arsenal.Service.Casino
{
    [DbSchema("AcnCasino_Banker")]
    public class Banker : Entity<Guid>
    {
        public void Statistic()
        {
            Contract.Requires(ID != null && !ID.Equals(Guid.Empty));

            var sql = string.Format(@"SELECT ISNULL(SUM(b.Bet), 0) - ISNULL(SUM(b.Earning), 0) AS BankerCash
                           FROM {0} c INNER JOIN {1} b ON c.CasinoItemGuid = b.CasinoItemGuid
                           WHERE (c.BankerID = @key)",
                Repository.GetTableAttr<CasinoItem>().Name,
                Repository.GetTableAttr<Bet>().Name);

            SqlParameter[] para = {new SqlParameter("@key", ID)};

            var ds = DataAccess.ExecuteDataset(sql, para);

            if (ds.Tables[0].Rows.Count > 0)
            {
                Cash = (double) ds.Tables[0].Rows[0]["BankerCash"];

                IRepository repo = new Repository();
                repo.Update(this);
            }
        }

        #region Members and Properties

        [DbColumn("BankerName")]
        public string BankerName { get; set; }

        [DbColumn("ClubID")]
        public int? ClubID { get; set; }

        [DbColumn("Cash")]
        public double Cash { get; set; }

        [DbColumn("IsActive")]
        public bool IsActive { get; set; }

        #endregion
    }
}