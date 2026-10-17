using GFinancas.Application.Dto.Usuarios;
using GFinancas.Application.Interfaces.Repository;
using GFinancas.Application.Exceptions;
using GFinancas.Domain.Entities;
using GFinancas.Application.Interfaces.Security;
using GFinancas.Infrastructure.Security;

namespace GFinancas.Application.Interfaces.Services;

/// <summary>
/// Aplica as regras de negócio de usuários e prepara os dados de resposta.
/// </summary>
public class UsuarioService : IUsuarioService
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
/// Serviço que fornece o identificador do usuário da requisição atual.
/// </summary>
private readonly IUsuarioAtualService _usuarioAtualService;

    /// <summary>
    /// Recebe e armazena as dependências necessárias às operações desta classe.
    /// </summary>
    /// <param name="usuarioRepository">Repositório usado para acessar os dados de usuários.</param>
    /// <param name="senhaHasher">Serviço de geração e verificação de hashes de senha.</param>
    /// <param name="usuarioAtualService">Serviço que fornece o identificador do usuário da requisição atual.</param>
    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        ISenhaHasher senhaHasher,
        IUsuarioAtualService usuarioAtualService)
    {
        _usuarioRepository = usuarioRepository;
        _senhaHasher = senhaHasher;
        _usuarioAtualService = usuarioAtualService;
    }
    /// <summary>
    /// Valida duplicidade de e-mail e senha, gera o hash e cadastra o usuário.
    /// </summary>
    /// <param name="dto">Dados de entrada necessários para executar a operação.</param>
    public async Task<UsuarioResponseDto> CriarAsync(CriarUsuarioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome) || dto.Nome.Trim().Length > 150)
            throw new ValidacaoException("O nome é obrigatório e deve possuir no máximo 150 caracteres.");

        var email = dto.Email?.Trim().ToLowerInvariant() ?? string.Empty;
        if (email.Length > 254 || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email) ||
            string.IsNullOrWhiteSpace(email))
            throw new ValidacaoException("Informe um e-mail válido.");

        // Usuário encontrado pelo e-mail informado, usado para impedir cadastro duplicado.
        var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(email);
        if (usuarioExistente != null)
        {
            throw new ConflitoException("Já existe um usuário cadastrado com este e-mail.");

        }
        
        if (string.IsNullOrWhiteSpace(dto.Senha))
        {
         throw new ValidacaoException(
        "A senha é obrigatória.");
        }

        if (dto.Senha.Length < 8 || dto.Senha.Length > 128)
        {
           throw new ValidacaoException(
          "A senha deve possuir entre 8 e 128 caracteres.");
       }    
        // Hash gerado para persistir a credencial sem armazenar a senha em texto puro.
        var senhaHash = _senhaHasher.GerarHash(dto.Senha);

        // Dados de usuário preparados para persistência após as validações.
        var usuario = new Usuario
        {
            Nome = dto.Nome.Trim(),
            Email = email,
            SenhaHash = senhaHash
        };    
    
        await _usuarioRepository.AdicionarAsync(usuario);

        return MapearParaResponse(usuario);
    }

    /// <summary>
    /// Lista usuários cadastrados no sistema e converte os registros em DTOs de resposta.
    /// </summary>
    public async Task<IEnumerable<UsuarioResponseDto>> ListarAsync()
    {
        // Usuários cadastrados obtidos para conversão em DTOs de resposta.
        var usuarios = await _usuarioRepository.ObterTodosAsync();

        return usuarios.Select(MapearParaResponse);

    }

    /// <summary>
    /// Busca usuário pelo identificador e retorna nulo quando não encontra o registro.
    /// </summary>
    /// <param name="id">Identificador do registro procurado ou alterado.</param>
    public async Task<UsuarioResponseDto?> ObterPorIdAsync(int id)
    {
        
        // Registro de usuário consultado para esta operação; sua existência é verificada antes do uso.
        var usuario = await _usuarioRepository.ObterPorIdAsync(id);

        if (usuario == null)
        {
            return null;
        }

        return MapearParaResponse(usuario);

    }

    /// <summary>
    /// Busca e retorna os dados cadastrais do usuário autenticado.
    /// </summary>
    public async Task<UsuarioResponseDto> ObterUsuarioAtualAsync()
{
    // Identificador do usuário autenticado, utilizado para associar ou restringir os dados.
    var usuarioId = _usuarioAtualService.UsuarioId;

    // Registro de usuário consultado para esta operação; sua existência é verificada antes do uso.
    var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);

    if (usuario is null)
        throw new NaoEncontradoException("Usuário não encontrado.");

    return new UsuarioResponseDto
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email
    };
}

    /// <summary>
    /// Converte os dados de usuário para o DTO de resposta.
    /// </summary>
    /// <param name="usuario">Usuário utilizado na operação.</param>
    private static UsuarioResponseDto MapearParaResponse(
        Usuario usuario)
    {
        return new UsuarioResponseDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email
        };
    }
}
