using System.Security.Cryptography;
using System.Text.Json;
using GFinancas.Application.DTOs.Auth;
using GFinancas.Application.Exceptions;
using GFinancas.Application.Interfaces.Repository;
using GFinancas.Application.Interfaces.Security;
using GFinancas.Application.Interfaces.Services;
using Microsoft.AspNetCore.DataProtection;

namespace GFinancas.API.Services;

public sealed class RecuperacaoSenhaService : IRecuperacaoSenhaService
{
    private const string LinkInvalido = "O link é inválido, expirou ou já foi utilizado. Solicite um novo link.";
    private readonly IUsuarioRepository _usuarios;
    private readonly ISenhaHasher _hasher;
    private readonly ITimeLimitedDataProtector _protector;
    private readonly IEmailRecuperacaoSenha _email;
    private readonly IConfiguration _configuration;

    public RecuperacaoSenhaService(IUsuarioRepository usuarios, ISenhaHasher hasher,
        IDataProtectionProvider provider, IEmailRecuperacaoSenha email, IConfiguration configuration)
    {
        _usuarios = usuarios;
        _hasher = hasher;
        _protector = provider.CreateProtector("GFinancas.RecuperacaoSenha.v1").ToTimeLimitedDataProtector();
        _email = email;
        _configuration = configuration;
    }

    public async Task SolicitarAsync(RecuperarSenhaDto dto)
    {
        var url = _configuration["RecuperacaoSenha:UrlFrontend"];
        if (!Uri.TryCreate(url, UriKind.Absolute, out var frontend) ||
            (frontend.Scheme != "https" && frontend.Scheme != "http"))
        {
            throw new ValidacaoException("O serviço de recuperação de senha não está configurado.");
        }

        _email.ValidarConfiguracao();
        var usuario = await _usuarios.ObterPorEmailAsync(dto.Email.Trim().ToLowerInvariant());
        if (usuario is null)
            return;

        // O hash vincula o link à senha atual. Uma troca de senha invalida todos os links anteriores.
        var dados = JsonSerializer.Serialize(new DadosRecuperacao(usuario.Id, usuario.SenhaHash));
        var token = _protector.Protect(dados, TimeSpan.FromMinutes(30));
        var link = new Uri(frontend, "redefinir-senha").AbsoluteUri + "?token=" + Uri.EscapeDataString(token);
        await _email.EnviarAsync(usuario.Email, link);
    }

    public async Task RedefinirAsync(RedefinirSenhaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Senha) || dto.Senha.Length < 8 || dto.Senha.Length > 128)
            throw new ValidacaoException("A senha deve possuir entre 8 e 128 caracteres.");

        DadosRecuperacao dados;
        try
        {
            dados = JsonSerializer.Deserialize<DadosRecuperacao>(_protector.Unprotect(dto.Token))
                ?? throw new JsonException();
        }
        catch (Exception ex) when (ex is CryptographicException or JsonException or ArgumentException)
        {
            throw new ValidacaoException(LinkInvalido);
        }

        var usuario = await _usuarios.ObterPorIdAsync(dados.UsuarioId);
        if (usuario is null || usuario.SenhaHash != dados.SenhaHash)
            throw new ValidacaoException(LinkInvalido);

        var alterou = await _usuarios.AtualizarSenhaAsync(usuario.Id, dados.SenhaHash, _hasher.GerarHash(dto.Senha));
        if (!alterou)
            throw new ValidacaoException(LinkInvalido);
    }

    private sealed record DadosRecuperacao(int UsuarioId, string SenhaHash);
}
