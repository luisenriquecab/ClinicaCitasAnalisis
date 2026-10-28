using Clinica.Application.Common;
using Clinica.Application.DTOs;
using Clinica.Application.Interfaces.Repositories;
using Clinica.Application.Interfaces.Services;
using Clinica.Domain.Enums;

namespace Clinica.Application.Services;

public class InasistenciaTracker : IInasistenciaTracker
{
    private readonly ICitaRepository _citaRepo;
    private readonly IMedicoRepository _medicoRepo;

    public InasistenciaTracker(ICitaRepository citaRepo, IMedicoRepository medicoRepo)
    {
        _citaRepo = citaRepo;
        _medicoRepo = medicoRepo;
    }

    public async Task<Result<bool>> RegistrarInasistenciaAsync(Guid citaId, string observaciones, CancellationToken ct = default)
    {
        var cita = await _citaRepo.ObtenerPorIdAsync(citaId, ct);
        if (cita == null)
        {
            return Result<bool>.Failure("La cita médica no fue encontrada.", "CITA_NO_ENCONTRADA");
        }

        try
        {
            cita.MarcarInasistencia(observaciones);
            await _citaRepo.ActualizarAsync(cita, ct);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al registrar inasistencia: {ex.Message}", "ERROR_INASISTENCIA");
        }
    }

    public async Task<ReporteInasistenciasDTO> GenerarReporteInasistenciasAsync(
        DateTime fechaInicio, 
        DateTime fechaFin, 
        Guid? medicoId = null, 
        CancellationToken ct = default)
    {
        var citas = (await _citaRepo.ObtenerCitasEnRangoAsync(fechaInicio.Date, fechaFin.Date.AddDays(1).AddTicks(-1), ct)).ToList();

        if (medicoId.HasValue && medicoId.Value != Guid.Empty)
        {
            citas = citas.Where(c => c.MedicoId == medicoId.Value).ToList();
        }

        var totalCitas = citas.Count;
        var totalInasistencias = citas.Count(c => c.Estado == EstadoCita.Inasistencia);
        var totalCanceladasTardias = citas.Count(c => c.Estado == EstadoCita.CanceladaTardia);

        double tasaGlobal = totalCitas > 0 ? Math.Round((double)totalInasistencias / totalCitas * 100.0, 2) : 0.0;

        var desglose = new List<InasistenciaPorMedicoDTO>();
        var medicos = await _medicoRepo.ObtenerTodosAsync(ct);

        foreach (var m in medicos)
        {
            var citasDeEsteMedico = citas.Where(c => c.MedicoId == m.Id).ToList();
            if (citasDeEsteMedico.Any() || (medicoId.HasValue && medicoId.Value == m.Id))
            {
                var citasCount = citasDeEsteMedico.Count;
                var inasistenciasCount = citasDeEsteMedico.Count(c => c.Estado == EstadoCita.Inasistencia);
                var pct = citasCount > 0 ? Math.Round((double)inasistenciasCount / citasCount * 100.0, 2) : 0.0;

                desglose.Add(new InasistenciaPorMedicoDTO(
                    m.Id,
                    m.NombreCompleto,
                    m.Especialidad,
                    citasCount,
                    inasistenciasCount,
                    pct
                ));
            }
        }

        return new ReporteInasistenciasDTO(
            fechaInicio,
            fechaFin,
            totalCitas,
            totalInasistencias,
            tasaGlobal,
            totalCanceladasTardias,
            desglose
        );
    }
}
