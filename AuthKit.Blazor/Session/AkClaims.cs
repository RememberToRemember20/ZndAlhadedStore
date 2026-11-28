using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Session
{
    public static class AkClaims
    {
        public const string Permission = "permission";   // نفس AuthKitClaims.Permission في الخادم (فيه اختبار يضمن التطابق)
    }
}
