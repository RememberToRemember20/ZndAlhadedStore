using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Interfaces
{
    public interface IAuthErrorDescriber
    {
        /// نص جاهز للعرض لكود خطأ، أو null لو غير معروف
        string? Describe(string code);
    }
}
