using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAlquilerAutos.Models;

public class Contrato
{
    public int Id { get; set; }

    [Required, Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    [Required, Display(Name = "Vehículo")]
    public int VehiculoId { get; set; }

    [Required, DataType(DataType.Date), Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    [Required, DataType(DataType.Date), Display(Name = "Fecha de devolución")]
    public DateTime FechaFin { get; set; } = DateTime.Today.AddDays(1);

    [Range(0.01, 10000)]
    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "Tarifa diaria")]
    public decimal TarifaDiaria { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Total { get; set; }

    public EstadoContrato Estado { get; set; } = EstadoContrato.Reservado;

    [StringLength(500)]
    public string? Observaciones { get; set; }

    public Cliente? Cliente { get; set; }
    public Vehiculo? Vehiculo { get; set; }
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    [NotMapped]
    public decimal TotalPagado => Pagos.Where(p => p.Estado == EstadoPago.Confirmado).Sum(p => p.Monto);

    [NotMapped]
    public decimal SaldoPendiente => Math.Max(0, Total - TotalPagado);
}
