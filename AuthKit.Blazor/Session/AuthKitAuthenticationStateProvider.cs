using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Session
{
    public sealed class AuthKitAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
    {
        private readonly AuthKitSession _session;

        public AuthKitAuthenticationStateProvider(AuthKitSession session)
        {
            _session = session;
            _session.Changed += OnChanged;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync() =>
            new(await _session.GetPrincipalAsync());

        private void OnChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        public void Dispose() => _session.Changed -= OnChanged;
    }
}
