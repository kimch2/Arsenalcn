﻿using System;
using System.Collections.Generic;
using System.Data;

namespace Arsenalcn.Common.Entity
{
    public class Config
    {
        public Config()
        {
        }

        public Config(DataRow dr)
        {
            InitConfig(dr);
        }

        private void InitConfig(DataRow dr)
        {
            if (dr != null)
            {
                ConfigSystem = (ConfigSystem) Enum.Parse(typeof (ConfigSystem), dr["ConfigSystem"].ToString());
                ConfigKey = dr["ConfigKey"].ToString();
                ConfigValue = dr["ConfigValue"].ToString();
            }
            else
                throw new Exception("Unable to init Config.");
        }

        public void Select()
        {
            var dr = DataAccess.Config.GetConfigByID(ConfigSystem.ToString(), ConfigKey);

            if (dr != null)
                InitConfig(dr);
        }

        public void Update()
        {
            using (var conn = SQLConn.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    DataAccess.Config.UpdateConfig(ConfigSystem.ToString(), ConfigKey, ConfigValue, trans);

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }

                //conn.Close();
            }
        }

        private void Insert()
        {
            using (var conn = SQLConn.GetConnection())
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    DataAccess.Config.InsertConfig(ConfigSystem.ToString(), ConfigKey, ConfigValue, trans);

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }

                //conn.Close();
            }
        }

        private void Delete()
        {
            DataAccess.Config.DeleteConfig(ConfigSystem.ToString(), ConfigKey);
        }

        public static List<Config> GetConfigs()
        {
            var dt = DataAccess.Config.GetConfigs();
            var list = new List<Config>();

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new Config(dr));
                }
            }

            return list;
        }

        protected static Dictionary<string, string> GetDictionaryByConfigSystem(ConfigSystem cs)
        {
            var list = Cache.ConfigList.FindAll(delegate(Config c) { return c.ConfigSystem.Equals(cs); });

            if (list != null && list.Count > 0)
            {
                var dict = new Dictionary<string, string>();

                foreach (var c in list)
                {
                    try
                    {
                        dict.Add(c.ConfigKey, c.ConfigValue);
                    }
                    catch
                    {
                    }
                }

                return dict;
            }
            return null;
        }

        public static class Cache
        {
            public static List<Config> ConfigList;

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
                ConfigList = GetConfigs();
            }

            public static Config Load(ConfigSystem cs, string key)
            {
                return
                    ConfigList.Find(delegate(Config c) { return c.ConfigSystem.Equals(cs) && c.ConfigKey.Equals(key); });
            }

            public static string LoadDict(ConfigSystem cs, string key)
            {
                return GetDictionaryByConfigSystem(cs)[key];
            }
        }

        #region Members and Properties

        public ConfigSystem ConfigSystem { get; set; }

        public string ConfigKey { get; set; }

        public string ConfigValue { get; set; }

        #endregion
    }

    public enum ConfigSystem
    {
        AcnClub,
        AcnCasino,
        iArsenal,
        Arsenal
    }
}