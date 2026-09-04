using System.ComponentModel.DataAnnotations;

namespace SistemaAlquilerAutos.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required, StringLength(10, MinimumLength = 10)]
    [Display(Name = "Cédula")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "La cédula debe contener 10 números.")]
    public string Cedula { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Nombres { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Apellidos { get; set; } = string.Empty;

    [Required, StringLength(15)]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();

    public string NombreCompleto => $"{Nombres} {Apellidos}";
}
