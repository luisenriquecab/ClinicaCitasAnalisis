using Clinica.Domain.Enums;
using Clinica.Domain.Exceptions;

namespace Clinica.Domain.Entities;

public class Paciente
{
    public Guid Id { get; private set; }
    public string DPI { get; private set; } = string.Empty;
    public string NombreCompleto { get; private set; } = string.Empty;
    public string Telefono { get; private set; } = string.Empty;
    public string CorreoElectronico { get; private set; } = string.Empty;
    public DateTime FechaNacimiento { get; private set; }
    public PreferenciaNotificacion PreferenciaCanal { get; private set; }
    public bool Activo { get; private set; }
    public DateTime FechaRegistro { get; private set; }

    private Paciente() { } // Constructor protegido para ORM

    public Paciente(Guid id, string dpi, string nombreCompleto, string telefono, string correo, DateTime fechaNacimiento, PreferenciaNotificacion preferencia)
    {
        if (string.IsNullOrWhiteSpace(dpi)) throw new ReglaDeNegocioException("El DPI del paciente es obligatorio.");
        if (string.IsNullOrWhiteSpace(nombreCompleto)) throw new ReglaDeNegocioException("El nombre del paciente es obligatorio.");
        if (string.IsNullOrWhiteSpace(telefono)) throw new ReglaDeNegocioException("El teléfono es obligatorio para enviar recordatorios.");

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        DPI = dpi.Trim();
        NombreCompleto = nombreCompleto.Trim();
        Telefono = telefono.Trim();
        CorreoElectronico = correo?.Trim() ?? string.Empty;
        FechaNacimiento = fechaNacimiento;
        PreferenciaCanal = preferencia;
        Activo = true;
        FechaRegistro = DateTime.UtcNow;
    }

    public void ActualizarContacto(string nuevoTelefono, string nuevoCorreo, PreferenciaNotificacion nuevaPreferencia)
    {
        Telefono = nuevoTelefono?.Trim() ?? Telefono;
        CorreoElectronico = nuevoCorreo?.Trim() ?? CorreoElectronico;
        PreferenciaCanal = nuevaPreferencia;
    }
}
