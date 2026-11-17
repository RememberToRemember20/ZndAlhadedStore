# AuthKit

مصادقة وصلاحيات لـ ASP.NET Core Web API: Identity + JWT (Access/Refresh) + صلاحيات (Role → Permissions) + تدقيق + حماية (Rate Limiting, Lockout, اكتشاف إعادة استخدام التوكن).

## ما تقدمه / ما لا تقدمه

| تقدمه | كل مشروع يكتبه بنفسه |
|---|---|
| Register / Login / Refresh / Logout / LogoutAll | `ApplicationUser` و`ApplicationRole` و`DbContext` |
| استرجاع وتغيير كلمة المرور، تأكيد البريد | **Controllers** (المرجع: `ZndAlHadedStore/Controllers`) |
| أدوار وصلاحيات + `[RequirePermission]` | `IAuthEmailSender<TUser>` (الإرسال والقوالب) |
| Audit Log، Pagination، Rate Limiting، CORS | أسماء الصلاحيات (`Permissions.cs`) والـ Seeding |

## البدء السريع

### 1) الحزم
```bash
dotnet add package AuthKit
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### 2) الكيانات والـ DbContext
```csharp
public class ApplicationUser : IdentityUser { /* حقولك: FullName, IsActive ... */ }
public class ApplicationRole : IdentityRole { }

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyAuthKit<ApplicationUser, ApplicationRole, string>();
    }
}
```
ثم `dotnet ef migrations add Initial` و`dotnet ef database update`.

### 3) الإعدادات
`appsettings.json`:
```json
"Jwt":  { "Issuer": "MyApp.Api", "Audience": "MyApp.Client",
          "AccessTokenExpirationMinutes": 15, "RefreshTokenExpirationDays": 7 },
"Cors": { "AllowedOrigins": [ "http://localhost:3000" ] }
```
الأسرار (User Secrets في التطوير، Environment Variables في الإنتاج):
```powershell
$b = New-Object byte[] 64; [Security.Cryptography.RandomNumberGenerator]::Fill($b)
dotnet user-secrets set "Jwt:Secret" ([Convert]::ToBase64String($b))
dotnet user-secrets set "ConnectionStrings:Default" "..."
```

### 4) `Program.cs`
```csharp
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
})
.AddRoles<ApplicationRole>()
.AddSignInManager<SignInManager<ApplicationUser>>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromMinutes(30));

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddPermissionAuthorization();
builder.Services.AddAuthRateLimiting();
builder.Services.AddConfiguredCors(builder.Configuration);

builder.Services.AddAuthKitCore<ApplicationUser, ApplicationRole, string, AppDbContext>();
builder.Services.AddAuthKitPasswordRecovery<ApplicationUser, string, AppDbContext>();   // يتطلب IAuthEmailSender
builder.Services.AddAuthKitEmailConfirmation<ApplicationUser, string>();                // اختياري، ويتطلب IAuthEmailSender
builder.Services.AddAuthKitAuditing<AppDbContext>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddAuthKitBehaviors();       // Validation ثم Audit بالترتيب الصحيح
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    foreach (var p in builder.Configuration.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>() ?? [])
        options.KnownProxies.Add(IPAddress.Parse(p));
});
```
الـ Pipeline (**الترتيب مهم**):
```csharp
app.UseExceptionHandler();
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

### 5) نقاط التوسع
| الواجهة | مطلوبة؟ | وظيفتها |
|---|---|---|
| `IAuthEmailSender<TUser>` | نعم، مع الاسترجاع/التأكيد | إرسال الرسائل |
| `IUserAccessGuard<TUser>` | موصى بها | منع الدخول والتجديد (مثلاً `IsActive`) |
| `ICustomClaimsProvider<TUser>` | لا | Claims إضافية في الـ JWT |

### 6) الصلاحيات والـ Seeding
عرّف الأسماء في كلاس ثابت (`Users.Create`...)، وازرعها وقت التشغيل في جدول `Permissions` واربطها بدور Admin (المرجع: `DatabaseSeeder`). استخدم `[RequirePermission(Permissions.X.Y)]` على الـ Endpoints.

### 7) الـ Controllers
انسخ من مشروع المتجر: `Auth` و`Users` و`Roles` و`Permissions` و`AuditLogs`. كل Action بتبعت أمراً عبر `ISender`.

## قبل الإنتاج
- `Jwt:Secret` عشوائي (64 بايت) من Environment/Key Vault، مش في الملفات.
- `KnownProxies` بعناوين الـ Proxy الحقيقية، و`Cors:AllowedOrigins` للواجهة الفعلية فقط.
- `IAuthEmailSender` حقيقي (مش المرسل التجريبي اللي بيطبع التوكنز).
- مستخدم قاعدة البيانات بصلاحية `INSERT`/`SELECT` فقط على `AuditLogs`.
- حفظ مفاتيح Data Protection (وإلا توكنز الاسترجاع بتبطل عند إعادة التشغيل).

## الإصدارات
`0.x` = الواجهة ممكن تتغير في أي إصدار. أي تغيير يكسر المشاريع يرفع رقم الـ Minor، والإصلاحات رقم الـ Patch. لما تستقر وتُستخدم في مشروع حقيقي → `1.0.0`.