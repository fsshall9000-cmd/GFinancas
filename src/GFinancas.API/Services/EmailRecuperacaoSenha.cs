using System.Net;
using System.Net.Mail;
using GFinancas.Application.Exceptions;

namespace GFinancas.API.Services;

public interface IEmailRecuperacaoSenha
{
    void ValidarConfiguracao();
    Task EnviarAsync(string destinatario, string link);
}

public sealed class EmailRecuperacaoSenha(IConfiguration configuration,
    ILogger<EmailRecuperacaoSenha> logger) : IEmailRecuperacaoSenha
{
    public void ValidarConfiguracao()
    {
        if (string.IsNullOrWhiteSpace(configuration["Smtp:Host"]) ||
            !MailAddress.TryCreate(configuration["Smtp:Remetente"], out _))
            throw new ValidacaoException("O serviço de envio de e-mail não está configurado.");
    }

    public async Task EnviarAsync(string destinatario, string link)
    {
        ValidarConfiguracao();
        using var client = new SmtpClient(configuration["Smtp:Host"], configuration.GetValue("Smtp:Porta", 587))
        {
            EnableSsl = configuration.GetValue("Smtp:UsarSsl", true),
            UseDefaultCredentials = false
        };
        var usuario = configuration["Smtp:Usuario"];
        if (!string.IsNullOrWhiteSpace(usuario))
            client.Credentials = new NetworkCredential(usuario, configuration["Smtp:Senha"]);

        using var mensagem = new MailMessage(configuration["Smtp:Remetente"]!, destinatario)
        {
            Subject = "GFinanças — redefinir senha",
            Body = $"Para definir uma nova senha, acesse o link abaixo. Ele é válido por 30 minutos.\n\n{link}\n\nSe você não solicitou esta alteração, ignore este e-mail.",
            IsBodyHtml = false
        };
        try
        {
            await client.SendMailAsync(mensagem);
        }
        catch (SmtpException ex)
        {
            // Não registra destinatários, credenciais ou o link de recuperação.
            logger.LogError("Falha no envio de recuperação de senha. Status SMTP: {Status}", ex.StatusCode);
            throw new ValidacaoException("Não foi possível enviar o e-mail. Tente novamente mais tarde.");
        }
    }
}
