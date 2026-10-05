using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ZndAlhadedStore;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Common.Behaviors;
using ZndAlhadedStore.Entity.DTOs;
using ZndAlhadedStore.Identity.Seeding;
using ZndAlhadedStore.ImageResolver;
using ZndAlhadedStore.Implment;
using ZndAlhadedStore.Interfaces;
using ZndAlhadedStore.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ZndAlHaded")));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperInitialiser>(), typeof(ImageUrlResolver).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IAuthTokenIssuer, AuthTokenIssuer>();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddIdentityServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplicationServices();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
using (var scope = app.Services.CreateScope())
{
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization(); 
app.MapControllers();

app.Run();
