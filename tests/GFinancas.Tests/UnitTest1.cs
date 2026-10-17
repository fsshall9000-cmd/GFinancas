namespace GFinancas.Tests;

public class CalculoFinanceiroTests
{
    [Fact]
    public void CalcularSaldo_DeveRetornarValorCorreto()
    {
        // Arrange - Preparação
        decimal totalAportado = 300m;
        decimal totalResgatado = 190m;

        // Act - Execução
        decimal saldo = totalAportado - totalResgatado;

        // Assert - Verificação
        Assert.Equal(120m, saldo);
    }
}