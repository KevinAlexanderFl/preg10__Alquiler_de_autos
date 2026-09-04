using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAlquilerAutos.Datos;
using SistemaAlquilerAutos.ViewModels;

namespace SistemaAlquilerAutos.Controllers;

public class ReportesController : Controller
{
    private readonly ContextoAlquiler _contexto;

    public ReportesController(ContextoAlquiler contexto) => _contexto = contexto;

    public async Task<IActionResult> ContratosPorCliente(int? clienteId)
    {
        var consulta = _contexto.Contratos.Include(x => x.Cliente).Include(x => x.Vehiculo)
            .Include(x => x.Pagos).AsNoTracking().AsQueryable();
        if (clienteId.HasValue) consulta = consulta.Where(x => x.ClienteId == clienteId.Value);

        var modelo = new ReporteContratosViewModel
        {
            ClienteId = clienteId,
            Clientes = await _contexto.Clientes.OrderBy(x => x.Apellidos).ThenBy(x => x.Nombres).AsNoTracking().ToListAsync(),
            Contratos = await consulta.OrderBy(x => x.Cliente!.Apellidos).ThenByDescending(x => x.FechaInicio).ToListAsync()
        };
        return View(modelo);
    }
}
