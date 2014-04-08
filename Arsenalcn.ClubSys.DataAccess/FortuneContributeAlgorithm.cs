﻿using System;
using System.Collections.Generic;
using System.Text;

using Arsenalcn.ClubSys.Entity;
using Discuz.Entity;

namespace Arsenalcn.ClubSys.DataAccess
{
    public class FortuneContributeAlgorithm
    {
        public static int CalcContributeFortune(UserInfo info, bool playerBonus)
        {
            return CalcContributeFortune(info.Uid, info.Credits, info.Extcredits1, (int)info.Extcredits2, playerBonus);
        }

        public static int CalcContributeFortune(ShortUserInfo info, bool playerBonus)
        {
            return CalcContributeFortune(info.Uid, info.Credits, info.Extcredits1, (int)info.Extcredits2, playerBonus);
        }

        internal static int CalcContributeFortune(int userID, int memberCredit, float memberMana, int memberFortune, bool playerBonus)
        {
            int returnValue = (int)(ConfigGlobal.ClubFortuneIncrementVariable * Math.Pow(Math.Log10(memberCredit), 4)) + (int)((memberMana + 1) * Math.Log10(memberFortune));

            if (returnValue < 0)
                returnValue = 0;

            //player contribution
            
            //returnValue += PlayerStrip.CalcPlayerPrice(userID);

            if( playerBonus )
                returnValue = (int)(returnValue * (1 + PlayerStrip.CalcPlayerContributionBonusRate(userID)));

            return returnValue;
        }
    }
}
