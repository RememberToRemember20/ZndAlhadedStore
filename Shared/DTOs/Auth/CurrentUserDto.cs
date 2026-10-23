using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.Auth
{
    public record CurrentUserDto(string Id, string Email, string FullName, bool EmailConfirmed);
}
