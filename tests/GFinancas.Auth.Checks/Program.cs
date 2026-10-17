using System.Text.Json;
using GFinancas.API.Services;
using GFinancas.Application.DTOs.Auth;
using GFinancas.Application.Exceptions;
using GFinancas.Application.Interfaces.Repository;
using GFinancas.Domain.Entities;
using GFinancas.Infrastructure.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

var hasher = new SenhaHasher();
var repository = new UsuarioFake(new Usuario
{
    Id = 1, Email = "teste@example.com", Nome = "Teste", SenhaHash = hasher.GerarHash("senha-antiga")
});
var provider = new EphemeralDataProtectionProvider();
var email = new EmailFake();
var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
{
    ["RecuperacaoSenha:UrlFrontend"] = "https://financas.example/"
}).Build();
var service = new RecuperacaoSenhaService(repository, hasher, provider, email, configuration);

await service.SolicitarAsync(new() { Email = "  TESTE@example.com  " });
Check(repository.UltimoEmail == "teste@example.com", "normalização do e-mail");
Check(email.Destinatario == repository.Usuario.Email, "destinatário do link");
Check(email.Link.StartsWith("https://financas.example/redefinir-senha?token="), "rota de recuperação");
var token = Uri.UnescapeDataString(new Uri(email.Link).Query[7..]);
var protector = provider.CreateProtector("GFinancas.RecuperacaoSenha.v1").ToTimeLimitedDataProtector();
protector.Unprotect(token, out var expiracao);
Check(expiracao > DateTimeOffset.UtcNow.AddMinutes(29) && expiracao <= DateTimeOffset.UtcNow.AddMinutes(30), "validade de 30 minutos");

await Invalid(() => service.RedefinirAsync(new() { Token = token, Senha = "curta" }), "senha curta");
await Invalid(() => service.RedefinirAsync(new() { Token = "token-alterado", Senha = "nova-senha" }), "token inválido");
var expirado = protector.Protect(JsonSerializer.Serialize(new { UsuarioId = 1, repository.Usuario.SenhaHash }), DateTimeOffset.UtcNow.AddMinutes(-1));
await Invalid(() => service.RedefinirAsync(new() { Token = expirado, Senha = "nova-senha" }), "token expirado");
Check(hasher.Verificar("senha-antiga", repository.Usuario.SenhaHash), "senha preservada nas falhas");

await service.RedefinirAsync(new() { Token = token, Senha = "nova-senha" });
Check(hasher.Verificar("nova-senha", repository.Usuario.SenhaHash), "nova senha aceita");
Check(!hasher.Verificar("senha-antiga", repository.Usuario.SenhaHash), "senha antiga rejeitada");
await Invalid(() => service.RedefinirAsync(new() { Token = token, Senha = "outra-senha" }), "reutilização rejeitada");

await service.SolicitarAsync(new() { Email = "teste@example.com" });
var tokenConcorrente = Uri.UnescapeDataString(new Uri(email.Link).Query[7..]);
repository.RejeitarAtualizacao = true;
await Invalid(() => service.RedefinirAsync(new() { Token = tokenConcorrente, Senha = "outra-senha" }), "troca concorrente rejeitada");
repository.RejeitarAtualizacao = false;
var envios = email.Envios;
await service.SolicitarAsync(new() { Email = "ausente@example.com" });
Check(email.Envios == envios, "e-mail inexistente não recebe link");
Console.WriteLine("Todas as verificações de recuperação de senha passaram.");

static void Check(bool condition, string name)
{
    if (!condition) throw new Exception($"Falhou: {name}");
    Console.WriteLine($"OK: {name}");
}
static async Task Invalid(Func<Task> operation, string name)
{
    try { await operation(); }
    catch (ValidacaoException) { Console.WriteLine($"OK: {name}"); return; }
    throw new Exception($"Falhou: {name}");
}

sealed class EmailFake : IEmailRecuperacaoSenha
{
    public string Link { get; private set; } = "";
    public string Destinatario { get; private set; } = "";
    public int Envios { get; private set; }
    public void ValidarConfiguracao() { }
    public Task EnviarAsync(string destinatario, string link)
    {
        Destinatario = destinatario; Link = link; Envios++;
        return Task.CompletedTask;
    }
}
sealed class UsuarioFake(Usuario usuario) : IUsuarioRepository
{
    public Usuario Usuario { get; } = usuario;
    public string UltimoEmail { get; private set; } = "";
    public bool RejeitarAtualizacao { get; set; }
    public Task<Usuario?> ObterPorIdAsync(int id) => Task.FromResult<Usuario?>(id == Usuario.Id ? Usuario : null);
    public Task<Usuario?> ObterPorEmailAsync(string email)
    {
        UltimoEmail = email;
        return Task.FromResult<Usuario?>(email == Usuario.Email ? Usuario : null);
    }
    public Task<IEnumerable<Usuario>> ObterTodosAsync() => Task.FromResult<IEnumerable<Usuario>>([Usuario]);
    public Task AdicionarAsync(Usuario novo) => throw new NotSupportedException();
    public Task<bool> AtualizarSenhaAsync(int id, string hashAtual, string novoHash)
    {
        if (RejeitarAtualizacao || id != Usuario.Id || hashAtual != Usuario.SenhaHash) return Task.FromResult(false);
        Usuario.SenhaHash = novoHash;
        return Task.FromResult(true);
    }
}
