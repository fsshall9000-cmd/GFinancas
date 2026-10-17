namespace GFinancas.Application.Exceptions;

/// <summary>
/// Sinaliza falha de autorização, convertida em HTTP 401 pelo tratamento global.
/// </summary>
public class NaoAutorizadoException : Exception
{
    /// <summary>
    /// Inicializa a exceção com a mensagem informada.
    /// </summary>
    /// <param name="mensagem">Mensagem que descreve a causa do erro.</param>
    public NaoAutorizadoException(string mensagem)
        : base(mensagem)
    {
    }
}