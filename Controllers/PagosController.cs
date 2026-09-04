using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaAlquilerAutos.Datos;
using SistemaAlquilerAutos.Models;
using SistemaAlquilerAutos.Servicios;

namespace SistemaAlquilerAutos.Controllers;

public class PagosController : Controller
{
    private readonly ContextoAlquiler _contexto;
    private readonly IServicioPagos _servicio;

    public PagosController(ContextoAlquiler contexto, IServicioPagos servicio)
    {
        _contexto = contexto;
        _servicio = servicio;
    }

    public async Task<IActionResult> Index(int? contratoId)
    {
        var consulta = _contexto.Pagos.Include(x => x.Contrato)!.ThenInclude(x => x!.Cliente)
            .AsNoTracking().AsQueryable();
        if (contratoId.HasValue) consulta = consulta.Where(x => x.ContratoId == contratoId);
        ViewBag.ContratoId = contratoId;
        return View(await consulta.OrderByDescending(x => x.FechaPago).ThenByDescending(x => x.Id).ToListAsync());
    }

    public async Task<IActionResult> Detalles(int id)
    {
        var pago = await _contexto.Pagos.Include(x => x.Contrato)!.ThenInclude(x => x!.Cliente)
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return pago is null ? NotFound() : View(pago);
    }

    public async Task<IActionResult> Crear(int? contratoId)
    {
        var pago = new Pago { ContratoId = contratoId ?? 0 };
        await CargarContratosAsync(pago.ContratoId);
        return View(pago);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Pago pago)
    {
        if (ModelState.IsValid)
        {
            var resultado = await _servicio.CrearAsync(pago);
            if (resultado.Exitoso)
            {
                TempData["Exito"] = resultado.Mensaje;
                return RedirectToAction(nameof(Index), new { contratoId = pago.ContratoId });
            }
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
        }
        await CargarContratosAsync(pago.ContratoId);
        return View(pago);
    }

    public async Task<IActionResult> Editar(int id)
    {
        var pago = await _contexto.Pagos.FindAsync(id);
        if (pago is null) return NotFound();
        await CargarContratosAsync(pago.ContratoId);
        return View(pago);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Pago pago)
    {
        if (id != pago.Id) return NotFound();
        if (ModelState.IsValid)
        {
            var resultado = await _servicio.ActualizarAsync(pago);
            if (resultado.Exitoso)
            {
                TempData["Exito"] = resultado.Mensaje;
                return RedirectToAction(nameof(Index), new { contratoId = pago.ContratoId });
            }
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
        }
        await CargarContratosAsync(pago.ContratoId);
        return View(pago);
    }

    public async Task<IActionResult> Eliminar(int id)
    {
        var pago = await _contexto.Pagos.Include(x => x.Contrato)!.ThenInclude(x => x!.Cliente)
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return pago is null ? NotFound() : View(pago);
    }

    [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarEliminar(int id)
    {
        var resultado = await _servicio.EliminarAsync(id);
        TempData[resultado.Exitoso ? "Exito" : "Error"] = resultado.Mensaje;
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarContratosAsync(int contratoId)
    {
        var contratos = (await _contexto.Contratos.Include(x => x.Cliente).Include(x => x.Pagos)
            .Where(x => x.Estado != EstadoContrato.Cancelado)
            .OrderByDescending(x => x.Id).AsNoTracking().ToListAsync())
            .Where(x => x.SaldoPendiente > 0).ToList();

        if (contratoId > 0 && contratos.All(x => x.Id != contratoId))
        {
            var actual = await _contexto.Contratos.Include(x => x.Cliente).Include(x => x.Pagos)
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == contratoId);
            if (actual is not null) contratos.Add(actual);
        }

        ViewBag.ContratoId = new SelectList(contratos.Select(x => new
        {
            x.Id,
            Nombre = $"Contrato #{x.Id:0000} · {x.Cliente!.NombreCompleto} · Saldo ${x.SaldoPendiente:N2}"
        }), "Id", "Nombre", contratoId);
    }
}
