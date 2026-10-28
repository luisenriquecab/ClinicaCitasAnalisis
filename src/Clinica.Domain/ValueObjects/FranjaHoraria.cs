namespace Clinica.Domain.ValueObjects;

public record FranjaHoraria
{
    public DateTime Inicio { get; init; }
    public DateTime Fin { get; init; }
    public bool Disponible { get; init; }

    public FranjaHoraria(DateTime inicio, DateTime fin, bool disponible = true)
    {
        if (fin <= inicio)
        {
            throw new ArgumentException("La hora de finalización debe ser posterior a la hora de inicio.");
        }

        Inicio = inicio;
        Fin = fin;
        Disponible = disponible;
    }

    public bool SeTraslapaCon(DateTime inicio, DateTime fin)
    {
        return Inicio < fin && Fin > inicio;
    }
}
