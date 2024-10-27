using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public record RegisterDTO(
    [Required] [Length(3, 20)] string username,
    [Required]
    [Length(12, 256)]
    [RegularExpression("^(?=.*[\\d])(?=.*[a-z])(?=.*[A-Z])(?=.*[!-/:-@[-`{-ÿ]).{12,}$")]
    string password,
    [Required] bool agb,
    [Required] [Length(9, 9)] string indentId
);