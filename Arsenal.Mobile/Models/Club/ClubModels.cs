﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Arsenalcn.Core;

namespace Arsenal.Mobile.Models.Club
{
    public class IndexDto
    {

    }

    public class MyLogSignInDto : SearchModel<LogSignInDto>
    {
    }

    public class SignInDailyDto
    {
        public int ContinuousSignInDays { get; set; }

        public double CurrentCash { get; set; }

        public double Bonus { get; set; }

        public double MyCash { get; set; }

        public string Tip { get; set; }
    }
}