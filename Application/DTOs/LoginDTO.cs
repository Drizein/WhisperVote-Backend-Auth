using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public record LoginDTO(
    [Required] string username,
    [Required] string password);