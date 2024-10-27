using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Domain.Enums;

namespace Application.DTOs;

public record UserDTO(
    [Required] Guid Id,
    [Required] Role role
);