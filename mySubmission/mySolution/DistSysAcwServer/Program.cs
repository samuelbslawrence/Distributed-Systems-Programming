using DistSysAcwServer.Middleware;
using DistSysAcwServer.Pipeline;
using DistSysAcwServer.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<SharedError>();
builder.Services.AddTransient<ErrorHandlingMiddleware>();
builder.Services.AddControllers(options =>
{
    options.AllowEmptyInputInBodyModelBinding = true;
    options.Filters.Add<ActionErrorHandlingFilter>();
});

builder.Services.AddDbContext<DistSysAcwServer.Models.UserContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DistSysAcw;"));

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "CustomAuthentication";
})
.AddScheme<AuthenticationSchemeOptions, DistSysAcwServer.Auth.CustomAuthenticationHandlerMiddleware>("CustomAuthentication", null);

builder.Services.AddTransient<DistSysAcwServer.Auth.CustomAuthenticationHandlerMiddleware>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();