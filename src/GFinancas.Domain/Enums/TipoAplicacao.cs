namespace GFinancas.Domain.Enums;

/// <summary>
/// Identifica as modalidades de aplicação financeira disponíveis para cadastro.
/// </summary>
public enum TipoAplicacao
{
    TesouroDireto = 1,
    CDB = 2,
    LCI = 3,
    LCA = 4,
    FundoInvestimento = 5,
    Acao = 6,
    ETF = 7,
    Outro = 8
}