using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MvcNetCoreUtilidades.Helpers;
using ProyectoGestorPeluqueria.Data;
using ProyectoGestorPeluqueria.Policies;
using ProyectoGestorPeluqueria.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
// Esto es para recoger el nombre del server y el puerto para crear la url de los ficheros subidos
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HelperPathProvider>();
builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddCookie(
    CookieAuthenticationDefaults.AuthenticationScheme,
    config =>
    {
        config.LoginPath = "/Managed/Login";
        config.AccessDeniedPath = "/Managed/ErrorAcceso";
    }
    );

// Add services to the container.

//LAS POLITICAS SE AGREGAN CON AUTHORIZATION
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.Admin,
        policy => policy.AddRequirements(new AdminOEmpresarioRequirement()));
    options.AddPolicy(PolicyNames.Empresario,
        policy => policy.AddRequirements(new EmpresarioRequirement()));
    options.AddPolicy(PolicyNames.Cliente,
        policy => policy.AddRequirements(new ClienteRequirement()));
    options.AddPolicy(PolicyNames.AdminOEmpresario,
        policy => policy.AddRequirements(new AdminOEmpresarioRequirement()));
    options.AddPolicy(PolicyNames.TienePeluquerias,
        policy => policy.AddRequirements(new TienePeluqueriasRequirement()));
});
builder.Services.AddSingleton<IAuthorizationHandler, EmpresarioRequirement>();
builder.Services.AddSingleton<IAuthorizationHandler, ClienteRequirement>();
builder.Services.AddSingleton<IAuthorizationHandler, AdminOEmpresarioRequirement>();
builder.Services.AddSingleton<IAuthorizationHandler, TienePeluqueriasRequirement>();
builder.Services
    .AddControllersWithViews(options => options.EnableEndpointRouting = false)
    .AddSessionStateTempDataProvider();
builder.Services.AddTransient<IRepositoryUsuarios, RepositoryUsuarios>();
builder.Services.AddTransient<IRepositoryGestorPeluqueria, RepositoryGestorPeluqueria>();

string connectionstring = builder.Configuration.GetConnectionString("SqlCasa");
builder.Services.AddDbContext<PeluqueriaContext>(options =>
    options.UseSqlServer(connectionstring));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
