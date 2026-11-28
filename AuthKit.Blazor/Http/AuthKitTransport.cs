using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace AuthKit.Blazor.Http
{
    public sealed class AuthKitTransport
    {
        private readonly HttpClient _http;
        public AuthKitTransport(HttpClient http) => _http = http;

        public async Task<ApiResult<T>> SendAsync<T>(
            HttpMethod method, string path, object? body = null, string? bearer = null, CancellationToken ct = default)
        {
            try
            {
                using var request = new HttpRequestMessage(method, path);
                if (body is not null)
                    request.Content = JsonContent.Create(body, AkJson.Default.Options.GetTypeInfo(body.GetType()));
                if (bearer is not null)
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

                using var response = await _http.SendAsync(request, ct);
                return await ReadAsync<T>(response, ct);
            }
            catch (HttpRequestException)
            {
                return ApiResult<T>.Fail(ApiError.Local(0, "network_error"));
            }
            catch (TaskCanceledException) when (!ct.IsCancellationRequested)
            {
                return ApiResult<T>.Fail(ApiError.Local(0, "network_error"));   // انتهت المهلة
            }
        }

        private static async Task<ApiResult<T>> ReadAsync<T>(HttpResponseMessage response, CancellationToken ct)
        {
            var status = (int)response.StatusCode;

            if (!response.IsSuccessStatusCode)
                return ApiResult<T>.Fail(await ParseErrorAsync(response, status, ct));

            if (typeof(T) == typeof(AkNone) || response.StatusCode == HttpStatusCode.NoContent)
                return ApiResult<T>.Ok(default, status);

            try
            {
                var info = (JsonTypeInfo<T>)AkJson.Default.Options.GetTypeInfo(typeof(T));
                return ApiResult<T>.Ok(await response.Content.ReadFromJsonAsync(info, ct), status);
            }
            catch (JsonException)
            {
                return ApiResult<T>.Fail(ApiError.Local(status, "invalid_response"));
            }
        }

        private static async Task<ApiError> ParseErrorAsync(HttpResponseMessage response, int status, CancellationToken ct)
        {
            var code = status switch
            {
                400 => "bad_request",
                401 => "unauthorized",
                403 => "forbidden",
                404 => "not_found",
                409 => "conflict",
                429 => "rate_limited",
                >= 500 => "server_error",
                _ => "unknown"
            };

            string? detail = null;
            var detailCodes = new List<string>();
            var fieldCodes = new Dictionary<string, string[]>();
            var explicitCode = false;

            try
            {
                var text = await response.Content.ReadAsStringAsync(ct);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    using var doc = JsonDocument.Parse(text);
                    var root = doc.RootElement;

                    if (root.ValueKind == JsonValueKind.Object)
                    {
                        if (root.TryGetProperty("code", out var c) && c.ValueKind == JsonValueKind.String)
                        {
                            code = c.GetString()!;
                            explicitCode = true;
                        }

                        if (root.TryGetProperty("detail", out var d) && d.ValueKind == JsonValueKind.String)
                            detail = d.GetString();

                        if (root.TryGetProperty("details", out var details) && details.ValueKind == JsonValueKind.Array)
                            foreach (var item in details.EnumerateArray())
                                if (item.ValueKind == JsonValueKind.Object && item.TryGetProperty("code", out var ic) && ic.ValueKind == JsonValueKind.String)
                                    detailCodes.Add(ic.GetString()!);

                        if (root.TryGetProperty("codes", out var codes) && codes.ValueKind == JsonValueKind.Object)
                            foreach (var p in codes.EnumerateObject().Where(p => p.Value.ValueKind == JsonValueKind.Array))
                                fieldCodes[p.Name] = p.Value.EnumerateArray()
                                    .Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString()!).ToArray();
                    }
                }
            }
            catch (JsonException) { /* جسم غير JSON: نكتفي برمز الحالة */ }

            if (!explicitCode && fieldCodes.Count > 0) code = "validation_failed";
            if (status == 429) code = "rate_limited";

            int? retryAfter = response.Headers.RetryAfter?.Delta is { } delta ? (int)Math.Ceiling(delta.TotalSeconds) : null;
            return new ApiError(status, code, detail, detailCodes, fieldCodes, retryAfter);
        }
    }
}
