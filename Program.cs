using Microsoft.EntityFrameworkCore;
using SistemaAlquilerAutos.Datos;
using SistemaAlquilerAutos.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var cadenaConexion = builder.Configuration.GetConnectionString("MySql")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'MySql'.");

builder.Services.AddDbContext<ContextoAlquiler>(opciones =>
    opciones.UseMySql(cadenaConexion, ServerVersion.AutoDetect(cadenaConexion)));

builder.Services.AddScoped<IServicioContratos, ServicioContratos>();
builder.Services.AddScoped<IServicioPagos, ServicioPagos>();

var app = builder.Build();

using (var alcance = app.Services.CreateScope())
{
    var contexto = alcance.ServiceProvider.GetRequiredService<ContextoAlquiler>();
    await contexto.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Inicio/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=Index}/{id?}");

app.Run();
