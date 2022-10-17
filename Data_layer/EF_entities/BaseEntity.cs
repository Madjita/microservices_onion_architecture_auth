using System;
namespace Data_layer.EF_entities
{

    //dotnet ef --startup-project ../Presentation_layer  --verbose migrations add test
    public class BaseEntity
    {
        //public Guid Id { get; set; }
        public int Id { get; set; }
    }
}

