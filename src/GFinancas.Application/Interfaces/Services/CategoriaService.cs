using GFinancas.Application.DTOs.Categorias;
using GFinancas.Application.Exceptions;
using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Application.Interfaces.Repository;
using GFinancas.Application.Interfaces.Security;
using GFinancas.Application.Interfaces.Services;
using GFinancas.Domain.Entities;
using GFinancas.Domain.Enums;

namespace GFinancas.Application.Services;

/// <summary>
/// Aplica as regras de negócio de categorias
/// e prepara os dados de resposta.
/// </summary>
public class CategoriaService : ICategoriaService
{
    /// <summary>
    /// Repositório usado para acessar categorias.
    /// </summary>
    private readonly ICategoriaRepository
        _categoriaRepository;

    /// <summary>
    /// Repositório usado para acessar usuários.
    /// </summary>
    private readonly IUsuarioRepository
        _usuarioRepository;

    /// <summary>
    /// Repositório usado para acessar orçamentos.
    /// </summary>
    private readonly IOrcamentoRepository
        _orcamentoRepository;

    /// <summary>
    /// Serviço que fornece o usuário da requisição atual.
    /// </summary>
    private readonly IUsuarioAtualService
        _usuarioAtualService;


    /// <summary>
    /// Recebe as dependências necessárias ao serviço.
    /// </summary>
    public CategoriaService(
        ICategoriaRepository categoriaRepository,
        IUsuarioRepository usuarioRepository,
        IUsuarioAtualService usuarioAtualService,
        IOrcamentoRepository orcamentoRepository)
    {
        _categoriaRepository =
            categoriaRepository;

        _usuarioRepository =
            usuarioRepository;

        _usuarioAtualService =
            usuarioAtualService;

        _orcamentoRepository =
            orcamentoRepository;
    }


    /// <summary>
    /// Valida e cadastra uma categoria para
    /// o usuário autenticado.
    /// </summary>
    public async Task<CategoriaResponseDto>
        CriarAsync(CriarCategoriaDto dto)
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;


        // ---------------------------------------------
        // 1. Valida nome
        // ---------------------------------------------

        if (string.IsNullOrWhiteSpace(dto.Nome))
        {
            throw new ValidacaoException(
                "O nome da categoria é obrigatório.");
        }

        var nome =
            dto.Nome.Trim();

        if (nome.Length > 100)
        {
            throw new ValidacaoException(
                "O nome da categoria deve ter no máximo 100 caracteres.");
        }


        // ---------------------------------------------
        // 2. Valida tipo
        // ---------------------------------------------

        if (!Enum.IsDefined(
                typeof(TipoTransacao),
                dto.Tipo))
        {
            throw new ValidacaoException(
                "Tipo de categoria inválido.");
        }


        // ---------------------------------------------
        // 3. Verifica duplicidade
        // ---------------------------------------------

        var categoriaExistente =
            await _categoriaRepository
                .ObterPorNomeTipoEUsuarioAsync(
                    nome,
                    dto.Tipo,
                    usuarioId);

        if (categoriaExistente is not null)
        {
            throw new ConflitoException(
                "Já existe uma categoria com esse nome e tipo.");
        }


        // ---------------------------------------------
        // 4. Cria entidade
        // ---------------------------------------------

        var categoria =
            new Categoria
            {
                Nome = nome,
                Tipo = dto.Tipo,
                UsuarioId = usuarioId
            };


        // ---------------------------------------------
        // 5. Persiste
        // ---------------------------------------------

        await _categoriaRepository
            .AdicionarAsync(categoria);


        // ---------------------------------------------
        // 6. Retorna
        // ---------------------------------------------

