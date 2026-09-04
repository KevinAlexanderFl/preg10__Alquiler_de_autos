namespace SistemaAlquilerAutos.Servicios;

public record ResultadoOperacion(bool Exitoso, string Mensaje)
{
    public static ResultadoOperacion Correcto(string mensaje) => new(true, mensaje);
    public static ResultadoOperacion Error(string mensaje) => new(false, mensaje);
}
