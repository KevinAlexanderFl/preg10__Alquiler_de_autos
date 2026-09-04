using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAlquilerAutos.Datos;
using SistemaAlquilerAutos.Models;
using SistemaAlquilerAutos.ViewModels;

namespace SistemaAlquilerAutos.Controllers;

public class InicioController : Controller
{
    private readonly ContextoAlquiler _contexto;

    public InicioController(ContextoAlquiler contexto)
    {
        _contexto = contexto;
    }

    public async Task<IActionResult> Index()
    {
        var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var modelo = new PanelViewModel
        {
            TotalClientes = await _contexto.Clientes.CountAsync(x => x.Activo),
            VehiculosDisponibles = await _contexto.Vehiculos.CountAsync(x => x.Activo && x.Estado == EstadoVehiculo.Disponible),
            ContratosActivos = await _contexto.Contratos.CountAsync(x => x.Estado == EstadoContrato.Activo),
            IngresosMes = await _contexto.Pagos
                .Where(x => x.Estado == EstadoPago.Confirmado && x.FechaPago >= inicioMes)
                .SumAsync(x => (decimal?)x.Monto) ?? 0,
            ContratosRecientes = await _contexto.Contratos
                .Include(x => x.Cliente)
                .Include(x => x.Vehiculo)
                .Include(x => x.Pagos)
                .OrderByDescending(x => x.Id)
                .Take(6)
                .AsNoTracking()
                .ToListAsync()
        };
        return View(modelo);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    });
}
