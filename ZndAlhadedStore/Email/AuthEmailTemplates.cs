using System.Net;

namespace ZndAlhadedStore.Email
{
    public static class AuthEmailTemplates
    {
        public static (string Subject, string Html, string Text) EmailConfirmation(string fullName, string link) =>
            Build("تأكيد بريدك الإلكتروني", fullName,
                  "شكراً لتسجيلك. اضغط على الزر أدناه لتأكيد بريدك الإلكتروني. الرابط صالح لمدة 30 دقيقة.",
                  "تأكيد البريد", link,
                  "إذا لم تنشئ حساباً لدينا، تجاهل هذه الرسالة.");

        public static (string Subject, string Html, string Text) PasswordReset(string fullName, string link) =>
            Build("إعادة تعيين كلمة المرور", fullName,
                  "وصلنا طلب لإعادة تعيين كلمة مرور حسابك. اضغط على الزر أدناه لاختيار كلمة مرور جديدة. الرابط صالح لمدة 30 دقيقة.",
                  "إعادة تعيين كلمة المرور", link,
                  "إذا لم تطلب ذلك، تجاهل هذه الرسالة وستبقى كلمة مرورك كما هي.");

        private static (string, string, string) Build(
            string title, string fullName, string body, string buttonText, string link, string footer)
        {
            var name = WebUtility.HtmlEncode(fullName);
            var safeLink = WebUtility.HtmlEncode(link);

            var html = $"""
            <div dir="rtl" style="font-family:Tahoma,Arial,sans-serif;max-width:480px;margin:auto;padding:24px;border:1px solid #e5e5e5;border-radius:8px">
              <h2 style="margin-top:0">{title}</h2>
              <p>مرحباً {name}،</p>
              <p>{body}</p>
              <p style="text-align:center;margin:28px 0">
                <a href="{safeLink}" style="background:#2563eb;color:#fff;padding:12px 24px;border-radius:6px;text-decoration:none">{buttonText}</a>
              </p>
              <p style="color:#666;font-size:13px">{footer}</p>
            </div>
            """;

            var text = $"مرحباً {fullName}،\n\n{body}\n\n{link}\n\n{footer}";
            return (title, html, text);
        }
    }
}
