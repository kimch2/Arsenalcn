﻿using System;
using Arsenal.Service.Casino;
using Arsenalcn.Core;
using Arsenalcn.Core.Logger;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arsenal.Service.Tests.Scheduler
{
    [TestClass]
    public class RefreshCacheTest
    {
        [TestMethod]
        public void ArsenalServiceRefreshCache_Test()
        {
            try
            {
                Config.Cache.RefreshCache();

                RelationLeagueTeam.Clean();
                RelationLeagueTeam.Cache.RefreshCache();

                League.Cache.RefreshCache();
                Match.Cache.RefreshCache();
                Player.Cache.RefreshCache();
                Team.Cache.RefreshCache();
                Video.Cache.RefreshCache();

                //AcnCasino
                CasinoItem.Clean();
                ChoiceOption.Clean();
                Bet.Clean();
                BetDetail.Clean();

                // Clean Log
                Log.Clean();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}