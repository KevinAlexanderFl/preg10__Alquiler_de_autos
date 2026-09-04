using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaAlquilerAutos.Datos;
using SistemaAlquilerAutos.Models;
using SistemaAlquilerAutos.Servicios;

namespace SistemaAlquilerAutos.Controllers;

public class ContratosController : Controller
{
    private readonly ContextoAlquiler _contexto;
    private readonly IServicioContratos _servicio;

    public ContratosController(ContextoAlquiler contexto, IServicioContratos servicio)
    {
        _contexto = contexto;
        _servicio = servicio;
    }

    public async Task<IActionResult> Index(string? buscar, EstadoContrato? estado)
    {
        var consulta = _contexto.Contratos
            .Include(x => x.Cliente).Include(x => x.Vehiculo).Include(x => x.Pagos)
            .AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            buscar = buscar.Trim();
            consulta = consulta.Where(x => x.Cliente!.Cedula.Contains(buscar) ||
                x.Cliente.Nombres.Contains(buscar) || x.Cliente.Apellidos.Contains(buscar) ||
                x.Vehiculo!.Placa.Contains(buscar));
        }
        if (estado.HasValue) consulta = consulta.Where(x => x.Estado == estado);
        ViewBag.Buscar = buscar;
        ViewBag.Estado = estado;
        return View(await consulta.OrderByDescending(x => x.Id).ToListAsync());
    }

    public async Task<IActionResult> Detalles(int id)
    {
        var contrato = await _contexto.Contratos.Include(x => x.Cliente).Include(x => x.Vehiculo)
            .Include(x => x.Pagos).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return contrato is null ? NotFound() : View(contrato);
    }

    public async Task<IActionResult> Crear(int? vehiculoId)
    {
        var contrato = new Contrato { VehiculoId = vehiculoId ?? 0 };
        if (vehiculoId.HasValue)
        {
            var vehiculo = await _contexto.Vehiculos.FindAsync(vehiculoId.Value);
            if (vehiculo is not null) contrato.TarifaDiaria = vehiculo.PrecioDiario;
        }
        await CargarListasAsync(contrato.ClienteId, contrato.VehiculoId);
        return View(contrato);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Contrato contrato)
    {
        if (ModelState.IsValid)
        {
            var resultado = await _servicio.CrearAsync(contrato);
            if (resultado.Exitoso)
            {
                TempData["Exito"] = resultado.Mensaje;
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
        }
        await CargarListasAsync(contrato.ClienteId, contrato.VehiculoId);
        return View(contrato);
    }

    public async Task<IActionResult> Editar(int id)
    {
        var contrato = await _contexto.Contratos.FindAsync(id);
        if (contrato is null) return NotFound();
        await CargarListasAsync(contrato.ClienteId, contrato.VehiculoId);
        return View(contrato);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Contrato contrato)
    {
        if (id != contrato.Id) return NotFound();
        if (ModelState.IsValid)
        {
            var resultado = await _servicio.ActualizarAsync(contrato);
            if (resultado.Exitoso)
            {
                TempData["Exito"] = resultado.Mensaje;
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
        }
        await CargarListasAsync(contrato.ClienteId, contrato.VehiculoId);
        return View(contrato);
    }

    public async Task<IActionResult> Eliminar(int id)
    {
        var contrato = await _contexto.Contratos.Include(x => x.Cliente).Include(x => x.Vehiculo)
            .Include(x => x.Pagos).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return contrato is null ? NotFound() : View(contrato);
    }

    [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarEliminar(int id)
    {
        var resultado = await _servicio.EliminarAsync(id);
        TempData[resultado.Exitoso ? "Exito" : "Error"] = resultado.Mensaje;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Finalizar(int id)
    {
        var resultado = await _servicio.FinalizarAsync(id);
        TempData[resultado.Exitoso ? "Exito" : "Error"] = resultado.Mensaje;
        return RedirectToAction(nameof(Detalles), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        var resultado = await _servicio.CancelarAsync(id);
        TempData[resultado.Exitoso ? "Exito" : "Error"] = resultado.Mensaje;
        return RedirectToAction(nameof(Detalles), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> TarifaVehiculo(int id)
    {
        var vehiculo = await _contexto.Vehiculos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return vehiculo is null ? NotFound() : Json(new { tarifa = vehiculo.PrecioDiario });
    }

    private async Task CargarListasAsync(int clienteId, int vehiculoId)
    {
        var clientes = await _contexto.Clientes.Where(x => x.Activo)
            .OrderBy(x => x.Apellidos).ThenBy(x => x.Nombres).AsNoTracking().ToListAsync();
        var vehiculos = await _contexto.Vehiculos.Where(x => x.Activo)
            .OrderBy(x => x.Marca).ThenBy(x => x.Modelo).AsNoTracking().ToListAsync();
        ViewBag.ClienteId = new SelectList(clientes.Select(x => new { x.Id, Nombre = $"{x.Cedula} · {x.NombreCompleto}" }), "Id", "Nombre", clienteId);
        ViewBag.VehiculoId = new SelectList(vehiculos.Select(x => new { x.Id, Nombre = $"{x.Descripcion} · {x.Estado}" }), "Id", "Nombre", vehiculoId);
    }
}
