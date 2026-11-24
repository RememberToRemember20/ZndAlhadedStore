using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Interfaces
{
    internal interface IAuthKitSeeder
    {
        Task SeedAsync(CancellationToken cancellationToken);
    }
}
