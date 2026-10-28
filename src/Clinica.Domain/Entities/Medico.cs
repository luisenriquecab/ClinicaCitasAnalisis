using Clinica.Domain.Exceptions;

namespace Clinica.Domain.Entities;

public class Medico
{
    public Guid Id { get; private set; }
    public string NumeroColegiado { get; private set; } = string.Empty;
    public string NombreCompleto { get; private set; } = string.Empty;
    public string Especialidad { get; private set; } = string.Empty;
    public string CorreoElectronico { get; private set; } = string.Empty;
    public TimeSpan HoraInicioJornada { get; private set; }
    public TimeSpan HoraFinJornada { get; private set; }
    public int DuracionCitaMinutos { get; private set; }
    public bool Activo { get; private set; }

    private Medico() { }

    public Medico(Guid id, string numeroColegiado, string nombreCompleto, string especialidad, string correo, 
                  TimeSpan horaInicio, TimeSpan horaFin, int duracionCitaMinutos = 30)
    {
        if (string.IsNullOrWhiteSpace(numeroColegiado)) throw new ReglaDeNegocioException("El número de colegiado es requerido.");
        if (string.IsNullOrWhiteSpace(nombreCompleto)) throw new ReglaDeNegocioException("El nombre del médico es requerido.");
        if (horaFin <= horaInicio) throw new ReglaDeNegocioException("La jornada laboral debe terminar después de haber iniciado.");
        if (duracionCitaMinutos <= 0) throw new ReglaDeNegocioException("La duración de la cita debe ser mayor a cero minutos.");

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        NumeroColegiado = numeroColegiado.Trim();
        NombreCompleto = nombreCompleto.Trim();
        Especialidad = especialidad.Trim();
        CorreoElectronico = correo.Trim();
        HoraInicioJornada = horaInicio;
        HoraFinJornada = horaFin;
        DuracionCitaMinutos = duracionCitaMinutos;
        Activo = true;
    }

    public bool EstaEnHorarioLaboral(DateTime fechaHora, int duracion)
    {
        var horaInicio = fechaHora.TimeOfDay;
        var horaFin = horaInicio.Add(TimeSpan.FromMinutes(duracion));

        return horaInicio >= HoraInicioJornada && horaFin <= HoraFinJornada;
    }
}
