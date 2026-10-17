using System;

namespace GFinancas.Application.Exceptions;

/// <summary>
/// Sinaliza dados inválidos, convertidos em HTTP 400 pelo tratamento global.
/// </summary>
public class ValidacaoException : Exception
{
	/// <summary>
	/// Inicializa a exceção com a mensagem informada.
	/// </summary>
	/// <param name="message">Mensagem que descreve a causa do erro.</param>
	public ValidacaoException(string message)
		: base(message)
	{
	}
}
