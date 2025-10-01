using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SGMG.Data;
using SGMG.Repository;
using SGMG.Repository.RepositoryImpl;
using SGMG.Services;
using SGMG.Services.ServiceImpl;
using SGMG.common.exception;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews(options =>
{

  options.Filters.Add<GlobalExceptionFilter>();
  options.Filters.Add<ValidationFilter>();

});

// Registro de repositorios (Inyección de dependencias)
builder.Services.AddScoped<IPacienteRepository, PacienteRepositoryImpl>();
builder.Services.AddScoped<IMedicoRepository, MedicoRepositoryImpl>();
builder.Services.AddScoped<IEnfermeriaRepository, EnfermeriaRepositoryImpl>();
builder.Services.AddScoped<IPersonalRepository, PersonalRepositoryImpl>();

// Registro de servicios (Inyección de dependencias)
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IMedicoService, MedicoServiceImpl>();
builder.Services.AddScoped<IEnfermeriaService, EnfermeriaServiceImpl>();
builder.Services.AddScoped<IPersonalService, PersonalServiceImpl>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseMigrationsEndPoint();
}
else
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
