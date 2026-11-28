using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Localization
{
    public static partial class AkTexts
    {
        public static readonly IReadOnlyDictionary<string, string> Ar = new Dictionary<string, string>
        {
            ["common.email"] = "البريد الإلكتروني",
            ["common.password"] = "كلمة المرور",
            ["common.current_password"] = "كلمة المرور الحالية",
            ["common.new_password"] = "كلمة المرور الجديدة",
            ["common.confirm_password"] = "تأكيد كلمة المرور",
            ["common.show"] = "إظهار",
            ["common.hide"] = "إخفاء",
            ["common.show_password"] = "إظهار كلمة المرور",
            ["common.hide_password"] = "إخفاء كلمة المرور",
            ["common.required"] = "هذا الحقل مطلوب.",
            ["common.invalid_email"] = "صيغة البريد الإلكتروني غير صحيحة.",
            ["common.passwords_mismatch"] = "كلمتا المرور غير متطابقتين.",
            ["common.too_short"] = "يجب ألا يقل الطول عن {0} أحرف.",
            ["common.back_to_login"] = "العودة إلى تسجيل الدخول",
            ["common.sign_out"] = "تسجيل الخروج",
            ["common.sign_out_all"] = "تسجيل الخروج من كل الأجهزة",
            ["common.loading"] = "جارٍ التحميل...",
            ["common.language"] = "اللغة",

            ["login.title"] = "تسجيل الدخول",
            ["login.subtitle"] = "أدخل بياناتك للمتابعة.",
            ["login.submit"] = "دخول",
            ["login.forgot"] = "نسيت كلمة المرور؟",
            ["login.register"] = "إنشاء حساب",
            ["login.resend_confirmation"] = "إعادة إرسال رسالة التأكيد",
            ["login.confirmation_sent"] = "إذا كان الحساب بحاجة لتأكيد، أرسلنا رسالة جديدة إلى بريدك.",

            ["register.title"] = "إنشاء حساب",
            ["register.subtitle"] = "املأ البيانات التالية للبدء.",
            ["register.submit"] = "إنشاء الحساب",
            ["register.have_account"] = "لديك حساب بالفعل؟ تسجيل الدخول",
            ["register.check_email_title"] = "تحقق من بريدك",
            ["register.check_email_body"] = "أرسلنا رسالة تأكيد إلى بريدك. اضغط على الرابط فيها لتفعيل الحساب.",

            ["forgot.title"] = "نسيت كلمة المرور",
            ["forgot.subtitle"] = "أدخل بريدك وسنرسل لك رابطاً لإعادة التعيين.",
            ["forgot.submit"] = "إرسال الرابط",
            ["forgot.sent"] = "إذا كان البريد مسجلاً لدينا، ستصلك رسالة بخطوات الاسترجاع.",

            ["reset.title"] = "تعيين كلمة مرور جديدة",
            ["reset.subtitle"] = "اختر كلمة مرور قوية لا تستخدمها في مكان آخر.",
            ["reset.submit"] = "حفظ كلمة المرور",
            ["reset.done"] = "تم تحديث كلمة المرور. يمكنك الآن تسجيل الدخول.",
            ["reset.missing_link"] = "الرابط غير مكتمل. افتح الرابط من الرسالة مرة أخرى.",

            ["confirm.title"] = "تأكيد البريد الإلكتروني",
            ["confirm.working"] = "جارٍ تأكيد بريدك...",
            ["confirm.done"] = "تم تأكيد بريدك الإلكتروني.",
            ["confirm.expired_hint"] = "لو انتهى الرابط، اطلب رسالة جديدة:",
            ["confirm.resend"] = "إرسال رسالة جديدة",
            ["confirm.resent"] = "إذا كان الحساب بحاجة لتأكيد، أرسلنا رسالة جديدة.",

            ["change.title"] = "تغيير كلمة المرور",
            ["change.submit"] = "تغيير",
            ["change.done"] = "تم تغيير كلمة المرور، وتم تسجيل خروجك من الأجهزة الأخرى.",

            ["profile.title"] = "حسابي",
            ["profile.email"] = "البريد الإلكتروني",
            ["profile.status"] = "حالة البريد",
            ["profile.confirmed"] = "مؤكد",
            ["profile.unconfirmed"] = "غير مؤكد",
            ["profile.roles"] = "الأدوار",
            ["profile.permissions"] = "الصلاحيات",
            ["profile.none"] = "لا يوجد",

            ["field.full_name"] = "الاسم الكامل",
            ["field.phone"] = "رقم الهاتف",

            // أكواد AuthKit
            ["error.invalid_credentials"] = "بيانات الدخول غير صحيحة.",
            ["error.locked_out"] = "تم قفل الحساب مؤقتاً بسبب محاولات دخول فاشلة متكررة.",
            ["error.email_not_confirmed"] = "يرجى تأكيد بريدك الإلكتروني أولاً.",
            ["error.token_reuse_detected"] = "تم اكتشاف نشاط مشبوه، وتم إنهاء كل الجلسات. سجّل الدخول من جديد.",
            ["error.invalid_token"] = "الجلسة غير صالحة. سجّل الدخول من جديد.",
            ["error.session_expired"] = "انتهت صلاحية الجلسة. سجّل الدخول من جديد.",
            ["error.account_unavailable"] = "الحساب غير متاح.",
            ["error.exception"] = "حدث خطأ غير متوقع. حاول مرة أخرى.",
            ["error.invalid_role"] = "الدور غير موجود.",
            ["error.privilege_escalation"] = "لا يمكنك تنفيذ هذا الإجراء لأنه يتضمن صلاحيات لا تملكها.",
            ["error.provisioning_rejected"] = "تم رفض إنشاء الحساب وفق قواعد التطبيق.",
            ["error.forbidden"] = "ليست لديك صلاحية لتنفيذ هذا الإجراء.",
            ["error.last_administrator"] = "لا يمكن تنفيذ ذلك لأنه سيترك النظام بدون مسؤول يملك هذه الصلاحية.",
            ["error.protected_role"] = "هذا الدور محمي ولا يمكن حذفه أو إعادة تسميته.",
            ["error.email_taken"] = "البريد الإلكتروني مستخدم بالفعل.",
            ["error.user_not_found"] = "المستخدم غير موجود.",
            ["error.invalid_link"] = "الرابط غير صالح أو منتهي الصلاحية.",
            ["error.invalid_current_password"] = "كلمة المرور الحالية غير صحيحة.",
            ["error.role_name_taken"] = "يوجد دور بنفس هذا الاسم بالفعل.",
            ["error.role_in_use"] = "لا يمكن حذف الدور لأنه مرتبط بمستخدمين. أزله منهم أولاً.",
            ["error.user_already_in_role"] = "المستخدم لديه هذا الدور بالفعل.",
            ["error.permission_not_found"] = "الصلاحية غير موجودة.",
            ["error.permission_already_assigned"] = "الصلاحية مرتبطة بالفعل بهذا الدور.",
            ["error.identity_error"] = "تعذر حفظ البيانات. راجع الملاحظات التالية:",
            ["error.page_invalid"] = "رقم الصفحة غير صالح.",
            ["error.page_size_invalid"] = "حجم الصفحة غير صالح.",
            ["error.roles_count_invalid"] = "حدد من دور واحد إلى 10 أدوار.",
            ["error.date_range_invalid"] = "تاريخ البداية يجب أن يسبق تاريخ النهاية.",

            // أكواد Identity
            ["error.DuplicateEmail"] = "البريد الإلكتروني مستخدم بالفعل.",
            ["error.DuplicateUserName"] = "البريد الإلكتروني مستخدم بالفعل.",
            ["error.InvalidEmail"] = "صيغة البريد الإلكتروني غير صحيحة.",
            ["error.PasswordTooShort"] = "كلمة المرور قصيرة جداً.",
            ["error.PasswordRequiresDigit"] = "كلمة المرور يجب أن تحتوي على رقم.",
            ["error.PasswordRequiresLower"] = "كلمة المرور يجب أن تحتوي على حرف صغير.",
            ["error.PasswordRequiresUpper"] = "كلمة المرور يجب أن تحتوي على حرف كبير.",
            ["error.PasswordRequiresNonAlphanumeric"] = "كلمة المرور يجب أن تحتوي على رمز خاص.",
            ["error.PasswordRequiresUniqueChars"] = "كلمة المرور تحتاج أحرفاً أكثر تنوعاً.",
            ["error.InvalidToken"] = "الرابط غير صالح أو منتهي الصلاحية.",
            ["error.DuplicateRoleName"] = "يوجد دور بنفس هذا الاسم بالفعل.",
            ["error.InvalidRoleName"] = "اسم الدور غير صالح.",

            // أكواد التحقق من شكل البيانات
            ["error.validation_failed"] = "راجع الحقول المحددة.",
            ["error.NotEmptyValidator"] = "هذا الحقل مطلوب.",
            ["error.NotNullValidator"] = "هذا الحقل مطلوب.",
            ["error.EmailValidator"] = "صيغة البريد الإلكتروني غير صحيحة.",
            ["error.MinimumLengthValidator"] = "القيمة أقصر من الحد المسموح.",
            ["error.MaximumLengthValidator"] = "القيمة أطول من الحد المسموح.",
            ["error.NotEqualValidator"] = "القيمة يجب أن تختلف عن القيمة الحالية.",
            ["error.GreaterThanOrEqualValidator"] = "القيمة أقل من الحد المسموح.",
            ["error.InclusiveBetweenValidator"] = "القيمة خارج النطاق المسموح.",
            ["error.PredicateValidator"] = "القيمة غير صالحة.",

            // أخطاء الشبكة والحالات العامة
            ["error.network_error"] = "تعذر الاتصال بالخادم. تحقق من الإنترنت وحاول مرة أخرى.",
            ["error.rate_limited"] = "محاولات كثيرة. حاول مرة أخرى بعد قليل.",
            ["error.rate_limited_after"] = "محاولات كثيرة. حاول مرة أخرى بعد {0} ثانية.",
            ["error.unauthorized"] = "يجب تسجيل الدخول أولاً.",
            ["error.bad_request"] = "الطلب غير صالح.",
            ["error.not_found"] = "العنصر المطلوب غير موجود.",
            ["error.conflict"] = "تعارض مع بيانات موجودة.",
            ["error.server_error"] = "حدث خطأ في الخادم. حاول لاحقاً.",
            ["error.invalid_response"] = "رد غير متوقع من الخادم.",
            ["error.unknown"] = "حدث خطأ غير متوقع.",
            ["error.csrf_rejected"] = "تم رفض الطلب لأسباب أمنية. أعد تحميل الصفحة وحاول مرة أخرى.",
            ["error.misconfigured_cookie_mode"] = "إعدادات الجلسة غير متوافقة مع الخادم. تواصل مع مسؤول النظام.",
            ["reset.request_new"] = "اطلب رابطاً جديداً"
        };
    }
}
