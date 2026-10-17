using GFinancas.Application.Interfaces.Security;
using Moq;

var usuarioMock = new Mock<IUsuarioAtualService>();

usuarioMock
    .Setup(x => x.UsuarioId)
    .Returns(1);