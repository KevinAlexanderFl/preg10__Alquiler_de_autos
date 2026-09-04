using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAlquilerAutos.Datos;
using SistemaAlquilerAutos.Models;

namespace SistemaAlquilerAutos.Controllers;

public class ClientesController : Controller
{
    private readonly ContextoAlquiler _contexto;

    public ClientesController(ContextoAlquiler contexto) => _contexto = contexto;

    public async Task<IActionResult> Index(string? buscar)
    {
        var consulta = _contexto.Clientes.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            buscar = buscar.Trim();
            consulta = consulta.Where(x => x.Cedula.Contains(buscar) ||
                x.Nombres.Contains(buscar) || x.Apellidos.Contains(buscar) || x.Correo.Contains(buscar));
        }
        ViewBag.Buscar = buscar;
        return View(await consulta.OrderBy(x => x.Apellidos).ThenBy(x => x.Nombres).ToListAsync());
    }

    public async Task<IActionResult> Detalles(int id)
    {
        var cliente = await _contexto.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return cliente is null ? NotFound() : View(cliente);
    }

    public IActionResult Crear() => View(new Cliente());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Cliente cliente)
    {
        Normalizar(cliente);
        await ValidarDuplicados(cliente);
        if (!ModelState.IsValid) return View(cliente);

        _contexto.Add(cliente);
        await _contexto.SaveChangesAsync();
        TempData["Exito"] = "Cliente registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Editar(int id)
    {
        var cliente = await _contexto.Clientes.FindAsync(id);
        return cliente is null ? NotFound() : View(cliente);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Cliente cliente)
    {
        if (id != cliente.Id) return NotFound();
        Normalizar(cliente);
        await ValidarDuplicados(cliente);
        if (!ModelState.IsValid) return View(cliente);

        _contexto.Update(cliente);
        await _contexto.SaveChangesAsync();
        TempData["Exito"] = "Cliente actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Eliminar(int id)
    {
        var cliente = await _contexto.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return cliente is null ? NotFound() : View(cliente);
    }

    [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarEliminar(int id)
    {
        var cliente = await _contexto.Clientes.Include(x => x.Contratos).FirstOrDefaultAsync(x => x.Id == id);
        if (cliente is null) return NotFound();
        if (cliente.Contratos.Any())
        {
            TempData["Error"] = "No se puede eliminar el cliente porque tiene contratos registrados. Puede marcarlo como inactivo.";
            return RedirectToAction(nameof(Index));
        }
        _contexto.Remove(cliente);
        await _contexto.SaveChangesAsync();
        TempData["Exito"] = "Cliente eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarDuplicados(Cliente cliente)
    {
        if (await _contexto.Clientes.AnyAsync(x => x.Id != cliente.Id && x.Cedula == cliente.Cedula))
            ModelState.AddModelError(nameof(cliente.Cedula), "Ya existe un cliente con esta cédula.");
        if (await _contexto.Clientes.AnyAsync(x => x.Id != cliente.Id && x.Correo == cliente.Correo))
            ModelState.AddModelError(nameof(cliente.Correo), "Ya existe un cliente con este correo.");
    }

    private static void Normalizar(Cliente cliente)
    {
        cliente.Cedula = cliente.Cedula.Trim();
        cliente.Nombres = cliente.Nombres.Trim();
        cliente.Apellidos = cliente.Apellidos.Trim();
        cliente.Telefono = cliente.Telefono.Trim();
        cliente.Correo = cliente.Correo.Trim().ToLowerInvariant();
        cliente.Direccion = cliente.Direccion?.Trim();
    }
}
