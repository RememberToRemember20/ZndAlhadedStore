using AuthKit.Common;
using AuthKit.Services.Interfaces;

namespace ZndAlhadedStore.Errors
{
    public class AppErrorDescriber : IAuthErrorDescriber
    {
        public string? Describe(string code) => code switch
        {
            AuthErrorCodes.InvalidCredentials => "بيانات الدخول غير صحيحة.",
            AuthErrorCodes.LockedOut => "تم قفل الحساب مؤقتاً بسبب محاولات دخول فاشلة متكررة.",
            AuthErrorCodes.EmailNotConfirmed => "يرجى تأكيد بريدك الإلكتروني أولاً.",
            AuthErrorCodes.TokenReuseDetected => "تم اكتشاف نشاط مشبوه، تم إنهاء جميع الجلسات. يرجى تسجيل الدخول مرة أخرى.",
            AuthErrorCodes.InvalidToken => "جلسة غير صالحة، يرجى تسجيل الدخول مرة أخرى.",
            AuthErrorCodes.SessionExpired => "انتهت صلاحية الجلسة، يرجى تسجيل الدخول مرة أخرى.",
            AuthErrorCodes.AccountUnavailable => "الحساب غير متاح.",
            AuthErrorCodes.Exception => "حدث خطأ غير متوقع. حاول مرة أخرى.",
            AuthErrorCodes.InvalidRole => "الدور غير موجود.",
            AuthErrorCodes.PrivilegeEscalation => "لا يمكنك تنفيذ هذا الإجراء لأنه يتضمن صلاحيات لا تملكها.",
            AuthErrorCodes.ProvisioningRejected => "تم رفض إنشاء الحساب وفق قواعد التطبيق.",
            AuthErrorCodes.Forbidden => "ليست لديك صلاحية لتنفيذ هذا الإجراء.",
            AuthErrorCodes.LastAdministrator => "لا يمكن تنفيذ ذلك لأنه سيُبقي النظام بدون أي مسؤول يملك هذه الصلاحية.",
            AuthErrorCodes.ProtectedRole => "هذا الدور محمي ولا يمكن حذفه أو إعادة تسميته.",
            AuthErrorCodes.EmailTaken => "البريد الإلكتروني مستخدم بالفعل.",
            AuthErrorCodes.UserNotFound => "المستخدم غير موجود.",
            AuthErrorCodes.InvalidLink => "الرابط غير صالح أو منتهي الصلاحية.",
            AuthErrorCodes.InvalidCurrentPassword => "كلمة المرور الحالية غير صحيحة.",
            AuthErrorCodes.RoleNameTaken => "يوجد دور بنفس هذا الاسم بالفعل.",
            AuthErrorCodes.RoleInUse => "لا يمكن حذف الدور لأنه مرتبط بمستخدمين. أزل الدور منهم أولاً.",
            AuthErrorCodes.UserAlreadyInRole => "المستخدم لديه هذا الدور بالفعل.",
            AuthErrorCodes.PermissionNotFound => "الصلاحية غير موجودة.",
            AuthErrorCodes.PermissionAlreadyAssigned => "الصلاحية مرتبطة بالفعل بهذا الدور.",
            AuthErrorCodes.IdentityError => "تعذر حفظ البيانات.",

            // أكواد Identity (تظهر في التفاصيل)
            "DuplicateEmail" or "DuplicateUserName" => "البريد الإلكتروني مستخدم بالفعل.",
            "InvalidEmail" => "صيغة البريد الإلكتروني غير صحيحة.",
            "PasswordTooShort" => "كلمة المرور قصيرة جداً.",
            "PasswordRequiresDigit" => "كلمة المرور يجب أن تحتوي على رقم.",
            "PasswordRequiresLower" => "كلمة المرور يجب أن تحتوي على حرف صغير.",
            "PasswordRequiresUpper" => "كلمة المرور يجب أن تحتوي على حرف كبير.",
            "PasswordRequiresNonAlphanumeric" => "كلمة المرور يجب أن تحتوي على رمز خاص.",

            // أكواد التحقق من الشكل
            "NotEmptyValidator" or "NotNullValidator" => "هذا الحقل مطلوب.",
            "EmailValidator" => "صيغة البريد الإلكتروني غير صحيحة.",
            "MinimumLengthValidator" => "القيمة أقصر من الحد المسموح.",
            "MaximumLengthValidator" => "القيمة أطول من الحد المسموح.",
            "NotEqualValidator" => "القيمة يجب أن تختلف عن القيمة الحالية.",
            ValidationCodes.PageInvalid => "رقم الصفحة غير صالح.",
            ValidationCodes.PageSizeInvalid => "حجم الصفحة غير صالح.",
            ValidationCodes.RolesCountInvalid => "حدد من دور واحد إلى 10 أدوار.",
            ValidationCodes.DateRangeInvalid => "تاريخ البداية يجب أن يسبق تاريخ النهاية.",

            // أكواد خاصة بمشروعك
            "staff_profile_required" => "بيانات الموظف مطلوبة.",
            "reports_to_invalid" => "المسؤول المباشر غير موجود أو غير نشط.",

            _ => null
        };
    }
}
