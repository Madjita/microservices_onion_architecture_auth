using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AuthDAL.Entities.Base;

//dotnet ef --startup-project ../AuthDomain  --verbose migrations add test
/// <summary>
///     Base class every Database Entity must inherit
/// </summary>
public abstract class EntityBase
{
    //public Guid Id { get; set; }

    [Key]
    [Column("ID")]
    [JsonProperty(PropertyName = "ID")]
    public virtual int Id { get; set; }

    [DataType(DataType.Date)]
    public DateTimeOffset CreatedAt { get; set; }

    [DataType(DataType.Date)]
    public DateTimeOffset UpdatedAt { get; set; }

    public bool IsNew()
    {
        return Id == default;
    }
}