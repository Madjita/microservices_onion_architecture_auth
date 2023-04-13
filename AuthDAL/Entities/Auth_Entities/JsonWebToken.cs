using System;
using System.ComponentModel.DataAnnotations;
using AuthDAL.Entities;
using AuthDAL.Entities.Base;

namespace AuthDAL.Entities;

public class JsonWebToken : EntityBase
{
    public string Token { get; set; }
    
    [DataType(DataType.Date)]
    public DateTimeOffset ExpiresAt { get; set; }
    
    [DataType(DataType.Date)]
    public DateTimeOffset DeleteAfter { get; set; }
    
    public int AccountId { get; set; }
    public virtual Account Account { get; set; }
}