using Jwt.Domain.Entities;

namespace Jwt.Application.Model;

public class RoleDto
{
    public required int UserId { get; set; }
    public required ERole Role { get; set; }
}
