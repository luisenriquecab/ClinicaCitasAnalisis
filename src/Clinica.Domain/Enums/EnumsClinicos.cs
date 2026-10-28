namespace Clinica.Domain.Enums;

public enum EstadoCita
{
    Programada = 1,
    Confirmada = 2,
    EnAtencion = 3,
    Atendida = 4,
    Cancelada = 5,
    CanceladaTardia = 6,
    Inasistencia = 7
}

public enum CanalOrigen
{
    Web = 1,
    Movil = 2,
    Recepcion = 3
}

public enum PreferenciaNotificacion
{
    Email = 1,
    SMS = 2,
    WhatsApp = 3,
    PushNotification = 4
}

public enum TipoNotificacion
{
    ConfirmacionReserva = 1,
    Recordatorio24Horas = 2,
    Recordatorio2Horas = 3,
    Reprogramacion = 4,
    Cancelacion = 5
}
