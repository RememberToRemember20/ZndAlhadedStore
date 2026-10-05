using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.Auth
{
    public record PermissionDto(int Id, string Name, string? Description);
}
