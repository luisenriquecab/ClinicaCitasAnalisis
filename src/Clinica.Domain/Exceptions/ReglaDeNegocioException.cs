namespace Clinica.Domain.Exceptions;

public class ReglaDeNegocioException : Exception
{
    public string CodigoError { get; }

    public ReglaDeNegocioException(string mensaje, string codigoError = "REGLA_NEGOCIO_VIOLADA") 
        : base(mensaje)
    {
        CodigoError = codigoError;
    }
}
