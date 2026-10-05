using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.Auth
{
    public record RoleDto(string Id, string Name, string? Description);
}
