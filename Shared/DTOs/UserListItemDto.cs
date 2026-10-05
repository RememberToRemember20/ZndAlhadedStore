using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public record UserListItemDto(string Id, string Email, string FullName, bool IsActive, IReadOnlyList<string> Roles);
}
