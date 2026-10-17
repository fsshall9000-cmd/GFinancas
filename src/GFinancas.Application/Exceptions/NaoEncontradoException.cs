	using System;

	namespace GFinancas.Application.Exceptions;

	/// <summary>
	/// Sinaliza recurso não encontrado, convertido em HTTP 404 pelo tratamento global.
	/// </summary>
	public class NaoEncontradoException : Exception
	{
		/// <summary>
		/// Inicializa a exceção sem mensagem personalizada.
		/// </summary>
		public NaoEncontradoException()
		{
		}

		/// <summary>
		/// Inicializa a exceção com a mensagem informada.
		/// </summary>
		/// <param name="message">Mensagem que descreve a causa do erro.</param>
		public NaoEncontradoException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Inicializa a exceção com a mensagem e a causa original.
		/// </summary>
		/// <param name="message">Mensagem que descreve a causa do erro.</param>
		/// <param name="innerException">Exceção original que causou esta falha.</param>
		public NaoEncontradoException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
    