namespace Greenlogist.Application.Common
{
    /// <summary>
    /// Excepción base para errores a nivel de aplicación (ej. validaciones de entrada, lógica de orquestación).
    /// </summary>
    public class ApplicationException : Exception
    {
        public ApplicationException() { }

        public ApplicationException(string message) : base(message) { }

        public ApplicationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
