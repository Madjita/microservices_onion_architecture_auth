using System.ComponentModel.DataAnnotations;
using AuthDAL.Models;

namespace AuthDAL.Dtos;

public class AuthSignInDto
{
    [Required]
    [RegularExpression(RegexExpressions.PersonellNumber)]
    public string PersonellNumber { get; set; }

    [Required]
    [MinLength(4)]
    public string Password { get; set; }
}