using GFinancas.Application.DTOs.Auth;
using GFinancas.Application.Exceptions;
using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Application.Interfaces.Repository;
using GFinancas.Application.Interfaces.Security;
using GFinancas.Application.Interfaces.Services;

namespace GFinancas.Application.Services;

/// <summary>
/// Valida as credenciais de acesso e produz a resposta de autenticação.
/// </summary>
public class AuthService : IAuthService
{
    /// <summary>
    /// Repositório usado para acessar os dados de usuários.
    /// </summary>
    private readonly IUsuarioRepository _usuarioRepository;
    /// <summary>
    /// Serviço de geração e verificação de hashes de senha.
    /// </summary>
    private readonly ISenhaHasher _senhaHasher;
    /// <summary>
    /// Serviço responsável pela emissão do token JWT.
    /// </summary>
    private readonly ITokenService _tokenService;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="usuarioRepository">Repositório usado para acessar os dados de usuários.</param>
    /// <param name="senhaHasher">Serviço de geração e verificação de hashes de senha.</param>
    /// <param name="tokenService">Serviço responsável pela emissão do token JWT.</param>
    public AuthService(
        IUsuarioRepository usuarioRepository,
        ISenhaHasher senhaHasher,
        ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _senhaHasher = senhaHasher;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Valida e-mail e senha, verifica o hash armazenado e retorna o token e os dados do usuário.
    /// </summary>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ValidacaoException("O e-mail é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Senha))
            throw new ValidacaoException("A senha é obrigatória.");

        // E-mail de login sem espaços nas extremidades e convertido para minúsculas.
        var email = dto.Email
            .Trim()
            .ToLowerInvariant();

        // Registro de usuário consultado para esta operação; sua existência é verificada antes do uso.
        var usuario =
            await _usuarioRepository.ObterPorEmailAsync(email);

        if (usuario is null)
            throw new NaoEncontradoException(
                "E-mail ou senha inválidos.");

        // Indica se a senha informada corresponde ao hash armazenado.
        var senhaValida = _senhaHasher.Verificar(
            dto.Senha,
            usuario.SenhaHash);

        if (!senhaValida)
            throw new ValidacaoException(
                "E-mail ou senha inválidos.");

        // Token JWT assinado que será devolvido ao cliente.
        var token = _tokenService.GerarToken(usuario);

        return new LoginResponseDto
        {
            Token = token,
            Expiracao = DateTime.UtcNow.AddHours(2),
            UsuarioId = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email
        };
    }
}