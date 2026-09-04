using System.ComponentModel.DataAnnotations;

namespace SistemaAlquilerAutos.Models;

public enum EstadoVehiculo
{
    Disponible,
    Reservado,
    Alquilado,
    Mantenimiento
}

public enum EstadoContrato
{
    Reservado,
    Activo,
    Finalizado,
    Cancelado
}

public enum MetodoPago
{
    Efectivo,
    Tarjeta,
    Transferencia
}

public enum EstadoPago
{
    [Display(Name = "Confirmado")]
    Confirmado,
    Anulado
}
