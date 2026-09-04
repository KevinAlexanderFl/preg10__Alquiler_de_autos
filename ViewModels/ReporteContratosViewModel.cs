using SistemaAlquilerAutos.Models;

namespace SistemaAlquilerAutos.ViewModels;

public class ReporteContratosViewModel
{
    public int? ClienteId { get; set; }
    public IReadOnlyList<Cliente> Clientes { get; set; } = Array.Empty<Cliente>();
    public IReadOnlyList<Contrato> Contratos { get; set; } = Array.Empty<Contrato>();
    public decimal TotalContratado => Contratos.Sum(x => x.Total);
    public decimal TotalPagado => Contratos.Sum(x => x.TotalPagado);
    public decimal SaldoPendiente => Contratos.Sum(x => x.SaldoPendiente);
}
