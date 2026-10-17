# Cadastro e recuperação de senha

Na tela `/login`, **Cadastrar usuário** solicita nome, e-mail, senha e confirmação. O cadastro usa `POST /api/Usuario/Adicionar` e retorna ao login. Os novos e-mails são armazenados em minúsculas e sem espaços nas extremidades. A senha deve ter de 8 a 128 caracteres.

**Esqueceu a senha?** solicita o e-mail e chama `POST /api/Auth/esqueceu-senha`. A resposta normal é a mesma para e-mails cadastrados e inexistentes. O e-mail contém um link para `/redefinir-senha?token=...`, válido por 30 minutos. A página solicita a nova senha e sua confirmação e chama `POST /api/Auth/redefinir-senha`. O token é protegido pelo ASP.NET Core Data Protection, vinculado ao hash atual da senha. A atualização atômica invalida os links anteriores e impede o uso simultâneo do mesmo link. Não é necessária uma migration.

## Configuração do envio

Configure `RecuperacaoSenha:UrlFrontend` com a URL pública do frontend, terminando em `/` (HTTPS em produção), e os parâmetros SMTP. As configurações vazias incluídas em `appsettings.json` precisam ser preenchidas para enviar e-mails. Sem configuração, a API informa que o serviço não está configurado.

Prefira variáveis de ambiente ou user secrets para as credenciais:

```text
RecuperacaoSenha__UrlFrontend=https://financas.example/
Smtp__Host=smtp.example.com
Smtp__Porta=587
Smtp__UsarSsl=true
Smtp__Remetente=contato@example.com
Smtp__Usuario=usuario-smtp
Smtp__Senha=<senha-do-provedor>
```

O cliente SMTP usa STARTTLS quando `UsarSsl=true`; configure a porta correspondente no provedor. Para um servidor SMTP local de teste sem autenticação, use `UsarSsl=false` e deixe usuário e senha vazios.

Em produção, mantenha as chaves do ASP.NET Core Data Protection persistentes e compartilhadas entre instâncias da API; perder ou trocar o chaveiro invalida os links pendentes. O host do frontend deve encaminhar `/redefinir-senha` para o aplicativo Angular. Os endpoints de recuperação permitem cinco requisições por minuto por IP; atrás de proxy, configure o encaminhamento de IPs somente para proxies confiáveis. A troca de senha não revoga JWTs já emitidos, que permanecem válidos até sua expiração.

## Verificação

```bash
dotnet run --project tests/GFinancas.Auth.Checks
cd frontend/gfinancas-web
npm run build -- --configuration development
npm test -- --watch=false --browsers=ChromeHeadless --include=src/app/features/auth/login/login.component.spec.ts
```

As verificações de backend não enviam e-mails reais: usam um remetente falso e validam expiração, token alterado, reutilização, atualização concorrente e troca do hash da senha. Para conferir a entrega real, configure um servidor SMTP de teste e solicite a recuperação de um usuário cadastrado.
