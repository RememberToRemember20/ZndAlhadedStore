using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Http
{

    public sealed class ApiError
    {
        public ApiError(int status, string code, string? detail = null, IEnumerable<string>? detailCodes = null,
            IReadOnlyDictionary<string, string[]>? fieldCodes = null, int? retryAfterSeconds = null)
        {
            Status = status;
            Code = code;
            Detail = detail;
            DetailCodes = detailCodes?.ToArray() ?? Array.Empty<string>();
            FieldCodes = fieldCodes ?? new Dictionary<string, string[]>();
            RetryAfterSeconds = retryAfterSeconds;
        }

        public int Status { get; }
        public string Code { get; }
        public string? Detail { get; }
        public IReadOnlyList<string> DetailCodes { get; }
        public IReadOnlyDictionary<string, string[]> FieldCodes { get; }
        public int? RetryAfterSeconds { get; }

        public static ApiError Local(int status, string code) => new(status, code);

        /// أول كود خطأ لحقل معين (مطابقة آخر جزء من الاسم، فـ "User.Email" تطابق "email")
        public string? FieldCode(string field)
        {
            foreach (var (name, codes) in FieldCodes)
            {
                var last = name[(name.LastIndexOf('.') + 1)..];
                if (codes.Length > 0 && string.Equals(last, field, StringComparison.OrdinalIgnoreCase)) return codes[0];
            }
            return null;
        }
    }

    public class ApiResult
    {
        public bool IsSuccess { get; init; }
        public int Status { get; init; }
        public ApiError? Error { get; init; }
    }

    public sealed class ApiResult<T> : ApiResult
    {
        public T? Value { get; init; }

        public static ApiResult<T> Ok(T? value, int status) => new() { IsSuccess = true, Status = status, Value = value };
        public static ApiResult<T> Fail(ApiError error) => new() { IsSuccess = false, Status = error.Status, Error = error };
    }
}
