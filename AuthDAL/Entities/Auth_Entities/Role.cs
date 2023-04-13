using System;
using System.Collections.Generic;
using AuthDAL.Entities.Base;

namespace AuthDAL.Entities
{
    public class Role : EntityBase
    {
        public string Name { get; set; }
        public List<Account> Accounts { get; set; }

        public Role()
        {
            Accounts = new List<Account>();
        }

    }
}

