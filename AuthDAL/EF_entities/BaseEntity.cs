using System;
namespace AuthDAL.EF_entities
{

    //dotnet ef --startup-project ../AuthDomain  --verbose migrations add test
    public class BaseEntity
    {
        //public Guid Id { get; set; }
        public int Id { get; set; }
    }
}

