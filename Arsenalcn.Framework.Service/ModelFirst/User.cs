//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Arsenalcn.Framework.DataAccess.ModelFirst
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.Accounts = new HashSet<Account>();
            this.Roles = new HashSet<Role>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Gender { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
