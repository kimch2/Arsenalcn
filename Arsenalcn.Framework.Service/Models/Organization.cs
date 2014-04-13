﻿using System;
using System.Collections.Generic;

namespace Arsenalcn.Framework.Models
{
    public class Organization
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }

        //public virtual ICollection<Organization> Organizations { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}