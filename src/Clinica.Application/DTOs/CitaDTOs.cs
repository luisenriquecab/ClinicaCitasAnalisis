using Clinica.Domain.Enums;

namespace Clinica.Application.DTOs;

public record SolicitudAgendarCitaDTO(
    Guid PacienteId,
    Guid MedicoId,
    DateTime FechaHoraInicio,
    string MotivoConsulta,
    CanalOrigen CanalOrigen = CanalOrigen.Web
);

public record SolicitudReprogramarCitaDTO(
    Guid CitaId,
    DateTime NuevaFechaHoraInicio,
    string MotivoReprogramacion
);

public record SolicitudCancelarCitaDTO(
    Guid CitaId,
    string MotivoCancelacion,
    string CanceladoPor
);

public record CitaDetalleDTO(
    Guid Id,
    Guid PacienteId,
    string NombrePaciente,
    string TelefonoPaciente,
    Guid MedicoId,
    string NombreMedico,
    string EspecialidadMedico,
    DateTime FechaHoraInicio,
    DateTime FechaHoraFin,
    EstadoCita Estado,
    string EstadoDescripcion,
    CanalOrigen CanalOrigen,
    string MotivoConsulta,
    bool Recordatorio24hEnviado,
    bool Recordatorio2hEnviado,
    bool AplicaCargoPorCancelacionTardia
);

public record FranjaHorariaDTO(
    DateTime Inicio,
    DateTime Fin,
    bool Disponible,
    string MotivoBloqueo = ""
);

public record InasistenciaPorMedicoDTO(
    Guid MedicoId,
    string NombreMedico,
    string Especialidad,
    int TotalCitas,
    int CitasInasistidas,
    double PorcentajeInasistencia
);

public record ReporteInasistenciasDTO(
    DateTime FechaInicio,
    DateTime FechaFin,
    int TotalCitasProgramadas,
    int TotalInasistencias,
    double TasaGlobalInasistenciaPorcentaje,
    int CitasCanceladasTardias,
    List<InasistenciaPorMedicoDTO> DesglosePorMedico
);
