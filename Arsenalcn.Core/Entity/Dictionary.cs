﻿using System.Collections.Generic;
using System.Linq;

namespace Arsenalcn.Core
{
    [DbSchema("Arsenalcn_Dictionary", Sort = "ID")]
    public class Dictionary : Entity<int>
    {
        //public Dictionary(DataRow dr) : base(dr) { }

        public static class Cache
        {
            public static List<Dictionary> DictionaryList;

            static Cache()
            {
                InitCache();
            }

            public static void RefreshCache()
            {
                InitCache();
            }

            private static void InitCache()
            {
                IRepository repo = new Repository();

                DictionaryList = repo.All<Dictionary>().ToList();
            }

            public static Dictionary Load(int id)
            {
                return DictionaryList.Find(x => x.ID.Equals(id));
            }
        }

        #region Members and Properties

        [DbColumn("Name")]
        public string Name { get; set; }

        [DbColumn("DisplayName")]
        public string DisplayName { get; set; }

        [DbColumn("StandardLevel")]
        public string StandardLevel { get; set; }

        [DbColumn("BusinessField")]
        public string BusinessField { get; set; }

        [DbColumn("StandardCode")]
        public string StandardCode { get; set; }

        [DbColumn("IsTreeDictionary")]
        public bool IsTreeDictionary { get; set; }

        [DbColumn("Description")]
        public string Description { get; set; }

        #endregion
    }

    [DbSchema("Arsenalcn_DictionaryItem", Sort = "OrderNum")]
    public class DictionaryItem : Entity<int>
    {
        //public DictionaryItem(DataRow dr) : base(dr) { }

        public static class Cache
        {
            public static List<DictionaryItem> DictionaryItemList_Region;

            static Cache()
            {
                InitCache();
            }

            public static void RefreshCache()
            {
                InitCache();
            }

            private static void InitCache()
            {
                IRepository repo = new Repository();

                DictionaryItemList_Region = repo.Query<DictionaryItem>(x => x.DictionaryID == 108);
            }

            public static DictionaryItem Load(int id)
            {
                return DictionaryItemList_Region.Find(x => x.ID.Equals(id));
            }
        }

        #region Members and Properties

        [DbColumn("Code")]
        public string Code { get; set; }

        [DbColumn("Name")]
        public string Name { get; set; }

        [DbColumn("Description")]
        public string Description { get; set; }

        [DbColumn("CustomCode")]
        public string CustomCode { get; set; }

        [DbColumn("Spell")]
        public string Spell { get; set; }

        [DbColumn("ShortSpell")]
        public string ShortSpell { get; set; }

        [DbColumn("ParentID")]
        public int ParentID { get; set; }

        [DbColumn("OrderNum")]
        public int OrderNum { get; set; }

        [DbColumn("DictionaryID")]
        public int DictionaryID { get; set; }

        #endregion
    }
}