using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAlquilerAutos.Models;

public class Vehiculo
{
    public int Id { get; set; }

    [Required, StringLength(8)]
    [RegularExpression(@"^[A-Za-z]{3}-?\d{3,4}$", ErrorMessage = "Ingrese una placa válida, por ejemplo: PBA-1234.")]
    public string Placa { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Marca { get; set; } = string.Empty;

    [Required, StringLength(60)]
    public string Modelo { get; set; } = string.Empty;

    [Range(1990, 2100)]
    [Display(Name = "Año")]
    public int Anio { get; set; }

    [Required, StringLength(30)]
    public string Color { get; set; } = string.Empty;

    [Range(0.01, 10000)]
    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "Tarifa diaria")]
    public decimal PrecioDiario { get; set; }

    public EstadoVehiculo Estado { get; set; } = EstadoVehiculo.Disponible;

    public bool Activo { get; set; } = true;

    public ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();

    public string Descripcion => $"{Marca} {Modelo} · {Placa}";
}
