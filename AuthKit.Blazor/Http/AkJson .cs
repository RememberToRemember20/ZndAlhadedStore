using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuthKit.Blazor.Http
{
    [JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
    [JsonSerializable(typeof(AkAuthTokens))]
    [JsonSerializable(typeof(AkMe))]
    [JsonSerializable(typeof(AkRegisterResult))]
    [JsonSerializable(typeof(AkLoginRequest))]
    [JsonSerializable(typeof(AkRefreshRequest))]
    [JsonSerializable(typeof(AkLogoutRequest))]
    [JsonSerializable(typeof(AkEmailRequest))]
    [JsonSerializable(typeof(AkResetPasswordRequest))]
    [JsonSerializable(typeof(AkChangePasswordRequest))]
    [JsonSerializable(typeof(AkConfirmEmailRequest))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    internal sealed partial class AkJson : JsonSerializerContext;
}
