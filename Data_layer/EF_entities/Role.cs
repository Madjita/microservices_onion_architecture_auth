using System;
using System.Collections.Generic;

namespace Data_layer.EF_entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public List<Account> Accounts { get; set; }

        public Role()
        {
            Accounts = new List<Account>();
        }

    }
}

