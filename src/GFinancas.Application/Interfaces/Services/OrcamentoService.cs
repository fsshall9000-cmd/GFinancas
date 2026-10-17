using GFinancas.Application.DTOs.Orcamentos;
using GFinancas.Application.Interfaces.Repositories;
using GFinancas.Application.Interfaces.Repository;
using GFinancas.Application.Interfaces.Security;
using GFinancas.Application.Interfaces.Services;
using GFinancas.Domain.Entities;
using GFinancas.Application.Exceptions;
using GFinancas.Domain.Enums;

namespace GFinancas.Application.Services;

public class OrcamentoService : IOrcamentoService
{
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IUsuarioAtualService _usuarioAtualService;

    public OrcamentoService(
        IOrcamentoRepository orcamentoRepository,
        ICategoriaRepository categoriaRepository,
        ITransacaoRepository transacaoRepository,
        IUsuarioAtualService usuarioAtualService)
    {
        _orcamentoRepository = orcamentoRepository;
        _categoriaRepository = categoriaRepository;
        _transacaoRepository = transacaoRepository;
        _usuarioAtualService = usuarioAtualService;
    }


    public async Task<OrcamentoResponseDto> CriarAsync(
        CriarOrcamentoDto dto)
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;


        // ==========================================
        // VALIDAR MÊS
        // ==========================================

        if (dto.Mes < 1 || dto.Mes > 12)
        {
            throw new Exception(
                "O mês deve estar entre 1 e 12."
            );
        }


        // ==========================================
        // VALIDAR ANO
        // ==========================================

        if (dto.Ano <= 0)
        {
            throw new Exception(
                "O ano informado é inválido."
            );
        }


        // ==========================================
        // VALIDAR VALOR
        // ==========================================

        if (dto.ValorLimite <= 0)
        {
            throw new Exception(
                "O valor limite deve ser maior que zero."
            );
        }


        // ==========================================
        // BUSCAR CATEGORIA
        // ==========================================

        var categoria =
            await _categoriaRepository
                .ObterPorIdAsync(
                    dto.CategoriaId
                );


        if (categoria is null ||
            categoria.UsuarioId != usuarioId)
        {
            throw new Exception(
                "Categoria não encontrada."
            );
        }


        // ==========================================
        // SOMENTE CATEGORIA DE DESPESA
        // ==========================================

        if (categoria.Tipo != TipoTransacao.Despesa)
        {
            throw new Exception(
                "O orçamento somente pode ser criado para categorias de despesa."
            );
        }


        // ==========================================
        // VERIFICAR DUPLICIDADE
        // ==========================================

        var existente =
            await _orcamentoRepository
                .ObterPorCategoriaMesAsync(
                    usuarioId,
                    dto.CategoriaId,
                    dto.Ano,
                    dto.Mes
                );


        if (existente is not null)
        {
            throw new Exception(
                "Já existe um orçamento para esta categoria neste período."
            );
        }


        // ==========================================
        // CRIAR ORÇAMENTO
        // ==========================================

        var orcamento = new Orcamento
        {
            Ano = dto.Ano,

            Mes = dto.Mes,

            ValorLimite = dto.ValorLimite,

            CategoriaId = dto.CategoriaId,

            UsuarioId = usuarioId
        };


        await _orcamentoRepository
            .AdicionarAsync(orcamento);


        return await MontarResponseAsync(
            orcamento,
            categoria.Nome
        );
    }


    public async Task<IEnumerable<OrcamentoResponseDto>>
        ListarPorMesAsync(
            int ano,
            int mes)
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;


      if (ano <= 0)
      {
        throw new ValidacaoException(
        "O ano informado é inválido.");
      }

      if (mes < 1 || mes > 12)
      {
        throw new ValidacaoException(
        "O mês deve estar entre 1 e 12.");
      }


        var orcamentos =
            await _orcamentoRepository
                .ListarPorMesAsync(
                    usuarioId,
                    ano,
                    mes
                );


        var resultado =
            new List<OrcamentoResponseDto>();


        foreach (var orcamento in orcamentos)
        {
            resultado.Add(
                await MontarResponseAsync(
                    orcamento,
                    orcamento.Categoria.Nome
                )
            );
        }


        return resultado;
    }


    public async Task<OrcamentoResponseDto>
        ObterPorIdAsync(
            int id)
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;


        var orcamento =
            await _orcamentoRepository
                .ObterPorIdAsync(
                    id,
                    usuarioId
                );


        if (orcamento is null)
        {
            throw new NaoEncontradoException(
            "Orçamento não encontrado."
            );
        }


        return await MontarResponseAsync(
            orcamento,
            orcamento.Categoria.Nome
        );
    }


    public async Task<OrcamentoResponseDto>
        AtualizarAsync(
            int id,
            AtualizarOrcamentoDto dto)
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;


        if (dto.ValorLimite <= 0)
        {
            throw new Exception(
                "O valor limite deve ser maior que zero."
            );
        }


        var orcamento =
            await _orcamentoRepository
                .ObterPorIdAsync(
                    id,
                    usuarioId
                );


        if (orcamento is null)
        {
            throw new Exception(
                "Orçamento não encontrado."
            );
        }


        orcamento.ValorLimite =
            dto.ValorLimite;


        await _orcamentoRepository
            .AtualizarAsync(
                orcamento
            );


        return await MontarResponseAsync(
            orcamento,
            orcamento.Categoria.Nome
        );
    }


    public async Task RemoverAsync(
        int id)
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;


        var orcamento =
            await _orcamentoRepository
                .ObterPorIdAsync(
                    id,
                    usuarioId
                );


        if (orcamento is null)
        {
            throw new Exception(
                "Orçamento não encontrado."
            );
        }


        await _orcamentoRepository
            .RemoverAsync(
                orcamento
            );
    }


    // ==============================================
    // MONTA O RESULTADO DO ORÇAMENTO
    // ==============================================

    private async Task<OrcamentoResponseDto>
        MontarResponseAsync(
            Orcamento orcamento,
            string categoria)
    {
        var usuarioId =
            _usuarioAtualService.UsuarioId;


        var dataInicio =
            new DateTime(
                orcamento.Ano,
                orcamento.Mes,
                1
            );


        var dataFim =
            dataInicio.AddMonths(1);


        var valorGasto =
            await _transacaoRepository
                .ObterTotalDespesasPorCategoriaAsync(
                    usuarioId,
                    orcamento.CategoriaId,
                    dataInicio,
                    dataFim
                );


        var valorDisponivel =
            orcamento.ValorLimite -
            valorGasto;


        var percentualUtilizado =
            orcamento.ValorLimite > 0

                ? valorGasto /
                  orcamento.ValorLimite * 100

                : 0;


        return new OrcamentoResponseDto
        {
            Id = orcamento.Id,

            Ano = orcamento.Ano,

            Mes = orcamento.Mes,

            CategoriaId =
                orcamento.CategoriaId,

            Categoria =
                categoria,

            ValorLimite =
                orcamento.ValorLimite,

            ValorGasto =
                valorGasto,

            ValorDisponivel =
                valorDisponivel,

            PercentualUtilizado =
                percentualUtilizado
        };
    }
}