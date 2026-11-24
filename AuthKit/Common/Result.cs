using System.Text.RegularExpressions;

namespace AuthKit.Common
{
    // Common/Result.cs (بعد التعديل)

    public class Result
    {
        private static readonly Regex CodeFormat = new("^[a-z][a-z0-9_]*$", RegexOptions.Compiled);

        public bool IsSuccess { get; }
        public string? ErrorCode { get; }
        public IReadOnlyList<string> Details { get; }

        /// نفس ErrorCode (للاختبارات والـ Logging). مش رسالة للمستخدم
        public string? Error => ErrorCode;

        protected Result(bool isSuccess, string? errorCode, IReadOnlyList<string>? details)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            Details = details ?? Array.Empty<string>();
        }

        public static Result Success() => new(true, null, null);

        public static Result Failure(string errorCode, IEnumerable<string>? details = null) =>
            new(false, ValidateCode(errorCode), details?.ToArray());

        protected static string ValidateCode(string code) =>
            CodeFormat.IsMatch(code)
                ? code
                : throw new ArgumentException($"'{code}' is not an error code (expected snake_case, not a message).", nameof(code));
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(bool isSuccess, T? value, string? errorCode, IReadOnlyList<string>? details)
            : base(isSuccess, errorCode, details) => Value = value;

        public static Result<T> Success(T value) => new(true, value, null, null);

        public static new Result<T> Failure(string errorCode, IEnumerable<string>? details = null) =>
            new(false, default, ValidateCode(errorCode), details?.ToArray());
    }
}
