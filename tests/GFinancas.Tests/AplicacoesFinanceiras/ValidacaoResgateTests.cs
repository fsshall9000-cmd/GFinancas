using Xunit;

namespace GFinancas.Tests.AplicacoesFinanceiras;

public class ValidacaoResgateTests
{
    [Theory]
    [InlineData(200, 100, true)]
    [InlineData(200, 200, true)]
    [InlineData(200, 250, false)]
    [InlineData(200, 0, false)]
    [InlineData(200, -50, false)]
    public void ValidarResgate_DeveRespeitarSaldoDisponivel(
        int saldoDisponivel,
        int valorResgate,
        bool resultadoEsperado)
    {
        // Arrange
        bool resultadoObtido;

        // Act
        resultadoObtido =
            valorResgate > 0 &&
            valorResgate <= saldoDisponivel;

        // Assert
        Assert.Equal(resultadoEsperado, resultadoObtido);
    }
}