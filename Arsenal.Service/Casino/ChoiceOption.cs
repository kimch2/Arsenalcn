﻿using System;
using System.Data;
using System.Data.SqlClient;
using Arsenalcn.Core;
using DataReaderMapper;

namespace Arsenal.Service.Casino
{
    [DbSchema("AcnCasino_ChoiceOption", Sort = "CasinoItemGuid, OrderID")]
    public class ChoiceOption : Entity<int>
    {
        public static void CreateMap()
        {
            var map = Mapper.CreateMap<IDataReader, ChoiceOption>();

            map.ForMember(d => d.OptionName,
                opt => opt.MapFrom(s => s.GetValue("OptionValue")));

            map.ForMember(d => d.OptionOrder,
                opt => opt.MapFrom(s => s.GetValue("OrderID")));
        }

        public static void Clean(SqlTransaction trans = null)
        {
            //DELETE FROM AcnCasino_ChoiceOption WHERE (CasinoItemGuid NOT IN(SELECT CasinoItemGuid FROM AcnCasino_CasinoItem))
            var sql =
                $@"DELETE FROM {Repository.GetTableAttr<ChoiceOption>().Name} 
                     WHERE (CasinoItemGuid NOT IN (SELECT CasinoItemGuid FROM {Repository.GetTableAttr<CasinoItem>().Name}))";

            var dapper = new DapperHelper();

            dapper.Execute(sql, trans);
        }

        #region Members and Properties

        [DbColumn("CasinoItemGuid")]
        public Guid CasinoItemGuid { get; set; }

        [DbColumn("OptionValue")]
        public string OptionName { get; set; }

        [DbColumn("OptionDisplay")]
        public string OptionDisplay { get; set; }

        [DbColumn("OptionRate")]
        public float OptionRate { get; set; }

        [DbColumn("OrderID")]
        public int OptionOrder { get; set; }

        #endregion
    }
}