        return MapearParaResponse(categoria);
    }


    /// <summary>
    /// Lista as categorias do usuário autenticado.
    /// </summary>
    public async Task<IEnumerable<CategoriaResponseDto>>
        ListarAsync()
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;

        var categorias =
            await _categoriaRepository
                .ObterPorUsuarioIdAsync(usuarioId);

        return categorias
            .Select(MapearParaResponse)
            .ToList();
    }


    /// <summary>
    /// Obtém uma categoria do usuário autenticado.
    /// </summary>
    public async Task<CategoriaResponseDto?>
        ObterPorIdAsync(int id)
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;

        var categoria =
            await _categoriaRepository
                .ObterPorIdAsync(id);

        if (categoria is null ||
            categoria.UsuarioId != usuarioId)
        {
            throw new NaoEncontradoException(
                "Categoria não encontrada.");
        }

        return MapearParaResponse(categoria);
    }


    /// <summary>
    /// Atualiza uma categoria pertencente ao
    /// usuário autenticado.
    /// </summary>
    public async Task<CategoriaResponseDto>
        AtualizarAsync(
            int id,
            AtualizarCategoriaDto dto)
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;


        // ---------------------------------------------
        // 1. Busca categoria
        // ---------------------------------------------

        var categoria =
            await _categoriaRepository
                .ObterPorIdAsync(id);

        if (categoria is null ||
            categoria.UsuarioId != usuarioId)
        {
            throw new NaoEncontradoException(
                "Categoria não encontrada.");
        }


        // ---------------------------------------------
        // 2. Valida nome
        // ---------------------------------------------

        if (string.IsNullOrWhiteSpace(dto.Nome))
        {
            throw new ValidacaoException(
                "O nome da categoria é obrigatório.");
        }

        var nome =
            dto.Nome.Trim();

        if (nome.Length > 100)
        {
            throw new ValidacaoException(
                "O nome da categoria deve ter no máximo 100 caracteres.");
        }


        // ---------------------------------------------
        // 3. Valida tipo
        // ---------------------------------------------

        if (!Enum.IsDefined(
                typeof(TipoTransacao),
                dto.Tipo))
        {
            throw new ValidacaoException(
                "Tipo de categoria inválido.");
        }


        // ---------------------------------------------
        // 4. Verifica duplicidade
        // ---------------------------------------------

        var categoriaExistente =
            await _categoriaRepository
                .ObterPorNomeTipoEUsuarioAsync(
                    nome,
                    dto.Tipo,
                    usuarioId);

        if (categoriaExistente is not null &&
            categoriaExistente.Id != id)
        {
            throw new ConflitoException(
                "Já existe uma categoria com esse nome e tipo.");
        }


        // ---------------------------------------------
        // 5. Verifica se o TIPO está sendo alterado
        // ---------------------------------------------

        var alterandoTipo =
            categoria.Tipo != dto.Tipo;

        if (alterandoTipo)
        {
            // -----------------------------------------
            // 5.1 Verifica transações
            // -----------------------------------------

            var possuiTransacoes =
                await _categoriaRepository
                    .PossuiTransacoesAsync(id);

            if (possuiTransacoes)
            {
                throw new ValidacaoException(
                    "Não é possível alterar o tipo de uma categoria que possui transações.");
            }


            // -----------------------------------------
            // 5.2 Verifica orçamentos
            // -----------------------------------------

            var possuiOrcamentos =
                await _orcamentoRepository
                    .ExistePorCategoriaAsync(id);

            if (possuiOrcamentos)
            {
                throw new ValidacaoException(
                    "Não é possível alterar o tipo de uma categoria que possui orçamentos.");
            }
        }


        // ---------------------------------------------
        // 6. Atualiza propriedades
        // ---------------------------------------------

        categoria.Nome =
            nome;

        categoria.Tipo =
            dto.Tipo;


        // ---------------------------------------------
        // 7. Persiste
        // ---------------------------------------------

        await _categoriaRepository
            .AtualizarAsync(categoria);


        // ---------------------------------------------
        // 8. Retorna
        // ---------------------------------------------

        return MapearParaResponse(categoria);
    }


    /// <summary>
    /// Remove uma categoria quando não possui
    /// transações nem orçamentos.
    /// </summary>
    public async Task RemoverAsync(int id)
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;


        // ---------------------------------------------
        // 1. Busca categoria
        // ---------------------------------------------

        var categoria =
            await _categoriaRepository
                .ObterPorIdAsync(id);

        if (categoria is null ||
            categoria.UsuarioId != usuarioId)
        {
            throw new NaoEncontradoException(
                "Categoria não encontrada.");
        }


        // ---------------------------------------------
        // 2. Verifica transações
        // ---------------------------------------------

        var possuiTransacoes =
            await _categoriaRepository
                .PossuiTransacoesAsync(id);

        if (possuiTransacoes)
        {
            throw new ValidacaoException(
                "Não é possível excluir uma categoria que possui transações.");
        }


        // ---------------------------------------------
        // 3. Verifica orçamentos
        // ---------------------------------------------

        var possuiOrcamentos =
            await _orcamentoRepository
                .ExistePorCategoriaAsync(id);

        if (possuiOrcamentos)
        {
            throw new ValidacaoException(
                "Não é possível excluir uma categoria que possui orçamentos.");
        }


        // ---------------------------------------------
        // 4. Remove
        // ---------------------------------------------

        await _categoriaRepository
            .RemoverAsync(categoria);
    }


    /// <summary>
    /// Converte Categoria para CategoriaResponseDto.
    /// </summary>
    private static CategoriaResponseDto
        MapearParaResponse(Categoria categoria)
    {
        return new CategoriaResponseDto
        {
            Id = categoria.Id,

            Nome = categoria.Nome,

            Tipo =
                (int)categoria.Tipo,

            TipoDescricao =
                categoria.Tipo.ToString(),

            UsuarioId =
                categoria.UsuarioId,

            Usuario =
                categoria.Usuario?.Nome
                ?? string.Empty
        };
    }
}