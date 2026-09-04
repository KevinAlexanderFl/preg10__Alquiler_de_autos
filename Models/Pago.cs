using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAlquilerAutos.Models;

public class Pago
{
    public int Id { get; set; }

    [Required, Display(Name = "Contrato")]
    public int ContratoId { get; set; }

    [Required, DataType(DataType.Date), Display(Name = "Fecha de pago")]
    public DateTime FechaPago { get; set; } = DateTime.Today;

    [Range(0.01, 100000)]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Monto { get; set; }

    [Display(Name = "Método de pago")]
    public MetodoPago Metodo { get; set; }

    [StringLength(60), Display(Name = "Número de referencia")]
    public string? NumeroReferencia { get; set; }

    public EstadoPago Estado { get; set; } = EstadoPago.Confirmado;

    public Contrato? Contrato { get; set; }
}
