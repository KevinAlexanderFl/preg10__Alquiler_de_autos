using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAlquilerAutos.Datos;
using SistemaAlquilerAutos.Models;

namespace SistemaAlquilerAutos.Controllers;

public class VehiculosController : Controller
{
    private readonly ContextoAlquiler _contexto;

    public VehiculosController(ContextoAlquiler contexto) => _contexto = contexto;

    public async Task<IActionResult> Index(string? buscar, EstadoVehiculo? estado)
    {
        var consulta = _contexto.Vehiculos.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            buscar = buscar.Trim();
            consulta = consulta.Where(x => x.Placa.Contains(buscar) || x.Marca.Contains(buscar) || x.Modelo.Contains(buscar));
        }
        if (estado.HasValue) consulta = consulta.Where(x => x.Estado == estado);
        ViewBag.Buscar = buscar;
        ViewBag.Estado = estado;
        return View(await consulta.OrderBy(x => x.Marca).ThenBy(x => x.Modelo).ToListAsync());
    }

    public async Task<IActionResult> Detalles(int id)
    {
        var vehiculo = await _contexto.Vehiculos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return vehiculo is null ? NotFound() : View(vehiculo);
    }

    public IActionResult Crear() => View(new Vehiculo { Anio = DateTime.Today.Year });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Vehiculo vehiculo)
    {
        Normalizar(vehiculo);
        await ValidarPlaca(vehiculo);
        if (!ModelState.IsValid) return View(vehiculo);
        _contexto.Add(vehiculo);
        await _contexto.SaveChangesAsync();
        TempData["Exito"] = "Vehículo registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Editar(int id)
    {
        var vehiculo = await _contexto.Vehiculos.FindAsync(id);
        return vehiculo is null ? NotFound() : View(vehiculo);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Vehiculo vehiculo)
    {
        if (id != vehiculo.Id) return NotFound();
        Normalizar(vehiculo);
        await ValidarPlaca(vehiculo);
        if (!ModelState.IsValid) return View(vehiculo);
        _contexto.Update(vehiculo);
        await _contexto.SaveChangesAsync();
        TempData["Exito"] = "Vehículo actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Eliminar(int id)
    {
        var vehiculo = await _contexto.Vehiculos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return vehiculo is null ? NotFound() : View(vehiculo);
    }

    [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarEliminar(int id)
    {
        var vehiculo = await _contexto.Vehiculos.Include(x => x.Contratos).FirstOrDefaultAsync(x => x.Id == id);
        if (vehiculo is null) return NotFound();
        if (vehiculo.Contratos.Any())
        {
            TempData["Error"] = "No se puede eliminar el vehículo porque tiene contratos registrados. Puede marcarlo como inactivo.";
            return RedirectToAction(nameof(Index));
        }
        _contexto.Remove(vehiculo);
        await _contexto.SaveChangesAsync();
        TempData["Exito"] = "Vehículo eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarPlaca(Vehiculo vehiculo)
    {
        if (await _contexto.Vehiculos.AnyAsync(x => x.Id != vehiculo.Id && x.Placa == vehiculo.Placa))
            ModelState.AddModelError(nameof(vehiculo.Placa), "Ya existe un vehículo con esta placa.");
    }

    private static void Normalizar(Vehiculo vehiculo)
    {
        vehiculo.Placa = vehiculo.Placa.Trim().ToUpperInvariant();
        vehiculo.Marca = vehiculo.Marca.Trim();
        vehiculo.Modelo = vehiculo.Modelo.Trim();
        vehiculo.Color = vehiculo.Color.Trim();
    }
}
