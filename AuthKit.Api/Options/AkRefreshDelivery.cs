using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Api.Options
{
    public enum AkRefreshDelivery
    {
        Body,     // الافتراضي: الـ Refresh Token في جسم الرد (تطبيقات الموبايل وأدوات الاختبار)
        Cookie,   // كوكي HttpOnly فقط (واجهات المتصفح). غير المتصفحات تترفض
        Both      // كوكي لمن يرسل الهيدر X-AuthKit-Token-Delivery: cookie، وجسم الرد لغيره
    }
}
