using System;

namespace GFinancas.Application.Exceptions;

/// <summary>
/// Sinaliza conflito com dados existentes, convertido em HTTP 409 pelo tratamento global.
/// </summary>
public class ConflitoException : Exception
{
	/// <summary>
	/// Inicializa a exceção com a mensagem informada.
	/// </summary>
	/// <param name="message">Mensagem que descreve a causa do erro.</param>
	public ConflitoException(string message) : base(message)
	{
	}
}
