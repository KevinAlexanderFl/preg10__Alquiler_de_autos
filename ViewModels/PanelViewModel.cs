using SistemaAlquilerAutos.Models;

namespace SistemaAlquilerAutos.ViewModels;

public class PanelViewModel
{
    public int TotalClientes { get; set; }
    public int VehiculosDisponibles { get; set; }
    public int ContratosActivos { get; set; }
    public decimal IngresosMes { get; set; }
    public IReadOnlyList<Contrato> ContratosRecientes { get; set; } = Array.Empty<Contrato>();
}
